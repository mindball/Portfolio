using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using System;
using System.Linq.Expressions;

namespace CarTrade.Web.Infrastructure.Extensions
{
    public static class _RecurringJobManagerExtensions
    {
        //get from: https://stackoverflow.com/questions/49136133/mocking-hangfire-recurringjob-dependency-in-net-core-2
        //public static void AddOrUpdate(this IRecurringJobManager manager, Expression<Action> methodCall, Func<string> cronExpression, TimeZoneInfo timeZone = null, string queue = EnqueuedState.DefaultQueue)
        //{
        //    var job = Job.FromExpression(methodCall);
        //    var id = $"{job.Type.ToGenericTypeString()}.{job.Method.Name}";

        //    manager.AddOrUpdate(id, job, cronExpression(), timeZone ?? TimeZoneInfo.Utc, queue);
        //}
    }
}
