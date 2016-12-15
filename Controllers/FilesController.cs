using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Help_Desk_2.Controllers
{
    public class FilesController : Controller
    {
        private HelpDeskContext db = new HelpDeskContext();

        // GET: File
        public ActionResult d(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Attachment attach = db.Attachments.Find(id);
            if (attach == null)
            {
                return HttpNotFound();
            }

            if(!System.IO.File.Exists(Server.MapPath(attach.filePath)))
            {
                return File(attach.filePath, System.Net.Mime.MediaTypeNames.Application.Octet, "MISSING FILE"); //Path.GetFileName(vp));
            }
            //return View();
            //return FileResult("adb.txt");
            //R:\Development\Clients\progrex\Renold\Engineering Help Desk\Help Desk 2\App_Data\Files\Customer-ORDER-FORM-2 (2).jpg

            //string vp = "~/App_Data/Files/Customer-ORDER-FORM-2 (2).jpg";
            return File(attach.filePath, System.Net.Mime.MediaTypeNames.Application.Octet, attach.fileName); //Path.GetFileName(vp));
        }

        public ActionResult dsp(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Attachment attach = db.Attachments.Find(id);
            if (attach == null)
            {
                return HttpNotFound();
            }
            //return View();
            //return FileResult("adb.txt");
            //R:\Development\Clients\progrex\Renold\Engineering Help Desk\Help Desk 2\App_Data\Files\Customer-ORDER-FORM-2 (2).jpg

            //string vp = "~/App_Data/Files/Customer-ORDER-FORM-2 (2).jpg";
            return File(attach.filePath, System.Net.Mime.MediaTypeNames.Application.Octet, attach.fileName); //Path.GetFileName(vp));
        }

        
        private FileResult images(int? id)
        {
            MemoryStream stream = new MemoryStream();
            //var bitmap = CreateThumbnail();
            //bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            Byte[] bytes = stream.ToArray();
            return File(bytes, "image/png");

        }
        
    }
}