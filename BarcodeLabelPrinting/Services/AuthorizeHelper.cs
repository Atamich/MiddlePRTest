using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace BarcodeLabelPrinting.Services
{
    public class AuthorizeHelper
    {
        public static string GetWinAuthAccount(HttpContextBase context)
        {
            IPrincipal p = context.User;
            return p.Identity.Name;
        }
    }
}