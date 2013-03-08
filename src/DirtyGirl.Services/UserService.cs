using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DirtyGirl.Models;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Services.Utils;
using System.IO;
using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Models.Enums;

namespace DirtyGirl.Services
{
    public class UserService : IUserService
    {
        #region private members

        IRepositoryGroup _repository;

        #endregion

        #region constructor

        public UserService(IRepositoryGroup repository)
        {
            this._repository = repository;
        }

        #endregion

        #region validation

        protected bool ValidateUser(User userToValidate, ServiceResult serviceResult)
        {
            //Users tied to a FB account will not have a password
            //When updating users, a password will not be included, thus the UserId condition
            if (userToValidate.UserId <= 0 && String.IsNullOrEmpty(userToValidate.Password) && userToValidate.FacebookId == null)
            {
                serviceResult.AddServiceError("Password", "A password is required");
            }
            if (_repository.Users.Filter(user => user.UserName.Trim().ToLower().Equals(userToValidate.UserName.Trim().ToLower()) && !user.UserId.Equals(userToValidate.UserId)).Count() > 0)
            {
                serviceResult.AddServiceError("UserName", "A user with this username already exists.");
            }
            if (_repository.Users.Filter(user => user.EmailAddress.Equals(userToValidate.EmailAddress) && !user.UserId.Equals(userToValidate.UserId)).Count() > 0)
            {
                serviceResult.AddServiceError("EmailAddress", "This email is already in use by another user.");
            }
            //TODO: Swear filter username


            if (userToValidate.Image != null)
            {
                userToValidate.Image = Utilities.ResizeImage(userToValidate.Image, 180, 180);
            }

            return serviceResult.Success;
        }

        public bool ValidateUser(int userID, string password)
        {
            var u = _repository.Users.Get(userID);

            if (u != null && Crypto.ValidatePassword(password.Trim(), new CryptoHashContainer(u.Salt, u.Password)))
                return true;

            return false;

        }
        #endregion

        #region public methods

        public IList<User> GetAllUsers()
        {
            return _repository.Users.All().ToList();
        }

        public IList<Models.Region> GetRegionsForCountry(int countryId)
        {
            return _repository.Regions.Filter(r => r.CountryId.Equals(countryId)).ToList();
        }

        public IList<Registration> GetActiveRegistrations(int userId)
        {
            return _repository.Registrations.Filter(x => x.UserId == userId && x.RegistrationStatus == RegistrationStatus.Active && x.EventWave.EventDate.DateOfEvent >= DateTime.Now).ToList();
        }

        public IList<RedemptionCode> GetActiveRedemptionCodes(int userId)
        {
            var redemptionCodes = _repository.RedemptionCodes.Filter(x => x.GeneratingRegistration.UserId == userId && x.ResultingRegistrationId == null).ToList();

            foreach (var code in redemptionCodes)
            {
                if (code.RedemptionCodeType == RedemptionCodeType.Transfer && code.DateAdded < DateTime.Now.AddDays(-DirtyGirlServiceConfig.Settings.MaxTransferHeldDays) && code.ResultingRegistrationId == null)
                {
                    code.RedemptionCodeType = RedemptionCodeType.StraightValue;
                    code.GeneratingRegistration.RegistrationStatus = RegistrationStatus.Cancelled;
                    _repository.SaveChanges();
                }
            }

            return redemptionCodes;
        }


        public User GetUserById(int userId)
        {
            return _repository.Users.Find(x => x.UserId == userId);
        }

        public User GetUserByUsername(string userName)        
        {
            var user = _repository.Users.Filter(x => x.UserName.Trim().ToUpper() == userName.Trim().ToUpper()).FirstOrDefault();
            return user;
        }

        public User GetUserByFacebookId(int facebookId)
        {
            return _repository.Users.Filter(x => x.FacebookId == facebookId).SingleOrDefault();
        }

