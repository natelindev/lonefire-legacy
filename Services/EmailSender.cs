using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lonefire.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            //TODO: Add Email sending functionality for verifing email and reseting password.
            return Task.CompletedTask;
        }
    }
}
