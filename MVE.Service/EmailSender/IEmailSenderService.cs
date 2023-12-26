using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Service
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string ToEmails, string subject, string message, string ccEmails = null, string bccEmails = null, string from = null, string fromDisplayName = null, string[] attachment = null, string replyToEmails = null);
    }
}
