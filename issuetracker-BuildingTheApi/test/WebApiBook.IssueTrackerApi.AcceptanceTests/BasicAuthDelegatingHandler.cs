using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public class BasicAuthDelegatingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string username = "Ivan";
            string password = "Test";

            string svcCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", "Basic " + svcCredentials);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
