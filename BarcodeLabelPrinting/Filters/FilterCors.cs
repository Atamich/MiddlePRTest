using System;
using System.Configuration;
using System.Web.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
                AllowMultiple = true, Inherited = true)]
public class FilterCorsAttribute : FilterAttribute, IActionFilter
{
    private const string IncomingOriginHeader = "Origin";
    private const string OutgoingOriginHeader = "Access-Control-Allow-Origin";
    private const string OutgoingMethodsHeader = "Access-Control-Allow-Methods";
    private const string OutgoingAgeHeader = "Access-Control-Max-Age";
    private const string OutgoingAllowCredentials = "Access-Control-Allow-Credentials";
    private readonly string FrontEndDomen = ConfigurationManager.AppSettings["frontEndDomen"];

    public void OnActionExecuted(ActionExecutedContext filterContext)
    {
        // Do nothing
    }

    public void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var isLocal = filterContext.HttpContext.Request.IsLocal;
        var originHeader =
             filterContext.HttpContext.Request.Headers.Get(IncomingOriginHeader);
        var response = filterContext.HttpContext.Response;

        if (!String.IsNullOrWhiteSpace(originHeader) &&
            (isLocal || originHeader.Equals(FrontEndDomen)))
        {
            response.AddHeader(OutgoingOriginHeader, FrontEndDomen);
            response.AddHeader(OutgoingMethodsHeader, "GET,POST,OPTIONS");
            response.AddHeader(OutgoingAgeHeader, "3600");
            response.AddHeader(OutgoingAllowCredentials, "true");
        }
        else
        {
            if (String.IsNullOrWhiteSpace(originHeader))
            {

            }
            else
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = new EmptyResult();
            }
        }
    }
}