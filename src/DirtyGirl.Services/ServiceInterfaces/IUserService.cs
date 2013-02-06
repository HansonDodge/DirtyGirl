using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DirtyGirl.Models;

namespace DirtyGirl.Services.ServiceInterfaces
{
    public interface IUserService
    {
        IList<User> GetAllUsers();
        
        IList<Region> GetRegionsForCountry(int countryId);

        IList<Registration> GetActiveRegistrations(int userId);

        IList<RedemptionCode> GetActiveRedemptionCodes(int userId);
        
        
        User GetUserById(int userId);

        User GetUserByUsername(string userName);

        User GetUserByFacebookId(int facebookId);


        ServiceResult CreateUser(User u);

        ServiceResult UpdateUser(User u, bool setIsActive);

        ServiceResult RemoveUser(User u);

        ServiceResult UpdatePassword(int userId, string password);   
        
     

        bool CheckUsernameAvailability(string username);        

        bool SendEmailConfirmation(User user);

        bool SendEmailConfirmation(string userName);

        bool SendEmailConfirmation(int userId);
        
        int? ConfirmAccount(string confirmationCode);

        ServiceResult GeneratePasswordResetRequest(string emailAddress);

        User GetUserByPasswordResetToken(string resetToken);
        decimal GetRegistrationValue(int registrationId);

        
        bool IsValidPasswordResetToken(string resetToken);
    }
}
