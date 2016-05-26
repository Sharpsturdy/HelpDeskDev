using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

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

        public static string displayMessage {
            get { return (string)HttpContext.Current.Session["message"]; }

            set { HttpContext.Current.Session["message"] = value; }
        }

        //public static string appAdmins { get { return "Administrators,AppGroup2,AppGroup1"; } }

        private static HelpDeskContext db { get { return new HelpDeskContext(); } }

        public static IEnumerable<WordList> FullWordList
        {
            get { return db.WordLists.OrderBy(x => x.text); }
        }

        public static IEnumerable<WordList> keywords
        {
            get { return FullWordList.Where(x => x.type == 1 && !x.deleted); }
        }

        public static IEnumerable<WordList> expertareas
        {
            get { return FullWordList.Where(x => x.type == 2 && !x.deleted); }
        }
        public static IEnumerable<UserProfile> AllUsers
        {
            get { return db.UserProfiles.Where(u => !u.deleted).OrderBy(u => (u.firstName + u.surName)); }
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

            //displayMessage += string.Join(",", newKeywords) + "#" + string.Join(",", delKeywords);

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

        public static void saveWordLists(string[] kwtmp, string[] eatmp, HelpDeskContext db, Ticket kbfaq)
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

            //displayMessage += string.Join(",", newKeywords) + "#" + string.Join(",", delKeywords);

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

        public static void saveWordLists(string[] kwtmp, string[] eatmp, HelpDeskContext db, UserProfile user, bool isKB = false)
        {

            if (kwtmp == null) kwtmp = new string[] { "0" };

            if (eatmp == null) eatmp = new string[] { "0" };

            string[] orgList = null;
            //Get exisiting wordlist  (keywords + expertAreas)             
            if (isKB)
            {
                db.Entry(user).Collection(x => x.kbsubs).Load();
                orgList = user.kbsubs.Select(x => "" + x.ID).ToArray<string>();
            } else {
                db.Entry(user).Collection(x => x.faqsubs).Load();
                orgList = user.faqsubs.Select(x => "" + x.ID).ToArray<string>();
            }

            string[] kwords = kwtmp.Union(eatmp).ToArray();

            //New keywords to be added 
            string[] newKeywords = kwords.Where(x => !orgList.Contains(x)).ToArray();

            string[] delKeywords = orgList.Where(x => !kwords.Contains(x)).ToArray();

            foreach (var w in newKeywords)
            {
                WordList wd = db.WordLists.Find(int.Parse(w));

                if (wd != null)
                {
                    if (isKB)
                        user.kbsubs.Add(wd);
                    else
                        user.faqsubs.Add(wd);
                }
            }

            foreach (var w in delKeywords)
            {
                WordList wd = db.WordLists.Find(int.Parse(w));

                if (wd != null)
                {
                    if (isKB)
                        user.kbsubs.Remove(wd);
                    else
                        user.faqsubs.Remove(wd);
                }
            }

        }

        public static void saveGSLists(HelpDeskContext db, string[] intmp, int type)
        {

            if (intmp == null) intmp = new string[] { "" };

            //All Users            
            string[] orgList = null;
            if (type == 1)
            {
                orgList = db.UserProfiles.Where(x => !x.deleted && x.isFaqApprover).Select(x => "" + x.userID).ToArray<string>();
            } else if (type == 2)
            {
                orgList = db.UserProfiles.Where(x => !x.deleted && x.isKbApprover).Select(x => "" + x.userID).ToArray<string>();
            } else if (type == 3)
            {
                orgList = db.UserProfiles.Where(x => !x.deleted && x.isResponsible).Select(x => "" + x.userID).ToArray<string>();
            }

            //displayMessage += string.Join(",", orgList) + "//" + string.Join(",", kwords) + "#";

            //New keywords to be added 
            string[] newUsers = intmp.Where(x => !orgList.Contains(x)).ToArray();

            string[] delUsers = orgList.Where(x => !intmp.Contains(x)).ToArray();

            //displayMessage +="<br/> [" + string.Join(",", newUsers) + "#" + string.Join(",", delUsers) + " of type = " + type + "]";

            GSListHelper(newUsers, db, type, true);

            GSListHelper(delUsers, db, type, false);

            db.SaveChanges();

        }

        private static void GSListHelper(string[] users, HelpDeskContext db, int type, bool value)
        {
            foreach (var uid in users)
            {
                if (!string.IsNullOrEmpty(uid))
                {
                    UserProfile user = db.UserProfiles.Find(new Guid(uid));

                    if (user != null)
                    {
                        if (type == 1)
                        {
                            user.isFaqApprover = value;
                        }
                        else if (type == 2)
                        {
                            user.isKbApprover = value;
                        }
                        else if (type == 3)
                        {
                            user.isResponsible = value;
                        }
                    }
                }
            }
        }

        public static int getExpiryDays(int type = 0)
        {
            GlobalSettings globalSettings = db.GlobalSettingss.FirstOrDefault<GlobalSettings>();

            if (globalSettings == null || globalSettings.ID == null)
            {

                return 0;
            } else
            {
                if (type == 0) return globalSettings.TicketExpiryDays;

                if (type == 1) return globalSettings.FAQsExpiryDays;

                if (type == 2) return globalSettings.TicketExpiryDays;

                return 0;
            }
        }

        public static int pageSize {
            get {
                string ps = ConfigurationManager.AppSettings["PageSize"];
                int psNum = 0;
                if (!int.TryParse(ps, out psNum))
                {
                    return 10; //Default
                }
                return psNum;

            } }

        public static void setNavProperties(dynamic ViewBag, bool isSinglePage = false)
        {
            HttpContext ctx = HttpContext.Current;
            if (isSinglePage)
            {
                try
                {
                    ViewBag.prevURL = string.IsNullOrEmpty((string)ctx.Session["lastView"]) ? ctx.Request.UrlReferrer.PathAndQuery : ctx.Session["lastView"];
                }
                catch (Exception e)
                {
                    ViewBag.prevURL = "~/";
                }
            } else
            {
                ctx.Session["lastView"] = ctx.Request.Url.PathAndQuery;    //Record this as last view
            }
        }

        public static bool userHasRole(string roleName) //Configured in web config as CSL
        {
            var appSettings = ConfigurationManager.AppSettings[roleName];
            var user = HttpContext.Current.User;

            if (string.IsNullOrEmpty(appSettings))
            {
                return false;
            }

            foreach (string grp in appSettings.Split(','))
            {
                try {
                    if (user.IsInRole(grp.Trim()))
                        return true;
                } catch (Exception ge)
                {
                    return false; //Group may not exist! So fail safe
                }
            }

            return false;
        }

        public static bool UserCan(string ActionName)
        {
            var user = HttpContext.Current.User.Identity.Name;
            if (ActionName == "ManageAll")
            {
                return userHasRole("AdminUsers");

            }
            else if (ActionName == "ManageKBs")
            {
                return userHasRole("AdminUsers") || db.UserProfiles.Where(u => (u.isKbApprover && u.loginName == user)).Count() > 0;

            }
            else if (ActionName == "ManageFAQs")
            {
                return userHasRole("AdminUsers") || db.UserProfiles.Where(u => (u.isFaqApprover && u.loginName == user)).Count() > 0;
            }
            else if (ActionName == "ManageTickets")
            {
                return userHasRole("AdminUsers") || db.UserProfiles.Where(u => (u.isResponsible && u.loginName == user)).Count() > 0;
            }
            else if (ActionName.StartsWith("Create"))
            {
                return AllSorts.userHasRole(ActionName);
            }
            else if (ActionName == "SuperFunctions")
            {
                return userHasRole("AdminUsers") || userHasRole("SuperUsers");
            }
            return false;
        }

        private static string up(string sessValue) {
            if (string.IsNullOrEmpty(sessValue))
                return null;

            if (string.IsNullOrEmpty((string)HttpContext.Current.Session[sessValue]))
            {
                UserData ud = new UserData();
                UserProfile up = ud.getUserProfile();
                if (sessValue == "UserID") {
                    return up.userID.ToString();
                } else if (sessValue == "UserDisplayName")
                {
                    return up.displayName;
                }
                return null;
            }
            else
            {
                return (string)HttpContext.Current.Session[sessValue];
            }
        }

        public static string getUserDisplayName()
        {
            return up("UserDisplayName");
        }

        public static string getUserID()
        {
            return up("UserID");
        }

        public static void addAuditTrail(HelpDeskContext db, int id, Guid user, string auditText)
        {
            try
            {
                db.AuditTrails.Add(new AuditTrail
                {
                    refID = id,
                    userID = user,
                    timeStamp = DateTime.Now,
                    text = auditText

                });
            } catch (Exception ex)
            {

            }
        }

        public static string getNewTicketMessage {
            get {

                GlobalSettings globalSettings = db.GlobalSettingss.FirstOrDefault<GlobalSettings>();

                if (globalSettings == null || globalSettings.ID == null || !globalSettings.TicketHeaderEnabled)
                {

                    return "";
                }

                return globalSettings.TicketHeader;
            }
        }

        /********************
        public static int getNextTicketNumber()
        {
            int num = 0;
            try {
                GlobalSettings gs = db.GlobalSettingss.First<GlobalSettings>();
                if (gs != null && gs.ID != Guid.Empty)
                {
                    num = gs.TicketSeeder;
                    HttpContext.Current.Response.Write("Current Ticket Number: " + num);
                    db.Entry(gs).State = EntityState.Modified;
                    gs.TicketSeeder = num + 1;
                    HttpContext.Current.Response.Write("Next Ticket Number: " + gs.TicketSeeder);
                    db.SaveChanges();
                }
            } catch (Exception ex) { HttpContext.Current.Response.Write("Some error message: " + ex.Message); }
            return num;
        }
        *****************/
    }
}