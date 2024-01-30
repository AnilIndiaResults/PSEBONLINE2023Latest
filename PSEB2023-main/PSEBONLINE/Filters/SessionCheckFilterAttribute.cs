using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Collections.Specialized;

namespace PSEBONLINE.Filters
{
    public class SessionCheckFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (HttpContext.Current.Session["SCHL"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                      new RouteValueDictionary
                    {
                        {"controller", "Login"},
                        {"action", "Index"}
                    });
            }

        }


    }
}