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

        public static string displayMessage { get ; set; }

        public static IEnumerable<WordList> FullWordList
        {
            get { HelpDeskContext db = new HelpDeskContext(); return db.WordLists.OrderBy(x => x.text); }
        }

        public static IEnumerable<UserProfile> AllUsers
        {
            get { HelpDeskContext db = new HelpDeskContext(); return db.UserProfiles.OrderBy(u => (u.firstName + u.surName)); }
        }

        public static void saveWordLists(string[] kwtmp, string[] eatmp, HelpDeskContext db, KnowledgeFAQ kbfaq)
        {
            
            if (kwtmp == null) kwtmp = new string[] { "0" };

            if (eatmp == null) eatmp = new string[] { "0" };

            //displayMessage = "KW=" + string.Join(",", kwtmp) + "<br/>" + "EA=" + string.Join(",", eatmp) + "//Top of Page//";

            //Get exisiting wordlist  (keywords + expertAreas)             
            db.Entry(kbfaq).Collection(x => x.wordList).Load();

            string[] orgList = kbfaq.wordList.Select(x => "" + x.ID).ToArray<string>();

            string[] kwords = kwtmp.Union(eatmp).ToArray();

            //displayMessage += string.Join(",", orgList) + "//" + string.Join(",", kwords) + "#";

            //New keywords to be added 
            string[] newKeywords = kwords.Where(x => !orgList.Contains(x)).ToArray();

            string[] delKeywords = orgList.Where(x => !kwords.Contains(x)).ToArray();

            displayMessage += string.Join(",", newKeywords) + "#" + string.Join(",", delKeywords);

            foreach (var w in newKeywords)
            {
                WordList wd = db.WordLists.Find(int.Parse(w));

                if (wd != null)
                    kbfaq.wordList.Add(wd);
                
            }

            foreach (var w in delKeywords)
            {
                WordList wd = db.WordLists.Find(int.Parse(w));

                if (wd != null)
                    kbfaq.wordList.Remove(wd);
            }

        }

        public static void saveGSLists(string[] intmp, HelpDeskContext db, int type)
        {

            if (intmp == null) intmp = new string[] { "0" };

            //All Users            
            string[] orgList = null;
            if (type == 1)
            {
                orgList = db.UserProfiles.Where(x => x.isFaqApprover).Select(x => "" + x.userID).ToArray<string>();
            } else if (type == 2)
            {
                orgList = db.UserProfiles.Where(x => x.isKbApprover).Select(x => "" + x.userID).ToArray<string>();
            } else if (type == 3)
            {
                orgList = db.UserProfiles.Where(x => x.isResponsible).Select(x => "" + x.userID).ToArray<string>();
            }

            //displayMessage += string.Join(",", orgList) + "//" + string.Join(",", kwords) + "#";

            //New keywords to be added 
            string[] newUsers = intmp.Where(x => !orgList.Contains(x)).ToArray();

            string[] delUsers = orgList.Where(x => !intmp.Contains(x)).ToArray();

            displayMessage += string.Join(",", newUsers) + "#" + string.Join(",", delUsers);

            foreach (var uid in newUsers)
            {
                UserProfile user = db.UserProfiles.Find(new Guid(uid));

                if (user != null)
                {
                    if (type == 1)
                    {
                        user.isFaqApprover = true;
                    }
                    else if (type == 2)
                    {
                        user.isKbApprover = true;
                    }
                    else if (type == 3)
                    {
                        user.isResponsible = true;
                    }
                }
            }

            foreach (var uid in delUsers)
            {
                UserProfile user = db.UserProfiles.Find(new Guid(uid));

                if (user != null)
                {
                    if (type == 1)
                    {
                        user.isFaqApprover = false;
                    }
                    else if (type == 2)
                    {
                        user.isKbApprover = false;
                    }
                    else if (type == 3)
                    {
                        user.isResponsible = false;
                    }
                }
            }

        }

        public static int getExpiryDays(HelpDeskContext db, bool kb = false)
        {
            GlobalSettings globalSettings = db.GlobalSettingss.FirstOrDefault<GlobalSettings>();

            if (globalSettings == null || globalSettings.ID == null)
            {

                return 0;
            } else
            {
                return kb ? globalSettings.KBFAQsExpiryDays : globalSettings.TicketExpiryDays;
            }
        }

        public static int pageSize {  get { return 5; } }
    }
}