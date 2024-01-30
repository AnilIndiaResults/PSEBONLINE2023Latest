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

namespace PSEBONLINE.AbstractLayer
{
    public class AffiliationDB
    {
        #region Check ConString
        private string CommonCon = "myDBConnection";
        public AffiliationDB()
        {
            CommonCon = "myDBConnection";
        }

        #endregion  Check ConString


        public AffiliationModel AffiliationContinuationBySchl(string schl, int type, out DataSet ds1)
        {
            AffiliationModel am = new AffiliationModel();
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AffiliationContinuationBySchl", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@type", type);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        ds1 = null;
                        return null;
                    }
                    else
                    {
                        ds1 = ds;
                        am.SCHLDIST = Convert.ToString(ds.Tables[0].Rows[0]["DIST"].ToString());
                        am.SCHLDISTNM = Convert.ToString(ds.Tables[0].Rows[0]["DISTNM"].ToString());
                        am.SCHLNM = Convert.ToString(ds.Tables[0].Rows[0]["SCHLNM"].ToString());
                        am.ID = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString() == "" ? "0" : ds.Tables[0].Rows[0]["ID"].ToString());
                        if (am.ID > 0)
                        {
                            am.ID = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString());
                            am.SCHL = Convert.ToString(ds.Tables[0].Rows[0]["SCHL"].ToString());
                            am.RS10GTotal2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GTotal2017"].ToString());
                            am.RS10GPass2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GPass2017"].ToString());
                            am.RS10GPercent2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GPercent2017"].ToString());
                            am.RS10GTotal2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GTotal2018"].ToString());
                            am.RS10GPass2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GPass2018"].ToString());
                            am.RS10GPercent2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GPercent2018"].ToString());
                            am.RS12HTotal2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HTotal2017"].ToString());
                            am.RS12HPass2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HPass2017"].ToString());
                            am.RS12HPercent2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HPercent2017"].ToString());
                            am.RS12HTotal2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HTotal2018"].ToString());
                            am.RS12HPass2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HPass2018"].ToString());
                            am.RS12HPercent2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HPercent2018"].ToString());
                            am.RS12STotal2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12STotal2017"].ToString());
                            am.RS12SPass2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12SPass2017"].ToString());
                            am.RS12SPercent2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12SPercent2017"].ToString());
                            am.RS12STotal2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12STotal2018"].ToString());
                            am.RS12SPass2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12SPass2018"].ToString());
                            am.RS12SPercent2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12SPercent2018"].ToString());
                            am.RS12CTotal2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CTotal2017"].ToString());
                            am.RS12CPass2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CPass2017"].ToString());
                            am.RS12CPercent2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CPercent2017"].ToString());
                            am.RS12CTotal2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CTotal2018"].ToString());
                            am.RS12CPass2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CPass2018"].ToString());
                            am.RS12CPercent2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CPercent2018"].ToString());
                            am.RS12VTotal2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VTotal2017"].ToString());
                            am.RS12VPass2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VPass2017"].ToString());
                            am.RS12VPercent2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VPercent2017"].ToString());
                            am.RS12VTotal2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VTotal2018"].ToString());
                            am.RS12VPass2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VPass2018"].ToString());
                            am.RS12VPercent2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VPercent2018"].ToString());
                            am.SF1TC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF1TC2017"].ToString());
                            am.SF1TF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF1TF2017"].ToString());
                            am.SF1TC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF1TC2018"].ToString());
                            am.SF1TF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF1TF2018"].ToString());
                            am.SF1Percent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF1Percent"].ToString());
                            am.SF2TC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF2TC2017"].ToString());
                            am.SF2TF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF2TF2017"].ToString());
                            am.SF2TC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF2TC2018"].ToString());
                            am.SF2TF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF2TF2018"].ToString());
                            am.SF2Percent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF2Percent"].ToString());
                            am.SF3TC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF3TC2017"].ToString());
                            am.SF3TF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF3TF2017"].ToString());
                            am.SF3TC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF3TC2018"].ToString());
                            am.SF3TF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF3TF2018"].ToString());
                            am.SF3Percent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF3Percent"].ToString());
                            am.SF4TC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF4TC2017"].ToString());
                            am.SF4TF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF4TF2017"].ToString());
                            am.SF4TC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF4TC2018"].ToString());
                            am.SF4TF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF4TF2018"].ToString());
                            am.SF4Percent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF4Percent"].ToString());
                            am.SF5TC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF5TC2017"].ToString());
                            am.SF5TF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF5TF2017"].ToString());
                            am.SF5TC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF5TC2018"].ToString());
                            am.SF5TF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF5TF2018"].ToString());
                            am.SF5Percent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF5Percent"].ToString());
                            am.SF6TC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF6TC2017"].ToString());
                            am.SF6TF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF6TF2017"].ToString());
                            am.SF6TC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF6TC2018"].ToString());
                            am.SF6TF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF6TF2018"].ToString());
                            am.SF6Percent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF6Percent"].ToString());
                            am.SF7TC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF7TC2017"].ToString());
                            am.SF7TF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF7TF2017"].ToString());
                            am.SF7TC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF7TC2018"].ToString());
                            am.SF7TF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF7TF2018"].ToString());
                            am.SF7Percent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF7Percent"].ToString());
                            am.SF8TC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF8TC2017"].ToString());
                            am.SF8TF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF8TF2017"].ToString());
                            am.SF8TC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF8TC2018"].ToString());
                            am.SF8TF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF8TF2018"].ToString());
                            am.SF8Percent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF8Percent"].ToString());
                            am.SF9TC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF9TC2017"].ToString());
                            am.SF9TF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF9TF2017"].ToString());
                            am.SF9TC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF9TC2018"].ToString());
                            am.SF9TF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF9TF2018"].ToString());
                            am.SF9Percent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF9Percent"].ToString());
                            am.SF10TC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF10TC2017"].ToString());
                            am.SF10TF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF10TF2017"].ToString());
                            am.SF10TC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF10TC2018"].ToString());
                            am.SF10TF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF10TF2018"].ToString());
                            am.SF10Percent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF10Percent"].ToString());
                            am.SF11HTC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11HTC2017"].ToString());
                            am.SF11HTF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11HTF2017"].ToString());
                            am.SF11HTC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11HTC2018"].ToString());
                            am.SF11HTF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11HTF2018"].ToString());
                            am.SF11HPercent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11HPercent"].ToString());
                            am.SF11STC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11STC2017"].ToString());
                            am.SF11STF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11STF2017"].ToString());
                            am.SF11STC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11STC2018"].ToString());
                            am.SF11STF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11STF2018"].ToString());
                            am.SF11SPercent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11SPercent"].ToString());
                            am.SF11CTC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11CTC2017"].ToString());
                            am.SF11CTF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11CTF2017"].ToString());
                            am.SF11CTC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11CTC2018"].ToString());
                            am.SF11CTF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11CTF2018"].ToString());
                            am.SF11CPercent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11CPercent"].ToString());
                            am.SF11VTC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11VTC2017"].ToString());
                            am.SF11VTF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11VTF2017"].ToString());
                            am.SF11VTC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11VTC2018"].ToString());
                            am.SF11VTF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11VTF2018"].ToString());
                            am.SF11VPercent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF11VPercent"].ToString());
                            am.SF12HTC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12HTC2017"].ToString());
                            am.SF12HTF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12HTF2017"].ToString());
                            am.SF12HTC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12HTC2018"].ToString());
                            am.SF12HTF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12HTF2018"].ToString());
                            am.SF12HPercent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12HPercent"].ToString());
                            am.SF12STC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12STC2017"].ToString());
                            am.SF12STF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12STF2017"].ToString());
                            am.SF12STC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12STC2018"].ToString());
                            am.SF12STF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12STF2018"].ToString());
                            am.SF12SPercent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12SPercent"].ToString());
                            am.SF12CTC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12CTC2017"].ToString());
                            am.SF12CTF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12CTF2017"].ToString());
                            am.SF12CTC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12CTC2018"].ToString());
                            am.SF12CTF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12CTF2018"].ToString());
                            am.SF12CPercent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12CPercent"].ToString());
                            am.SF12VTC2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12VTC2017"].ToString());
                            am.SF12VTF2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12VTF2017"].ToString());
                            am.SF12VTC2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12VTC2018"].ToString());
                            am.SF12VTF2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12VTF2018"].ToString());
                            am.SF12VPercent = Convert.ToInt32(ds.Tables[0].Rows[0]["SF12VPercent"].ToString());
                            am.BSDSES = Convert.ToString(ds.Tables[0].Rows[0]["BSDSES"].ToString());
                            am.BSDTINC = Convert.ToDecimal(ds.Tables[0].Rows[0]["BSDTINC"].ToString());
                            // am.BSDTEXP = float.Parse(ds.Tables[0].Rows[0]["BSDTEXP"].ToString(), System.Globalization.CultureInfo.InvariantCulture.NumberFormat); 
                            am.BSDTEXP = Convert.ToDecimal(ds.Tables[0].Rows[0]["BSDTEXP"].ToString());
                            am.BSDNAME = Convert.ToString(ds.Tables[0].Rows[0]["BSDNAME"].ToString());
                            am.BSDIDNO = Convert.ToString(ds.Tables[0].Rows[0]["BSDIDNO"].ToString());
                            am.BSDADD = Convert.ToString(ds.Tables[0].Rows[0]["BSDADD"].ToString());
                            am.BSDFILE = Convert.ToString(ds.Tables[0].Rows[0]["BSDFILE"].ToString());

