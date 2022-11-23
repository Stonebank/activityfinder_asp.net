using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace activityfinder_asp.net.Security
{
    public class Email
    {

        public static void Send(string recipent, string subject, string body)
        {
            var fromAddress = new MailAddress("activityjava@gmail.com", "ActivityFinder .net");
            var toAddress = new MailAddress(recipent, recipent);
            const string fromPassword = "vdvrdvofdiewbgmk";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

    }
}
