using Should;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class OutputCaching : IssuesFeature
    {
        private Uri _uriIssues = new Uri("http://localhost/issue");

        [Scenario]
        public void RetrievingAllIssues()
        {
            IssuesState issuesState = null;
            "Given existing issues".
              f(() =>
              {
                  MockIssueStore.Setup(i => i.FindAsync()).Returns(Task.FromResult(FakeIssues));
              });
            "When all issues are retrieved".
              f(() =>
              {
                  Request.RequestUri = _uriIssues;
                  Response = Client.SendAsync(Request).Result;
                  issuesState = Response.Content.ReadAsAsync<IssuesState>().Result;
              });
            "Then a CacheControl header is returned".
              f(() =>
              {
                  Response.Headers.CacheControl.Public
                  .ShouldBeTrue(); // <1>
                  Response.Headers.CacheControl.MaxAge
                  .ShouldEqual(TimeSpan.FromMinutes(5)); // <2>
              });
            "Then a '200 OK' status is returned".
              f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK));
            "Then they are returned".
              f(() =>
              {
                  issuesState.Issues.FirstOrDefault(i => i.Id == "1").ShouldNotBeNull();
                  issuesState.Issues.FirstOrDefault(i => i.Id == "2").ShouldNotBeNull();
              });
        }
    }
}
