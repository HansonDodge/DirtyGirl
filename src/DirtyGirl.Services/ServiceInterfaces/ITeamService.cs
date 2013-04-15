using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Services.ServiceInterfaces
{
    public interface ITeamService
    {
        ServiceResult CreateTeam(Team team, int registrationId);
        ServiceResult ChangeTeamName(int teamId, string newTeamName);
        ServiceResult CreateTeamPost(TeamPost teamPost);

        Team GetTeamById(int teamId);

        Team GetTeamByCode(int eventId, string code);

        bool CheckTeamNameAvailability(int eventId, string teamName);
        bool CheckTeamNameForDirtyWords(string teamName);

        string GenerateTeamCode(int eventId);
    }
}
