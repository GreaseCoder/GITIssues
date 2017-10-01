using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GITIssues.Models_Alternative
//namespace GITIssues.Models
{
    public class GitIssue
    {
        public string Title;
        public string Body;
        public string Submitter;
        public string Assignee;
    }

    /// <summary>
    /// This implementation of IssueCollector uses Octokit instead of parsing the API manually.
    /// </summary>
    public class IssueCollector
    {
        private GitHubClient client = new GitHubClient(new ProductHeaderValue("WesHasIssues"));
        private SearchIssuesResult searchResults;

        public DateTime LastUpdate;

        public List<GitIssue> Issues
        {
            get
            {
                if (staleData || searchResults == null)
                {
                    RequestAsync().Wait();
                }

                return GenerateIssues().ToList();
            }
        }

        private bool staleData
        {
            get { return (LastUpdate < DateTime.Now.AddMinutes(-5)); }
        }


        public IssueCollector()
        {
        }

        private IEnumerable<GitIssue> GenerateIssues()
        {
            foreach(Octokit.Issue issue in searchResults.Items)
            {
                yield return new GitIssue()
                {
                    Title = issue.Title ?? "Could not retrieve title",
                    Body = issue.Body ?? "Could not retreive body",
                    Submitter = issue.User == null ? "Unknown" : issue.User.Login ?? "Unknown",
                    Assignee = issue.Assignee == null ? "Anonymous" : issue.Assignee.Login ?? "Anonymous"
                };
            }
        }

        private async Task RequestAsync()
        {
            SearchIssuesRequest request = new SearchIssuesRequest();
            request.Repos.Add("angular/angular");
            request.Created = new DateRange(DateTime.Now.Subtract(TimeSpan.FromDays(7)), SearchQualifierOperator.GreaterThan);

            request.SortField = IssueSearchSort.Created;
            request.Order = SortDirection.Descending;

            searchResults = await client.Search.SearchIssues(request);
            LastUpdate = DateTime.Now;
        }
    }
}
