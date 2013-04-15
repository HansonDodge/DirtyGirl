using System.Collections.Generic;
using System.Web.Hosting;
using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Data.RepositoryGroups;
using DirtyGirl.Models;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Services.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DirtyGirl.Services
{

    public class TeamService:ServiceBase, ITeamService
    {
        
        public TeamService(IRepositoryGroup repository):base (repository, false){}
        public TeamService(IRepositoryGroup repository, bool isShared) : base(repository, isShared) { }

        public ServiceResult CreateTeam(Team team, int registrationId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                team.Name = team.Name.Trim();
                team.DateAdded = DateTime.Now;
                if (ValidateTeam(team, result))
                {
                    team.Code = GenerateTeamCode(team.EventId);
                    _repository.Teams.Create(team);
                    
                    var reg = _repository.Registrations.Find(x => x.RegistrationId == registrationId);
                    reg.Team = team;
                    _repository.Registrations.Update(reg);

                    if (!_sharedRepository)
                        _repository.SaveChanges();

                }
            }
            catch (Exception ex)
            { result.AddServiceError(ex.Message); }
            return result;
        }

        public ServiceResult ChangeTeamName(int teamId, string newTeamName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var team = this.GetTeamById(teamId);
                team.Name = newTeamName;
                _repository.Teams.Update(team); 
                _repository.SaveChanges();
            }

            catch (Exception ex)
            { result.AddServiceError(ex.Message); }
            return result;
        }

        public Team GetTeamById(int teamId)
        {
            return _repository.Teams.Filter(t => t.TeamId == teamId).FirstOrDefault();
        }

        public Team GetTeamByCode(int eventId, string code)
        {
            return _repository.Teams.Find(x => x.EventId == eventId && x.Code.ToLower() == code.ToLower());
        }

        public ServiceResult CreateTeamPost(TeamPost teamPost)
        {
            var result = new ServiceResult();
            if(teamPost == null || teamPost.TeamId <= 0 || teamPost.UserId <= 0)
            {
                throw new ArgumentException("TeamPost must contain a valid TeamId and User Id", "teamPost");
            }

            try
            {
                var team = this.GetTeamById(teamPost.TeamId);
                teamPost.DateAdded = DateTime.Now;
                _repository.TeamPosts.Create(teamPost);
                _repository.SaveChanges();
            }
            catch (Exception ex)
            { result.AddServiceError(Utilities.GetInnerMostException(ex)); }
            return result;
        }

        private Team GetTeamByUser(int eventId, int userId)
        {
            return null;
        }

        public bool CheckTeamNameAvailability(int eventId, string teamName)
        {
            if (_repository.Teams.Filter(t => t.Name.ToLower().Equals(teamName.Trim().ToLower()) && t.EventId == eventId).Any())
                return false;

            return true;
        }

        public bool CheckTeamNameForDirtyWords(string teamName)
        {
            var badWords = _repository.DirtyWord.All().Select(x => x.Word).ToList();
            return badWords.Count(badWord => teamName.ToLower().Contains(badWord.ToLower())) == 0;
        }

        protected bool ValidateTeam(Team teamToValidate, ServiceResult serviceResult)
        {
            if (_repository.Teams.Filter(t => t.EventId == teamToValidate.EventId && t.Name.Equals(teamToValidate.Name)).Any())
            {
                serviceResult.AddServiceError("Name", "A team with that name already exists for this event.");
            }
            return serviceResult.Success;
        }

        public string GenerateTeamCode(int eventId)
        {
            string code = Path.GetRandomFileName().ToUpper().Replace(".", "").Substring(0, 5);
            //Codes must be unique for the team/event combination (teams on different events can have same key)
            while (_repository.Teams.Filter(t => t.Code == code && t.EventId == eventId).Any())
            {
                code = Path.GetRandomFileName().ToUpper().Replace(".", "").Substring(0, 5);
            }

            return code;
        }

        


    }
}
