using DirtyGirl.Models;

namespace DirtyGirl.Web.Models
{
    public class vmPasswordReset
    {
        public User User { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string ResetToken { get; set; }

    }
}