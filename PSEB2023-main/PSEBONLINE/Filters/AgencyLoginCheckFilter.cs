using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Collections.Specialized;


namespace PSEBONLINE.Filters
{
    public class AgencyLoginCheckFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (HttpContext.Current.Session["AgencyLoginSession"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                      new RouteValueDictionary
                    {
                        {"controller", "Agency"},
                        {"action", "Index"}
                    });
            }

        }
    }
}