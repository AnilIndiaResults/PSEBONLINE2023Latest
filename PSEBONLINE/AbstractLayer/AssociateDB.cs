using PSEBONLINE.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PSEBONLINE.AbstractLayer
{
    public class AssociateDB
    {
        private string CommonCon = "myDBConnection";

        public static AssociateModel AssociateContinuationBySchlTemp(string schl, int type, out DataSet ds1)
        {
            AssociateModel am = new AssociateModel();
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AssociateContinuationBySchlTemp", con);
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
        public static AssociateModel AssociateContinuationBySchl(string schl, int type, out DataSet ds1)
        {
             AssociateModel am = new AssociateModel();
            AssociateContinuationDashBoardViews AssModel = new AssociateContinuationDashBoardViews();
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            ds1 = null;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AssociateContinuationBySchl", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@type", type);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        ds = null;
                        return am;
                    }
                    else
                    {

                        //ds1 = ds;
                        AssModel.DistrictName = Convert.ToString(ds.Tables[0].Rows[0]["DistrictName"].ToString());
                        AssModel.Address = Convert.ToString(ds.Tables[0].Rows[0]["Address"].ToString());
                        AssModel.ClassLevel = Convert.ToString(ds.Tables[0].Rows[0]["ClassLevel"].ToString());
                        AssModel.DateofBirth = Convert.ToString(ds.Tables[0].Rows[0]["DateofBirth"].ToString());

                        //AssModel.SCHLICode = Convert.ToString(ds.Tables[0].Rows[0]["SCHLICode"].ToString());
                        AssModel.SCHLName = Convert.ToString(ds.Tables[0].Rows[0]["SCHLName"].ToString());
                        AssModel.PrincipalName = Convert.ToString(ds.Tables[0].Rows[0]["PrincipalName"].ToString());
                        AssModel.PinCode = Convert.ToString(ds.Tables[0].Rows[0]["PinCode"].ToString());
                        AssModel.OtherContactPerson = Convert.ToString(ds.Tables[0].Rows[0]["OtherContactPerson"].ToString());
                        AssModel.Qualifications = Convert.ToString(ds.Tables[0].Rows[0]["Qualifications"].ToString());
                        AssModel.StdCodephone = Convert.ToString(ds.Tables[0].Rows[0]["StdCodephone"].ToString());
                        AssModel.TehsilName = Convert.ToString(ds.Tables[0].Rows[0]["TehsilName"].ToString());
                        AssModel.UDISECode = Convert.ToString(ds.Tables[0].Rows[0]["UDISECode"].ToString());
                        AssModel.EastablishmentYear = Convert.ToString(ds.Tables[0].Rows[0]["EastablishmentYear"].ToString());
                        AssModel.DateofJoining = Convert.ToString(ds.Tables[0].Rows[0]["DateofJoining"].ToString());
                        AssModel.Experience = Convert.ToString(ds.Tables[0].Rows[0]["Experience"].ToString());

                    }

                    am.AssociateContinuationDashBoardViews = AssModel;
                }
                    return am;

                }
            catch (Exception ex)
            {
                ds1 = null;
                return am = null;
            }
        }


        public static int AssociateContinuation(AssociateModel am, out string OutError)
        {
            int result;
            OutError = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AssociateContinuationSP", con);
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
        //public static string AssociateContinuationBySafty(AssociateContinuationBuildingSafty AssSaftyModel)
        //{

        //    string result = "OK";


        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
        //        {
        //            SqlCommand cmd = new SqlCommand("AssociateContinuationAddSafty", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@Id", AssSaftyModel.Id);
        //            cmd.Parameters.AddWithValue("@buildingFrom", AssSaftyModel.buildingFrom);
        //            cmd.Parameters.AddWithValue("@buildingUpto", AssSaftyModel.buildingUpto);
        //            cmd.Parameters.AddWithValue("@buildingIssuingAuthority", AssSaftyModel.buildingIssuingAuthority);
        //            cmd.Parameters.AddWithValue("@buildingMemoDispatchNo", AssSaftyModel.buildingMemoDispatchNo);
        //            cmd.Parameters.AddWithValue("@buildingIssuingDate", AssSaftyModel.buildingIssuingDate);
        //            cmd.Parameters.AddWithValue("@FireFrom", AssSaftyModel.FireFrom);
        //            cmd.Parameters.AddWithValue("@FireUpto", AssSaftyModel.FireUpto);
        //            cmd.Parameters.AddWithValue("@FireIssuingAuthority", AssSaftyModel.FireIssuingAuthority);
        //            cmd.Parameters.AddWithValue("@FireMemoDispatchNo", AssSaftyModel.FireMemoDispatchNo);
        //            cmd.Parameters.AddWithValue("@FireIssuingDate", AssSaftyModel.FireIssuingDate);
        //            cmd.ExecuteNonQuery();

        //        }
        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        return result = null;
        //    }
        //}
    
        public static string SaveRoomDetails(AssociateModel roomDetails)
        {
            
            string result = "";


            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Save_AssociateContinuationRoomDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "2");
                    cmd.Parameters.AddWithValue("@Id", roomDetails.RoomDetailsModel.Id);
                    cmd.Parameters.AddWithValue("@SCHL", roomDetails.RoomDetailsModel.SCHL);
                    cmd.Parameters.AddWithValue("@RoomType", roomDetails.RoomDetailsModel.RoomType);
                    cmd.Parameters.AddWithValue("@FloorName", roomDetails.RoomDetailsModel.FloorName);
                    cmd.Parameters.AddWithValue("@Height", roomDetails.RoomDetailsModel.Height);
                    cmd.Parameters.AddWithValue("@width", roomDetails.RoomDetailsModel.width);
                    cmd.Parameters.AddWithValue("@Quantity", roomDetails.RoomDetailsModel.Quantity);
                    cmd.Parameters.AddWithValue("@Area", roomDetails.RoomDetailsModel.Area);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    result = "OK";




                }
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
               
            }
        }

        public static List<RoomDetailsModel> GetAssociateRoomDetails()
        {
            AssociateModel ViewModel = new AssociateModel();
            ViewModel.RoomDetailsModelList = new List<RoomDetailsModel>();
            ViewModel.RoomDetailsModel = new RoomDetailsModel();


            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
           
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Save_AssociateContinuationRoomDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "1");
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dataTable = ds.Tables[0];

                        foreach (DataRow row in dataTable.Rows)
                        {
                            RoomDetailsModel room = new RoomDetailsModel();
                            room.SCHL = Convert.ToString(row["SCHL"]);
                            room.Id = Convert.ToInt32(row["Id"]);
                            room.RoomType = Convert.ToString(row["RoomType"]);
                            room.FloorName = Convert.ToString(row["FloorName"]);
                            room.Height = Convert.ToInt32(row["Height"]);
                            room.width = Convert.ToInt32(row["Width"]);
                            room.Quantity = Convert.ToInt32(row["Quantity"]);
                            room.Area = Convert.ToInt32(row["Area"]);

                            ViewModel.RoomDetailsModelList.Add(room);
                        }
                    }
                    else
                    {


                        ViewModel.RoomDetailsModelList = null;
                        //AssModel.Id = Convert.ToInt32(ds.Tables[0].Rows[0]["Id"]);
                        //AssModel.RoomType = Convert.ToString(ds.Tables[0].Rows[0]["RoomType"].ToString());
                        //AssModel.FloorName = Convert.ToString(ds.Tables[0].Rows[0]["FloorName"].ToString());
                        //AssModel.Height = Convert.ToInt32(ds.Tables[0].Rows[0]["Height"]);
                        //AssModel.width = Convert.ToInt32(ds.Tables[0].Rows[0]["width"]);
                        //AssModel.Quantity = Convert.ToInt32(ds.Tables[0].Rows[0]["Quantity"]);
                        //AssModel.Area = Convert.ToInt32(ds.Tables[0].Rows[0]["Area"]);


                    }

                    return ViewModel.RoomDetailsModelList;
                }
                return ViewModel.RoomDetailsModelList;

            }
            catch (Exception ex)
            {
               
                return ViewModel.RoomDetailsModelList = null;
            }
        }


        public static List<StudentCountModel> GetAssociateStudentCount(string schl)
        {
            AssociateModel ViewModel = new AssociateModel();
            ViewModel.StudentCountModelList = new List<StudentCountModel>();
            ViewModel.StudentCountModel = new StudentCountModel();


            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("sp_studentCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "1");
                    cmd.Parameters.AddWithValue("@SCHL", schl); 
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dataTable = ds.Tables[0];

                        foreach (DataRow row in dataTable.Rows)
                        {
                            StudentCountModel room = new StudentCountModel();
                            //room.Id = Convert.ToInt32(row["Id"]);
                            //room.Class = Convert.ToString(row["Class"]);
                            //room.TotalSection = Convert.ToString(row["TotalSection"]);
                            //room.TotalStudent = Convert.ToString(row["TotalStudent"]);
                           

                            ViewModel.StudentCountModelList.Add(room);
                        }
                    }
                    else
                    {


                        ViewModel.StudentCountModelList = null;
                        //AssModel.Id = Convert.ToInt32(ds.Tables[0].Rows[0]["Id"]);
                        //AssModel.RoomType = Convert.ToString(ds.Tables[0].Rows[0]["RoomType"].ToString());
                        //AssModel.FloorName = Convert.ToString(ds.Tables[0].Rows[0]["FloorName"].ToString());
                        //AssModel.Height = Convert.ToInt32(ds.Tables[0].Rows[0]["Height"]);
                        //AssModel.width = Convert.ToInt32(ds.Tables[0].Rows[0]["width"]);
                        //AssModel.Quantity = Convert.ToInt32(ds.Tables[0].Rows[0]["Quantity"]);
                        //AssModel.Area = Convert.ToInt32(ds.Tables[0].Rows[0]["Area"]);


                    }

                    return ViewModel.StudentCountModelList;
                }
                return ViewModel.StudentCountModelList;

            }
            catch (Exception ex)
            {

                return ViewModel.StudentCountModelList = null;
            }
        }


        public static List<AssociationDocumentMaster> AssociationDocumentMasterList(DataTable dataTable)
        {
            List<AssociationDocumentMaster> item = new List<AssociationDocumentMaster>();
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                item.Add(new AssociationDocumentMaster { DocumentName = @dr["DocumentName"].ToString().Trim(), DocID = Convert.ToInt32(@dr["DocID"].ToString()) });
            }
            return item;
        }

        public static DataSet GetAssociationDocumentDetails(int type, int eDocId, string schl, string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAssociationDocumentDetailsSP", con);
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

        public static int InsertAssociationDocumentDetails(AssociationDocumentDetailsModel model, int action, out string OutError)
        {
            int result;
            OutError = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))

                {
                    SqlCommand cmd = new SqlCommand("InsertAssociationDocumentDetailsSP", con);//
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


        public static int InsertAssociationSchoolInfrastructure(SchoolInfraModel model, int action, out string OutError)
        {
            int result;
            OutError = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))

                {
                    SqlCommand cmd = new SqlCommand("sp_SchoolInfrastructure", con);//
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", model.ID);
                    cmd.Parameters.AddWithValue("@SCHL", model.SCHL);
                    cmd.Parameters.AddWithValue("@TAS", model.TAS);
                    cmd.Parameters.AddWithValue("@CAS", model.CAS);
                    cmd.Parameters.AddWithValue("@Playgl", model.Playgl);
                    cmd.Parameters.AddWithValue("@PlayGDS", model.PlayGDS);
                    cmd.Parameters.AddWithValue("@SBC", model.SBC);
                    cmd.Parameters.AddWithValue("@DBC", model.DBC);
                    cmd.Parameters.AddWithValue("@TotalNoOfcomputer", model.TotalNoOfcomputer);
                    cmd.Parameters.AddWithValue("@smartClass", model.smartClass);
                    cmd.Parameters.AddWithValue("@ComputerLab", model.ComputerLab);
                    cmd.Parameters.AddWithValue("@PhysicsLab", model.PhysicsLab);
                    cmd.Parameters.AddWithValue("@ChemistryLab", model.ChemistryLab);
                    cmd.Parameters.AddWithValue("@liabrary", model.liabrary);
                    cmd.Parameters.AddWithValue("@Noofbooks", model.Noofbooks);
                    cmd.Parameters.AddWithValue("@typeinternet", model.typeinternet);
                    cmd.Parameters.AddWithValue("@isTransport", model.isTransport);
                    cmd.Parameters.AddWithValue("@NoofPrinter", model.NoofPrinter);
                    cmd.Parameters.AddWithValue("@pagePrintCapacity", model.pagePrintCapacity);
                    cmd.Parameters.AddWithValue("@isCasePending", model.isCasePending);
                    cmd.Parameters.AddWithValue("@isAccountCheque", model.isAccountCheque);
                    cmd.Parameters.AddWithValue("@isRoAvail", model.isRoAvail);
                    cmd.Parameters.AddWithValue("@isToilet", model.isToilet);
                    cmd.Parameters.AddWithValue("@isDisplayBoard", model.isDisplayBoard);
                    cmd.Parameters.AddWithValue("@OtherActivities", model.OtherActivities);
                    cmd.Parameters.AddWithValue("@Action",action);
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


        public static int InsertAssociationStudentCount(StudentCountModel model, int action, out string OutError)
        {
            int result;
            OutError = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))

                {
                    SqlCommand cmd = new SqlCommand("sp_AssociateStudentCount", con);//
                     cmd.CommandType = CommandType.StoredProcedure;
                     cmd.Parameters.AddWithValue("@ID", model.Id);
                     cmd.Parameters.AddWithValue("@SCHL ",model.SCHL);
                     cmd.Parameters.AddWithValue("@Sec1 ", model.Sec1);
                     cmd.Parameters.AddWithValue("@Sec2 ", model.Sec2);
                     cmd.Parameters.AddWithValue("@Sec3 ", model.Sec3);
                     cmd.Parameters.AddWithValue("@Sec4  ", model.Sec4);
                     cmd.Parameters.AddWithValue("@Sec5  ", model.Sec5);
                     cmd.Parameters.AddWithValue("@Sec6 ", model.Sec6);
                     cmd.Parameters.AddWithValue("@Sec7 ", model.Sec7);
                     cmd.Parameters.AddWithValue("@Sec8 ", model.Sec8);
                     cmd.Parameters.AddWithValue("@Sec9", model.Sec9);
                     cmd.Parameters.AddWithValue("@Sec10 ", model.Sec10);
                     cmd.Parameters.AddWithValue("@Sec11Hum", model.Sec11Hum);
                     cmd.Parameters.AddWithValue("@Sec11Voc", model.Sec11Voc);
                     cmd.Parameters.AddWithValue("@Sec11Comm ", model.Sec11Comm);
                     cmd.Parameters.AddWithValue("@Sec11Sci ", model.Sec11Sci);
                     cmd.Parameters.AddWithValue("@Sec12Hum", model.Sec12Hum);
                     cmd.Parameters.AddWithValue("@Sec12Voc ", model.Sec12Voc);
                     cmd.Parameters.AddWithValue("@Sec12Comm ", model.Sec12Comm);
                     cmd.Parameters.AddWithValue("@Sec12Sci", model.Sec12Sci);
                     cmd.Parameters.AddWithValue("@TotStu1 ", model.TotStu1);
                     cmd.Parameters.AddWithValue("@TotStu2", model.TotStu2);
                     cmd.Parameters.AddWithValue("@TotStu3  ", model.TotStu3);
                     cmd.Parameters.AddWithValue("@TotStu4  ", model.TotStu4);
                     cmd.Parameters.AddWithValue("@TotStu5  ", model.TotStu5);
                     cmd.Parameters.AddWithValue("@TotStu6  ", model.TotStu6);
                     cmd.Parameters.AddWithValue("@TotStu7  ", model.TotStu7);
                     cmd.Parameters.AddWithValue("@TotStu8  ", model.TotStu8);
                     cmd.Parameters.AddWithValue("@TotStu9  ", model.TotStu9);
                     cmd.Parameters.AddWithValue("@TotStu10 ", model.TotStu10);
                     cmd.Parameters.AddWithValue("@TotStu11Hum ", model.TotStu11Hum);
                     cmd.Parameters.AddWithValue("@TotStu11Voc ", model.TotStu11Voc);
                     cmd.Parameters.AddWithValue("@TotStu11Comm ", model.TotStu11Comm);
                     cmd.Parameters.AddWithValue("@TotStu11Sci ", model.TotStu11Sci);
                     cmd.Parameters.AddWithValue("@TotStu12Hum ", model.TotStu12Hum);
                     cmd.Parameters.AddWithValue("@TotStu12Voc ", model.TotStu12Voc);
                     cmd.Parameters.AddWithValue("@TotStu12Comm  ", model.TotStu12Comm);
                     cmd.Parameters.AddWithValue("@TotStu12Sci ", model.TotStu12Sci);
                    cmd.Parameters.AddWithValue("@Action", action);
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


    }
}