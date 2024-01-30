using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSEBONLINE.AbstractLayer;
using PSEBONLINE.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PSEBONLINE.Controllers
{
    public class TestController : Controller
    {
        private DBContext _context = new DBContext();

        public string Generatehash512(string text)
        {

            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;

        }


        public string GetTxnStatus(string TransactionID)
        {
            string Url = "https://info.payu.in/merchant/postservice?form=2";
            string method = "verify_payment";
            string salt = "BskWEhiW";
            string key = "I2w5wx";
            //string var1 = "77700026F713588A09466596";//(Failure)
            string var1 = "77700040A8EEBF6D14088757";//(Cancelled)            
            //string var1 = "88800008070920200154";//(SUCCESS)

            //string var1 = "77700026F713588A09466596|77700040A8EEBF6D14088757|88800017BCC1A58309391895";//Transaction ID of the merchant

            string toHash = key + "|" + method + "|" + var1 + "|" + salt;

            string Hashed = Generatehash512(toHash);

            string postString = "key=" + "I2w5wx" +
                "&command=" + method +
                "&hash=" + Hashed +
                "&var1=" + var1;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


            WebRequest myWebRequest = WebRequest.Create(Url);
            myWebRequest.Method = "POST";
            myWebRequest.ContentType = "application/x-www-form-urlencoded";         
            myWebRequest.Timeout = 180000;
            StreamWriter requestWriter = new StreamWriter(myWebRequest.GetRequestStream());
            requestWriter.Write(postString);
            requestWriter.Close();

            //StreamReader readStream = new StreamReader(myWebRequest.GetResponse().GetResponseStream());
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Stream ReceiveStream = myWebResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(ReceiveStream, Encoding.UTF8);
            string response = readStream.ReadToEnd();

            JObject account = JObject.Parse(response);
            string status = (string)account.SelectToken("transaction_details." + var1 + ".status");
            PayUVerifyResponseModel payUVerifyResponseModel = JsonConvert.DeserializeObject<PayUVerifyResponseModel>(response);
            if(payUVerifyResponseModel != null)
            {

                if (payUVerifyResponseModel.Status == 1)
                {
                    PayUTransactionDetails payUTransactionDetails = new PayUTransactionDetails()
                    {
                        Mihpayid = (string)account.SelectToken("transaction_details." + var1 + ".mihpayid"),
                        Amt = (string)account.SelectToken("transaction_details." + var1 + ".amt"),
                        Txnid = (string)account.SelectToken("transaction_details." + var1 + ".txnid"),
                        Productinfo = (string)account.SelectToken("transaction_details." + var1 + ".productinfo"),
                        Mode = (string)account.SelectToken("transaction_details." + var1 + ".mode"),
                        Status = (string)account.SelectToken("transaction_details." + var1 + ".status"),
                        RequestId = (string)account.SelectToken("transaction_details." + var1 + ".request_id"),
                        BankRefNum = (string)account.SelectToken("transaction_details." + var1 + ".bank_ref_num"),
                        TransactionAmount = (string)account.SelectToken("transaction_details." + var1 + ".transaction_amount   "),
                        AdditionalCharges = (string)account.SelectToken("transaction_details." + var1 + ".additional_charges"),
                        Firstname = (string)account.SelectToken("transaction_details." + var1 + ".firstname"),
                        Bankcode = (string)account.SelectToken("transaction_details." + var1 + ".bankcode"),
                        Udf1 = (string)account.SelectToken("transaction_details." + var1 + ".udf1"),
                        Udf2 = (string)account.SelectToken("transaction_details." + var1 + ".udf2"),
                        Udf3 = (string)account.SelectToken("transaction_details." + var1 + ".udf3"),
                        Udf4 = (string)account.SelectToken("transaction_details." + var1 + ".udf4"),
                        Udf5 = (string)account.SelectToken("transaction_details." + var1 + ".udf5"),
                        ErrorCode = (string)account.SelectToken("transaction_details." + var1 + ".error_code"),
                        ErrorMessage  = (string)account.SelectToken("transaction_details." + var1 + ".error_Message"),
                        Unmappedstatus = (string)account.SelectToken("transaction_details." + var1 + ".unmappedstatus"),


                    };
                }



            }
            //
            return status;
        }


        // GET: Test
        public ActionResult Index()
        {

            int emploginStatus1 = 0;
            try
            {

                // https://ulbhryndc.org/api/Payment/GetPaymentDetailsforVendor?McCode=050&Date=2021-11-30
                //API_VENSONIPAT: SUSrS2F7Ju

                HttpClientHandler handler = new HttpClientHandler();
                HttpClient client = new HttpClient(handler);

                string AdminEmployeeUserId = "API_VENSONIPAT";
                string AdminEmployeePassword = "SUSrS2F7Ju";

                var authenticationString = $"{AdminEmployeeUserId}:{AdminEmployeePassword}";

              //  System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                 ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization",
                            Convert.ToBase64String(Encoding.Default.GetBytes(authenticationString)));
                //Need to change the PORT number where your WEB API service is running         
                string url = String.Format("https://ulbhryndc.org/api/Payment/GetPaymentDetailsforVendor?McCode=050&Date=2021-11-30");
                var result = client.GetAsync(new Uri(url)).Result;
                if (result.IsSuccessStatusCode)
                {
                    // Console.WriteLine("Done" + result.StatusCode);
                    var JsonContent = result.Content.ReadAsStringAsync().Result;
                    var readTask = result.Content.ReadAsAsync<dynamic>();

                   // DataTable dt = readTask.c

                    // var readTask = result.Content.ReadAsAsync<AdminEmployeeAPIModel>();
                    readTask.Wait();
                    emploginStatus1 = 1;
                }
                else
                {
                    emploginStatus1 = -1;
                }
            }
            catch (Exception ex1 )
            {

                
            }
            //var address = "Stavanger, Norway";

            //var locationService = new GoogleLocationService();
            //var point = locationService.GetLatLongFromAddress(address);

            //var latitude = point.Latitude;
            //var longitude = point.Longitude;

            // Save lat/long values to DB...

            //  string result = GetTxnStatus("77700040A8EEBF6D14088757");


            //List<OpenUserLogin> openUserLogin = _context.OpenUserLogin.ToList();
            //if (openUserLogin == null)
            //{
            //    return HttpNotFound();
            //}

            //PaymentSuccessModel _PaymentSuccessModel = new PaymentSuccessModel();

            //_PaymentSuccessModel = new PaymentSuccessModel()
            //{
            //    order_id = "3014020500001",
            //    tracking_id = "309006206602",
            //    amount = "5050.00",
            //    trans_date = "16/07/2020 16:19:56",
            //    bank_ref_no = "1594896557975",
            //    order_status = "success".ToLower(),
            //    payment_mode = "Net Banking",
            //    merchant_param1 = "",
            //    bankname = "HDFC",
            //    bankcode = "301",

            //};

            //int OutStatus;      
            //string Mobile = "0";

            //try
            //{
            //    string dtResult2 = GatewayService.InsertOnlinePaymentMIS(_PaymentSuccessModel, out OutStatus, out Mobile);// OutStatus mobile
            //    if (OutStatus == 1)
            //    {
            //        //send sms and email
            //        string Sms = "You have paid Rs. " + _PaymentSuccessModel.amount + " against challan no " + _PaymentSuccessModel.order_id + " on " + _PaymentSuccessModel.trans_date + " with ref no " + _PaymentSuccessModel.tracking_id + ". Regards PSEB";
            //        try
            //        {
            //            string getSms = new PSEBONLINE.AbstractLayer.DBClass().gosms(Mobile, Sms);


            //        }
            //        catch (Exception) { }
            //    }
            //}
            //catch (Exception)
            //{
            //}
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {

                string filename = Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);            
                string fileLocation = "";
                fileLocation = Server.MapPath("~/Upload/" + filename + extension);
                postedFile.SaveAs(fileLocation);

                string conString = string.Empty;
                conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                               fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                                fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                string table = "PB_marksheet_HSCER2018";
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[""].ToString()))
                {                 
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = table;
                        conn.Open();
                        var schema = conn.GetSchema("Columns", new[] { null, null, table, null });

                        DataColumnCollection columnsCol = dt.Columns;//12
                        DataRowCollection rowCol = schema.Rows;//255

                        foreach (DataRow row in schema.Rows)//255
                        {
                            string colnm = (string)row["COLUMN_NAME"];
                            var myColumn = dt.Columns.Cast<DataColumn>().SingleOrDefault(col => col.ColumnName.ToLower().Trim() == colnm.ToLower().Trim());
                            if (myColumn != null)
                            {
                                int index = dt.Columns[colnm].Ordinal;
                                string sourceColNM = dt.Columns[index].ColumnName;
                                bulkCopy.ColumnMappings.Add(sourceColNM, (string)row["COLUMN_NAME"]);
                            }
                        }


                        //string colnm = row1[0].Table.Columns[0].ColumnName;

                        //foreach (DataColumn sourceColumn in dt.Columns)//255
                        //{
                           

                        //    foreach (DataRow row in schema.Rows)//255
                        //    {
                        //        string colnm = (string)row["COLUMN_NAME"];
                        //        if (columnsCol.Contains(colnm))
                        //        {
                        //            int index = dt.Columns[colnm].Ordinal;                                  
                        //            string sourceColNM = dt.Columns[index].ColumnName;
                        //            bulkCopy.ColumnMappings.Add(sourceColNM, (string)row["COLUMN_NAME"]);
                        //            break;
                        //        }
                        //    }
                        //    //foreach (DataRow row in schema.Rows)//255
                        //    //{
                        //    //    if (string.Equals(sourceColumn.ColumnName, (string)row["COLUMN_NAME"], StringComparison.OrdinalIgnoreCase))
                        //    //    {
                        //    //        bulkCopy.ColumnMappings.Add(sourceColumn.ColumnName, (string)row["COLUMN_NAME"]);
                        //    //        break;
                        //    //    }
                        //    //}
                        //}
                        bulkCopy.WriteToServer(dt);
                    }
                }
            }

            return View();
        }


    


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}