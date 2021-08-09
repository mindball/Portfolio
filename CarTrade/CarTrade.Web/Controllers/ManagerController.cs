using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using CarTrade.Common;

namespace CarTrade.Web.Controllers
{
    [Authorize(Roles = DataConstants.ManagerRole + "," + DataConstants.AdministratorRole)]
    public abstract class ManagerController : Controller
    {       
    }
}
