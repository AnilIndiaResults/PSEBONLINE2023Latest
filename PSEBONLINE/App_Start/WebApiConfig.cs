using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace PSEBONLINE
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			config.MapHttpAttributeRoutes();

			//config.Routes.MapHttpRoute(
			//					   "DefaultApiCtrl",
			//					   "api/{controller}/{action}/{id}",
			//					   new { id = RouteParameter.Optional });

			config.Routes.MapHttpRoute(
name: "API Default",
routeTemplate: "api/{controller}/{action}/{id}",
defaults: new { id = RouteParameter.Optional });
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
		}
	}
}
