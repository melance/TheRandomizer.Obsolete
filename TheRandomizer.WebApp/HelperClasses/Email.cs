using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheRandomizer.WebApp.HelperClasses
{
    public class Email
    {
        public static dynamic Send(string from, string to, string subject, string body)
        {
            string apiKey = HelperClasses.Settings.GetSetting<string>("sendGridApiKey");
            var toAddress = new EmailAddress(to);
            EmailAddress fromAddress;
            SendGridClient client = new SendGridClient(apiKey);
            if (string.IsNullOrWhiteSpace(from))
            {
                fromAddress = new EmailAddress(Settings.GetSetting<string>("fromAddress"));
            }
            else
            {
                fromAddress = new EmailAddress(from);
            }
            var mail = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject, body, body);
            dynamic response = client.SendEmailAsync(mail);
            return response;
        }
    }
}