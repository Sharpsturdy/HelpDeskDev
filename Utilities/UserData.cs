using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Utilities
{
    public class UserData
    {

        private HelpDeskContext db = new HelpDeskContext();

        public UserProfile getUserProfile() {

            UserProfile userProfile = null;
            if (HttpContext.Current.Session["UserID"] == null)
            {
                bool isLoggedIn = (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated);
                if (isLoggedIn)
                {
                    var user = HttpContext.Current.User;
                    string loginName = user.Identity.Name;

                    PrincipalContext ctx =null;
                    try {
                        if (loginName.IndexOf(@"\") > 0)
                        {
                            ctx = new PrincipalContext(ContextType.Domain, loginName.Substring(0, loginName.IndexOf(@"\")));
                        }
                        else
                        {
                            ctx = new PrincipalContext(ContextType.Domain);
                        }
                    } catch(Exception pe)
                    {
                        //AllSorts.displayMessage = "PE: " + pe.Message;
                        try {
                            ctx = new PrincipalContext(ContextType.Machine,null);
                            loginName = loginName.Substring(loginName.IndexOf(@"\")+1);
                          //  AllSorts.displayMessage += "Login Name: " + loginName;
                        } catch (Exception me)
                        {
                            AllSorts.displayMessage = "ME: " + me.Message;
                        }
                    }

                    var userPrincipal = UserPrincipal.FindByIdentity(ctx, loginName);
                    /*AllSorts.displayMessage += ". UP" + userPrincipal.UserPrincipalName +
                        "<br>Surname" + userPrincipal.Surname +
                        "<br>Surname" + userPrincipal.DisplayName + "<br>" + userPrincipal.DistinguishedName +
                        "<br>" + userPrincipal.EmailAddress + "<br>" + userPrincipal.GivenName +
                        "<br>" + userPrincipal.Name + "<br>" + userPrincipal.SamAccountName + "<br>" +
                        "<br>" + userPrincipal.Sid.Value;
                        */
                    try
                    {
                        
                        HelpDeskContext db = new HelpDeskContext();

                        //Grab current user from profile
                        if (userPrincipal.Guid != null)
                        {
                            userProfile = db.UserProfiles.Find(userPrincipal.Guid);
                            //AllSorts.displayMessage += "GUID : " + userPrincipal.Guid.ToString();
                        }
                        else {
                            //Try Using userpprincipal name
                            var ups = db.UserProfiles.Where(u => u.principalName == userPrincipal.SamAccountName);
                            if (ups.Count() > 0) { userProfile = ups.First(); }

                            //AllSorts.displayMessage += "Looking for local user? : ";// + userProfile.ToString();
                        }
                        if (userProfile == null) // If user has no profile add it
                        {
                            AllSorts.displayMessage += "0#Please ensure that all mandatory fields are completed";
                            //Add profile to database then populate userProfile object from database
                            userProfile = new UserProfile
                            {
                                userID = userPrincipal.Guid== null ? Guid.NewGuid() : (Guid)userPrincipal.Guid,
                                firstName = "",
                                loginName = loginName,
                                principalName = userPrincipal.SamAccountName,
                                surName = userPrincipal.DisplayName,
                                displayName = "#"+userPrincipal.DisplayName,
                                lastSignOn = DateTime.Now
                                // not using d ud.displayName,

                            };
                            db.UserProfiles.Add(userProfile);
                            db.SaveChanges();
                            userProfile = db.UserProfiles.Find(userProfile.userID);
                        } else
                        {
                            db.Entry(userProfile).State = EntityState.Modified;
                            userProfile.lastSignOn = DateTime.Now;
                            db.SaveChanges();
                        }
                                                    
                    }
                    catch (Exception ex)
                    {
                        //if (Globals.debug) Globals.form1Err += "Part2 Error: " + ex.Message;
                        //AllSorts.displayMessage += ex.Message;
                        userProfile = new UserProfile();
                        userProfile.firstName = "";
                        userProfile.surName = "Anonymous";
                    }

                    if (userProfile != null && userProfile.userID != Guid.Empty)
                    {
                        HttpContext.Current.Session.Add("UserID", userProfile.userID.ToString());
                        HttpContext.Current.Session.Add("UserPrincipal", userProfile.principalName);
                        HttpContext.Current.Session.Add("UserDisplayName", userProfile.displayName);
                    }

                }
                else {
                    userProfile = new UserProfile();
                    userProfile.firstName = "";
                    userProfile.surName = "Anonymous";
                }

                return userProfile;

            } else
            {
                return db.UserProfiles.Find(new Guid((string)HttpContext.Current.Session["UserID"]));
            }
                                  
        }
    }
}