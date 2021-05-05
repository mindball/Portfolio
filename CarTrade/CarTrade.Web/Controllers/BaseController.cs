using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    [Authorize(Roles = AdministratorRole)]
    public abstract class BaseController : Controller
    {
    }
}
