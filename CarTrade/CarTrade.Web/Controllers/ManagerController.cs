using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    [Authorize(Roles = ManagerRole + "," + AdministratorRole)]
    public abstract class ManagerController : Controller
    {       
    }
}
