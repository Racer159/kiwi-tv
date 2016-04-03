using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;

namespace Kiwi_TV.Helpers
{
    class MailHelper
    {
        public static async void SendFeedbackEmail(string Email, string Type, string Message)
        {
            EmailMessage emailMessage = new EmailMessage();
            string messageBody = "From: " + Email + "\n\nType: " + Type + "\n\n" + Message;
            emailMessage.Body = messageBody;
            emailMessage.Subject = "[" + Type + "] Kiwi TV"; 
            
            var emailRecipient = new EmailRecipient("racer159@outloook.com");
            emailMessage.To.Add(emailRecipient);

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }
    }
}
