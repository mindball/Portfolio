using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarTrade.Common;

namespace CarTrade.Web.Controllers
{
    [Authorize(Roles = DataConstants.AdministratorRole)]
    public abstract class BaseController : Controller
    {
    }
}
