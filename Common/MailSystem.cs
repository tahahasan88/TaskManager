using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Common
{
    public class MailSystem
    {

        public static string RequestFollowUpEmailTemplate = "test request followUp email template";
        public static string FollowUpResponseEmailTemplate = "test response followUp email template";

        public static async Task<bool> SendEmail(string emailBody, List<string> recipients)
        {
            bool isEmailSent = false;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("mail.trade-monsters.com", 587);
                //SmtpClient smtpServer = new SmtpClient("webhosting2014.is.cc", 465);


                mail.From = new MailAddress("lordoffriends@trade-monsters.com");
                foreach (string emailAddress in recipients)
                {
                    mail.To.Add(emailAddress);
                }
                mail.Subject = "Bot email at day end..";
                mail.Body = emailBody;
                mail.IsBodyHtml = true;

                //smtpServer.Port = 465;
                smtpServer.UseDefaultCredentials = false;
                smtpServer.EnableSsl = true;
                smtpServer.Credentials = new System.Net.NetworkCredential("lordoffriends@trade-monsters.com", "spidey241");

                smtpServer.Send(mail);
                isEmailSent = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                isEmailSent = false;
            }

            return await Task.FromResult(isEmailSent);
        }
    }
}
