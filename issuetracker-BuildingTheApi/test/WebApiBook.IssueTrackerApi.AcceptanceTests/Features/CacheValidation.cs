﻿using Should;
using Should.Core.Assertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiBook.IssueTrackerApi.Models;
using Xbehave;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests.Features
{
    public class CacheValidation : IssuesFeature
    {
        private Uri _uriIssue1 = new Uri("http://localhost/issue/1");
        [Scenario]
        public void RetrievingNonModifiedIssue()
        {
            IssueState issue = null;
            var fakeIssue = FakeIssues.FirstOrDefault();
            "Given an existing issue".
                f(() =>
                {
                    MockIssueStore.Setup(i => i.FindAsync("1")).Returns(Task.FromResult(fakeIssue));
                });
            "When it is retrieved with an IfModifiedSince header".
                f(() =>
                {
                    Request.RequestUri = _uriIssue1;
                    Request.Headers.IfModifiedSince = fakeIssue.LastModified; // <1>
                    Response = Client.SendAsync(Request).Result;
                    issue = Response.Content?.ReadAsAsync<IssueState>().Result;
                });
            "Then a CacheControl header is returned".
                f(() =>
                {
                    Response.Headers.CacheControl.Public.ShouldBeTrue();
                    Response.Headers.CacheControl.MaxAge.ShouldEqual(TimeSpan.FromMinutes(5));
                });
            "Then a '304 NOT MODIFIED' status is returned".
                f(() => Response.StatusCode.ShouldEqual(HttpStatusCode.NotModified)); // <2>
            "Then it is not returned".
             f(() => Assert.Null(issue));
        }
    }
}
