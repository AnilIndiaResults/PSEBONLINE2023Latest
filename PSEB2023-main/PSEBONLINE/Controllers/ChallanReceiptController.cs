using PSEBONLINE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSEBONLINE.Controllers
{
    public class ChallanReceiptController : Controller
    {
        // GET: ChallanReceipt
        public async Task<ActionResult> Index(string id)
        {

            ChallanModels oChallanModels = new ChallanModels();
            if (id != null)
            {   
                oChallanModels = await new AbstractLayer.SchoolDB().GetChallanDetail(id);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(oChallanModels);
        }
    }
}