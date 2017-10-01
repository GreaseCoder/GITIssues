using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GITIssues.Models
{
    /// <summary>
    /// A display container to act as a buffer between JSON deserialization and our view.
    /// </summary>
    public class GitIssue
    {
        [JsonProperty("title")]
        public string Title;

        [JsonProperty("body")]
        public string Body;

        [JsonProperty("user")]
        public Dictionary<string, string> user;

        [JsonProperty("assignee")]
        public Dictionary<string, string> assignee;

        [JsonIgnore]
        public string Assignee
        {
            get { return assignee != null ? assignee["login"] : "Annonymous"; }
        }

        [JsonIgnore]
        public string Submitter
        {
            get { return user != null ? user["login"] : "Unknown"; }
        }
    }

    /// <summary>
    /// Responsible for fetching and containing  our issues.
    /// </summary>
    public class IssueCollector : IDisposable
    {
        public DateTime LastUpdate;
        private HttpClient client;
        private List<GitIssue> issues;

        /// <summary>
        /// List of issues for show.  staleData can serve as a quick and dirty cache mechanism.
        /// </summary>
        public List<GitIssue> Issues
        {
            get
            {
                if (staleData || issues == null)
                {
                    FetchIssues().Wait();
                }

                return issues;
            }
        }

        private bool staleData
        {
            get { return (LastUpdate < DateTime.Now.AddMinutes(-5)); }
        }

        public IssueCollector()
        {
            // Stand up our persisting client.  
            client = new HttpClient() { BaseAddress = new Uri("https://api.github.com/search/issues") };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("WesHasIssues");
        }

        public void Dispose()
        {
            client.Dispose();
        }

        /// <summary>
        /// Gets the Github API response we're interested in.
        /// </summary>
        private async Task FetchIssues()
        {
            // Fabricate our query string using 7 days ago from now.  We'll handle page details later.
            string baseQuery = "?q=repo:angular/angular+type:issue+is:open+created:" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-ddTHH:mm:ssZ") + "..*";
            List<GitIssue> allIssues = new List<GitIssue>();

            LastUpdate = DateTime.Now;
            int itemsPerPage = 100;
            int pageNum = 1;

            // If there are more than 100 issues, we need to be able to continue through each page to pull all of them in.
            List<GitIssue> issueBatch = new List<GitIssue>();
            do
            {
                // Handle page details.
                string query = baseQuery += "&per_page=" + itemsPerPage + "&page=" + pageNum;

                HttpResponseMessage response = await client.GetAsync(query);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    issueBatch = JsonConvert.DeserializeObject<List<GitIssue>>(JObject.Parse(json)["items"].ToString());
                    allIssues.AddRange(issueBatch);
                }
                else
                {
                    Console.WriteLine("Received status code " + response.StatusCode + " from " + response.RequestMessage);
                }

                ++pageNum;
            } while (issueBatch.Count == itemsPerPage && pageNum < 100);    // max at 100 just for safety

            // Copy our new list of issues all in one go.
            issues = allIssues;
        }
    }
}
