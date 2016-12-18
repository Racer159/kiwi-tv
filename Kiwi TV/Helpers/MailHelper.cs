using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Kiwi_TV.Helpers
{
    /// <summary>
    /// A helper to handle authorization with MailGun
    /// </summary>
    public class PlugInFilter : IHttpFilter
    {
        private IHttpFilter innerFilter;

        public PlugInFilter(IHttpFilter innerFilter)
        {
            if (innerFilter == null)
            {
                throw new ArgumentException("innerFilter cannot be null.");
            }
            this.innerFilter = innerFilter;
        }

        public IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> SendRequestAsync(HttpRequestMessage request)
        {
            return AsyncInfo.Run<HttpResponseMessage, HttpProgress>(async (cancellationToken, progress) =>
            {
                request.Headers.Add("Authorization", "Basic YXBpOmtleS01NzhlNDM1YjIwYWM3MTc1YTlmODZhY2M5MGNhMzRlOQ==");
                HttpResponseMessage response = await innerFilter.SendRequestAsync(request).AsTask(cancellationToken, progress);

                cancellationToken.ThrowIfCancellationRequested();
                
                return response;
            });
        }

        public void Dispose()
        {
            innerFilter.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// A helper to send out feedback emails
    /// </summary>
    class MailHelper
    {
        /* Sends out a feedback email to the given email with the given message */
        public static async Task<object> SendFeedbackEmail(string Email, string Type, string Message)
        {
            try {
                IHttpFilter filter = new HttpBaseProtocolFilter();
                filter = new PlugInFilter(filter);
                HttpClient httpClient = new HttpClient(filter);
                Uri mailgun = new Uri("https://api.mailgun.net/v3/sandbox70096dc0d22a4d0bb787da751806054c.mailgun.org/messages");
                HttpMultipartFormDataContent form = new HttpMultipartFormDataContent();
                form.Add(new HttpStringContent("Mailgun Sandbox <postmaster@sandbox70096dc0d22a4d0bb787da751806054c.mailgun.org>"), "from");
                form.Add(new HttpStringContent("Wes <racer159@outlook.com>"), "to");
                form.Add(new HttpStringContent("[" + Type + "] Kiwi TV"), "subject");
                form.Add(new HttpStringContent("From: " + Email + "\n\nType: " + Type + "\n\n" + Message), "text");
                HttpResponseMessage response = await httpClient.PostAsync(mailgun, form);

                return response;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}
