using System;
using System.Collections.Generic;
using System.Web.Configuration;

public static class Globals
{
    public static string systemAdministrators { get; set; }

    public static string superUsers { get; set; }
        
    static Globals()
    {
        try {
            var appData = WebConfigurationManager.AppSettings;
            systemAdministrators = appData["SystemAdministrators"].Trim();
            superUsers = appData["SuperUsers"].Trim();
            //vlookup_limit = int.Parse(appData["VlookupLimit"].Trim());

        } catch (Exception ex)
        {
            //do nothing. This is not always a show stopper
        }
       
    }

   
}

