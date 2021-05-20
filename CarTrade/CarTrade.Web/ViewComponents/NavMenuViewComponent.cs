using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarTrade.Web.ViewComponents
{
    public class NavMenuViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string pageName)
        {
            var links = GetLinksForPage(pageName);
            return View(links);
        }

        private Dictionary<string, string> GetLinksForPage(string pageName)
        {
            Dictionary<string, string> links = null;
            switch (pageName)
            {
                case "Index":
                    links = new Dictionary<string, string> {
                    { "Link 1", "https://www.google.com" },
                    { "Link 2", "https://www.example.com" },
                };

                    break;

                case "Privacy":
                    links = new Dictionary<string, string> {
                    { "Link 3", "https://www.stackoverflow.com" },
                };

                    break;
            }


            return links;
        }
    }
}
