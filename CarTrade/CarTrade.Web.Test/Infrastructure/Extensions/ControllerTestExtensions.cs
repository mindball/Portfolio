using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace CarTrade.Web.Test.Infrastructure.Extensions
{
    public static class ControllerTestExtensions
    {
        public static T WithIdentity<T>(this T controller, string role) where T : Controller
        {
            controller.EnsureHttpContext();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                 new Claim(ClaimTypes.Role, role)
                            }));

            controller.ControllerContext.HttpContext.User = principal;

            return controller;
        }

        public static T WithAnonymousIdentity<T>(this T controller) where T : Controller
        {
            controller.EnsureHttpContext();

            var principal = new ClaimsPrincipal(new ClaimsIdentity());

            controller.ControllerContext.HttpContext.User = principal;

            return controller;
        }

        private static T EnsureHttpContext<T>(this T controller) where T : Controller
        {
            if (controller.ControllerContext == null)
            {
                controller.ControllerContext = new ControllerContext();
            }

            if (controller.ControllerContext.HttpContext == null)
            {
                controller.ControllerContext.HttpContext = new DefaultHttpContext();
            }

            return controller;
        }
    }
}
