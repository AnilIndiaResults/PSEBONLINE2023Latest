using System.Web;
using System.Web.Mvc;
using FilterSkipping.Filters;

namespace PSEBONLINE
{
    public class FilterConfig 
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //
            filters.Add(new ImportantTaskFilterAttribute());
        }       

    }
}
