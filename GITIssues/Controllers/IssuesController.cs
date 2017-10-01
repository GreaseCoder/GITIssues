using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using GITIssues.Models;

namespace GITIssues.Controllers
{
    public class IssuesController : Controller
    {
        // Use dependancy injection to persist the collector
        private readonly IssueCollector _issueCollector;

        public IssuesController(IssueCollector issueCollector)
        {
            _issueCollector = issueCollector;
        }

        public IActionResult Index()
        {
            ViewData["Issues"] = _issueCollector.Issues;
            ViewData["RequestTime"] = _issueCollector.LastUpdate.ToString();

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
