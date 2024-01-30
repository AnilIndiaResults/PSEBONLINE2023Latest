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
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Web.UI;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace PSEBONLINE.AbstractLayer
{
    public class GatewayService
    {
        private DBContext _context = new DBContext();

        #region Update Data in Challan master


        public static string InsertOnlinePaymentMIS(PaymentSuccessModel BM,  out int OutStatus, out string Mobile, out string OutError, out string OutSCHLREGID, out string OutAPPNO)
        {
            try
            {                
                decimal d = decimal.Parse(BM.amount);
                int payamount = Decimal.ToInt32(d);
                //
                DataSet result = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "InsertOnlinePaymentMISSPNEW"; //InsertOnlinePaymentMISSP // InsertOnlinePaymentMISSPNEW
                cmd.Parameters.AddWithValue("@order_id", BM.order_id);
                cmd.Parameters.AddWithValue("@trans_id", BM.tracking_id);
                cmd.Parameters.AddWithValue("@amount", payamount.ToString());
                cmd.Parameters.AddWithValue("@trans_date", BM.trans_date);
                cmd.Parameters.AddWithValue("@bank_ref_no", BM.bank_ref_no);
                cmd.Parameters.AddWithValue("@order_status", BM.order_status);
                cmd.Parameters.AddWithValue("@payment_mode", BM.payment_mode);
                cmd.Parameters.AddWithValue("@merchant_param1", BM.merchant_param1);
                cmd.Parameters.AddWithValue("@bankname", BM.bankname);             
                cmd.Parameters.AddWithValue("@bankcode", BM.bankcode);               
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Mobile", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                //
                cmd.Parameters.Add("@OutSCHLREGID", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutAPPNO", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;

                result = db.ExecuteDataSet(cmd);
                OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                Mobile = (string)cmd.Parameters["@Mobile"].Value;
                OutError = (string)cmd.Parameters["@OutError"].Value;

                OutSCHLREGID = (string)cmd.Parameters["@OutSCHLREGID"].Value;
                OutAPPNO = (string)cmd.Parameters["@OutAPPNO"].Value;
                return OutStatus.ToString();
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                Mobile = "";
                OutAPPNO = "";
                OutSCHLREGID = "";
                OutError = "ERR : " + ex.Message;
                return null;
            }
        }



        public static ChallanMasterModel GetAnyChallanDetailsById(string ChallanId)  // Type 1=Regular, 2=Open
        {
            ChallanMasterModel challanMasterModel = new ChallanMasterModel();
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetChallanDetailsByIdSPBank";// LoginSP(old)
                cmd.Parameters.AddWithValue("@CHALLANID", ChallanId);                
                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        challanMasterModel.CHALLANID = DBNull.Value != reader["CHALLANID"] ? (string)reader["CHALLANID"] : default(string);
                        challanMasterModel.APPNO = DBNull.Value != reader["APPNO"] ? (string)reader["APPNO"] : default(string);
                        challanMasterModel.SCHLREGID = DBNull.Value != reader["SCHLREGID"] ? (string)reader["SCHLREGID"] : default(string);
                        challanMasterModel.DepositoryMobile = DBNull.Value != reader["DepositoryMobile"] ? (string)reader["DepositoryMobile"] : default(string);


                        challanMasterModel.FEECODE = DBNull.Value != reader["FEECODE"] ? (string)reader["FEECODE"] : default(string);
                        challanMasterModel.FEECAT = DBNull.Value != reader["FEECAT"] ? (string)reader["FEECAT"] : default(string);
                        challanMasterModel.BCODE = DBNull.Value != reader["BCODE"] ? (string)reader["BCODE"] : default(string);
                        challanMasterModel.BANK = DBNull.Value != reader["BANK"] ? (string)reader["BANK"] : default(string);
                        challanMasterModel.ChallanGDateN = DBNull.Value != reader["ChallanGDateN"] ? (DateTime)reader["ChallanGDateN"] : default(DateTime);

                        
                        challanMasterModel.FEE = DBNull.Value != reader["FEE"] ? Convert.ToSingle(Convert.ToInt32(reader["FEE"])) : default(float);
                        challanMasterModel.TOTFEE = DBNull.Value != reader["TOTFEE"] ? Convert.ToSingle(Convert.ToInt32(reader["TOTFEE"])) : default(float);
                        challanMasterModel.J_REF_NO = DBNull.Value != reader["J_REF_NO"] ? (string)reader["J_REF_NO"] : default(string);
                        
                    }
                }
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                challanMasterModel = null;
            }
            return challanMasterModel;
        }


        public static string InsertOnlinePaymentMIS_ALLTrans(PaymentSuccessModel BM, out int OutStatus, out string Mobile, out string OutError, out string OutSCHLREGID, out string OutAPPNO)
        {
            try
            {
                decimal d = decimal.Parse(BM.amount);
                int payamount = Decimal.ToInt32(d);
                //
                DataSet result = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "InsertOnlinePaymentMISSPNEW_ALLTrans"; //InsertOnlinePaymentMISSP // InsertOnlinePaymentMISSPNEW
                cmd.Parameters.AddWithValue("@order_id", BM.order_id);
                cmd.Parameters.AddWithValue("@trans_id", BM.tracking_id);
                cmd.Parameters.AddWithValue("@amount", payamount.ToString());
                cmd.Parameters.AddWithValue("@trans_date", BM.trans_date);
                cmd.Parameters.AddWithValue("@bank_ref_no", BM.bank_ref_no);
                cmd.Parameters.AddWithValue("@order_status", BM.order_status);
                cmd.Parameters.AddWithValue("@payment_mode", BM.payment_mode);
                cmd.Parameters.AddWithValue("@merchant_param1", BM.merchant_param1);
                cmd.Parameters.AddWithValue("@bankname", BM.bankname);
                cmd.Parameters.AddWithValue("@bankcode", BM.bankcode);
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Mobile", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                //
                cmd.Parameters.Add("@OutSCHLREGID", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutAPPNO", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;

                result = db.ExecuteDataSet(cmd);
                OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                Mobile = (string)cmd.Parameters["@Mobile"].Value;
                OutError = (string)cmd.Parameters["@OutError"].Value;

                OutSCHLREGID = (string)cmd.Parameters["@OutSCHLREGID"].Value;
                OutAPPNO = (string)cmd.Parameters["@OutAPPNO"].Value;
                return OutStatus.ToString();
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                Mobile = "";
                OutAPPNO = "";
                OutSCHLREGID = "";
                OutError = "ERR : " + ex.Message;
                return null;
            }
        }

        #endregion

        #region Algorithm
        public static String sha256_hash(String value)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));
                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }

        public string Encrypt(string input, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(input);
            Aes kgen = Aes.Create("AES");
            kgen.Mode = CipherMode.ECB;
            //kgen.Padding = PaddingMode.None;
            kgen.Key = keyArray;
            ICryptoTransform cTransform = kgen.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        #endregion


        public int InsertAtomSettlementTransactions(List<AtomSettlementTransactions> listAtomSettlementTransactions)
        {
            int status = 0;
            try
            {

                _context.AtomSettlementTransactions.AddRange(listAtomSettlementTransactions);
                status = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                status = -1;
            }
           
            return status;
        }


        public static List<AtomSettlementTransactionsResponse> GetAtomSettlementTransactions(string LastId, out int resultStatus)  // Type 1=Regular, 2=Open
        {
            List<AtomSettlementTransactionsResponse> listAtomSettlementTransactions = new List<AtomSettlementTransactionsResponse>();
            int resultStatus1 = 0;
            try
            {

                HttpClientHandler handler = new HttpClientHandler();
                HttpClient client = new HttpClient(handler);
                string firmUserId = "firm";
                string firmUserPwd = "firm@123$";

                var authenticationString = $"{firmUserId}:{firmUserPwd}";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization",
                      Convert.ToBase64String(Encoding.Default.GetBytes(authenticationString)));



                List<KeyValuePair<string, string>> PostParams = new List<KeyValuePair<string, string>>();
                PostParams.Add(new KeyValuePair<string, string>("id", LastId));
                FormUrlEncodedContent contentPost = new FormUrlEncodedContent(PostParams);

                Uri uri = new Uri("https://www.test.pseb.ac.in/Atom");
 
                var result = client.PostAsync(uri, contentPost).Result;

              if (result.IsSuccessStatusCode)
                {
                    var JsonContent = result.Content.ReadAsStringAsync().Result;

                    var readTask = result.Content.ReadAsAsync<AtomAPIViewModel>();
                    readTask.Wait();

                    AtomAPIViewModel atomAPIViewModel = JsonConvert.DeserializeObject<AtomAPIViewModel>(JsonContent);
                    listAtomSettlementTransactions = JsonConvert.DeserializeObject<List<AtomSettlementTransactionsResponse>>(atomAPIViewModel.results);


                   // List<AtomSettlementTransactionsResponse> atomSettlementTransactionsResponse2 = JsonConvert.DeserializeObject<List<AtomSettlementTransactionsResponse>>(atomAPIViewModel.results);

                    resultStatus1 = 1;       
                }
                else //web api sent error response 
                {
                    resultStatus1 = -5;
                    listAtomSettlementTransactions = null;

                }

            }
            catch (Exception ex)
            {
                resultStatus1 = -10;
            }
            resultStatus = resultStatus1;
            return listAtomSettlementTransactions;
        }



        public static DataSet InsertBulkAtomSettlementTransactions(string adminid,DataTable dt1, out string OutError)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertBulkAtomSettlementTransactionsSP"; //GetAllFormName_sp
                cmd.Parameters.AddWithValue("@adminid", adminid);
                cmd.Parameters.AddWithValue("@tblInsertBulkAtomSettlementTransactions", dt1);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                ds = db.ExecuteDataSet(cmd);
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return ds;

            }
            catch (Exception ex)
            {
                OutError = "-1";
                return null;
            }

        }



    }
}