using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
using Moq;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public abstract class IssuesFeature
    {
        public Mock<ITraceWriter> MockTracer;
        public Mock<IIssueStore> MockIssueStore;
        public HttpResponseMessage Response;
        public IssueLinkFactory IssueLinks;
        public IssueStateFactory StateFactory;
        public IEnumerable<Issue> FakeIssues;
        public HttpRequestMessage Request { get; private set; }
        public HttpClient Client;
        public HttpConfiguration Configuration;

        protected IssuesFeature()
        {           
            MockIssueStore = new Mock<IIssueStore>();
            Request = new HttpRequestMessage();
            Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.issue+json"));
            IssueLinks = new IssueLinkFactory(Request);
            StateFactory = new IssueStateFactory(IssueLinks);
            FakeIssues = GetFakeIssues();
            Configuration = new HttpConfiguration();
            MockTracer = new Mock<ITraceWriter>(MockBehavior.Loose);
            IssueTrackerApi.WebApiConfiguration.Configure(Configuration, MockIssueStore.Object);
            Configuration.Services.Replace(typeof(ITraceWriter), MockTracer.Object);
            var server = new HttpServer(Configuration);

            Client = HttpClientFactory.Create(new BasicAuthDelegatingHandler(), server);
        }

        private IEnumerable<Issue> GetFakeIssues()
        {
            return new List<Issue>
            {
                new Issue
                {
                    Id = "1",
                    Title = "An issue",
                    Description = "This is an issue", 
                    Status = IssueStatus.Open,
                    LastModified = new DateTimeOffset(new DateTime(2013, 9, 4))
                },
                new Issue
                {
                    Id = "2",
                    Title = "Another issue",
                    Description = "This is another issue",
                    Status = IssueStatus.Closed,
                    LastModified = new DateTimeOffset(new DateTime(2013, 9, 4))
                },
            };
        }
    }
}