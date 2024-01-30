using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using PSEBONLINE.Models;
using System.IO;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.Data.Odbc;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PSEBONLINE.AbstractLayer
{
    public class PsebAPIServiceDB : IDisposable
    {
        private static HttpClient _httpClient = null;
        private static readonly object threadlock = new object();

        public List<BarAPIModel> GetBar12DetailsByBarCodeAPI(string allbarcode)
        {
            List<BarAPIModel> barAPIModelsList = new List<BarAPIModel>();
            try
            {
                //https://data.indiaresults.com/irc1/pseb/psebapi.php?barID=1000001
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["RECHECKBARAPI"]);
                    //HTTP GET
                    //var responseTask = client.GetAsync("?barID=" + allbarcode);
                    var responseTask = client.GetAsync("?barID=" + allbarcode);

                    List<System.Net.Http.Formatting.MediaTypeFormatter> formatters = new List<System.Net.Http.Formatting.MediaTypeFormatter>();
                    formatters.Add(new System.Net.Http.Formatting.JsonMediaTypeFormatter());


                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        string readTask11 = result.Content.ReadAsStringAsync().Result;
                        //For multiple
                        barAPIModelsList = JsonConvert.DeserializeObject<List<BarAPIModel>>(readTask11);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return barAPIModelsList;
        }

        public SchoolApiViewModel UpdateUSIPSEBMainToPsebJunior(SchoolModels model)
        {
            SchoolApiViewModel savm = new SchoolApiViewModel();
            string status = "0";
            try
            {
                using (var client = new HttpClient())
                {

                    //string url = "https://localhost:57470/api/";
                    //client.BaseAddress = new Uri(url);

                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["PSEBAPI"]);

                    //HTTP POST
                    var putTask = client.PutAsJsonAsync<SchoolModels>("School/UpdateUSIPSEBMainToPsebJunior/@pSeb4395m/" + model.SCHL, model);
                    putTask.Wait();

                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<SchoolApiViewModel>();
                        readTask.Wait();

                        if (readTask.Result.statusCode == "200")
                        {
                            savm.Object = readTask.Result.Object;
                            savm.statusCode = readTask.Result.statusCode;
                            savm.Success = readTask.Result.Success;
                            status = "200";
                        }
                        else
                        {
                            savm.Object = readTask.Result.Object;
                            savm.statusCode = readTask.Result.statusCode;
                            savm.Success = readTask.Result.Success;
                            status = readTask.Result.statusCode;
                        }
                    }
                    else
                    {
                        savm.Object = null;
                        savm.statusCode = "000";
                        savm.Success = "000";
                        status = "000";
                    }
                }
            }
            catch (Exception ex)
            {
                savm.Object = null;
                savm.Success = ex.Message;
                string err = ex.Message;
                if (err.Contains("OutOfMemoryException") || err.ToLower().Contains("one or more errors"))
                {
                    status = "200";
                    savm.statusCode = "200";
                }
                else
                {
                    status = ex.Message;
                    savm.statusCode = "404";
                }
            }
            return savm;
        }


        public async Task<SchoolApiViewModel> UpdateSMFPSEBMainToPsebJunior(SchoolModels model)
        {
            string status = "0";
            SchoolModelAPI smApi = new SchoolModelAPI();
            SchoolApiViewModel savm = new SchoolApiViewModel();
            try
            {
                using (var client = new HttpClient())
                {

                    //string url = "https://localhost:57470/api/";
                    //client.BaseAddress = new Uri(url);


                    //string url = "https://api.psebonline.in/api/";
                    //client.BaseAddress = new Uri(url);

                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["PSEBAPI"]);

                    //HTTP POST
                    var putTask = client.PutAsJsonAsync<SchoolModels>("School/UpdateSMFPSEBMainToPsebJunior/@pSeb4395m/" + model.SCHL, model);
                    putTask.Wait();

                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = await result.Content.ReadAsAsync<SchoolApiViewModel>();
                        //readTask.Wait();
                        if (readTask.statusCode == "200")
                        {
                            savm.Object = readTask.Object;
                            savm.statusCode = readTask.statusCode;
                            savm.Success = readTask.Success;
                            status = "200";
                        }
                        else
                        {
                            savm.Object = readTask.Object;
                            savm.statusCode = readTask.statusCode;
                            savm.Success = readTask.Success;
                            status = readTask.statusCode;
                        }
                    }
                    else
                    {
                        savm.Object = null;
                        savm.statusCode = "000";
                        savm.Success = "000";
                        status = "000";
                    }
                }
            }
            catch(Exception ex)
            {
                savm.Object = null;             
                savm.Success = ex.Message;
                string err = ex.Message;
                if (err.Contains("OutOfMemoryException") || err.ToLower().Contains("one or more errors"))
                {
                    status = "200";
                    savm.statusCode = "200";
                }
                else
                {
                    status = ex.Message;
                    savm.statusCode = "404";
                }
               
            }
            return savm;
        }


        public SchoolApiViewModel InsertSMFPSEBMainToPsebJunior(SchoolModels model, out string newschlcode)
        {
            string status = "0";
            SchoolApiViewModel savm = new SchoolApiViewModel();
            try
            {
                using (var client = new HttpClient())
                {

                    //string url = "https://localhost:57470/api/";
                    //client.BaseAddress = new Uri(url);

                    //<add key="PSEBAPI" value="https://api.psebonline.in/api/" />

                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["PSEBAPI"]);

                    //HTTP POST
                    var putTask = client.PostAsJsonAsync<SchoolModels>("School/InsertSMFPSEBMainToPsebJunior/@pSeb4395m/", model);
                    putTask.Wait();

                    var result = putTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<SchoolApiViewModel>();
                        readTask.Wait();

                        if (readTask.Result.statusCode == "200")
                        {
                            savm.Object = readTask.Result.Object;
                            savm.statusCode = readTask.Result.statusCode;
                            savm.Success = readTask.Result.Success;
                            status = "200";
                        }
                        else
                        {
                            savm.Object = readTask.Result.Object;
                            savm.statusCode = readTask.Result.statusCode;
                            savm.Success = readTask.Result.Success;
                            status = readTask.Result.statusCode;
                        }
                    }
                    else
                    {
                        savm.Object = null;
                        savm.statusCode = "000";
                        savm.Success = "000";
                        status = "000";
                        model = null;
                    }
                }
                newschlcode = model.SCHL;

            }
            catch (Exception ex)
            {
                newschlcode = "";
                savm.Object = null;
                savm.Success = ex.Message;
                string err = ex.Message;
                if (err.Contains("OutOfMemoryException") || err.ToLower().Contains("one or more errors"))
                {
                    status = "200";
                    savm.statusCode = "200";
                }
                else
                {
                    status = ex.Message;
                    savm.statusCode = "404";
                }
            }

            return savm;

        }



        public async Task<string> SchoolChangePasswordPSEBMAIN(SchoolChangePasswordModel model)
        {
            string status = "0";
            try
            {

                // string url = "https://localhost:57470/api/";
                lock (threadlock)
                {
                    if (_httpClient == null)
                    {
                        _httpClient = new HttpClient();
                        // _httpClient.BaseAddress = new Uri(url);
                        _httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["PSEBAPI"]);
                    }
                }
                var putTask = await _httpClient.PutAsJsonAsync<SchoolChangePasswordModel>("School/SchoolChangePasswordPSEBMAIN/@pSeb4395m/" + model.SCHL, model);
                var result = putTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<SchoolPasswordAPIViewModel>();
                    readTask.Wait();

                    if (readTask.Result.statusCode == "200")
                    {
                        model = readTask.Result.Object;
                        status = "200";
                    }
                    else
                    {
                        model = readTask.Result.Object;
                        status = readTask.Result.statusCode;
                    }
                }
                else
                {
                    model = null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return status;
        }



        //Result api  ( insert)

        public ResultAPIViewModel InsertResultDataAPI()
        {
            string status = "0";
            ResultAPIViewModel savm = new ResultAPIViewModel();
            try
            {
                using (var client = new HttpClient())
                {
                   
                    client.BaseAddress = new Uri(ConfigurationManager.AppSettings["PSEBAPI"]);
                    //  DataTable dt = Models.ResultDataCS.gDataSet1().Tables[0];
                    List<ResultApiMasters> listData = new List<ResultApiMasters>();

                    if (listData.Count > 0)
                    {
                        var dataAsString = JsonConvert.SerializeObject(listData);
                        var dataContent = new StringContent(dataAsString);


                        //HTTP POST
                        var putTask = client.PostAsJsonAsync("Result/InsertResultData?eKey=@pSeb4395m", dataContent);
                        putTask.Wait();

                        var result = putTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<ResultAPIViewModel>();
                            readTask.Wait();

                            if (readTask.Result.statusCode == "200")
                            {
                                savm.Object = readTask.Result.Object;
                                savm.statusCode = readTask.Result.statusCode;
                                savm.Success = readTask.Result.Success;
                                status = "200";
                            }
                            else
                            {
                                savm.Object = readTask.Result.Object;
                                savm.statusCode = readTask.Result.statusCode;
                                savm.Success = readTask.Result.Success;
                                status = readTask.Result.statusCode;
                            }
                        }
                        else
                        {
                            savm.Object = null;
                            savm.statusCode = "000";
                            savm.Success = "000";
                            status = "000";                          
                        }
                    }
                }
               

            }
            catch (Exception ex)
            {               
                savm.Object = null;
                savm.Success = ex.Message;
                string err = ex.Message;
                if (err.Contains("OutOfMemoryException") || err.ToLower().Contains("one or more errors"))
                {
                    status = "200";
                    savm.statusCode = "200";
                }
                else
                {
                    status = ex.Message;
                    savm.statusCode = "404";
                }
            }

            return savm;

        }




        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_httpClient != null)
                {
                    _httpClient.Dispose();
                }
            }
        }

        ~PsebAPIServiceDB()
        {
            Dispose(false);
        }

    }
}