                            am.BSFROM = Convert.ToString(ds.Tables[0].Rows[0]["BSFROM"].ToString());
                            am.BSTO = Convert.ToString(ds.Tables[0].Rows[0]["BSTO"].ToString());
                            am.BSIA = Convert.ToString(ds.Tables[0].Rows[0]["BSIA"].ToString());
                            am.BSMEMO = Convert.ToString(ds.Tables[0].Rows[0]["BSMEMO"].ToString());
                            am.BSIDATE = Convert.ToString(ds.Tables[0].Rows[0]["BSIDATE"].ToString());
                            //am.BSIDATE = Convert.ToDateTime(string.IsNullOrEmpty(ds.Tables[0].Rows[0]["BSIDATE"].ToString()) ? "1990-01-01 00:00:00.000" : ds.Tables[0].Rows[0]["BSIDATE"].ToString());
                            am.BSFILE = Convert.ToString(ds.Tables[0].Rows[0]["BSFILE"].ToString());

                            am.FSFROM = Convert.ToString(ds.Tables[0].Rows[0]["FSFROM"].ToString());
                            am.FSTO = Convert.ToString(ds.Tables[0].Rows[0]["FSTO"].ToString());
                            am.FSIA = Convert.ToString(ds.Tables[0].Rows[0]["FSIA"].ToString());
                            am.FSMEMO = Convert.ToString(ds.Tables[0].Rows[0]["FSMEMO"].ToString());
                            // am.FSIDATE = Convert.ToDateTime(string.IsNullOrEmpty(ds.Tables[0].Rows[0]["FSIDATE"].ToString()) ? "1990-01-01 00:00:00.000" : ds.Tables[0].Rows[0]["FSIDATE"].ToString());
                            am.FSIDATE = Convert.ToString(ds.Tables[0].Rows[0]["FSIDATE"].ToString());
                            am.FSFILE = Convert.ToString(ds.Tables[0].Rows[0]["FSFILE"].ToString());

