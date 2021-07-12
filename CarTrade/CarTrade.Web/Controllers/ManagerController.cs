using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static CarTrade.Web.WebConstants;

namespace CarTrade.Web.Controllers
{
    [Authorize(Roles = ManagerRole + "," + AdministratorRole)]
    public abstract class ManagerController : Controller
    {       
    }
}
