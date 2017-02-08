using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;

public static class Globals
{
    public static string appAdministrators { get; set; }

    public static string superUsers { get; set; }

    public static string baseURL { get; set;  }
        
    static Globals()
    {
	    try
	    {
		    var appData = ConfigurationManager.AppSettings;
		    appAdministrators = appData["AppAdministrators"].Trim();
		    superUsers = appData["SuperUsers"].Trim();
		    baseURL = appData["BaseURL"].Trim();
		    //vlookup_limit = int.Parse(appData["VlookupLimit"].Trim());
	    }
	    catch (Exception ex)
	    {
		    //do nothing. This is not always a show stopper
	    }
       
    }

	public static string LocalDevMachine { get; } = "DESKTOP-PTOFCJ0";

}

