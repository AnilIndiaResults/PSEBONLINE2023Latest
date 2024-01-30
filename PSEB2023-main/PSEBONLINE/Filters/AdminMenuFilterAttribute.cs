using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Collections.Specialized;
using PSEBONLINE.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PSEBONLINE.AbstractLayer;

namespace PSEBONLINE.Filters
{
    public class AdminMenuFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string actionName = context.ActionDescriptor.ActionName;
            string controllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName;
            base.OnActionExecuting(context);
            if (HttpContext.Current.Session["AdminId"] != null)
            {
                //  AdminLoginSession adminLoginSession = (AdminLoginSession)HttpContext.Current.Session["AdminLoginSession"];             
                int AdminId = Convert.ToInt32(HttpContext.Current.Session["AdminId"]);
                string AdminType = HttpContext.Current.Session["AdminType"].ToString();
                string CurrentSession= HttpContext.Current.Session["Session"].ToString();


                List<SiteMenu> siteMenuList = new List<SiteMenu>();
                    DataSet result = new AbstractLayer.DBClass().GetAdminDetailsById(AdminId, Convert.ToInt32(CurrentSession.ToString().Substring(0, 4)));
                    if (result.Tables[2].Rows.Count > 0)
                    {
                        bool exists = true;
                        DataSet dsIsExists = new AbstractLayer.DBClass().GetActionOfSubMenu(0, controllerName, actionName);
                        int IsExists = Convert.ToInt32(dsIsExists.Tables[0].Rows[0]["IsExist"].ToString());
                        if (IsExists == 1 || AdminType.ToString().ToUpper() == "ADMIN" || actionName.ToString().ToUpper() == "PAGENOTAUTHORIZED" || actionName.ToString().ToUpper() == "INDEX" || actionName.ToString().ToUpper() == "LOGOUT" || actionName.ToString().ToUpper() == "Change_Password")
                        {
                            exists = true;
                        }
                        else
                        {
                            exists = result.Tables[2].AsEnumerable().Where(c => c.Field<string>("Controller").ToUpper().Equals(controllerName.ToUpper()) && c.Field<string>("Action").ToUpper().Equals(actionName.ToUpper())).Count() > 0;
                        }

                        if (exists == false)
                        {
                            context.Result = new RedirectToRouteResult(
                             new RouteValueDictionary(new { controller = "Admin", action = "PageNotAuthorized" }));
                            return;
                        }
                        else
                        {

                        var eList = result.Tables[2].AsEnumerable().Select(dataRow => new SiteMenu
                        {
                            MenuID = dataRow.Field<int>("MenuID"),
                            MenuName = dataRow.Field<string>("MenuName"),
                            MenuUrl = dataRow.Field<string>("MenuUrl"),
                            ParentMenuID = dataRow.Field<int>("ParentMenuID"),
                            IsMenu = dataRow.Field<int>("IsMenu"), 
                        }).ToList();

                        siteMenuList = eList.ToList();                       
                        }
                    }
                    else
                    {
                        context.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(new { controller = "Admin", action = "PageNotAuthorized" }));
                        return;
                    }
                var ViewBag = context.Controller.ViewBag;
                ViewBag.SiteMenu = siteMenuList;               
            }

        }
    }
}