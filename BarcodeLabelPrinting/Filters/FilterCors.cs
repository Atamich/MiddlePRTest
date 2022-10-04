using System;
using System.Web.Mvc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
                AllowMultiple = true, Inherited = true)]
public class FilterCorsAttribute : FilterAttribute, IActionFilter
{
    private const string IncomingOriginHeader = "Origin";
    private const string OutgoingOriginHeader = "Access-Control-Allow-Origin";
    private const string OutgoingMethodsHeader = "Access-Control-Allow-Methods";
    private const string OutgoingAgeHeader = "Access-Control-Max-Age";

    public void OnActionExecuted(ActionExecutedContext filterContext)
    {
        // Do nothing
    }

    public void OnActionExecuting(ActionExecutingContext filterContext)
    {
        //#if DEBUG
        //#else
        var isLocal = filterContext.HttpContext.Request.IsLocal;
        var originHeader =
             filterContext.HttpContext.Request.Headers.Get(IncomingOriginHeader);
        var response = filterContext.HttpContext.Response;

        if (!String.IsNullOrWhiteSpace(originHeader) &&
            (isLocal || IsAllowedOrigin(originHeader)))
        {
            response.AddHeader(OutgoingOriginHeader, "*");
            response.AddHeader(OutgoingMethodsHeader, "GET,POST,OPTIONS");
            response.AddHeader(OutgoingAgeHeader, "3600");
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
        //#endif
    }

    protected bool IsAllowedOrigin(string origin)
    {
        if(origin.Equals("https://www.laredoute.ru") ||
           origin.Equals("http://www.laredoute.ru") ||
           origin.Equals("https://m.laredoute.ru") ||
           origin.Equals("http://m.laredoute.ru") ||
           origin.Equals("https://preview.laredoute.ru") ||
           origin.Equals("http://preview.laredoute.ru") ||
           origin.Equals("http://preprod.laredoute.ru") ||
           origin.Equals("https://preprod.laredoute.ru") ||
           origin.Equals("https://m-preview.laredoute.ru") ||
           origin.Equals("http://m-preview.laredoute.ru") ||
           origin.Equals("http://www.laredoute.su") ||
           origin.Equals("https://www.laredoute.su:8443") ||
           origin.Equals("https://www.laredoute.su") ||
           origin.Equals("https://www2.laredoute.ru") ||
           origin.Equals("http://www2.laredoute.ru"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}