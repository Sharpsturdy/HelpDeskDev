using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Help_Desk_2.Controllers
{
    public class FilesController : Controller
    {
        // GET: File
        public ActionResult d()
        {
            //return View();
            //return FileResult("adb.txt");
            //R:\Development\Clients\progrex\Renold\Engineering Help Desk\Help Desk 2\App_Data\Files\Customer-ORDER-FORM-2 (2).jpg
            string vp = "~/App_Data/Files/Customer-ORDER-FORM-2 (2).jpg";
            return File(vp, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(vp));
        }
    }
}