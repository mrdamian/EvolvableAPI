using Moq;
using Newtonsoft.Json.Linq;
using Should;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class ConflictDetection : IssuesFeature
    {
        private Uri _uriIssue1 = new Uri("http://localhost/issue/1");

        [Scenario]
        public void UpdatingAnIssueWithNoConflict()
        {
            var fakeIssue = FakeIssues.FirstOrDefault();
            "Given an existing issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                    MockIssueStore.Setup(i => i.UpdateAsync("1", It.IsAny<Issue>())).Returns(Task.FromResult(""));
                });
            "When a PATCH request is made with IfModifiedSince".
                f(() =>
                {
                    dynamic issue = new JObject();
                    issue.title = "Updated title";
                    issue.description = "Updated description";
                    Request.Method = new HttpMethod("PATCH");
                    Request.RequestUri = _uriIssue1;
                    Request.Content = new ObjectContent<dynamic>(issue, new JsonMediaTypeFormatter());
                    Request.Headers.IfModifiedSince = fakeIssue.LastModified; // <1>
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '200 OK' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.OK)); // <2>
            "Then the issue should be updated".
                f(() => MockIssueStore.Verify(i => i.UpdateAsync("1", It.IsAny<Issue>()))); // <3>
        }

        [Scenario]
        public void UpdatingAnIssueWithConflicts()
        {
            var fakeIssue = FakeIssues.FirstOrDefault();
            "Given an existing issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                });
            "When a PATCH request is made with IfModifiedSince".
                f(() =>
                {
                    var issue = new Issue();
                    issue.Title = "Updated title";
                    issue.Description = "Updated description";
                    Request.Method = new HttpMethod("PATCH");
                    Request.RequestUri = _uriIssue1;
                    Request.Content = new ObjectContent<Issue>(issue, new JsonMediaTypeFormatter());
                    Request.Headers.IfModifiedSince = fakeIssue.LastModified.AddDays(1); // <1>
                    Response = Client.SendAsync(Request).Result;
                });
            "Then a '409 CONFLICT' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.Conflict)); // <2>
            "Then the issue should be not updated".
                f(() => MockIssueStore.Verify(i =>  i.UpdateAsync("1", It.IsAny<Issue>()), Times.Never())); // <3>
        }
    }
}