        public ServiceResult CreateUser(User u)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (ValidateUser(u, result))
                {
                    
                    //Non-Facebook Users
                    if (u.FacebookId == null)
                    {
                        var chc = Crypto.CreateHash(u.Password);
                        u.Password = chc.hash;
                        u.Salt = chc.salt;
                        u.EmailVerificationCode = DirtyGirlServiceConfig.Settings.UsersMustConfirmEmail ? GenerationEmailConfirmationCode() : null;
                        
                    }

                    //If a user is created with both image data and "User Facebook Image" selected, the uploaded image will be used instead.
                    if (u.Image != null) { u.UseFacebookImage = false; }

                    u.IsActive = true;
                    u.UserName = u.UserName.Trim();

                    _repository.Users.Create(u);

                    u.Roles = new List<Role>();
                    u.Roles.Add(_repository.Roles.Find(x => x.Name == "Registrant"));
                   
                    if (result.Success)
                    {
                        _repository.SaveChanges();                                              
                        SendEmailConfirmation(u.UserId);
                    }
                    
                }
            }
            catch (Exception ex)
            { result.AddServiceError(Utilities.GetInnerMostException(ex)); }
            return result;
        }

        public ServiceResult UpdateUser(User u, bool setIsActive)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (ValidateUser(u, result))
                {

                    var updateUser = GetUserById(u.UserId);
                    updateUser.UserName = u.UserName.Trim();
                    updateUser.FirstName = u.FirstName;
                    updateUser.LastName = u.LastName;
                    updateUser.Address1 = u.Address1;
                    updateUser.Address2 = u.Address2;
                    updateUser.Locality = u.Locality;
                    updateUser.RegionId = u.RegionId;
                    updateUser.PostalCode = u.PostalCode;
                    updateUser.EmailAddress = u.EmailAddress;

                    if (setIsActive)
                    {
                        updateUser.IsActive = u.IsActive;
                    }

                    //if uploading a new imagine but also selected "use facebook image", use uploaded photo instead
                    if (u.Image != null)
                    {
                        updateUser.Image = u.Image;
                        u.UseFacebookImage = false;
                    }

                    updateUser.UseFacebookImage = u.UseFacebookImage;
                    //Clear out their uploaded image if they opt to use their FB image
                    if (u.UseFacebookImage) { updateUser.Image = null; }

                    _repository.Users.Update(updateUser);

                    if (result.Success)
                    {
                        _repository.SaveChanges();
                        _repository.Users.LoadProperties(updateUser);
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult RemoveUser(User u)
        {

            throw new NotImplementedException();
        }

        public ServiceResult UpdatePassword(int userId, string password)
        {
            //TODO: Check password requirements - length, complexity, etc
            ServiceResult result = new ServiceResult();
            try
            {
                
                var updateUser = _repository.Users.Get(userId);
                var chc = Crypto.CreateHash(password);
                updateUser.Password = chc.hash;
                updateUser.Salt = chc.salt;
                _repository.Users.Update(updateUser);

                if (result.Success)
                {
                    _repository.SaveChanges();
                    _repository.Users.LoadProperties(updateUser);
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }


        public bool CheckUsernameAvailability(string username)
        {
            return !_repository.Users.Filter(u => u.UserName.Trim().ToLower().Equals(username.Trim().ToLower())).Any();
        }

        public bool SendEmailConfirmation(int userId)
        {
            return SendEmailConfirmation(_repository.Users.Find(x => x.UserId == userId));
            }

        public bool SendEmailConfirmation(string userName)
        {
            return SendEmailConfirmation(_repository.Users.Find(x => x.UserName.ToLower() == userName.ToLower()));
        }

        public bool SendEmailConfirmation(User user)
        {
            try
            {
                if (user.EmailVerificationCode == null)
                {
                    // Auto-Activate user.  Ability to have email validation exists in the app and to enable it
                    // Uncomment the following line and then change the Email Confirmation Template.
                    user.EmailVerificationCode = DirtyGirlServiceConfig.Settings.UsersMustConfirmEmail ? GenerationEmailConfirmationCode() : null;
                    _repository.SaveChanges();
                }

                IEmailService emailService = new EmailService();                
                emailService.SendRegistrationConfirmationEmail(user.UserId);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int? ConfirmAccount(string confirmationCode)
        {
            var user = _repository.Users.Filter(u => u.EmailVerificationCode.Equals(confirmationCode.Trim())).FirstOrDefault();
            if (user != null)
            {
                user.EmailVerificationCode = null;
                _repository.Users.Update(user);
                _repository.SaveChanges();
                return user.UserId;
            }

            return null;
        }

        public ServiceResult GeneratePasswordResetRequest(string emailAddress)
        {
            var user = _repository.Users.Find(u => u.EmailAddress == emailAddress);
            if (user == null)
            {
                var result = new ServiceResult();
                result.AddServiceError("No account is associated with this email address");
                return result;
            }
            return GeneratePasswordResetRequest(user);
        }

        private ServiceResult GeneratePasswordResetRequest(User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            // Get user object
            var result = new ServiceResult();
            try
            {
                // check for existing Reset Token
                if (user.PasswordResetToken != null &&
                    DateTime.Compare(
                        (user.PasswordResetRequested.HasValue
                             ? user.PasswordResetRequested.Value
                             : new DateTime(1900, 1, 1)),
                        DateTime.Now.AddHours(DirtyGirlServiceConfig.Settings.RequestValidForHours * -1)) > 0)
                {
                    SendPasswordResetEmail(user);
                }
                else
                {
                    var byteArray = new byte[128];
                    // Create new Cryptography object
                    using (var crypto = new RNGCryptoServiceProvider())
                    {
                        crypto.GetNonZeroBytes(byteArray);

                        user.PasswordResetToken = Convert.ToBase64String(byteArray);
                        user.PasswordResetToken = user.PasswordResetToken.Replace("+", "");
                        user.PasswordResetRequested = DateTime.Now;
                        _repository.SaveChanges();

                        SendPasswordResetEmail(user);
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }
            return result;
        }

        private static void SendPasswordResetEmail(User user)
        {
            IEmailService emailService = new EmailService();
            emailService.SendPasswordResetRequestEmail(user.UserId);
        }

        static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            var plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        public User GetUserByPasswordResetToken(string resetToken)
        {
            var user = _repository.Users.Find(x => x.PasswordResetToken == resetToken);
            if (user != null &&
                DateTime.Compare(
                    (user.PasswordResetRequested.HasValue
                         ? user.PasswordResetRequested.Value
                         : new DateTime(1900, 1, 1)),
                    DateTime.Now.AddHours(DirtyGirlServiceConfig.Settings.RequestValidForHours*-1)) > 0)
            {
                return user;
            }
            return null;
        }

        public bool IsValidPasswordResetToken(string resetToken)
        {
            return (_repository.Users.Find(x => x.PasswordResetToken == resetToken) != null);
        }

        public decimal GetRegistrationValue(int registrationId)
        {
            IRegistrationService regService = new RegistrationService(this._repository, false);
            return regService.GetRegistrationPathValue(registrationId);
        }

        #endregion

        #region private methods
        
        private string GenerationEmailConfirmationCode()
        {
            string confirmationCode = Path.GetRandomFileName().ToUpper().Replace(".", "");
            while (_repository.Users.Filter(u => u.EmailVerificationCode == confirmationCode).Any())
            {
                confirmationCode = Path.GetRandomFileName().ToUpper().Replace(".", "");
            }

            return confirmationCode;
        }

        #endregion

    }
}
