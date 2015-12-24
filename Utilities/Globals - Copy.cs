using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Utilities
{
    public class Globals
    {
        public UserProfile userProfile { get; set;  }
        public string errorMsg = "All good actually";

        public UserProfile getUserProfile() {

            if (userProfile != null )
            {
                return userProfile;
            }
            bool isLoggedIn = (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated);
            if (isLoggedIn)
            {
                var user = HttpContext.Current.User;
                string loginName = user.Identity.Name;

                PrincipalContext ctx;

                if (loginName.IndexOf(@"\") > 0)
                {
                    ctx = new PrincipalContext(ContextType.Domain, loginName.Substring(0, loginName.IndexOf(@"\"))); 
                }
                else
                {
                    ctx = new PrincipalContext(ContextType.Domain);
                }

                var userPrincipal = UserPrincipal.FindByIdentity(ctx, loginName);

                try
                {
                    if (user != null)
                    {
                        HelpDeskContext db = new HelpDeskContext();

                        //Grab current user from profile
                        userProfile = db.UserProfiles.Find(userPrincipal.Guid);

                        if (userProfile == null) // If user has no profile add it
                        {
                            //Add profile to database then populate userProfile object from database
                            userProfile = new UserProfile
                            {
                                userID = (Guid)userPrincipal.Guid,
                                firstName = userPrincipal.GivenName,
                                loginName = loginName,
                                principalName = userPrincipal.UserPrincipalName,
                                surName = userPrincipal.Surname,
                                // not using d ud.displayName,

                            };
                            db.UserProfiles.Add(userProfile);
                            db.SaveChanges();
                            userProfile = db.UserProfiles.Find(userPrincipal.Guid);
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    //if (Globals.debug) Globals.form1Err += "Part2 Error: " + ex.Message;
                    errorMsg = ex.Message;
                    userProfile = new UserProfile();
                    userProfile.firstName = "";
                    userProfile.surName = "Anonymous";
                }

            } else {
                userProfile = new UserProfile();
                userProfile.firstName = "";
                userProfile.surName = "Anonymous";
            }

            return userProfile;
        }
    }
}