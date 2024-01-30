using CCA.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using Newtonsoft.Json;
using PSEBONLINE.AbstractLayer;
using PSEBONLINE.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace PSEBONLINE.Controllers
{
    public class Item
    {
        public string reference_no;
        public string order_no;
        public DateTime order_date_time;
        public string order_capt_amt;
        public string order_card_name;
        public string order_status;
    }
    public class TestcallapiController : Controller
    {
        // GET: Testcallapi
        public ActionResult Index()
        {

            callBackAPI("3015123993043", 1250);



            return View();
        }
        public bool callBackAPI(string orderid, int amount)
        {
            try
            {
                string accessCode = ConfigurationManager.AppSettings["CcAvenueAccessCode"];// ConfigurationManager.AppSettings["AWSAccessKey"];//from avenues
                string workingKey = ConfigurationManager.AppSettings["CcAvenueWorkingKey"];// from avenues

                string orderStatusQueryJson = "{ \"order_no\":" + orderid + " }"; //Ex. { "reference_no":"CCAvenue_Reference_No" , "order_no":"123456"} 
                string encJson = "";

                //string queryUrl = "https://login.ccavenue.com/apis/servlet/DoWebTrans";
                string queryUrl = "https://api.ccavenue.com/apis/servlet/DoWebTrans?";
                CCACrypto ccaCrypto = new CCACrypto();
                encJson = ccaCrypto.Encrypt(orderStatusQueryJson, workingKey);
                //ViewBag.result = encJson;
                // make query for the status of the order to ccAvenues change the command param as per your need
                //  string authQueryUrlParam = "enc_request=" + encJson + "&access_code=" + accessCode + "&command=orderStatusTracker&request_type=JSON&response_type=JSON&version=1.2";
                string authQueryUrlParam = "enc_request=" + encJson + "&access_code=" + accessCode + "&request_type=JSON&response_type=JSON&command=orderStatusTracker&version=1.2";
                // Url Connection
                String message = PaymentRequestToGateway(queryUrl, authQueryUrlParam);
                //ViewBag.result = message; 
                //Response.Write(message);
                NameValueCollection param = ResponseMap(message);
                ViewBag.result = "A" + encJson + "<br>" + message + "<br>" + param.Count;
                String status = "";
                String encResJson = "";
                if (param != null && param.Count == 2)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        if ("status".Equals(param.Keys[i]))
                        {
                            status = param[i];
                        }
                        if ("enc_response".Equals(param.Keys[i]))
                        {
                            encResJson += param[i];
                            ViewBag.result = "Final Result::" + ccaCrypto.Decrypt(encResJson, workingKey);
                            //Response.Write(encResXML);
                        }
                    }
                    if (!"".Equals(status) && status.Equals("0"))
                    {
                        String ResJson = ccaCrypto.Decrypt(encResJson, workingKey);
                        // Response.Write(ResJson);
                        // List<Item> items = JsonConvert.DeserializeObject<List<Item>>(ResJson);
                        string str = "";
                        dynamic jsonObj = JsonConvert.DeserializeObject(ResJson);
                        string reference_no = jsonObj.reference_no;

                        string order_no = jsonObj.order_no;

                        string order_capt_amt = jsonObj.order_capt_amt;
                        string order_card_name = jsonObj.order_card_name;
                        string order_status = jsonObj.order_status;
                        int vamt = 0;
                        try
                        {
                            vamt = Convert.ToInt32(order_capt_amt);
                        }
                        catch
                        {
                            vamt = 0;
                        }
                        if (vamt != amount && vamt != 0)
                        {
                            ViewBag.result = "Amount Mismatched";
                        }
                        else if (order_status == "Shipped")
                        {
                            DateTime order_date_time = jsonObj.order_date_time;
                            DataTable dt1 = new DataTable();

                            dt1.Columns.Add("CHALLANID");
                            dt1.Columns.Add("TOTFEE");
                            dt1.Columns.Add("BRCODE");
                            dt1.Columns.Add("BRANCH");
                            dt1.Columns.Add("PAYMETHOD");
                            dt1.Columns.Add("J_REF_NO");
                            dt1.Columns.Add("DEPOSITDT");
                            dt1.Columns.Add("PAYSTATUS");
                            dt1.AcceptChanges();
                            DataRow myDataRow = dt1.NewRow();
                            myDataRow["CHALLANID"] = order_no;
                            myDataRow["TOTFEE"] = order_capt_amt.ToString();
                            myDataRow["BRCODE"] = "301";
                            myDataRow["BRANCH"] = "HDFC";
                            myDataRow["PAYMETHOD"] = order_card_name;
                            myDataRow["J_REF_NO"] = reference_no;
                            myDataRow["DEPOSITDT"] = order_date_time.ToString("dd/MM/yyyy hh:mm:ss");
                            myDataRow["PAYSTATUS"] = "Success";
                            dt1.Rows.Add(myDataRow);
                            dt1.AcceptChanges();

                            AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                            BankModels BM = new BankModels();
                            BM.BCODE = "301";
                            BM.MIS_FILENM = "D"; int OutStatus = 0; string OutError = "";
                            DataTable dtResult = objDB.BulkOnlinePayment(dt1, 0, 0, BM, out OutStatus, out OutError);  // 
                            if (OutStatus > 0)
                                return true;
                            else
                                return false;
                        }
                        ViewBag.result = str + "<br>" + ResJson;
                        return false;
                        // return ResJson;
                    }
                    else if (!"".Equals(status) && status.Equals("1"))
                    {
                        Console.WriteLine("failure response from ccAvenues: " + encResJson);
                        ViewBag.result = "failure response from ccAvenues: " + encResJson;
                    }

                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private string PaymentRequestToGateway(String queryUrl, String urlParam)
        {

            String message = "";
            //  try
            {
                StreamWriter myWriter = null;// it will open a http connection with provided url
                WebRequest objRequest = WebRequest.Create(queryUrl);//send data using objxmlhttp object
                objRequest.Method = "POST";
                //objRequest.ContentLength = TranRequest.Length;
                objRequest.ContentType = "application/x-www-form-urlencoded";//to set content type
                myWriter = new System.IO.StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(urlParam);//send data
                myWriter.Close();//closed the myWriter object

                // Getting Response
                System.Net.HttpWebResponse objResponse = (System.Net.HttpWebResponse)objRequest.GetResponse();//receive the responce from objxmlhttp object 
                using (System.IO.StreamReader sr = new System.IO.StreamReader(objResponse.GetResponseStream()))
                {
                    message = sr.ReadToEnd();
                    //Response.Write(message);
                }
            }
            //catch (Exception exception)
            //{
            //    Console.Write("Exception occured while connection." + exception);
            //}
            return message;

        }

        private NameValueCollection ResponseMap(String message)
        {
            NameValueCollection Params = new NameValueCollection();
            if (message != null || !"".Equals(message))
            {
                string[] segments = message.Split('&');
                foreach (string seg in segments)
                {
                    string[] parts = seg.Split('=');
                    if (parts.Length > 0)
                    {
                        string Key = parts[0].Trim();
                        string Value = parts[1].Trim();
                        Params.Add(Key, Value);
                    }
                }
            }
            return Params;
        }

        public ActionResult ATOM()
        {
            callBackAPIATOM();
            return View();
        }
        public static string ATOMbyteToHexString(byte[] byData)
        {
            StringBuilder sb = new StringBuilder((byData.Length * 2));
            for (int i = 0; (i < byData.Length); i++)
            {
                int v = (byData[i] & 255);
                if ((v < 16))
                {
                    sb.Append('0');
                }

                sb.Append(v.ToString("X"));

            }
            return sb.ToString();
        }
        public bool callBackAPIATOM()
        {
            //try
            //{

            NameValueCollection nvc = Request.Form;
            string fileName = @"C:\inetpub\wwwroot\PSEB\registration2023.pseb.ac.in\Mahesh.txt";

            try
            {


                // Create a new file     
                using (FileStream fs = System.IO.File.Create(fileName))
                {
                    // Add some text to file    
                    Byte[] title = new UTF8Encoding(true).GetBytes(nvc.ToString());
                    fs.Write(title, 0, title.Length);
                    return false;

                }

                // Open the stream and read it back.    

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }




            if (Request.Params["mmp_txn"] != null)
            {
                string postingmmp_txn = Request.Params["mmp_txn"].ToString();
                string postingmer_txn = Request.Params["mer_txn"].ToString();
                string postinamount = Request.Params["amt"].ToString();
                string postingprod = Request.Params["prod"].ToString();
                string postingdate = Request.Params["date"].ToString();
                string postingbank_txn = Request.Params["bank_txn"].ToString();
                string postingf_code = Request.Params["f_code"].ToString();
                string postingbank_name = Request.Params["bank_name"].ToString();
                string signature = Request.Params["signature"].ToString();
                string postingdiscriminator = Request.Params["discriminator"].ToString();

                string respHashKey = ConfigurationManager.AppSettings["ATOMRespHashKey"].ToString();
                string ressignature = "";
                string strsignature = postingmmp_txn + postingmer_txn + postingf_code + postingprod + postingdiscriminator + postinamount + postingbank_txn;
                byte[] bytes = Encoding.UTF8.GetBytes(respHashKey);
                byte[] b = new System.Security.Cryptography.HMACSHA512(bytes).ComputeHash(Encoding.UTF8.GetBytes(strsignature));
                ressignature = ATOMbyteToHexString(b).ToLower();
                try
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand("insert into calllog(resonse)values('" + postingmmp_txn + "|" + postinamount + "|" + postingbank_txn + "|" + postingdate + "|" + postingf_code + "')", con); // BulkChallanBankSPTest
                        cmd.CommandType = CommandType.Text;

                        con.Open();
                        cmd.ExecuteNonQuery();

                    }
                }

                catch (Exception ex)
                {

                }
                //Status 
                if (signature == ressignature)
                {
                    if (postingf_code.ToLower() == "ok")
                    {

                        DataTable dt1 = new DataTable();

                        dt1.Columns.Add("CHALLANID");
                        dt1.Columns.Add("TOTFEE");
                        dt1.Columns.Add("BRCODE");
                        dt1.Columns.Add("BRANCH");
                        dt1.Columns.Add("J_REF_NO");
                        dt1.Columns.Add("DEPOSITDT");
                        dt1.Columns.Add("Debit Card");
                        dt1.Columns.Add("PAYSTATUS");
                        dt1.AcceptChanges();
                        DataRow myDataRow = dt1.NewRow();
                        myDataRow["CHALLANID"] = postingmer_txn;
                        myDataRow["TOTFEE"] = postinamount.ToString();
                        myDataRow["BRCODE"] = "302";
                        myDataRow["BRANCH"] = "ATOM";
                        myDataRow["J_REF_NO"] = postingbank_txn;
                        myDataRow["DEPOSITDT"] = postingdate;
                        myDataRow["Debit Card"] = postingf_code;
                        myDataRow["PAYSTATUS"] = "Success";
                        dt1.Rows.Add(myDataRow);
                        dt1.AcceptChanges();

                        AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                        BankModels BM = new BankModels();
                        BM.BCODE = "301";
                        BM.MIS_FILENM = "D"; int OutStatus = 0; string OutError = "";
                        DataTable dtResult = objDB.BulkOnlinePayment(dt1, 0, 0, BM, out OutStatus, out OutError);  // 
                        if (OutStatus > 0)
                            return true;
                        else
                            return false;
                    }
                    else { return false; }

                }
                else { return false; }
            }
            else { return false; }

            //}
            //catch
            //{
            //    return false;
            //}
        }

    }
}