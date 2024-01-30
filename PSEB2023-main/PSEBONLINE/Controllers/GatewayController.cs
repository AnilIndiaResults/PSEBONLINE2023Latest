using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.Configuration;
using CCA.Util;
using System.Reflection;
using System.Data;
using PSEBONLINE.AbstractLayer;
using System.Text;
using System.Collections.Specialized;
using System.Net.Http;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Net;
using System.Threading.Tasks;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace PsebPrimaryMiddle.Controllers
{
    public class GatewayController : Controller
    {
        //string AccessCode = ConfigurationManager.AppSettings["CcAvenueAccessCode"];
        //string CheckoutUrl = ConfigurationManager.AppSettings["CcAvenueCheckoutUrl"];
        //string WorkingKey = ConfigurationManager.AppSettings["CcAvenueWorkingKey"];
        //string MerchantId = ConfigurationManager.AppSettings["CcAvenueMerchantId"];
        private readonly DBContext _context = new DBContext();

        public ActionResult Index()
        {
            return View();
        }


        #region ATOM PG


        public ActionResult AtomCheckoutUrl(string ChallanNo, string amt, string clientCode, string cmn, string cme, string cmno)
        {
            string strURL;
            string MerchantLogin = ConfigurationManager.AppSettings["ATOMLoginId"].ToString();
            string MerchantPass = ConfigurationManager.AppSettings["ATOMPassword"].ToString();
            string MerchantDiscretionaryData = "NB";  // for netbank
            //string ClientCode = "PSEBONLINE";
            string ClientCode = clientCode;
            string ProductID = ConfigurationManager.AppSettings["ATOMProductID"].ToString();
            string CustomerAccountNo = "0123456789";
            string TransactionType = "NBFundTransfer";  // for netbank
                                                        //string TransactionAmount = "1";
            string TransactionAmount = encrypt.QueryStringModule.Decrypt(amt);
            // string TransactionAmount = "100";
            string TransactionCurrency = "INR";
            string TransactionServiceCharge = "0";
            string TransactionID = encrypt.QueryStringModule.Decrypt(ChallanNo);
            string TransactionDateTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
            // string TransactionDateTime = "18/10/2019 13:15:19";
            string BankID = "ATOM";
            string ru = ConfigurationManager.AppSettings["ATOMRU"].ToString();
            // User Details
            string udf1CustName = encrypt.QueryStringModule.Decrypt(cmn);
            string udf2CustEmail = !string.IsNullOrEmpty(cme) ? cme : "";
            string udf3CustMob = encrypt.QueryStringModule.Decrypt(cmno);

            strURL = GatewayController.ATOMTransferFund(MerchantLogin, MerchantPass, MerchantDiscretionaryData, ProductID, ClientCode, CustomerAccountNo, TransactionType,
              TransactionAmount, TransactionCurrency, TransactionServiceCharge, TransactionID, TransactionDateTime, BankID, ru, udf1CustName, udf2CustEmail, udf3CustMob);


            if (!string.IsNullOrEmpty(strURL))
            {
                return View(new AtomViewModel(strURL));
            }


            return Redirect(strURL);
        }




        public static string Encrypt(string plainText, string passphrase, string salt, Byte[] iv, int iterations)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            string data = ByteArrayToHexString(Encrypt(plainBytes, GetSymmetricAlgorithm(passphrase, salt, iv, iterations))).ToUpper();

            return data;
        }

        public static byte[] Encrypt(byte[] plainBytes, SymmetricAlgorithm sa)
        {
            return sa.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        }
        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }


        public static SymmetricAlgorithm GetSymmetricAlgorithm(String passphrase, String salt, Byte[] iv, int iterations)
        {
            var saltBytes = new byte[16];
            var ivBytes = new byte[16];
            Rfc2898DeriveBytes rfcdb = new System.Security.Cryptography.Rfc2898DeriveBytes(passphrase, Encoding.UTF8.GetBytes(salt), iterations);
            saltBytes = rfcdb.GetBytes(32);
            var tempBytes = iv;
            Array.Copy(tempBytes, ivBytes, Math.Min(ivBytes.Length, tempBytes.Length));
            var rij = new RijndaelManaged(); //SymmetricAlgorithm.Create();
            rij.Mode = CipherMode.CBC;
            rij.Padding = PaddingMode.PKCS7;
            rij.FeedbackSize = 128;
            rij.KeySize = 128;

            rij.BlockSize = 128;
            rij.Key = saltBytes;
            rij.IV = ivBytes;
            return rij;
        }

        public static string ATOMTransferFund(string MerchantLogin, string MerchantPass, string MerchantDiscretionaryData, string ProductID, string ClientCode, string CustomerAccountNo, string TransactionType, string TransactionAmount, string TransactionCurrency,
                                           string TransactionServiceCharge, string TransactionID, string TransactionDateTime, string BankID, string returnURL, string udf1CustName, string udf2CustEmail, string udf3CustMob)
        {

            string strURL, strClientCode, strClientCodeEncoded;
            byte[] b;
            string strResponse = "";
            try
            {




                b = Encoding.UTF8.GetBytes(ClientCode);
                strClientCode = Convert.ToBase64String(b);
                strClientCodeEncoded = HttpUtility.UrlEncode(strClientCode);
                strURL = "" + ConfigurationManager.AppSettings["ATOMTransferURL"].ToString();///
                strURL = strURL.Replace("[MerchantLogin]", MerchantLogin + "&");
                strURL = strURL.Replace("[MerchantPass]", MerchantPass + "&");
                strURL = strURL.Replace("[TransactionType]", TransactionType + "&");
                strURL = strURL.Replace("[ProductID]", ProductID + "&");
                strURL = strURL.Replace("[TransactionAmount]", TransactionAmount + "&");
                strURL = strURL.Replace("[TransactionCurrency]", TransactionCurrency + "&");
                strURL = strURL.Replace("[TransactionServiceCharge]", TransactionServiceCharge + "&");
                strURL = strURL.Replace("[ClientCode]", strClientCodeEncoded + "&");
                strURL = strURL.Replace("[TransactionID]", TransactionID + "&");
                strURL = strURL.Replace("[TransactionDateTime]", TransactionDateTime + "&");
                strURL = strURL.Replace("[CustomerAccountNo]", CustomerAccountNo + "&");
                strURL = strURL.Replace("[ru]", returnURL + "&");// Remove on Production
                                                                 //*****************
                string reqHashKey = ConfigurationManager.AppSettings["ATOMReqHashKey"].ToString();
                string signature = "";
                string strsignature = MerchantLogin + MerchantPass + TransactionType + ProductID + TransactionID + TransactionAmount + TransactionCurrency;
                byte[] bytes = Encoding.UTF8.GetBytes(reqHashKey);
                byte[] bt = new System.Security.Cryptography.HMACSHA512(bytes).ComputeHash(Encoding.UTF8.GetBytes(strsignature));
                signature = ATOMbyteToHexString(bt).ToLower();
                strURL = strURL.Replace("[signature]", signature + "&");
                strURL = strURL.Replace("[udf1]", udf1CustName + "&");
                strURL = strURL.Replace("[udf2]", udf2CustEmail + "&");
                strURL = strURL.Replace("[udf3]", udf3CustMob);


                string passphrase = ConfigurationManager.AppSettings["ATOMPassphraseRequest"].ToString();
                string salt = passphrase;
                byte[] iv = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                int iterations = 65536;

                var enc_request = Encrypt(strURL, passphrase, salt, iv, iterations);
                //var enc_request = decryption.Encrypt(strURL, "1E713E5B4F546BA92815D1BD25C17752");
                string s = "?login=111512&encdata=";
                string sURLWithEnc = "" + ConfigurationManager.AppSettings["ATOMPaymentURL"].ToString() + s + enc_request;
                strURL = sURLWithEnc;

                return strURL;
            }
            catch (Exception ex)
            {
                strURL = "Exception encountered. " + ex.Message;
                return strURL;
            }

        }

        public ActionResult ATOMPaymentResponse(string id)
        {
            return View();
        }


        [HttpPost]
        public ActionResult ATOMPaymentResponse()
        {
            PaymentSuccessModel _PaymentSuccessModel = new PaymentSuccessModel();
            try
            {

                NameValueCollection nvc = Request.Form;

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


                    //Status 
                    if (signature == ressignature)
                    {
                        ViewBag.lblStatus = "Signature matched...";

                        if (postingf_code.ToLower() == "ok")
                        {
                            ViewData["response"] = "success";
                            _PaymentSuccessModel = new PaymentSuccessModel()
                            {
                                order_id = postingmer_txn,
                                tracking_id = postingmmp_txn,
                                amount = postinamount,
                                trans_date = postingdate,
                                bank_ref_no = postingbank_txn,
                                order_status = "success",
                                payment_mode = postingf_code,
                                merchant_param1 = postingprod,
                                bankname = "ATOM",
                                bankcode = "302",
                            };

                            // Update Data in Challan Master
                            int OutStatus;
                            string Mobile = "0";
                            string feecode = _PaymentSuccessModel.order_id.Substring(3, 2);
                            ViewData["feecodePG"] = feecode;

                            try
                            {
                                string OutError, OutSCHLREGID = "", OutAPPNO = "";

                                string dtResult2 = GatewayService.InsertOnlinePaymentMIS(_PaymentSuccessModel, out OutStatus, out Mobile, out OutError, out OutSCHLREGID, out OutAPPNO);// OutStatus mobile
                                ViewData["OutErrorPG"] = OutError;
                                //ViewData["OutSCHLREGID"] = OutSCHLREGID;
                                //ViewData["OutAPPNO"] = OutAPPNO;

                                if (OutStatus == 1)
                                {
                                    //send sms and email
                                    string Sms = "You have paid Rs. " + _PaymentSuccessModel.amount + " against challan no " + _PaymentSuccessModel.order_id + " on " + _PaymentSuccessModel.trans_date + " with ref no " + _PaymentSuccessModel.tracking_id + ". Regards PSEB";
                                    try
                                    {
                                        //string getSms = new PSEBONLINE.AbstractLayer.DBClass().gosms(Mobile, Sms);


                                    }
                                    catch (Exception) { }
                                }
                            }
                            catch (Exception)
                            {
                            }


                        }
                        else if (postingf_code.ToLower() == "f")
                        {
                            ViewData["response"] = "failure";
                            _PaymentSuccessModel = new PaymentSuccessModel()
                            {
                                order_id = postingmer_txn,
                                tracking_id = postingmmp_txn,
                                amount = postinamount,
                                trans_date = postingdate,
                                bank_ref_no = postingbank_txn,
                                order_status = "failure",
                                payment_mode = postingf_code,
                                merchant_param1 = postingprod,
                                bankname = "ATOM",
                                bankcode = "302",
                            };

                            int OutStatus;
                            string Mobile = "0";
                            string feecode = _PaymentSuccessModel.order_id.Substring(3, 2);
                            ViewData["feecodePG"] = feecode;

                            try
                            {
                                string OutError, OutSCHLREGID = "", OutAPPNO = "";
                                string dtResult2 = GatewayService.InsertOnlinePaymentMIS(_PaymentSuccessModel, out OutStatus, out Mobile, out OutError, out OutSCHLREGID, out OutAPPNO);// OutStatus mobile
                                ViewData["OutErrorPG"] = OutError;
                                //ViewData["OutSCHLREGID"] = OutSCHLREGID;
                                //ViewData["OutAPPNO"] = OutAPPNO;
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (postingf_code.ToLower() == "c")
                        {
                            ViewData["response"] = "cancel";
                            _PaymentSuccessModel = new PaymentSuccessModel()
                            {
                                order_id = postingmer_txn,
                                tracking_id = postingmmp_txn,
                                amount = postinamount,
                                trans_date = postingdate,
                                bank_ref_no = postingbank_txn,
                                order_status = "cancel",
                                payment_mode = postingf_code,
                                merchant_param1 = postingprod,
                                bankname = "ATOM",
                                bankcode = "302",
                            };

                            string feecode = _PaymentSuccessModel.order_id.Substring(3, 2);
                            ViewData["feecodePG"] = feecode;
                        }



                        try
                        {
                            int OutStatusAll;
                            string MobileAll = "0";
                            string OutErrorAll, OutSCHLREGIDAll = "", OutAPPNOAll = "";
                            string dtResultAll = GatewayService.InsertOnlinePaymentMIS_ALLTrans(_PaymentSuccessModel, out OutStatusAll, out MobileAll, out OutErrorAll, out OutSCHLREGIDAll, out OutAPPNOAll);// OutStatus mobile

                        }
                        catch (Exception ex2)
                        {
                        }

                        ChallanMasterModel challanMasterModel = GatewayService.GetAnyChallanDetailsById(_PaymentSuccessModel.order_id.ToString());
                        if (challanMasterModel != null)
                        {
                            _PaymentSuccessModel.FEECODE = challanMasterModel.FEECODE;
                            _PaymentSuccessModel.APPNO = challanMasterModel.APPNO;
                            _PaymentSuccessModel.SCHLREGID = challanMasterModel.SCHLREGID;
                        }

                    }
                    else
                    {
                        ViewData["response"] = "failed";
                        ViewBag.lblStatus = "Signature Mismatched...";
                    }
                }
                return View(_PaymentSuccessModel);
            }

            catch (Exception ex)
            {

            }
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
        #endregion ATOM PG



        #region CcAvenue HDFC


        public ActionResult CcAvenue()
        {
            return View();
        }

        public ActionResult Payment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Payment(string invoiceNumber)
        {
            invoiceNumber = DateTime.Now.ToString("yyMMddHHmmssff"); // must be challan id
            string amount = "2";
            var queryParameter = new CCACrypto();
            //CCACrypto is the dll you get when you download the ASP.NET 3.5 integration kit from //ccavenue account.

            return View("CcAvenue", new CcAvenueViewModel(queryParameter.Encrypt
           (BuildCcAvenueRequestParameters(invoiceNumber, amount), ConfigurationManager.AppSettings["CcAvenueWorkingKey"]), ConfigurationManager.AppSettings["CcAvenueAccessCode"], ConfigurationManager.AppSettings["CcAvenueCheckoutUrl"]));
        }


        public static string BuildCcAvenueRequestParameters(string invoiceNumber, string amount)
        {
            var queryParameters = new Dictionary<string, string>
             {
             {"order_id", invoiceNumber},
             {"merchant_id", ConfigurationManager.AppSettings["CcAvenueMerchantId"].ToString()},
             {"amount", amount},
             {"currency","INR" },
             //{"redirect_url","https://localhost:57360/Gateway/CCAvenuePaymentSuccessful" },
             //{"cancel_url","https://localhost:57360/Gateway/CCAvenuePaymentCancelled"},
             {"redirect_url", ConfigurationManager.AppSettings["CCAvenuePaymentSuccessful"].ToString()},
             {"cancel_url", ConfigurationManager.AppSettings["CCAvenuePaymentCancelled"].ToString()},
             {"request_type","JSON" },
             {"response_type","JSON" },
             {"version","1.1" }
        }.Select(item => string.Format("{0}={1}", item.Key, item.Value));
            return string.Join("&", queryParameters);
        }


        //public ActionResult CCAvenuePaymentSuccessful()
        //{
        //    return View();
        //}

        [HttpPost]
        public ActionResult CCAvenuePaymentSuccessful(string encResp)
        {
            
            Dictionary<string, string> result = new Dictionary<string, string>();
            PaymentSuccessModel _PaymentSuccessModel = new PaymentSuccessModel();


            var decryption = new CCACrypto();
            var decryptedParameters = decryption.Decrypt(encResp, ConfigurationManager.AppSettings["CcAvenueWorkingKey"]);

            var keyValuePairs = decryptedParameters.Split('&');
            var splittedKeyValuePairs = new Dictionary<string, string>();

            foreach (var value in keyValuePairs)
            {
                var keyValuePair = value.Split('=');
                splittedKeyValuePairs.Add(keyValuePair[0], keyValuePair[1]);
            }

            //Here you can check the consistency of data i.e what you send is what you get back,
            //Make sure its not corrupted....
            //After that Save the details of the transaction into a db if you want to...

            string orderStatusValue;
            if (splittedKeyValuePairs.TryGetValue("order_status", out orderStatusValue))
            {
                if (orderStatusValue.ToLower() == "success" || orderStatusValue.ToLower() == "shipped") 
                {
                    _PaymentSuccessModel = new PaymentSuccessModel()
                    {
                        order_id = splittedKeyValuePairs["order_id"],
                        tracking_id = splittedKeyValuePairs["tracking_id"],
                        amount = splittedKeyValuePairs["amount"],
                        trans_date = splittedKeyValuePairs["trans_date"],
                        bank_ref_no = splittedKeyValuePairs["bank_ref_no"],
                        order_status = "success".ToLower(),
                        payment_mode = splittedKeyValuePairs["payment_mode"],
                        merchant_param1 = splittedKeyValuePairs["merchant_param1"],
                        bankname = "HDFC",
                        bankcode = "301",

                    };


                    int OutStatus; 
                    string Mobile = "0";
                    string feecode = _PaymentSuccessModel.order_id.Substring(3, 2);
                    ViewData["feecodePG"] = feecode;
                   // ViewData["STEP"] = "STEP1";

                    try
                    {
                        string OutError, OutSCHLREGID = "", OutAPPNO = "";

                        string dtResult2 = GatewayService.InsertOnlinePaymentMIS(_PaymentSuccessModel, out OutStatus, out Mobile, out OutError, out OutSCHLREGID, out OutAPPNO);// OutStatus mobile

                        ViewData["OutErrorPG"] = OutError;
                        //ViewData["OutSCHLREGID"] = OutSCHLREGID;
                        //ViewData["OutAPPNO"] = OutAPPNO;
                        if (OutStatus == 1)
                        {
                            //ViewData["STEP"] = "STEP2";
                            //send sms and email
                            //You have paid Rs. {#var#} against challan no. {#var#} on {#var#} with ref no. {#var#}. Regards PSEB
                            string Sms = "You have paid Rs. " + _PaymentSuccessModel.amount + " against challan no " + _PaymentSuccessModel.order_id + " on " + _PaymentSuccessModel.trans_date + " with ref no " + _PaymentSuccessModel.tracking_id + ". Regards PSEB";
                            try
                            {
                                string getSms = new PSEBONLINE.AbstractLayer.DBClass().gosms(Mobile, Sms);
                            }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception)
                    {
                        //ViewData["STEP"] = "STEP-ERR";
                    }

                }
                else if (orderStatusValue.ToLower() == "failure")
                {
                    _PaymentSuccessModel = new PaymentSuccessModel()
                    {
                        order_id = splittedKeyValuePairs["order_id"],
                        tracking_id = splittedKeyValuePairs["tracking_id"],
                        amount = splittedKeyValuePairs["amount"],
                        trans_date = splittedKeyValuePairs["trans_date"],
                        bank_ref_no = splittedKeyValuePairs["bank_ref_no"],
                        order_status = "failure".ToLower(),
                        payment_mode = splittedKeyValuePairs["payment_mode"],
                        merchant_param1 = splittedKeyValuePairs["merchant_param1"],
                        bankname = "HDFC",
                        bankcode = "301",
                    };

                    int OutStatus;
                    string Mobile = "";
                    string feecode = _PaymentSuccessModel.order_id.Substring(3, 2);
                    ViewData["feecodePG"] = feecode;
                   // ViewData["STEP"] = "STEP1";

                    try
                    {
                        string OutError, OutSCHLREGID = "", OutAPPNO = "";

                        string dtResult2 = GatewayService.InsertOnlinePaymentMIS(_PaymentSuccessModel, out OutStatus, out Mobile, out OutError, out OutSCHLREGID, out OutAPPNO);// OutStatus mobile

                        ViewData["OutErrorPG"] = OutError;
                        //ViewData["OutSCHLREGID"] = OutSCHLREGID;
                        //ViewData["OutAPPNO"] = OutAPPNO;
                        // ViewData["STEP"] = "STEP2";


                    }
                    catch (Exception)
                    {
                       // ViewData["STEP"] = "STEP-ERR";
                    }
                }
            }

            try
            {
                int OutStatusAll;
                string MobileAll = "0";
                string OutErrorAll, OutSCHLREGIDAll = "", OutAPPNOAll = "";
                string dtResultAll = GatewayService.InsertOnlinePaymentMIS_ALLTrans(_PaymentSuccessModel, out OutStatusAll, out MobileAll, out OutErrorAll, out OutSCHLREGIDAll, out OutAPPNOAll);// OutStatus mobile

            }
            catch (Exception ex2)
            {
            }


            ChallanMasterModel challanMasterModel = GatewayService.GetAnyChallanDetailsById(_PaymentSuccessModel.order_id.ToString());
            if (challanMasterModel != null)
            {
                _PaymentSuccessModel.FEECODE = challanMasterModel.FEECODE;
                _PaymentSuccessModel.APPNO = challanMasterModel.APPNO;
                _PaymentSuccessModel.SCHLREGID = challanMasterModel.SCHLREGID;
            }

            return View(_PaymentSuccessModel);
        }

        [HttpPost]
        public ActionResult CCAvenuePaymentCancelled(string encResp)
        {

            Dictionary<string, string> result = new Dictionary<string, string>();
            PaymentSuccessModel _PaymentSuccessModel = new PaymentSuccessModel();


            var decryption = new CCACrypto();
            var decryptedParameters = decryption.Decrypt(encResp, ConfigurationManager.AppSettings["CcAvenueWorkingKey"]);

            var keyValuePairs = decryptedParameters.Split('&');
            var splittedKeyValuePairs = new Dictionary<string, string>();

            foreach (var value in keyValuePairs)
            {
                var keyValuePair = value.Split('=');
                splittedKeyValuePairs.Add(keyValuePair[0], keyValuePair[1]);
            }
            string orderStatusValue;
            if (splittedKeyValuePairs.TryGetValue("order_status", out orderStatusValue))
            {                
                    _PaymentSuccessModel = new PaymentSuccessModel()
                    {
                        order_id = splittedKeyValuePairs["order_id"],
                        tracking_id = splittedKeyValuePairs["tracking_id"],
                        amount = splittedKeyValuePairs["amount"],
                        trans_date = splittedKeyValuePairs["trans_date"],
                        bank_ref_no = splittedKeyValuePairs["bank_ref_no"],
                        order_status = "cancelled".ToLower(),
                        payment_mode = splittedKeyValuePairs["payment_mode"],
                        merchant_param1 = splittedKeyValuePairs["merchant_param1"],
                        bankname = "HDFC",
                        bankcode = "301",
                    };

                    int OutStatus;
                    string Mobile = "";
                    string feecode = _PaymentSuccessModel.order_id.Substring(3, 2);
                    ViewData["feecodePG"] = feecode;
                
            }
            ///return RedirectToAction("Home", "Home");
            return View(_PaymentSuccessModel);
        }

        #endregion CcAvenue


        #region 

        public async Task<JsonResult> AtomSettlementTransactionsByDateToDB(string date)
        {
            ServiceResponce<AtomSettlementTransactions> serviceResponce = new ServiceResponce<AtomSettlementTransactions>();
           // ATOMServiceResponce<AtomSettlementTransactions> serviceResponce = new ATOMServiceResponce<AtomSettlementTransactions>();
            
            try
            {
                if (string.IsNullOrEmpty(date))
                {
                    return Json(serviceResponce, JsonRequestBehavior.AllowGet);
                }
                try
                {
                    List<AtomSettlementTransactions> LIST = new List<AtomSettlementTransactions>();
                    //DateTime dt = Convert.ToDateTime("2020-10-29");
                    DateTime dt = Convert.ToDateTime(date);
                    int TOTAL = 0;
                    //for (int day = 15; day <= 31; day++)
                    {
                        string url = "https://pgreports.atomtech.in/SettlementReport/generateReport?merchantId=111512&settlementDate=" + date + "&responseType=json";

                        using (var client = new HttpClient())
                        {
                            var content = new StringContent("", Encoding.UTF8, "application/json");
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                            HttpResponseMessage response = await client.PostAsync(url, content);
                            response.EnsureSuccessStatusCode();
                            string responseBody = await response.Content.ReadAsStringAsync();
                            
                            if (responseBody.Contains("Invalid IP address"))
                            {                                
                                //serviceResponce.StatusResponse = JsonConvert.DeserializeObject<StatusResponse>(responseBody);
                                serviceResponce.Message =  "Invalid IP address or IP is not configured with merchantId";
                            }
                             else  if (!responseBody.Contains("No data found"))
                            {
                                JavaScriptSerializer j = new JavaScriptSerializer();
                                var dataList = j.Deserialize<List<AtomSettlementTransactions>>(responseBody);
                                j.MaxJsonLength = int.MaxValue;
                                if (dataList != null && dataList.Count > 0)
                                {
                                    //_handlerATOMBL = new AtomSettlementBL();
                                    //_handlerATOMBL.AddTransactionList(dataList);
                                    List<AtomSettlementTransactions> list = dataList;
                                    TOTAL = TOTAL + dataList.Count;
                                    // Add in Database 
                                    //LIST.AddRange(dataList);
                                    using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                                    {
                                        try
                                        {                                            
                                            _context.AtomSettlementTransactions.AddRange(dataList);
                                            int insertedRecords = await _context.SaveChangesAsync();

                                            transaction.Commit();//transaction commit
                                        }
                                        catch (Exception ex1)
                                        {
                                            transaction.Rollback();
                                        }
                                    }
                                }
                               
                            }
                            else
                            {
                                serviceResponce.Message = responseBody;
                            }
                        }
                    }
                    serviceResponce.DataList = LIST;
                    //serviceResponce.Message = responseBody;
                    serviceResponce.TotalRecord = TOTAL;
                }
                catch (Exception ex)
                {

                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    StaticDB.AddExceptionInDB(ex, "Gateway", "AtomSettlementTransactionsByDateToDB", line.ToString(), "", "AtomSettlementTransactionsByDateToDB", "EXPT inner");
                    serviceResponce.Success = false;
                    serviceResponce.Message = ex.Message;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                serviceResponce.Message = ex.Message + " line";
                StaticDB.AddExceptionInDB(ex, "Gateway", "AtomSettlementTransactionsByDateToDB", line.ToString(), "", "AtomSettlementTransactionsByDateToDB", "EXPT Outer");

            }

            return Json(serviceResponce, JsonRequestBehavior.AllowGet);
        }


        [Route("Atom/SettlementApi")]
        public ActionResult AtomSettlementApi()
        {
            try
            {
                List<AtomSettlementTransactionsResponse> listAtomSettlementTransactions = new List<AtomSettlementTransactionsResponse>();
                int resultStatus = 0;
                //string lastId = "65535";
                string lastId = _context.AtomSettlementTransactions.Max(s=>s.AtomId).ToString() ;
                ViewBag.lastId = lastId;
                listAtomSettlementTransactions = GatewayService.GetAtomSettlementTransactions(lastId, out resultStatus);
                if (resultStatus == 1)
                {
                    int count = listAtomSettlementTransactions.Count();
                     //Insert into AtomSettlementTransactions
                    if (count > 0)
                    {
                        //_context.AtomSettlementTransactions.AddRange(listAtomSettlementTransactions);
                        //int records = _context.SaveChanges();
                        string OutStatus = "";
                        string adminId = "1";
                        DataTable dt = StaticDB.ConvertListToDataTable(listAtomSettlementTransactions);
                        if (dt.Rows.Count > 0)
                        {
                            DataSet ds1 = GatewayService.InsertBulkAtomSettlementTransactions(adminId, dt, out OutStatus);
                            if (OutStatus == "1")
                            {
                                resultStatus = count;
                                TempData["resultStatusError"] = "SUCCESS";
                            }
                            else
                            {
                                TempData["resultStatusError"] = "FAILURE : " +  OutStatus;
                            }
                        }
                    }
                    else
                    {
                        TempData["resultStatusError"] = "No Records Found".ToUpper();
                    }

                }
                TempData["resultStatus"] = resultStatus;
            }
            catch (Exception ex)
            {
                TempData["resultStatus"] = -5;
                TempData["resultStatusError"] = ex.Message.ToUpper();
            }
            return View();
            //List<AtomSettlementTransactions> GetAtomSettlementTransactions(string LastId, out int resultStatus)
        }
        #endregion'



        //// Dispose
        ///

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