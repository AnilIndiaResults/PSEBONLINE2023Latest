using System;
using System.Web.Mvc;
using FilterSkipping.Models;

namespace FilterSkipping.Filters
{
    public class ImportantTaskFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            bool skipImportantTaskFilter = filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipImportantTaskAttribute), true) ||
                filterContext.ActionDescriptor.IsDefined(typeof(SkipImportantTaskAttribute), true);

            if (!skipImportantTaskFilter)
            {
                PerformModelAlteration(filterContext);
            }

            base.OnActionExecuted(filterContext);
        }

        private void PerformModelAlteration(ActionExecutedContext filterContext)
        {
            var model = filterContext.Controller.ViewData.Model as BaseModel;

            if (model != null)
            {
                model.ModelAlterationPerformed = true;
                model.MeetingDate = DateTime.Now; 
            }
        }
    }
}