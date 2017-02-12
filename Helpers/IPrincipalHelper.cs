using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Help_Desk_2.Helpers
{
	public static class IPrincipalHelper
	{
		public static bool CustomIsInRole(this IPrincipal user, string role)
		{
#if DEBUG
			if (Globals.LocalDevMachine.Contains(Environment.MachineName))
			{
				return IsLocalDevInRole(role);
			}
#endif
			return user.IsInRole(role);
		}

		private static bool IsLocalDevInRole(string targetRole)
		{
			if (targetRole.Equals(Globals.LocalDevRole, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			return false;
		}
	}
}