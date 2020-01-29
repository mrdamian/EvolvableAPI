using System.Net.Http;
using System.Web.Http;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public abstract class HomeFeature
    {
        public HttpClient Client;
        public HttpRequestMessage Request;
        public HttpResponseMessage Response;

        protected HomeFeature()
        {
            Request = new HttpRequestMessage();

            var config = new HttpConfiguration();
            IssueTrackerApi.WebApiConfiguration.Configure(config);
            var server = new HttpServer(config);
            Client = HttpClientFactory.Create(new BasicAuthDelegatingHandler(), server);
        }
    }
}
