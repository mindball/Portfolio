using CarTrade.Common;
using System;
using System.Linq;
using System.Reflection;

namespace CarTrade.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var types = Assembly.GetEntryAssembly()
                             .GetReferencedAssemblies()
                             .Select(Assembly.Load);

        }
    }
}
