﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
using Newtonsoft.Json.Linq;
using WebApiBook.IssueTrackerApi.Infrastructure;
using WebApiBook.IssueTrackerApi.Models;

namespace WebApiBook.IssueTrackerApi.Controllers
{
    public class IssueController : ApiController
    {
        private readonly IIssueStore _store;
        private readonly IStateFactory<Issue, IssueState> _stateFactory;
        private readonly IssueLinkFactory _linkFactory;

        public IssueController(IIssueStore store, IStateFactory<Issue, IssueState> stateFactory, IssueLinkFactory linkFactory )
        {
            _store = store;
            _stateFactory = stateFactory;
            _linkFactory = linkFactory;
        }

        public async Task<HttpResponseMessage> Get()
        {
            var issues = await _store.FindAsync();
            var issuesState = new IssuesState();
            issuesState.Issues = issues.Select(i => _stateFactory.Create(i));
            issuesState.Links.Add(new Link { Href = Request.RequestUri, Rel = LinkFactory.Rels.Self });
            var response = Request.CreateResponse(HttpStatusCode.OK, issuesState);
            response.Headers.CacheControl = new CacheControlHeaderValue();
            response.Headers.CacheControl.Public = true; // <1>
            response.Headers.CacheControl.MaxAge = TimeSpan.FromMinutes(5); // <2>
            return response;
        }

        public async Task<HttpResponseMessage> Get(string id)
        {
            var issue = await _store.FindAsync(id);

            var tracer = this.Configuration.Services.GetTraceWriter(); // <1>
            tracer.Trace(Request, typeof(IssueController).FullName, TraceLevel.Info,
                (traceRecord) =>
                {
                    traceRecord.Message = string.Format("Fetching issue with id {0}", id);
                });
            

            if (issue == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            HttpResponseMessage response;

            if (Request.Headers.IfModifiedSince.HasValue && Request.Headers.IfModifiedSince == issue.LastModified) // <1>
            {
                response = Request
                .CreateResponse(HttpStatusCode.NotModified); // <2>
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(issue));
                response.Content.Headers.LastModified = issue.LastModified;
            }

            response.Headers.CacheControl = new CacheControlHeaderValue(); // <3>
            response.Headers.CacheControl.Public = true;
            response.Headers.CacheControl.MaxAge = TimeSpan.FromMinutes(5);
            return response;
        }

        public async Task<HttpResponseMessage> GetSearch(string searchText)
        {
            var issues = await _store.FindAsyncQuery(searchText);
            var issuesState = new IssuesState();
            issuesState.Issues = issues.Select(i => _stateFactory.Create(i));
            issuesState.Links.Add(new Link { Href = Request.RequestUri, Rel = LinkFactory.Rels.Self });
            return Request.CreateResponse(HttpStatusCode.OK, issuesState);
        }

        public async Task<HttpResponseMessage> Post(dynamic newIssue)
        {
            var issue = new Issue { Title = newIssue.title, Description = newIssue.description };
            await _store.CreateAsync(issue);
            var response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = _linkFactory.Self(issue.Id).Href;
            return response;
        }

        public async Task<HttpResponseMessage> Patch(string id, dynamic issueUpdate)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            if (!Request.Headers.IfModifiedSince.HasValue)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Missing IfModifiedSince header");
            }

            if (Request.Headers.IfModifiedSince != issue.LastModified)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }

            foreach (JProperty prop in issueUpdate)
            {
                if (prop.Name == "title")
                    issue.Title = prop.Value.ToObject<string>();
                else if (prop.Name == "description")
                    issue.Description = prop.Value.ToObject<string>();
            }
            await _store.UpdateAsync(id, issue);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
      
        public async Task<HttpResponseMessage> Delete(string id)
        {
            var issue = await _store.FindAsync(id);
            if (issue == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);
            await _store.DeleteAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}