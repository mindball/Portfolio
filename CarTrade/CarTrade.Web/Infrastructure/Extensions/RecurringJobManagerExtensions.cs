﻿using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CarTrade.Web.Infrastructure.Extensions
{
    public static class RecurringJobManagerExtensions
    {
        public static void AddOrUpdate(this IRecurringJobManager manager, Expression<Action> methodCall, Func<string> cronExpression, TimeZoneInfo timeZone = null, string queue = EnqueuedState.DefaultQueue)
        {
            var job = Job.FromExpression(methodCall);
            var id = $"{job.Type.ToGenericTypeString()}.{job.Method.Name}";

            manager.AddOrUpdate(id, job, cronExpression(), timeZone ?? TimeZoneInfo.Utc, queue);
        }
    }
}
