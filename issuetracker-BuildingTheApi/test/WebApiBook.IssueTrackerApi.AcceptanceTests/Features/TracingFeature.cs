using Moq;
using Should;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Tracing;
using WebApiBook.IssueTrackerApi.Controllers;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class TracingFeature : IssuesFeature
    {
        private Uri _uriIssue1 = new Uri("http://localhost/issue/1");

        [Scenario]
        public void RetrievingAnIssue()
        {
            IssueState issue = null;
            var fakeIssue = FakeIssues.FirstOrDefault();
            "Given an existing or new issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                });
            "When a request is made".
                f(() =>
                {
                    Request.RequestUri = _uriIssue1;
                    Response = Client.SendAsync(Request).Result;
                    issue = Response.Content.ReadAsAsync<IssueState>().Result;
                 });
            "When the diagnostics tracing is enabled".
                f(() =>
                {
                    Configuration.Services.GetService(typeof(ITraceWriter)).ShouldNotBeNull();
                });
            "Then the diagnostics tracing information is generated".
                f(() =>
                {
                    MockTracer.Verify(m => m.Trace(It.IsAny<HttpRequestMessage>(),typeof(IssueController).FullName, TraceLevel.Info, It.IsAny<Action<TraceRecord>>()));
                });
        }
    }
}
