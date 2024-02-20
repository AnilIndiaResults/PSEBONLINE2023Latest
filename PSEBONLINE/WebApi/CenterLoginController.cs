using Amazon.Runtime.Internal;
using PSEBONLINE.AbstractLayer;
using PSEBONLINE.Models;
using PSEBONLINE.ModelVM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


namespace PSEBONLINE.WebApi
{
	//[RoutePrefix("api/CenterLogin")]
	public class CenterLoginController : ApiController
	{
		AssociateDB pObj = new AssociateDB();
		public CenterLoginController()
		{
			string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["myDBConnection"].ConnectionString;
		}

		[System.Web.Http.HttpPost]
		//[Route("SearchDataApi")]
		//[ActionName("SearchDataApi")]
		public HttpResponseMessage SearchDataApi()
		{
			CommonResponse cr = new CommonResponse();
			try
			{
				var json = Request.Content.ReadAsStringAsync().Result;
				TempApiModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<TempApiModel>(json);

				if (!string.IsNullOrEmpty(model.Type) && !string.IsNullOrEmpty(model.SearchText))
				{
					DataSet ds = pObj.GetMeritoriousAdmissionDataByStudentRollNoAPI(model.SearchText, model.Type);
					if (ds != null && ds.Tables.Count > 0)
					{


						cr.data = new
						{
							data = ds
						};
						cr.status = ResponseCode.Success;
						cr.message = "Data Found";


					}
					else
					{
						//cr.message = ErrorMessage.SomethingWrong;
						cr.message = "No Record Found";
						cr.error.details = "No Record Found";
					}
				}
				else
				{
					//cr.message = ErrorMessage.SomethingWrong;
					cr.message = "No Record Found";
					cr.error.details = "No Record Found";
				}
			}
			catch (Exception ex)
			{
				cr.message = ex.Message;
				cr.error.details = ex.Message;
			}

			return Request.CreateResponse(HttpStatusCode.OK, cr, "application/json");
		}

		[System.Web.Http.HttpPost]
		//[Route("SearchDataApi")]
		//[ActionName("SearchDataApi")]
		public HttpResponseMessage SearchUdicCodeApi()
		{

			CommonResponse cr = new CommonResponse();
			try
			{
				var json = Request.Content.ReadAsStringAsync().Result;
				TempApiModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<TempApiModel>(json);

				if (!string.IsNullOrEmpty(model.SearchText))
				{
					DataSet ds = pObj.SearchUdicCodeByApi(model.SearchText);
					if (ds != null && ds.Tables.Count > 0)
					{


						cr.data = new
						{
							data = ds
						};
						cr.status = ResponseCode.Success;
						cr.message = "Data Found";


					}
					else
					{
						//cr.message = ErrorMessage.SomethingWrong;
						cr.message = "No Record Found";
						cr.error.details = "No Record Found";
					}
				}
				else
				{
					//cr.message = ErrorMessage.SomethingWrong;
					cr.message = "No Record Found";
					cr.error.details = "No Record Found";
				}
			}
			catch (Exception ex)
			{
				cr.message = ex.Message;
				cr.error.details = ex.Message;
			}

			return Request.CreateResponse(HttpStatusCode.OK, cr, "application/json");
		}





	}

}

