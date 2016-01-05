using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Utilities
{
    public static class AllSorts
    {
        public static void saveAttachments(int ID, HelpDeskContext db, string deleteList = null, int attachType = 0)
        {
            var rFiles = HttpContext.Current.Request.Files;
            if (rFiles.Count > 0)
            {
                //Remove files
                if (deleteList != null)
                {
                    var fileIDs = deleteList.Split(new char[',']);

                    foreach (string strID in fileIDs)
                    {
                        if (!string.IsNullOrEmpty(strID))
                        {
                            Attachment f = db.Attachments.Find(int.Parse(strID));
                            db.Attachments.Remove(f);
                        }
                    }
                }

                //Add Files
                for (int i = 0; i < rFiles.Count; i++)
                {
                    HttpPostedFile file = rFiles[i];
                    if (file.ContentLength > 0)
                    {

                        //Get physical path to directory
                        string savePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/Files"), DateTime.Now.Year.ToString());
                        if (!Directory.Exists(savePath))
                        {
                            Directory.CreateDirectory(savePath);
                        }

                        //Get unique random name, duplication not very possible
                        string randomName = "";
                        do
                        {
                            randomName = Path.GetRandomFileName();
                        } while (System.IO.File.Exists(Path.Combine(savePath, randomName)));

                        //Save file
                        //file.SaveAs(savePath + "/" + randomName);
                        file.SaveAs(Path.Combine(savePath, randomName));

                        //Add file data to database
                        Attachment attachment = new Attachment();
                        attachment.fileName = Path.GetFileName(file.FileName);
                        attachment.filePath = "~/App_Data/Files/" + DateTime.Now.Year + "/" + randomName;
                        if (attachType == 1)
                        {
                            attachment.commonID = ID;
                        }
                        else {
                            attachment.parentID = ID;
                        }

                        db.Attachments.Add(attachment);
                    }
                }

            }
        }
    }
}