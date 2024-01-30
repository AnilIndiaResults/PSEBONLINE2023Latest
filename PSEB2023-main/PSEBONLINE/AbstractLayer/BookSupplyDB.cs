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
using System.Globalization;
using System.Text;

namespace PSEBONLINE.AbstractLayer
{
    public class BookSupplyDB
    {
        #region Check ConString

        private string CommonCon = "myDBConnection";
        public BookSupplyDB()
        {
            CommonCon = "myDBConnection";
        }
        #endregion  Check ConString


        public List<SelectListItem> GetDepotList(string DepotId)  // GetBankDataByBCODE
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDepotSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DepotId", "0");                  
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);               
                    if (dataTable.Tables[0].Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow dr in dataTable.Tables[0].Rows)
                        {
                            _list.Add(new SelectListItem { Text = @dr["USER"].ToString() +" - "+ @dr["DISTNM"].ToString(), Value = @dr["USER"].ToString() });
                        }
                        return _list;
                    }
                    else
                    { return null; }
                }
            }
            catch (Exception ex)
            {              
                return null;
            }
        }


        public PrinterModel BookSupplyLogin(LoginModel LM, out int OutStatus)  // BankLoginSP
        {
            
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BookSupplyLoginSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", LM.username);
                    cmd.Parameters.AddWithValue("@Password", LM.Password);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    if (dataTable.Rows.Count > 0)
                    {
                        PrinterModel PM = new PrinterModel()
                        {
                            Id = Convert.ToInt32(dataTable.Rows[0]["Id"].ToString()),
                            PrinterId = Convert.ToString(dataTable.Rows[0]["PrinterId"].ToString()),
                            Password = Convert.ToString(dataTable.Rows[0]["Password"].ToString()),
                            PrinterName = Convert.ToString(dataTable.Rows[0]["PrinterName"].ToString()),
                            Address = Convert.ToString(dataTable.Rows[0]["Address"].ToString()),
                            District = Convert.ToString(dataTable.Rows[0]["District"].ToString()),
                            City = Convert.ToString(dataTable.Rows[0]["City"].ToString()),
                            State = Convert.ToString(dataTable.Rows[0]["State"].ToString()),
                            Pin = Convert.ToString(dataTable.Rows[0]["Pin"].ToString()),
                            ContactPerson1 = Convert.ToString(dataTable.Rows[0]["ContactPerson1"].ToString()),
                            MobileCP1 = Convert.ToString(dataTable.Rows[0]["MobileCP1"].ToString()),
                            ContactPerson2 = Convert.ToString(dataTable.Rows[0]["ContactPerson2"].ToString()),
                            MobileCP2 = Convert.ToString(dataTable.Rows[0]["MobileCP2"].ToString()),
                            TAN = Convert.ToString(dataTable.Rows[0]["TAN"].ToString()),
                            PAN = Convert.ToString(dataTable.Rows[0]["PAN"].ToString()),
                            GSTN = Convert.ToString(dataTable.Rows[0]["GSTN"].ToString()),
                            EMAILID = Convert.ToString(dataTable.Rows[0]["EMAILID"].ToString()),
                            Remarks = Convert.ToString(dataTable.Rows[0]["Remarks"].ToString()),
                            Status = Convert.ToString(dataTable.Rows[0]["Status"].ToString()),
                            CreatedDate = Convert.ToString(dataTable.Rows[0]["CreatedDate"].ToString()),
                            UpdatedDate = Convert.ToString(dataTable.Rows[0]["UpdatedDate"].ToString()),
                            UserId = Convert.ToString(dataTable.Rows[0]["UserId"].ToString()),
                            UserType = Convert.ToString(dataTable.Rows[0]["UserType"].ToString())
                        };

                        return PM;
                    }
                    else
                    { return null; }

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }

        public DataSet ChangePasswordPrinter(PrinterModel PM, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ChangePasswordPrinter", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", PM.UserId);
                    cmd.Parameters.AddWithValue("@oldpassword", PM.OldPassword);
                    cmd.Parameters.AddWithValue("@newpassword", PM.Newpassword);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }

        #region BookSupply

        public List<SelectListItem> GetPrintMasterList(string UserId, out int OutStatus)  // GetBankDataByBCODE
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDataByPrinterUserIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", "0");
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    if (dataTable.Tables[0].Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow dr in dataTable.Tables[0].Rows)
                        {
                            _list.Add(new SelectListItem { Text = @dr["PrinterName"].ToString(), Value = @dr["PrinterId"].ToString() });
                        }
                        return _list;
                    }
                    else
                    { return null; }
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }

        public DataSet GetPrintMaster(string UserId, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDataByPrinterUserIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", "0");
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }

        public PrinterModel GetDataByPrinterUserId(string UserId, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDataByPrinterUserIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    if (dataTable.Rows.Count > 0)
                    {
                        PrinterModel PM = new PrinterModel()
                        {
                            Id = Convert.ToInt32(dataTable.Rows[0]["Id"].ToString()),
                            PrinterId = Convert.ToString(dataTable.Rows[0]["PrinterId"].ToString()),
                            Password = Convert.ToString(dataTable.Rows[0]["Password"].ToString()),
                            PrinterName = Convert.ToString(dataTable.Rows[0]["PrinterName"].ToString()),
                            Address = Convert.ToString(dataTable.Rows[0]["Address"].ToString()),
                            District = Convert.ToString(dataTable.Rows[0]["District"].ToString()),
                            City = Convert.ToString(dataTable.Rows[0]["City"].ToString()),
                            State = Convert.ToString(dataTable.Rows[0]["State"].ToString()),
                            Pin = Convert.ToString(dataTable.Rows[0]["Pin"].ToString()),
                            ContactPerson1 = Convert.ToString(dataTable.Rows[0]["ContactPerson1"].ToString()),
                            MobileCP1 = Convert.ToString(dataTable.Rows[0]["MobileCP1"].ToString()),
                            ContactPerson2 = Convert.ToString(dataTable.Rows[0]["ContactPerson2"].ToString()),
                            MobileCP2 = Convert.ToString(dataTable.Rows[0]["MobileCP2"].ToString()),
                            TAN = Convert.ToString(dataTable.Rows[0]["TAN"].ToString()),
                            PAN = Convert.ToString(dataTable.Rows[0]["PAN"].ToString()),
                            GSTN = Convert.ToString(dataTable.Rows[0]["GSTN"].ToString()),
                            EMAILID = Convert.ToString(dataTable.Rows[0]["EMAILID"].ToString()),
                            Remarks = Convert.ToString(dataTable.Rows[0]["Remarks"].ToString()),
                            Status = Convert.ToString(dataTable.Rows[0]["Status"].ToString()),
                            CreatedDate = Convert.ToString(dataTable.Rows[0]["CreatedDate"].ToString()),
                            UpdatedDate = Convert.ToString(dataTable.Rows[0]["UpdatedDate"].ToString()),
                            UserId = Convert.ToString(dataTable.Rows[0]["UserId"].ToString()),
                            UserType = Convert.ToString(dataTable.Rows[0]["UserType"].ToString())
                        };

                        return PM;
                    }
                    else
                    { return null; }

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }

        public DataSet UpdatePrinterDataByUserId(PrinterModel PM, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UpdatePrinterDataByUserIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", PM.UserId);
                    cmd.Parameters.AddWithValue("@PrinterId", PM.PrinterId);                    
                    cmd.Parameters.AddWithValue("@PrinterName", PM.PrinterName);
                    cmd.Parameters.AddWithValue("@Address", PM.Address);
                    cmd.Parameters.AddWithValue("@District", PM.District);
                    cmd.Parameters.AddWithValue("@City", PM.City);
                    cmd.Parameters.AddWithValue("@State", PM.State);
                    cmd.Parameters.AddWithValue("@Pin", PM.Pin);
                    cmd.Parameters.AddWithValue("@ContactPerson1", PM.ContactPerson1);
                    cmd.Parameters.AddWithValue("@MobileCP1", PM.MobileCP1);
                    cmd.Parameters.AddWithValue("@ContactPerson2", PM.ContactPerson2);
                    cmd.Parameters.AddWithValue("@MobileCP2", PM.MobileCP2);
                    cmd.Parameters.AddWithValue("@TAN", PM.TAN);
                    cmd.Parameters.AddWithValue("@PAN", PM.PAN);
                    cmd.Parameters.AddWithValue("@GSTN", PM.GSTN);
                    cmd.Parameters.AddWithValue("@EMAILID", PM.EMAILID);
                    cmd.Parameters.AddWithValue("@Remarks", PM.Remarks);
                    cmd.Parameters.AddWithValue("@Status", PM.Status);            
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }
        #endregion BookSupply


        #region BookPrintMaster

        public List<SelectListItem> GetBookPrintMasterList(string BookId, out int OutStatus)  // GetBankDataByBCODE
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetBookPrintMasterByBookIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BookId", BookId);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    if (dataTable.Tables[0].Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow dr in dataTable.Tables[0].Rows)
                        {
                            _list.Add(new SelectListItem { Text = @dr["BookName"].ToString(), Value = @dr["BookId"].ToString() });
                        }
                        return _list;
                    }
                    else
                    { return null; }
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }


        public DataSet GetBookPrintMaster(string BookId, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetBookPrintMasterByBookIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BookId", BookId);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }


        public BookPrintMaster GetBookPrintMasterByBookId(string BookId, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetBookPrintMasterByBookIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BookId", BookId);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    if (dataTable.Rows.Count > 0)
                    {
                        BookPrintMaster PM = new BookPrintMaster()
                        {    
                            Id = Convert.ToInt32(dataTable.Rows[0]["Id"].ToString()),
                            BookId = Convert.ToString(dataTable.Rows[0]["BookId"].ToString()),
                            BookName = Convert.ToString(dataTable.Rows[0]["BookName"].ToString()),
                            Color = Convert.ToString(dataTable.Rows[0]["Color"].ToString()),
                            Class = Convert.ToString(dataTable.Rows[0]["Class"].ToString()),
                            Medium = Convert.ToString(dataTable.Rows[0]["Medium"].ToString()),
                            Size = Convert.ToString(dataTable.Rows[0]["Size"].ToString()),
                            NOPB = Convert.ToInt32(dataTable.Rows[0]["NOPB"].ToString()),
                            RNN = Convert.ToString(dataTable.Rows[0]["RNN"].ToString()),
                            TextPaper = Convert.ToString(dataTable.Rows[0]["TextPaper"].ToString()),
                            CoverPaper = Convert.ToString(dataTable.Rows[0]["CoverPaper"].ToString()),
                            Numberofbookstobeprinted = Convert.ToInt32(dataTable.Rows[0]["Numberofbookstobeprinted"].ToString()),
                            IsActive = Convert.ToInt32(dataTable.Rows[0]["IsActive"].ToString()),
                            CreatedDate = Convert.ToString(dataTable.Rows[0]["CreatedDate"].ToString()),
                            UpdatedDate = Convert.ToString(dataTable.Rows[0]["UpdatedDate"].ToString()),
                        };

                        return PM;
                    }
                    else
                    { return null; }

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }

        public DataSet BookPrintMaster(BookPrintMaster PM, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BookPrintMasterSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;                   
                    cmd.Parameters.AddWithValue("@Id", PM.Id);
                    cmd.Parameters.AddWithValue("@BookId", PM.BookId);
                    cmd.Parameters.AddWithValue("@BookName", PM.BookName);
                    cmd.Parameters.AddWithValue("@Color", PM.Color);
                    cmd.Parameters.AddWithValue("@Class", PM.Class);
                    cmd.Parameters.AddWithValue("@Medium", PM.Medium);
                    cmd.Parameters.AddWithValue("@Size", PM.Size);
                    cmd.Parameters.AddWithValue("@NOPB", PM.NOPB);
                    cmd.Parameters.AddWithValue("@RNN", PM.RNN);
                    cmd.Parameters.AddWithValue("@TextPaper", PM.TextPaper);
                    cmd.Parameters.AddWithValue("@CoverPaper", PM.CoverPaper);
                    cmd.Parameters.AddWithValue("@Numberofbookstobeprinted", PM.Numberofbookstobeprinted);
                    cmd.Parameters.AddWithValue("@IsActive", PM.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedDate", PM.CreatedDate);
                    cmd.Parameters.AddWithValue("@UpdatedDate", PM.UpdatedDate);
                    cmd.Parameters.AddWithValue("@Sale", PM.Sale);
                    cmd.Parameters.AddWithValue("@Ssadwo", PM.Ssadwo);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }
        #endregion BookPrintMaster




        #region AssignmentofBooksforprinting

        public List<SelectListItem> GetTransIdList(int IsType, string TransId, out int OutStatus)  // GetBankDataByBCODE
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                string Search = "a.Id like '%'";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAssignmentofBooksforprintingByIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IsType", IsType);
                    cmd.Parameters.AddWithValue("@TransId", TransId);
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    if (dataTable.Tables[0].Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow dr in dataTable.Tables[0].Rows)
                        {
                            _list.Add(new SelectListItem { Text = @dr["TransId"].ToString()+ " - "+@dr["BookName"].ToString(), Value = @dr["TransId"].ToString() });
                        }
                        return _list;
                    }
                    else
                    { return null; }
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }


        public DataSet GetAssignmentofBooksforprinting(int IsType, string TransId, out int OutStatus,string Search)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAssignmentofBooksforprintingByIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@IsType", IsType);
                    cmd.Parameters.AddWithValue("@TransId", TransId);
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }



        public AssignmentofBooksforprinting GetAssignmentofBooksforprintingById(int IsType, string TransId, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                string Search = "a.Id like '%'";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAssignmentofBooksforprintingByIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IsType", IsType);
                    cmd.Parameters.AddWithValue("@TransId", TransId);
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    if (dataTable.Rows.Count > 0)
                    {
                        AssignmentofBooksforprinting PM = new AssignmentofBooksforprinting()
                        {
                            Id = Convert.ToInt32(dataTable.Rows[0]["Id"].ToString()),
                            TransId = Convert.ToString(dataTable.Rows[0]["TransId"].ToString()),
                            PrinterId = Convert.ToString(dataTable.Rows[0]["PrinterId"].ToString()),
                            BookId = Convert.ToString(dataTable.Rows[0]["BookId"].ToString()),
                            QtyofBooksforprinting = Convert.ToInt32(dataTable.Rows[0]["QtyofBooksforprinting"].ToString()),
                            DateStamp = Convert.ToString(dataTable.Rows[0]["DateStamp"].ToString()),
                            Remarks = Convert.ToString(dataTable.Rows[0]["Remarks"].ToString()),
                            Lot = Convert.ToInt32(dataTable.Rows[0]["Lot"].ToString()),
                            Typeofbooks = Convert.ToString(dataTable.Rows[0]["Typeofbooks"].ToString()),
                            IsActive = Convert.ToInt32(dataTable.Rows[0]["IsActive"].ToString()),
                            CreatedDate = Convert.ToString(dataTable.Rows[0]["CreatedDate"].ToString()),
                            UpdatedDate = Convert.ToString(dataTable.Rows[0]["UpdatedDate"].ToString()),
                        };
                        return PM;
                    }
                    else
                    { return null; }

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }

        public DataSet AssignmentofBooksforprinting(AssignmentofBooksforprinting PM, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AssignmentofBooksforprintingSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", PM.Id);
                    cmd.Parameters.AddWithValue("@TransId", PM.TransId);
                    cmd.Parameters.AddWithValue("@PrinterId", PM.PrinterId);
                    cmd.Parameters.AddWithValue("@BookId", PM.BookId);
                    cmd.Parameters.AddWithValue("@QtyofBooksforprinting", PM.QtyofBooksforprinting);
                    cmd.Parameters.AddWithValue("@DateStamp", PM.DateStamp);
                    cmd.Parameters.AddWithValue("@Remarks", PM.Remarks);
                    cmd.Parameters.AddWithValue("@Lot", PM.Lot);
                    cmd.Parameters.AddWithValue("@Typeofbooks", PM.Typeofbooks);
                    cmd.Parameters.AddWithValue("@IsActive", PM.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedDate", PM.CreatedDate);
                    cmd.Parameters.AddWithValue("@UpdatedDate", PM.UpdatedDate);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }
        #endregion AssignmentofBooksforprinting


        #region SupplyofBooks

        public DataSet GetSupplyofBooks(int IsType, string SupplyId, out int OutStatus,string Search)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                //string Search = "a.SupplyId like '%'";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSupplyofBooksByIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IsType", IsType);
                    cmd.Parameters.AddWithValue("@SupplyId", SupplyId);
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }



        public SupplyofBooks GetSupplyofBooksById(int IsType,string SupplyId, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                string Search = "a.SupplyId like '%'";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSupplyofBooksByIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IsType", IsType);
                    cmd.Parameters.AddWithValue("@SupplyId", SupplyId);
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    if (dataTable.Rows.Count > 0)
                    {
                        SupplyofBooks PM = new SupplyofBooks()
                        {                            
                            SupplyId = Convert.ToInt32(dataTable.Rows[0]["SupplyId"].ToString()),
                            DepotUserId = Convert.ToString(dataTable.Rows[0]["DepotUserId"].ToString()),
                            TransId = Convert.ToString(dataTable.Rows[0]["TransId"].ToString()),
                            PrinterId = Convert.ToString(dataTable.Rows[0]["PrinterId"].ToString()),
                            NumberofSuppliedbooks = Convert.ToInt32(dataTable.Rows[0]["NumberofSuppliedbooks"].ToString()),
                            DateStamp = Convert.ToString(dataTable.Rows[0]["DateStamp"].ToString()),
                            Remarks = Convert.ToString(dataTable.Rows[0]["Remarks"].ToString()),
                            SupplyLot = Convert.ToInt32(dataTable.Rows[0]["SupplyLot"].ToString()),
                            Type = Convert.ToString(dataTable.Rows[0]["Type"].ToString()),
                            IsActive = Convert.ToInt32(dataTable.Rows[0]["IsActive"].ToString()),
                            CreatedDate = Convert.ToString(dataTable.Rows[0]["CreatedDate"].ToString()),
                            UpdatedDate = Convert.ToString(dataTable.Rows[0]["UpdatedDate"].ToString()),
                        };
                        return PM;
                    }
                    else
                    { return null; }

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }

        public DataSet SupplyofBooks(SupplyofBooks PM, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SupplyofBooksSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SupplyId", PM.SupplyId);
                    cmd.Parameters.AddWithValue("@DepotUserId", PM.DepotUserId);
                    cmd.Parameters.AddWithValue("@TransId", PM.TransId);
                    cmd.Parameters.AddWithValue("@PrinterId", PM.PrinterId);
                    cmd.Parameters.AddWithValue("@NumberofSuppliedbooks", PM.NumberofSuppliedbooks);
                    cmd.Parameters.AddWithValue("@DateStamp", PM.DateStamp);
                    cmd.Parameters.AddWithValue("@Remarks", PM.Remarks);
                    cmd.Parameters.AddWithValue("@SupplyLot", PM.SupplyLot);
                    cmd.Parameters.AddWithValue("@Type", PM.Type);
                    cmd.Parameters.AddWithValue("@IsActive", PM.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedDate", PM.CreatedDate);
                    cmd.Parameters.AddWithValue("@UpdatedDate", PM.UpdatedDate);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }

        public DataSet SupplyofBooksFinalSubmit(string PrinterId,  out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {                
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SupplyofBooksFinalSubmitSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;                    
                    cmd.Parameters.AddWithValue("@PrinterId", PrinterId);                    
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }

        public DataSet BookSupplySummaryByTransId(int IsType,string PrinterId, string TransId, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                //string Search = "a.SupplyId like '%'";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BookSupplySummaryByTransIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IsType", IsType);
                    cmd.Parameters.AddWithValue("@PrinterId", PrinterId);
                    cmd.Parameters.AddWithValue("@TransId", TransId);           
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }


        public DataSet ViewSupplyofBooks(int IsType, string PrinterId, int SupplyLot, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                //string Search = "a.SupplyId like '%'";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ViewSupplyofBooks", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IsType", IsType);
                    cmd.Parameters.AddWithValue("@PrinterId", PrinterId);
                    cmd.Parameters.AddWithValue("@SupplyLot", SupplyLot);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }

        public DataSet ActionBySupplyId(int IsType, int SupplyId, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ActionBySupplyIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IsType", IsType);
                    cmd.Parameters.AddWithValue("@SupplyId", SupplyId);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }


        public void ReceiveSupplyBookDetails(string remarks,string NORB, string SupplyId, out string outstatus, int AdminId, int IsType)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ReceiveSupplyBookDetailsSP", con);//ChallanDetailsCancelSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SupplyId", SupplyId);
                    cmd.Parameters.AddWithValue("@NORB", NORB);
                    cmd.Parameters.AddWithValue("@remarks", remarks);
                    cmd.Parameters.AddWithValue("@IsType", IsType);
                    cmd.Parameters.AddWithValue("@AdminId", AdminId);                  
                    cmd.Parameters.Add("@OutStatus", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    outstatus = Convert.ToString(cmd.Parameters["@OutStatus"].Value);

                }
            }
            catch (Exception ex)
            {
                outstatus = "-1";
            }
        }



        #endregion SupplyofBooks
    }
}