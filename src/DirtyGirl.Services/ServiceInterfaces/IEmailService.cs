using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Services.ServiceInterfaces
{
    public interface IEmailService
    {
        bool SendRegistrationConfirmationEmail(int userId);
        bool SendPaymentConfirmationEmail(int CartId);
        bool SendTransferEmail(int discountItemId, string toName, string email);        
        bool SendCancellationEmail(int discountItemId);
        bool SendTeamShareEmail(string[] toAddresses, string subject, string messageBody);
        string GetShareBodyText(Team team, EventWave eventWave, User user, EventDate eventDate,
                                bool includeJavascriptLineBreaks);
        bool SendPasswordResetRequestEmail(int userId);
    }
}
