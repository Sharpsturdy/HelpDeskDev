﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Help_Desk_2.Utilities
{
	public class UserRoles
	{
		//Andriy: I think it's not really necessary;
		private const string AppAdminRole     = "AdminUsers";
		//Andriy: I think it's not really necessary;
		private const string AppSuperUserRole = "SuperUsers";

		public const string DomainAdminRole = "rEngDeskAdmin";
		public const string DomainSuperUserRole = "rEngDeskSuperUser";
		public const string AdminAndSuperUserRoles = "AdminUsers SuperUsers";

	}
}