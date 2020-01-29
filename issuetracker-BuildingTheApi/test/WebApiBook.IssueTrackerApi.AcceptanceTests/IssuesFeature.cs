﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Moq;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApp.AcceptanceTests
{
    public abstract class IssuesFeature
    {
        private class BasicInfoMessageHandler : DelegatingHandler
        {
            public BasicInfoMessageHandler()
            {
            }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                string username = "Ivan";
                string password = "Test";

                string svcCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
                request.Headers.Add("Authorization", "Basic " + svcCredentials);
                return base.SendAsync(request, cancellationToken);
            }
        }

        public Mock<IIssueStore> MockIssueStore;
        public HttpResponseMessage Response;
        public IssueLinkFactory IssueLinks;
        public IssueStateFactory StateFactory;
        public IEnumerable<Issue> FakeIssues;
        public HttpRequestMessage Request { get; private set; }
        public HttpClient Client;

        protected IssuesFeature()
        {           
            MockIssueStore = new Mock<IIssueStore>();
            Request = new HttpRequestMessage();
            Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.issue+json"));
            IssueLinks = new IssueLinkFactory(Request);
            StateFactory = new IssueStateFactory(IssueLinks);
            FakeIssues = GetFakeIssues();
            var config = new HttpConfiguration();
            IssueTrackerApi.WebApiConfiguration.Configure(config, MockIssueStore.Object);
            var server = new HttpServer(config);

            Client = HttpClientFactory.Create(new BasicInfoMessageHandler(), server);
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