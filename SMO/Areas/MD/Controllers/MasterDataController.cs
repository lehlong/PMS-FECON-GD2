using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO.Areas.MD.Controllers
{
    public class MasterDataController : Controller
    {
        // GET: MD/MasterData
        public ActionResult Index()
        {
            return PartialView();
        }
    }
}