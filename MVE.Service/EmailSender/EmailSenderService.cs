using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MVE.Core.Code.LIBS;
using Microsoft.Extensions.Configuration;

namespace MVE.Service
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;
        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string ToEmails, string subject, string message, string ccEmails = null, string bccEmails = null, string from = null, string fromDisplayName = null, string[] attachment = null, string replyToEmails = null)
        {
            if (string.IsNullOrWhiteSpace(ToEmails))
            {
                throw new ArgumentNullException("ToEmails");
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException("subject");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException("message");
            }

            try
            {
                using (var client = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = SiteKeys.SMTPUserName, // configuration["Email:SmtpMail"],
                        Password = SiteKeys.SMTPUserPassword //configuration["Email:SmtpPassword"]
                    };

                    if (!SiteKeys.SmtpServer.Contains("localhost"))
                    {
                        client.Credentials = credential;
                    }

                    client.Host = SiteKeys.SmtpServer; //configuration["Email:SmtpHost"];
                    client.Port = int.Parse(SiteKeys.SmtpPort);//int.Parse(configuration["Email:SmtpPort"]);
                    client.EnableSsl = SiteKeys.SmtpSsl.ToLower() == "true" ? true : false;

                    using (var emailMessage = new MailMessage())
                    {
                        emailMessage.IsBodyHtml = true;

                        if (from == null)
                        {
                            //emailMessage.From = new MailAddress(configuration["Email:SmtpMail"], configuration["Email:DisplayName"]);
                            emailMessage.From = new MailAddress(SiteKeys.DefaultFromEmail, SiteKeys.DefaultFromName);
                        }
                        else
                        {
                            emailMessage.From = new MailAddress(from, fromDisplayName);
                        }

                        //---Set recipients in To List 
                        var _ToList = ToEmails.Replace(";", ",");
                        if (_ToList != "")
                        {
                            string[] arr = _ToList.Split(',');
                            emailMessage.To.Clear();
                            if (arr.Length > 0)
                            {
                                foreach (string address in arr)
                                {
                                    if (address != "")
                                    {
                                        emailMessage.To.Add(new MailAddress(address));
                                    }
                                }
                            }
                            else
                            {
                                emailMessage.To.Add(new MailAddress(_ToList));
                            }
                        }

                        //---Set recipients in CC List 
                        if (!string.IsNullOrWhiteSpace(ccEmails))
                        {
                            var _CCList = ccEmails.Replace(";", ",");
                            if (_CCList != "")
                            {
                                string[] arr = _CCList.Split(',');
                                emailMessage.CC.Clear();
                                if (arr.Length > 0)
                                {
                                    foreach (string address in arr)
                                    {
                                        if (address != "")
                                        {
                                            emailMessage.CC.Add(new MailAddress(address));
                                        }
                                    }
                                }
                                else
                                {
                                    emailMessage.CC.Add(new MailAddress(_CCList));
                                }
                            }
                        }

                        //---Set recipients in BCC List 
                        if (!string.IsNullOrWhiteSpace(bccEmails))
                        {
                            var _BCCList = bccEmails.Replace(";", ",");
                            if (_BCCList != "")
                            {
                                string[] arr = _BCCList.Split(',');
                                emailMessage.Bcc.Clear();
                                if (arr.Length > 0)
                                {
                                    foreach (string address in arr)
                                    {
                                        if (address != "")
                                        {
                                            emailMessage.Bcc.Add(new MailAddress(address));
                                        }
                                    }
                                }
                                else
                                {
                                    emailMessage.Bcc.Add(new MailAddress(_BCCList));
                                }
                            }
                        }

                        //---Set recipients in ReplyTo List 
                        if (!string.IsNullOrWhiteSpace(replyToEmails))
                        {
                            var _ReplyToList = replyToEmails.Replace(";", ",");
                            if (_ReplyToList != "")
                            {
                                string[] arr = _ReplyToList.Split(',');
                                emailMessage.ReplyToList.Clear();
                                if (arr.Length > 0)
                                {
                                    foreach (string address in arr)
                                    {
                                        if (address != "")
                                        {
                                            emailMessage.ReplyToList.Add(new MailAddress(address));
                                        }
                                    }
                                }
                                else
                                {
                                    emailMessage.ReplyToList.Add(new MailAddress(_ReplyToList));
                                }
                            }
                        }

                        // set attachment 
                        if (attachment != null)
                        {
                            for (int i = 0; i < attachment.Length; i++)
                            {
                                if (attachment[i] != null)
                                    emailMessage.Attachments.Add(new Attachment(attachment[i]));
                            }

                        }


                        emailMessage.Subject = subject;
                        emailMessage.Body = message;
                        client.Timeout = 2000000;
                        if (!SiteKeys.IsLive)
                        {
                          await client.SendMailAsync(emailMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            await  Task.CompletedTask;            
        }
    }
}
