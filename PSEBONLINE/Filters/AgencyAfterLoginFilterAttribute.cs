using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Collections.Specialized;
using PSEBONLINE.Models;
using PSEBONLINE.Repository;
using System.Collections.Generic;
using PSEBONLINE.AbstractLayer;
using System.Data;

namespace PSEBONLINE.Filters
{
    public class AgencyAfterLoginFilterAttribute  : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["AgencyLoginSession"] != null)
            {
                AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)HttpContext.Current.Session["AgencyLoginSession"];
                List<AgencySchoolModel> AgencySchoolModels = new List<AgencySchoolModel>();
                string id = string.Empty;
                string schl = string.Empty;
                string sub = string.Empty;
                if (filterContext.ActionParameters.ContainsKey("id"))
                {
                    id = filterContext.ActionParameters["id"].ToString();
                }
                if (filterContext.ActionParameters.ContainsKey("schl"))
                {
                    schl = filterContext.ActionParameters["schl"].ToString();
                }
                if (filterContext.ActionParameters.ContainsKey("sub"))
                {
                    sub = filterContext.ActionParameters["sub"].ToString();
                }

                bool isSchoolExists = false;
                if (!string.IsNullOrEmpty(schl))
                {
                    //   DataSet ds = AgencyRepository.CheckSchlAllowToAgency(0, AgencyLoginSession.AgencyId.ToString(), schl, "");
                    DataSet ds = AgencyRepository.CheckSchlAllowToAgency(0, AgencyLoginSession.AgencyId.ToString(), schl, sub, "");

                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            isSchoolExists = true;
                        }
                    }
                }

                if (!isSchoolExists)
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
}