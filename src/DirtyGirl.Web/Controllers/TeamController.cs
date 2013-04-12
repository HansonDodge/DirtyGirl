using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using DirtyGirl.Data.RepositoryGroups;
using DirtyGirl.Models;
using DirtyGirl.Services;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Models;
using System.Web.Mvc;
using DirtyGirl.Web.Utils;
using WebGrease.Css.Extensions;

namespace DirtyGirl.Web.Controllers
{
    [Authorize(Roles = "Registrant, Admin, SuperUser")]
    public class TeamController : BaseController        
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public ActionResult ViewTeam(int teamId)
        {
            var team = _teamService.GetTeamById(teamId);
            var emailService = new EmailService();
            var teamDetails = new vmTeam_Details();
            if (team != null)
            {
                teamDetails = new vmTeam_Details
                                      {
                                          Team = team,
                                          RegistrationList = team.Registrations,
                                          Event = team.Event,
                                          EventWave = GetRegistrationsByEventWave(team.Registrations),
                                          IsTeamMember = (CurrentUser != null && team.Registrations.FirstOrDefault(x => x.UserId == CurrentUser.UserId) != null ? true : false),
                                          User = CurrentUser
                                      };
                var eventDate = team.Event.EventDates.OrderBy(x => x.DateOfEvent).FirstOrDefault() ?? new EventDate();
                var myEventWave = teamDetails.RegistrationList.FirstOrDefault(x => x.User.UserName == User.Identity.Name) ?? new Registration();
                if (myEventWave.EventWave != null && CurrentUser != null)
                {
                    ViewBag.ShareBody = emailService.GetShareBodyText(team, myEventWave.EventWave, CurrentUser, eventDate, false);
                }

            }
            return View(teamDetails);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize]
        public ActionResult CreateTeamPost(int teamId, string post)
        {
            ITeamService teamService = new TeamService(new RepositoryGroup(), false);
            var team = _teamService.GetTeamById(teamId);
            if (team == null)
            {
                Response.StatusCode = 403;
                return Content("Invalid TeamId Submitted");
            }
            if (CurrentUser != null && team.Registrations.FirstOrDefault(x => x.UserId == CurrentUser.UserId) != null)
            {
                var teamPost = new TeamPost {TeamId = teamId, Post = post, UserId = CurrentUser.UserId};
                var dbResult = teamService.CreateTeamPost(teamPost);
                if (dbResult.Success)
                {
                    //success
                    var teamRefresh = _teamService.GetTeamById(teamId);

                    var entity = teamRefresh.TeamPosts.OrderBy(x => x.DateAdded).ToList();

                    return PartialView("Partial/MessageBoardPosts", entity);
                }
                Response.StatusCode = 500;
                return Content("Invalid TeamId Submitted");
            }
            Response.StatusCode = 403;
            return Content("Only members of a team may post messages.");
        }

        [HttpGet]
        public ActionResult TeamRegistration(int eventId)
        {
            return View();
        }

        public ActionResult CreateTeam(int eventId, int registrationId, int teamId)
        {
            var team = (teamId >= 0) ? _teamService.GetTeamById(teamId) : null;

            vmTeam_Create vm = new vmTeam_Create { 
                EventId = eventId,
                RegistrationId = registrationId,
                ReturnUrl = Request.UrlReferrer.AbsolutePath,
                CurrentTeam = team
            }; 
            
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public JsonResult CreateTeam(vmTeam_Create teamCreate)
        {
            string errors = string.Empty;
            if (ModelState.IsValid)
            {
                if (!_teamService.CheckTeamNameForDirtyWords(teamCreate.TeamName))
                {
                    return Json("The requested team name contains a naughty word.");
                }

                if (_teamService.CheckTeamNameAvailability(teamCreate.EventId, teamCreate.TeamName))
                {
                    var newTeam = new Team { EventId = teamCreate.EventId, Name = teamCreate.TeamName, CreatorID = CurrentUser.UserId};
                    ServiceResult result = _teamService.CreateTeam(newTeam, teamCreate.RegistrationId);
                    if (result.Success)
                    {
                        return Json(new { redirect = Url.Action("ViewTeam", "Team", new { teamId = newTeam.TeamId}) });
                    }
                    result.GetServiceErrors().ForEach(x => errors += x.ErrorMessage + "<br/>");
                    errors += string.Format("\nEventID: {0}, name: {1}, regid: {2}\n", teamCreate.EventId,
                                            teamCreate.TeamName, teamCreate.RegistrationId);
                    return Json(errors);
                }
                return Json("The requested team name is already in use for this event. Please select a different team name.");
            }
            ModelState.Values.ForEach(x => errors += x.Errors + "<br/>");
            return Json(errors);
        }

        [HttpPost]
        public JsonResult JoinTeam(int registrationId, string teamCode, int eventId)
        {
            string errors = string.Empty;
            // Get Team for the team Code
            var team = _teamService.GetTeamByCode(eventId, teamCode);

            // Get the current registration for the user
            IRegistrationService regService = new RegistrationService(new RepositoryGroup(), false);
            var registration = regService.GetRegistrationById(registrationId);

            if (team != null && registration != null)
            {
                if (team.EventId != registration.EventWave.EventDate.EventId)
                    return Json(string.Format("The code entered is not associated with a team for this event."));
                registration.TeamId = team.TeamId;
                var result = regService.Save();
                if (result.Success)
                {
                    return Json(new {redirect = Url.Action("ViewTeam", "Team", new {teamId = team.TeamId})});
                }
                result.GetServiceErrors().ForEach(x => errors += x.ErrorMessage + "<br/>");
                return Json(errors);
            }
            return Json("The code entered is not associated with a team for this event.");
        }

        private IEnumerable<EventWave> GetRegistrationsByEventWave(IEnumerable<Registration> registrations)
        {
            var eventWaves = new List<EventWave>();

            foreach (Registration registration in registrations)
            {
                if (eventWaves.FirstOrDefault(y => y.EventWaveId == registration.EventWaveId) == null)
                {
                    registration.EventWave.Registrations.Clear();
                    eventWaves.Add(registration.EventWave);
                }
                var firstOrDefault = eventWaves.FirstOrDefault(x => x.EventWaveId == registration.EventWaveId);
                if (firstOrDefault != null)
                    firstOrDefault.Registrations.Add(registration);
            }
            return eventWaves;
        }

        #region Ajax Functions

        [HttpPost]
        [Authorize]
        public JsonResult VerifyTeamNameAvailability(vmTeam_Create createTeam)
        {
            if (!_teamService.CheckTeamNameForDirtyWords(createTeam.TeamName))
            {
                return Json("The requested team name contains a naughty word.");
            }

            return Json(!_teamService.CheckTeamNameAvailability(createTeam.EventId, createTeam.TeamName) ? "The requested team name is already in use for this event. Please select a different team name." : string.Empty);
        }

        [HttpPost]
        [Authorize]
        public JsonResult LeaveTeam(int regId, int teamId)
        {
            IRegistrationService regService = new RegistrationService(new RepositoryGroup(), false);
            Registration registration = regService.GetRegistrationById(regId);            
            registration.TeamId = null;
            var verify = regService.UpdateRegistration(registration); 

            return Json("sucess");
        }

        #endregion
    }
}
