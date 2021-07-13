using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using System.IO;

namespace CarTrade.Web.Filters.Action
{
    public class TimeFilterAttribute : ActionFilterAttribute
    {
        private Stopwatch watch;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            this.watch = new Stopwatch();
            this.watch.Start();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            this.watch.Stop();
            var elapseTime = this.watch.Elapsed;
            var controllerInfo = context.Controller.GetType().Name;
            var dateTimeNow = DateTime.UtcNow.ToString();
            var action = context.RouteData.Values["action"];
            var logMsg = $"{dateTimeNow} – {controllerInfo}.{action} – {elapseTime}";

            using (var streamWriter = new StreamWriter("timeElapse.txt", true))
            {
                streamWriter.WriteLine(logMsg);
            }

        }
    }
}
