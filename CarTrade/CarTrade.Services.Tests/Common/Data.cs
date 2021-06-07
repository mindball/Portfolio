using CarTrade.Services.Tests.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CarTrade.Services.Tests.Common
{
    public abstract class Data
    {    
        public static IEnumerable<object[]> ExpireDates => new[]
        {
            new object[] { DateTime.UtcNow.AddYears(-(int)TimesPeriod.Year)},
            new object[] { DateTime.UtcNow.AddMonths(-(int)TimesPeriod.Monthly) },
            new object[] { DateTime.UtcNow.AddDays(-(int)TimesPeriod.Dayly)},
            new object[] { DateTime.UtcNow.AddHours(-(int)TimesPeriod.Hour)},
            new object[] { DateTime.UtcNow.AddMinutes(-(int)TimesPeriod.Minute),},
            new object[] { DateTime.UtcNow.AddSeconds(-(int)TimesPeriod.Second)},
            new object[] { DateTime.UtcNow.AddMilliseconds(-(int)TimesPeriod.Milisecond)}
        };

        public static IEnumerable<object[]> StartDateBiggerThanEndDateData => new[]
        {
            new object[] {DateTime.UtcNow, DateTime.UtcNow.AddYears(-(int)TimesPeriod.Year)},
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddMonths(-(int)TimesPeriod.Monthly) },
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddDays(-(int)TimesPeriod.Dayly)},
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddHours(-(int)TimesPeriod.Hour)},
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddMinutes(-(int)TimesPeriod.Minute),},
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddSeconds(-(int)TimesPeriod.Second)},
            new object[] { DateTime.UtcNow, DateTime.UtcNow.AddMilliseconds(-(int)TimesPeriod.Milisecond)}
        };
    }
}