                            am.BPTS = Convert.ToInt32(ds.Tables[0].Rows[0]["BPTS"].ToString());
                            am.BPBOOKPERCENT = Convert.ToInt32(ds.Tables[0].Rows[0]["BPBOOKPERCENT"].ToString());
                            am.BPAMOUNT = Convert.ToDecimal(ds.Tables[0].Rows[0]["BPAMOUNT"].ToString());
                            am.BPNAME = Convert.ToString(ds.Tables[0].Rows[0]["BPNAME"].ToString());
                            am.BPBILL = Convert.ToString(ds.Tables[0].Rows[0]["BPBILL"].ToString());
                            am.BPBILLDATE = Convert.ToString(ds.Tables[0].Rows[0]["BPBILLDATE"].ToString());
                            //am.BPBILLDATE = Convert.ToDateTime(string.IsNullOrEmpty(ds.Tables[0].Rows[0]["BPBILLDATE"].ToString()) ? "1990-01-01 00:00:00.000" : ds.Tables[0].Rows[0]["BPBILLDATE"].ToString());


                            am.BPFILE = Convert.ToString(ds.Tables[0].Rows[0]["BPFILE"].ToString());
                            am.ASDIST = Convert.ToString(ds.Tables[0].Rows[0]["ASDIST"].ToString());
                            am.ASZONE = Convert.ToString(ds.Tables[0].Rows[0]["ASZONE"].ToString());
                            am.ASNATIONAL = Convert.ToString(ds.Tables[0].Rows[0]["ASNATIONAL"].ToString());
                            am.ASINTER = Convert.ToString(ds.Tables[0].Rows[0]["ASINTER"].ToString());
                            am.ASSTATE = Convert.ToString(ds.Tables[0].Rows[0]["ASSTATE"].ToString());
                            am.AOTH = Convert.ToString(ds.Tables[0].Rows[0]["AOTH"].ToString());
                            //
                            am.OI1 = Convert.ToString(ds.Tables[0].Rows[0]["OI1"].ToString());
                            am.OI2 = Convert.ToString(ds.Tables[0].Rows[0]["OI2"].ToString());
                            am.OI3 = Convert.ToString(ds.Tables[0].Rows[0]["OI3"].ToString());
                            am.OI4 = Convert.ToString(ds.Tables[0].Rows[0]["OI4"].ToString());
                            am.OI5 = Convert.ToString(ds.Tables[0].Rows[0]["OI5"].ToString());
                            am.OI6 = Convert.ToString(ds.Tables[0].Rows[0]["OI6"].ToString());
                            am.OI7 = Convert.ToString(ds.Tables[0].Rows[0]["OI7"].ToString());
                            am.ISACTIVE = Convert.ToBoolean(ds.Tables[0].Rows[0]["ISACTIVE"].ToString());
                            am.CREATEDDATE = Convert.ToDateTime(string.IsNullOrEmpty(ds.Tables[0].Rows[0]["CREATEDDATE"].ToString()) ? "1990-01-01 00:00:00.000" : ds.Tables[0].Rows[0]["CREATEDDATE"].ToString());
                            am.ChallanCategory = Convert.ToInt32(ds.Tables[0].Rows[0]["ChallanCategory"].ToString());
                            am.ChallanId = Convert.ToString(ds.Tables[0].Rows[0]["ChallanId"].ToString());
                            am.ChallanDt = Convert.ToString(ds.Tables[0].Rows[0]["ChallanDt"].ToString());
                            am.challanVerify = Convert.ToInt32(ds.Tables[0].Rows[0]["challanVerify"].ToString());
                            am.OldRecieptNo = Convert.ToString(ds.Tables[0].Rows[0]["OldRecieptNo"].ToString());
                            am.oldChallanId = Convert.ToString(ds.Tables[0].Rows[0]["oldChallanId"].ToString());
                            am.OldAmount = Convert.ToInt32(ds.Tables[0].Rows[0]["OldAmount"].ToString());
                            am.OldDepositDate = Convert.ToString(ds.Tables[0].Rows[0]["OldDepositDate"].ToString());
                            //
                            am.CourtCaseFile = Convert.ToString(ds.Tables[0].Rows[0]["CourtCaseFile"].ToString());

                        }
                    }
                    return am;
                }
            }
            catch (Exception ex)
            {
                ds1 = null;
                return am = null;
            }
        }


        public int AffiliationContinuation(AffiliationModel am, out string OutError)
        {
            int result;
            OutError = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AffiliationContinuationSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", am.ID);// if ID=0 then Insert else Update
                    cmd.Parameters.AddWithValue("@SCHL", am.SCHL);
                    cmd.Parameters.AddWithValue("@RS10GTotal2017", am.RS10GTotal2017);
                    cmd.Parameters.AddWithValue("@RS10GPass2017", am.RS10GPass2017);
                    cmd.Parameters.AddWithValue("@RS10GPercent2017", am.RS10GPercent2017);
                    cmd.Parameters.AddWithValue("@RS10GTotal2018", am.RS10GTotal2018);
                    cmd.Parameters.AddWithValue("@RS10GPass2018", am.RS10GPass2018);
                    cmd.Parameters.AddWithValue("@RS10GPercent2018", am.RS10GPercent2018);
                    cmd.Parameters.AddWithValue("@RS12HTotal2017", am.RS12HTotal2017);
                    cmd.Parameters.AddWithValue("@RS12HPass2017", am.RS12HPass2017);
                    cmd.Parameters.AddWithValue("@RS12HPercent2017", am.RS12HPercent2017);
                    cmd.Parameters.AddWithValue("@RS12HTotal2018", am.RS12HTotal2018);
                    cmd.Parameters.AddWithValue("@RS12HPass2018", am.RS12HPass2018);
                    cmd.Parameters.AddWithValue("@RS12HPercent2018", am.RS12HPercent2018);
                    cmd.Parameters.AddWithValue("@RS12STotal2017", am.RS12STotal2017);
                    cmd.Parameters.AddWithValue("@RS12SPass2017", am.RS12SPass2017);
                    cmd.Parameters.AddWithValue("@RS12SPercent2017", am.RS12SPercent2017);
                    cmd.Parameters.AddWithValue("@RS12STotal2018", am.RS12STotal2018);
                    cmd.Parameters.AddWithValue("@RS12SPass2018", am.RS12SPass2018);
                    cmd.Parameters.AddWithValue("@RS12SPercent2018", am.RS12SPercent2018);
                    cmd.Parameters.AddWithValue("@RS12CTotal2017", am.RS12CTotal2017);
                    cmd.Parameters.AddWithValue("@RS12CPass2017", am.RS12CPass2017);
                    cmd.Parameters.AddWithValue("@RS12CPercent2017", am.RS12CPercent2017);
                    cmd.Parameters.AddWithValue("@RS12CTotal2018", am.RS12CTotal2018);
                    cmd.Parameters.AddWithValue("@RS12CPass2018", am.RS12CPass2018);
                    cmd.Parameters.AddWithValue("@RS12CPercent2018", am.RS12CPercent2018);
                    cmd.Parameters.AddWithValue("@RS12VTotal2017", am.RS12VTotal2017);
                    cmd.Parameters.AddWithValue("@RS12VPass2017", am.RS12VPass2017);
                    cmd.Parameters.AddWithValue("@RS12VPercent2017", am.RS12VPercent2017);
                    cmd.Parameters.AddWithValue("@RS12VTotal2018", am.RS12VTotal2018);
                    cmd.Parameters.AddWithValue("@RS12VPass2018", am.RS12VPass2018);
                    cmd.Parameters.AddWithValue("@RS12VPercent2018", am.RS12VPercent2018);
                    cmd.Parameters.AddWithValue("@SF1TC2017", am.SF1TC2017);
                    cmd.Parameters.AddWithValue("@SF1TF2017", am.SF1TF2017);
                    cmd.Parameters.AddWithValue("@SF1TC2018", am.SF1TC2018);
                    cmd.Parameters.AddWithValue("@SF1TF2018", am.SF1TF2018);
                    cmd.Parameters.AddWithValue("@SF1Percent", am.SF1Percent);
                    cmd.Parameters.AddWithValue("@SF2TC2017", am.SF2TC2017);
                    cmd.Parameters.AddWithValue("@SF2TF2017", am.SF2TF2017);
                    cmd.Parameters.AddWithValue("@SF2TC2018", am.SF2TC2018);
                    cmd.Parameters.AddWithValue("@SF2TF2018", am.SF2TF2018);
                    cmd.Parameters.AddWithValue("@SF2Percent", am.SF2Percent);
                    cmd.Parameters.AddWithValue("@SF3TC2017", am.SF3TC2017);
                    cmd.Parameters.AddWithValue("@SF3TF2017", am.SF3TF2017);
                    cmd.Parameters.AddWithValue("@SF3TC2018", am.SF3TC2018);
                    cmd.Parameters.AddWithValue("@SF3TF2018", am.SF3TF2018);
                    cmd.Parameters.AddWithValue("@SF3Percent", am.SF3Percent);
                    cmd.Parameters.AddWithValue("@SF4TC2017", am.SF4TC2017);
                    cmd.Parameters.AddWithValue("@SF4TF2017", am.SF4TF2017);
                    cmd.Parameters.AddWithValue("@SF4TC2018", am.SF4TC2018);
                    cmd.Parameters.AddWithValue("@SF4TF2018", am.SF4TF2018);
                    cmd.Parameters.AddWithValue("@SF4Percent", am.SF4Percent);
                    cmd.Parameters.AddWithValue("@SF5TC2017", am.SF5TC2017);
                    cmd.Parameters.AddWithValue("@SF5TF2017", am.SF5TF2017);
                    cmd.Parameters.AddWithValue("@SF5TC2018", am.SF5TC2018);
                    cmd.Parameters.AddWithValue("@SF5TF2018", am.SF5TF2018);
                    cmd.Parameters.AddWithValue("@SF5Percent", am.SF5Percent);
                    cmd.Parameters.AddWithValue("@SF6TC2017", am.SF6TC2017);
                    cmd.Parameters.AddWithValue("@SF6TF2017", am.SF6TF2017);
                    cmd.Parameters.AddWithValue("@SF6TC2018", am.SF6TC2018);
                    cmd.Parameters.AddWithValue("@SF6TF2018", am.SF6TF2018);
                    cmd.Parameters.AddWithValue("@SF6Percent", am.SF6Percent);
                    cmd.Parameters.AddWithValue("@SF7TC2017", am.SF7TC2017);
                    cmd.Parameters.AddWithValue("@SF7TF2017", am.SF7TF2017);
                    cmd.Parameters.AddWithValue("@SF7TC2018", am.SF7TC2018);
                    cmd.Parameters.AddWithValue("@SF7TF2018", am.SF7TF2018);
                    cmd.Parameters.AddWithValue("@SF7Percent", am.SF7Percent);
                    cmd.Parameters.AddWithValue("@SF8TC2017", am.SF8TC2017);
                    cmd.Parameters.AddWithValue("@SF8TF2017", am.SF8TF2017);
                    cmd.Parameters.AddWithValue("@SF8TC2018", am.SF8TC2018);
                    cmd.Parameters.AddWithValue("@SF8TF2018", am.SF8TF2018);
                    cmd.Parameters.AddWithValue("@SF8Percent", am.SF8Percent);
                    cmd.Parameters.AddWithValue("@SF9TC2017", am.SF9TC2017);
                    cmd.Parameters.AddWithValue("@SF9TF2017", am.SF9TF2017);
                    cmd.Parameters.AddWithValue("@SF9TC2018", am.SF9TC2018);
                    cmd.Parameters.AddWithValue("@SF9TF2018", am.SF9TF2018);
                    cmd.Parameters.AddWithValue("@SF9Percent", am.SF9Percent);
                    cmd.Parameters.AddWithValue("@SF10TC2017", am.SF10TC2017);
                    cmd.Parameters.AddWithValue("@SF10TF2017", am.SF10TF2017);
                    cmd.Parameters.AddWithValue("@SF10TC2018", am.SF10TC2018);
                    cmd.Parameters.AddWithValue("@SF10TF2018", am.SF10TF2018);
                    cmd.Parameters.AddWithValue("@SF10Percent", am.SF10Percent);
                    cmd.Parameters.AddWithValue("@SF11HTC2017", am.SF11HTC2017);
                    cmd.Parameters.AddWithValue("@SF11HTF2017", am.SF11HTF2017);
                    cmd.Parameters.AddWithValue("@SF11HTC2018", am.SF11HTC2018);
                    cmd.Parameters.AddWithValue("@SF11HTF2018", am.SF11HTF2018);
                    cmd.Parameters.AddWithValue("@SF11HPercent", am.SF11HPercent);
                    cmd.Parameters.AddWithValue("@SF11STC2017", am.SF11STC2017);
                    cmd.Parameters.AddWithValue("@SF11STF2017", am.SF11STF2017);
                    cmd.Parameters.AddWithValue("@SF11STC2018", am.SF11STC2018);
                    cmd.Parameters.AddWithValue("@SF11STF2018", am.SF11STF2018);
                    cmd.Parameters.AddWithValue("@SF11SPercent", am.SF11SPercent);
                    cmd.Parameters.AddWithValue("@SF11CTC2017", am.SF11CTC2017);
                    cmd.Parameters.AddWithValue("@SF11CTF2017", am.SF11CTF2017);
                    cmd.Parameters.AddWithValue("@SF11CTC2018", am.SF11CTC2018);
                    cmd.Parameters.AddWithValue("@SF11CTF2018", am.SF11CTF2018);
                    cmd.Parameters.AddWithValue("@SF11CPercent", am.SF11CPercent);
                    cmd.Parameters.AddWithValue("@SF11VTC2017", am.SF11VTC2017);
                    cmd.Parameters.AddWithValue("@SF11VTF2017", am.SF11VTF2017);
                    cmd.Parameters.AddWithValue("@SF11VTC2018", am.SF11VTC2018);
                    cmd.Parameters.AddWithValue("@SF11VTF2018", am.SF11VTF2018);
                    cmd.Parameters.AddWithValue("@SF11VPercent", am.SF11VPercent);
                    cmd.Parameters.AddWithValue("@SF12HTC2017", am.SF12HTC2017);
                    cmd.Parameters.AddWithValue("@SF12HTF2017", am.SF12HTF2017);
                    cmd.Parameters.AddWithValue("@SF12HTC2018", am.SF12HTC2018);
                    cmd.Parameters.AddWithValue("@SF12HTF2018", am.SF12HTF2018);
                    cmd.Parameters.AddWithValue("@SF12HPercent", am.SF12HPercent);
                    cmd.Parameters.AddWithValue("@SF12STC2017", am.SF12STC2017);
                    cmd.Parameters.AddWithValue("@SF12STF2017", am.SF12STF2017);
                    cmd.Parameters.AddWithValue("@SF12STC2018", am.SF12STC2018);
                    cmd.Parameters.AddWithValue("@SF12STF2018", am.SF12STF2018);
                    cmd.Parameters.AddWithValue("@SF12SPercent", am.SF12SPercent);
                    cmd.Parameters.AddWithValue("@SF12CTC2017", am.SF12CTC2017);
                    cmd.Parameters.AddWithValue("@SF12CTF2017", am.SF12CTF2017);
                    cmd.Parameters.AddWithValue("@SF12CTC2018", am.SF12CTC2018);
                    cmd.Parameters.AddWithValue("@SF12CTF2018", am.SF12CTF2018);
                    cmd.Parameters.AddWithValue("@SF12CPercent", am.SF12CPercent);
                    cmd.Parameters.AddWithValue("@SF12VTC2017", am.SF12VTC2017);
                    cmd.Parameters.AddWithValue("@SF12VTF2017", am.SF12VTF2017);
                    cmd.Parameters.AddWithValue("@SF12VTC2018", am.SF12VTC2018);
                    cmd.Parameters.AddWithValue("@SF12VTF2018", am.SF12VTF2018);
                    cmd.Parameters.AddWithValue("@SF12VPercent", am.SF12VPercent);
                    cmd.Parameters.AddWithValue("@BSDSES", am.BSDSES);
                    cmd.Parameters.AddWithValue("@BSDTINC", am.BSDTINC);
                    cmd.Parameters.AddWithValue("@BSDTEXP", am.BSDTEXP);
                    cmd.Parameters.AddWithValue("@BSDNAME", am.BSDNAME);
                    cmd.Parameters.AddWithValue("@BSDIDNO", am.BSDIDNO);
                    cmd.Parameters.AddWithValue("@BSDADD", am.BSDADD);
                    cmd.Parameters.AddWithValue("@BSDFILE", am.BSDFILE);
                    cmd.Parameters.AddWithValue("@BSFROM", am.BSFROM);
                    cmd.Parameters.AddWithValue("@BSTO", am.BSTO);
                    cmd.Parameters.AddWithValue("@BSIA", am.BSIA);
                    cmd.Parameters.AddWithValue("@BSMEMO", am.BSMEMO);
                    cmd.Parameters.AddWithValue("@BSIDATE", am.BSIDATE);

                    //  cmd.Parameters.AddWithValue("@BSIDATE", (am.BSIDATE == null  am.BSIDATE == DateTime.MinValue) ? Convert.ToDateTime("1990-01-01 00:00:00.000") : am.BSIDATE);
                    cmd.Parameters.AddWithValue("@BSFILE", am.BSFILE);
                    cmd.Parameters.AddWithValue("@FSFROM", am.FSFROM);
                    cmd.Parameters.AddWithValue("@FSTO", am.FSTO);
                    cmd.Parameters.AddWithValue("@FSIA", am.FSIA);
                    cmd.Parameters.AddWithValue("@FSMEMO", am.FSMEMO);
                    cmd.Parameters.AddWithValue("@FSIDATE", am.FSIDATE);
                    //  cmd.Parameters.AddWithValue("@FSIDATE", (am.FSIDATE == DateTime.MinValue) ? Convert.ToDateTime("1990-01-01 00:00:00.000") : am.FSIDATE);
                    cmd.Parameters.AddWithValue("@FSFILE", am.FSFILE);
                    cmd.Parameters.AddWithValue("@BPTS", am.BPTS);
                    cmd.Parameters.AddWithValue("@BPBOOKPERCENT", am.BPBOOKPERCENT);
                    cmd.Parameters.AddWithValue("@BPAMOUNT", am.BPAMOUNT);
                    cmd.Parameters.AddWithValue("@BPNAME", am.BPNAME);
                    cmd.Parameters.AddWithValue("@BPBILL", am.BPBILL);
                    cmd.Parameters.AddWithValue("@BPBILLDATE", am.BPBILLDATE);
                    // cmd.Parameters.AddWithValue("@BPBILLDATE", (am.BPBILLDATE == null  am.BPBILLDATE == DateTime.MinValue) ? Convert.ToDateTime("1990-01-01 00:00:00.000") : am.BPBILLDATE); 
                    cmd.Parameters.AddWithValue("@BPFILE", am.BPFILE);
                    cmd.Parameters.AddWithValue("@ASDIST", am.ASDIST);
                    cmd.Parameters.AddWithValue("@ASZONE", am.ASZONE);
                    cmd.Parameters.AddWithValue("@ASNATIONAL", am.ASNATIONAL);
                    cmd.Parameters.AddWithValue("@ASINTER", am.ASINTER);
                    cmd.Parameters.AddWithValue("@ASSTATE", am.ASSTATE);
                    cmd.Parameters.AddWithValue("@AOTH", am.AOTH);
                    //
                    cmd.Parameters.AddWithValue("@OI1", am.OI1);
                    cmd.Parameters.AddWithValue("@OI2", am.OI2);
                    cmd.Parameters.AddWithValue("@OI3", am.OI3);
                    cmd.Parameters.AddWithValue("@OI4", am.OI4);
                    cmd.Parameters.AddWithValue("@OI5", am.OI5);
                    cmd.Parameters.AddWithValue("@OI6", am.OI6);
                    cmd.Parameters.AddWithValue("@OI7", am.OI7);
                    cmd.Parameters.AddWithValue("@ISACTIVE", true);
                    cmd.Parameters.AddWithValue("@OldAmount", am.OldAmount);
                    cmd.Parameters.AddWithValue("@oldChallanId", am.oldChallanId);
                    cmd.Parameters.AddWithValue("@OldDepositDate", am.OldDepositDate);
                    cmd.Parameters.AddWithValue("@OldRecieptNo", am.OldRecieptNo);
                    cmd.Parameters.AddWithValue("@ChallanCategory", am.ChallanCategory);
                    cmd.Parameters.AddWithValue("@ReceiptScannedCopy", am.ReceiptScannedCopy);
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return result;

                    //con.Open();
                    //result = cmd.ExecuteNonQuery();
                    //return result;

                }
            }
            catch (Exception ex)
            {
                OutError = "-1";
                return result = -1;
            }
            finally
            {
                // con.Close();
            }
        }


        public AffiliationModel GetAffiliationContinuationBySchlAndType(string schl, int type, out DataSet ds1)
        {
            AffiliationModel am = new AffiliationModel();
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAffiliationContinuationBySchlAndType", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@type", type);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        ds1 = null;
                        return null;
                    }
                    else
                    {
                        ds1 = ds;
                        am.SCHL = ds.Tables[0].Rows[0]["schl"].ToString();
                        if (am.SCHL != "")
                        {
                            am.RS10GTotal2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GTotal2017"].ToString());
                            am.RS10GPass2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GPass2017"].ToString());
                            am.RS10GPercent2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GPercent2017"].ToString());
                            am.RS10GTotal2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GTotal2018"].ToString());
                            am.RS10GPass2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GPass2018"].ToString());
                            am.RS10GPercent2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS10GPercent2018"].ToString());
                            am.RS12HTotal2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HTotal2017"].ToString());
                            am.RS12HPass2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HPass2017"].ToString());
                            am.RS12HPercent2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HPercent2017"].ToString());
                            am.RS12HTotal2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HTotal2018"].ToString());
                            am.RS12HPass2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HPass2018"].ToString());
                            am.RS12HPercent2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12HPercent2018"].ToString());
                            am.RS12STotal2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12STotal2017"].ToString());
                            am.RS12SPass2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12SPass2017"].ToString());
                            am.RS12SPercent2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12SPercent2017"].ToString());
                            am.RS12STotal2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12STotal2018"].ToString());
                            am.RS12SPass2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12SPass2018"].ToString());
                            am.RS12SPercent2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12SPercent2018"].ToString());
                            am.RS12CTotal2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CTotal2017"].ToString());
                            am.RS12CPass2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CPass2017"].ToString());
                            am.RS12CPercent2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CPercent2017"].ToString());
                            am.RS12CTotal2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CTotal2018"].ToString());
                            am.RS12CPass2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CPass2018"].ToString());
                            am.RS12CPercent2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12CPercent2018"].ToString());
                            am.RS12VTotal2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VTotal2017"].ToString());
                            am.RS12VPass2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VPass2017"].ToString());
                            am.RS12VPercent2017 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VPercent2017"].ToString());
                            am.RS12VTotal2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VTotal2018"].ToString());
                            am.RS12VPass2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VPass2018"].ToString());
                            am.RS12VPercent2018 = Convert.ToInt32(ds.Tables[0].Rows[0]["RS12VPercent2018"].ToString());
                        }
                    }
                    return am;
                }
            }
            catch (Exception ex)
            {
                ds1 = null;
                return am = null;
            }
        }


        public AffiliationFee AffiliationFee(int Cat, string schl, DateTime dt1)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AffiliationFeeSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Cat", Cat);
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@Challandt", dt1);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    try
                    {
                        if (result.Tables[0].Rows.Count > 0)
                        {
                            AffiliationFee _affiliationFee = new AffiliationFee();
                            DataRow dr = result.Tables[0].Rows[0];
                            _affiliationFee.SCHL = dr["SCHL"].ToString();
                            _affiliationFee.Class = dr["Class"].ToString();
                            _affiliationFee.Form = dr["Form"].ToString();
                            _affiliationFee.sDate = dr["sDate"].ToString();
                            _affiliationFee.eDate = Convert.ToString(dr["eDate"].ToString());
                            _affiliationFee.fee = Convert.ToInt32(dr["fee"].ToString());
                            _affiliationFee.latefee = Convert.ToInt32(dr["latefee"].ToString());
                            _affiliationFee.totfee = Convert.ToInt32(dr["totfee"].ToString());
                            _affiliationFee.FEECODE = Convert.ToInt32(dr["FEECODE"].ToString());
                            _affiliationFee.FEECAT = Convert.ToString(dr["FEECAT"].ToString());
                            _affiliationFee.BankLastdate = Convert.ToString(dr["BankLastDate"].ToString());
                            _affiliationFee.Type = dr["Type"].ToString();
                            _affiliationFee.AllowBanks = dr["AllowBanks"].ToString();
                            _affiliationFee.TotalFeesInWords = dr["TotalFeesInWords"].ToString();
                            _affiliationFee.ChallanCategory = Cat;
                            return _affiliationFee;
                        }
                        else
                        {
                            return new AffiliationFee();
                        }
                    }
                    catch (Exception ex)
                    {
                        return new AffiliationFee();
                    }
                }
            }
            catch (Exception ex)
            {
                return new AffiliationFee();
            }
        }

        public int UpdateAlreadyPaidAffiliationFee(AffiliationModel am)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UpdateAlreadyPaidAffiliationFeeSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", am.SCHL);
                    cmd.Parameters.AddWithValue("@OldAmount", am.OldAmount);
                    cmd.Parameters.AddWithValue("@oldChallanId", am.oldChallanId);
                    cmd.Parameters.AddWithValue("@OldDepositDate", am.OldDepositDate);
                    cmd.Parameters.AddWithValue("@OldRecieptNo", am.OldRecieptNo);
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    return result;

                }
            }
            catch (Exception ex)
            {
                return result = -1;
            }
        }


        public string IsValidForChallan(string schl)
        {   
            string res = string.Empty;         
            DataSet outDsAm = new DataSet();
            AffiliationModel am1 = AffiliationContinuationBySchl(schl, 2, out outDsAm);
            if (am1.ID > 0)
            {

                int TotalStaff = outDsAm.Tables[3].Rows.Count;
                string principal = outDsAm.Tables[2].Rows[0]["PRINCIPAL"].ToString();
                string mobile = outDsAm.Tables[2].Rows[0]["MOBILE"].ToString();
                if (TotalStaff == 0) { res += "TotalStaff must be greater than Zero, "; }
                if (string.IsNullOrEmpty(principal)) { res += "Principal, "; }
                if (string.IsNullOrEmpty(mobile)) { res += "Mobile, "; }


                // Comment due to hidden in 2019-20
                ////if (string.IsNullOrEmpty(am1.BSDSES)) { res += "Balance Sheet : Session, "; }
                ////if (string.IsNullOrEmpty(am1.BSDNAME)) { res += "Balance Sheet : CA Name, "; }
                ////if (am1.BSDTEXP == 0 ) { res += "Balance Sheet : Total Expenditure must be greater than Zero, "; }
                ////if (am1.BSDTINC == 0) { res += "Balance Sheet : Total Income  must be greater than Zero, "; }
                ////if (string.IsNullOrEmpty(am1.BSDIDNO)) { res += "Balance Sheet : CA Identity No , "; }
                ////if (string.IsNullOrEmpty(am1.BSDADD)) { res += "Balance Sheet : CA Address, "; }

                ////if (string.IsNullOrEmpty(am1.BSFROM)) { res += "Building Safety : Validity Period, "; }
                ////if (string.IsNullOrEmpty(am1.BSTO)) { res += "Building Safety : Validity Period, "; }
                ////if (string.IsNullOrEmpty(am1.BSIA)) { res += "Building Safety : Issuing Authority, "; }
                ////if (string.IsNullOrEmpty(am1.BSMEMO)) { res += "Building Safety : Memo No, "; }
                ////if (string.IsNullOrEmpty(am1.BSIDATE)) { res += "Building Safety : Issuing Date, "; }

                ////if (string.IsNullOrEmpty(am1.FSFROM)) { res += "Fire Safety : Validity Period, "; }
                ////if (string.IsNullOrEmpty(am1.FSTO)) { res += "Fire Safety : Validity Period, "; }
                ////if (string.IsNullOrEmpty(am1.FSIA)) { res += "Fire Safety : Issuing Authority, "; }
                ////if (string.IsNullOrEmpty(am1.FSMEMO)) { res += "Fire Safety : Memo No, "; }
                ////if (string.IsNullOrEmpty(am1.FSIDATE)) { res += "Fire Safety : Issuing Date, "; }     

                ////if (am1.BPTS == 0) { res += "Book Purchase : Total Students must be greater than Zero, "; }
                ////if (am1.BPAMOUNT == 0) { res += "Book Purchase : Total Amount must be greater than Zero, "; }
                ////if (string.IsNullOrEmpty(am1.BPBILL)) { res += "Book Purchase : Bill No, "; }
                ////if (string.IsNullOrEmpty(am1.BPBILLDATE)) { res += "Book Purchase : Bill Date, "; }
                ////if (am1.BPBOOKPERCENT == 0) { res += "Book Purchase : Percentage must be greater than Zero, "; }
                ////if (string.IsNullOrEmpty(am1.BPNAME)) { res += "Book Purchase : Name, "; }

                if (string.IsNullOrEmpty(am1.OI1)) { res += "OtherInformation : No-1, "; }
                if (string.IsNullOrEmpty(am1.OI2)) { res += "OtherInformation : No-2, "; }
                if (string.IsNullOrEmpty(am1.OI3)) { res += "OtherInformation : No-3, "; }
                if (string.IsNullOrEmpty(am1.OI4)) { res += "OtherInformation : No-4, "; }
                if (string.IsNullOrEmpty(am1.OI5)) { res += "OtherInformation : No-5, "; }
                if (string.IsNullOrEmpty(am1.OI6)) { res += "OtherInformation : No-6, "; }
                if (string.IsNullOrEmpty(am1.OI7)) { res += "OtherInformation : No-7, "; }


                if (string.IsNullOrEmpty(am1.BSDFILE)) { res += "Balance Sheet Document, "; }
                if (string.IsNullOrEmpty(am1.BSFILE)) { res += "Building Safety Document, "; }
                if (string.IsNullOrEmpty(am1.FSFILE)) { res += "Fire Safety Document, "; }

                if (string.IsNullOrEmpty(am1.CourtCaseFile) && am1.OI1 == "YES")
                {
                    res += "Court Case Document, ";
                }
            }
            return res;
        }

        #region AffiliationContinuationReport
        public DataSet AffiliationContinuationReport()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AffiliationContinuationReport", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet ViewAffiliationContinuation(int type1, string search, string schl, int pageIndex, out int OutStatus, int adminid)
        {
            SchoolPremisesInformation sm = new SchoolPremisesInformation();
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ViewAffiliationContinuation", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@PageNumber", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Adminid", adminid);
                    cmd.Parameters.AddWithValue("@Type", type1);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return ds;

                }
            }
            catch (Exception ex)
            {
                OutStatus = 0;
                return null;
            }

        }

        #endregion AffiliationContinuationReport

        public List<AffiliationDocumentMaster> AffiliationDocumentMasterList(DataTable dataTable)
        {
            List<AffiliationDocumentMaster> item = new List<AffiliationDocumentMaster>();
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                item.Add(new AffiliationDocumentMaster { DocumentName = @dr["DocumentName"].ToString().Trim(), DocID = Convert.ToInt32(@dr["DocID"].ToString()) });
            }
            return item;
        }

        public DataSet GetAffiliationDocumentDetails(int type, int eDocId, string schl, string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAffiliationDocumentDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@eDocId", eDocId);
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@search", search);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int InsertAffiliationDocumentDetails(AffiliationDocumentDetailsModel model, int action, out string OutError)
        {
            int result;
            OutError = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("InsertAffiliationDocumentDetailsSP", con);//
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", action);
                    cmd.Parameters.AddWithValue("@eDocId", model.eDocId);
                    cmd.Parameters.AddWithValue("@SCHL", model.SCHL);
                    cmd.Parameters.AddWithValue("@DocID", model.DocID);
                    cmd.Parameters.AddWithValue("@DocFile", model.DocFile);
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                OutError = ex.Message;
                return result = -1;
            }
            finally
            {
                // con.Close();
            }
        }

        public DataSet AffiliationContinuationList(string AdminUser,string search, string clas, int PageNumber, int type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AffiliationContinuationListSP", con);  //SelectPrintList_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AdminUser", AdminUser);
                    cmd.Parameters.AddWithValue("@type", type); // O for Admin 1 for School else Openstudent
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@class", clas);
                    cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public int AffiliationContinuationAction(AffiliationModel am, int actionType, out string OutError)
        {
            int result;
            OutError = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AffiliationContinuationActionSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", actionType);// if ID=0 then Insert else Update
                    cmd.Parameters.AddWithValue("@ID", am.ID);// if ID=0 then Insert else Update
                    cmd.Parameters.AddWithValue("@SCHL", am.SCHL);
                    cmd.Parameters.AddWithValue("@ObjectionLetter", am.ObjectionLetter);
                    cmd.Parameters.AddWithValue("@InspectionReport", am.InspectionReport);
                    cmd.Parameters.AddWithValue("@UpdatedBy", am.UpdatedBy);
                    cmd.Parameters.AddWithValue("@Remarks", am.Remarks);
                    cmd.Parameters.AddWithValue("@EmpUserId", am.EmpUserId);
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;//
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return result;

                    //con.Open();
                    //result = cmd.ExecuteNonQuery();
                    //return result;

                }
            }
            catch (Exception ex)
            {
                OutError = "-1";
                return result = -1;
            }
            finally
            {
                // con.Close();
            }
        }


    }
}