﻿using BarcodeLabelPrinting.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BarcodeLabelPrinting.Controllers
{
	[RoutePrefix("authorize")]
	[AllowAnonymous]
	public class AuthorizeController : Controller
    {
        [Route("win")]
        [HttpGet]
        public string Get()
        {
            var context = Request.RequestContext.HttpContext;
            return AuthorizeHelper.GetWinAuthAccount(context);
        }

    }
}
