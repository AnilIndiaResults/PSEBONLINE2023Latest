using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PSEBONLINE.Models
{
    public class PsebAPIModel
    {
    }
    public class AtomSettlementTransactionsResponse
    {       
        public string AtomId { get; set; }

        public string srNo { get; set; }

        public string merchantName { get; set; }

        public string merchantID { get; set; }

        public string atomTxnID { get; set; }

        public string txnState { get; set; }

        public string txnDate { get; set; }

        public string clientCode { get; set; }

        public string merchantTxnID { get; set; }

        public string product { get; set; }

        public string discriminator { get; set; }

        public string bankCardName { get; set; }

        public string bankRefNo { get; set; }

        public string refundRefNo { get; set; }

        public string grossTxnAmount { get; set; }

        public string txnCharges { get; set; }

        public string serviceTax { get; set; }

        public string sbCess { get; set; }

        public string krishiKalyanCess { get; set; }

        public string totalChargeable { get; set; }

        public string netAmountToBePaid { get; set; }

        public string paymentStatus { get; set; }

        public string settlementDate { get; set; }

        public string refundStatus { get; set; }


    }
    public class AtomAPIViewModelFormat
    {
        public List<AtomSettlementTransactionsResponse> results { get; set; }
    }

        public class AtomAPIViewModel
    {
        public string results { get; set; }
    }
    public class ResultApiMasters
    {
        public string firm { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string session { get; set; }
        public string rp { get; set; }
        public string cat { get; set; }
        public string exam { get; set; }
        public string nsqf { get; set; }
        public string set { get; set; }
        public string dist { get; set; }
        public string schl { get; set; }
        public string cent { get; set; }
        public double id { get; set; }
        [Key]
        public string roll { get; set; }
        public string regno { get; set; }
        public string withheld { get; set; }
        public string appsubcd { get; set; }
        public string sub1 { get; set; }
        public string spl1 { get; set; }
        public string type1 { get; set; }
        public string subname1 { get; set; }
        public string th1 { get; set; }
        public string thmin1 { get; set; }
        public string thmax1 { get; set; }
        public string ina1 { get; set; }
        public string inamin1 { get; set; }
        public string inamax1 { get; set; }
        public string pr1 { get; set; }
        public string prmin1 { get; set; }
        public string prmax1 { get; set; }
        public string tot1 { get; set; }
        public string min1 { get; set; }
        public string max1 { get; set; }
        public string res1 { get; set; }
        public string grade1 { get; set; }
        public string sub2 { get; set; }
        public string spl2 { get; set; }
        public string type2 { get; set; }
        public string subname2 { get; set; }
        public string th2 { get; set; }
        public string thmin2 { get; set; }
        public string thmax2 { get; set; }
        public string ina2 { get; set; }
        public string inamin2 { get; set; }
        public string inamax2 { get; set; }
        public string pr2 { get; set; }
        public string prmin2 { get; set; }
        public string prmax2 { get; set; }
        public string tot2 { get; set; }
        public string min2 { get; set; }
        public string max2 { get; set; }
        public string res2 { get; set; }
        public string grade2 { get; set; }
        public string sub3 { get; set; }
        public string spl3 { get; set; }
        public string type3 { get; set; }
        public string subname3 { get; set; }
        public string th3 { get; set; }
        public string thmin3 { get; set; }
        public string thmax3 { get; set; }
        public string ina3 { get; set; }
        public string inamin3 { get; set; }
        public string inamax3 { get; set; }
        public string pr3 { get; set; }
        public string prmin3 { get; set; }
        public string prmax3 { get; set; }
        public string tot3 { get; set; }
        public string min3 { get; set; }
        public string max3 { get; set; }
        public string res3 { get; set; }
        public string grade3 { get; set; }
        public string sub4 { get; set; }
        public string spl4 { get; set; }
        public string type4 { get; set; }
        public string subname4 { get; set; }
        public string th4 { get; set; }
        public string thmin4 { get; set; }
        public string thmax4 { get; set; }
        public string ina4 { get; set; }
        public string inamin4 { get; set; }
        public string inamax4 { get; set; }
        public string pr4 { get; set; }
        public string prmin4 { get; set; }
        public string prmax4 { get; set; }
        public string tot4 { get; set; }
        public string min4 { get; set; }
        public string max4 { get; set; }
        public string res4 { get; set; }
        public string grade4 { get; set; }
        public string sub5 { get; set; }
        public string spl5 { get; set; }
        public string type5 { get; set; }
        public string subname5 { get; set; }
        public string th5 { get; set; }
        public string thmin5 { get; set; }
        public string thmax5 { get; set; }
        public string ina5 { get; set; }
        public string inamin5 { get; set; }
        public string inamax5 { get; set; }
        public string pr5 { get; set; }
        public string prmin5 { get; set; }
        public string prmax5 { get; set; }
        public string tot5 { get; set; }
        public string min5 { get; set; }
        public string max5 { get; set; }
        public string res5 { get; set; }
        public string grade5 { get; set; }
        public string sub6 { get; set; }
        public string spl6 { get; set; }
        public string type6 { get; set; }
        public string subname6 { get; set; }
        public string th6 { get; set; }
        public string thmin6 { get; set; }
        public string thmax6 { get; set; }
        public string ina6 { get; set; }
        public string inamin6 { get; set; }
        public string inamax6 { get; set; }
        public string pr6 { get; set; }
        public string prmin6 { get; set; }
        public string prmax6 { get; set; }
        public string tot6 { get; set; }
        public string min6 { get; set; }
        public string max6 { get; set; }
        public string res6 { get; set; }
        public string grade6 { get; set; }
        public string sub7 { get; set; }
        public string spl7 { get; set; }
        public string type7 { get; set; }
        public string subname7 { get; set; }
        public string th7 { get; set; }
        public string thmin7 { get; set; }
        public string thmax7 { get; set; }
        public string ina7 { get; set; }
        public string inamin7 { get; set; }
        public string inamax7 { get; set; }
        public string pr7 { get; set; }
        public string prmin7 { get; set; }
        public string prmax7 { get; set; }
        public string tot7 { get; set; }
        public string min7 { get; set; }
        public string max7 { get; set; }
        public string res7 { get; set; }
        public string grade7 { get; set; }
        public string sub8 { get; set; }
        public string spl8 { get; set; }
        public string type8 { get; set; }
        public string subname8 { get; set; }
        public string th8 { get; set; }
        public string thmin8 { get; set; }
        public string thmax8 { get; set; }
        public string ina8 { get; set; }
        public string inamin8 { get; set; }
        public string inamax8 { get; set; }
        public string pr8 { get; set; }
        public string prmin8 { get; set; }
        public string prmax8 { get; set; }
        public string tot8 { get; set; }
        public string min8 { get; set; }
        public string max8 { get; set; }
        public string res8 { get; set; }
        public string grade8 { get; set; }
        public string sub9 { get; set; }
        public string spl9 { get; set; }
        public string type9 { get; set; }
        public string subname9 { get; set; }
        public string th9 { get; set; }
        public string thmin9 { get; set; }
        public string thmax9 { get; set; }
        public string ina9 { get; set; }
        public string inamin9 { get; set; }
        public string inamax9 { get; set; }
        public string pr9 { get; set; }
        public string prmin9 { get; set; }
        public string prmax9 { get; set; }
        public string tot9 { get; set; }
        public string min9 { get; set; }
        public string max9 { get; set; }
        public string res9 { get; set; }
        public string grade9 { get; set; }
        public string sub10 { get; set; }
        public string spl10 { get; set; }
        public string type10 { get; set; }
        public string subname10 { get; set; }
        public string th10 { get; set; }
        public string thmin10 { get; set; }
        public string thmax10 { get; set; }
        public string ina10 { get; set; }
        public string inamin10 { get; set; }
        public string inamax10 { get; set; }
        public string pr10 { get; set; }
        public string prmin10 { get; set; }
        public string prmax10 { get; set; }
        public string tot10 { get; set; }
        public string min10 { get; set; }
        public string max10 { get; set; }
        public string res10 { get; set; }
        public string grade10 { get; set; }
        public string s138 { get; set; }
        public string s139 { get; set; }
        public string s146 { get; set; }
        public string hot { get; set; }
        public string total { get; set; }
        public string totmax { get; set; }
        public string result { get; set; }
        public string grade { get; set; }
        public string resultdtl { get; set; }
        public string rsub1 { get; set; }
        public string rsub2 { get; set; }
        public string rsub3 { get; set; }
        public string rsub4 { get; set; }
        public string rsub5 { get; set; }
        public string rsub6 { get; set; }
        public string rsub7 { get; set; }
        public string spref { get; set; }
        public string spmks { get; set; }
        public string upd { get; set; }
        //public DateTime? insDate { get; set; }
        //public int userid { get; set; }
        //public DateTime? updDate { get; set; }



        public string cls { get; set; }

        //public bool importflag { get; set; }
        //public string imptblnameto { get; set; }
        //public DateTime? importDate { get; set; }
        //public DateTime? RecheckLastDate { get; set; }
        //public string Sub11 { get; set; }
        //public string subname11 { get; set; }
        //public string res11 { get; set; }
        //public string grade11 { get; set; }
        //public string th11 { get; set; }
        //public string nm { get; set; }
        //public string detailres { get; set; }

    }

    public class ResultAPIViewModel
    {
        public string Success { get; set; }
        public string statusCode { get; set; }
        public string SuccessMessage { get; set; }
        public string Object { get; set; }
    }

    public class SchoolPasswordAPIViewModel
    {
        public string Success { get; set; }
        public string statusCode { get; set; }
        public string SuccessMessage { get; set; }
        public SchoolChangePasswordModel Object { get; set; }
    }
    public class SchoolApiViewModel
    {
        public string Success { get; set; }
        public string statusCode { get; set; }
        public string SuccessMessage { get; set; }
        public SchoolModelAPI Object { get; set; }  
    }

    public class SchoolModelAPI
    {       
        public string SCHL { get; set; }
        public string MOBILE { get; set; }
        public string EMAILID { get; set; }
        public string REMARKS { get; set; }
        public string CorrectionNo { get; set; }       
    }

    public class BarAPIModel
    {
        //public string Roll { get; set; }
        //public string Sub { get; set; }
        public string Bag { get; set; }
        public string Bar { get; set; }
        public string Marks { get; set; }
        public string ExEpunjabId { get; set; }
        public string ExDetails { get; set; }
        public string HeEpunjabId { get; set; }
        public string HeDetails { get; set; }
        public string CaEpunjabid { get; set; }
        public string CaDetails { get; set; }
        public string EvnCode { get; set; }
        public string EvnDetail { get; set; }
    }

    public class Bar12Details
    {
        // public string Level { get; set; }
        public string Roll { get; set; }
        public string SubCode { get; set; }
        public string Bag1 { get; set; }
        public string Bar1 { get; set; }
        public string Marks1 { get; set; }
        public string ExEpunjab1 { get; set; }
        public string ExDetail1 { get; set; }
        public string HeEpunjab1 { get; set; }
        public string HeDetail1 { get; set; }
        public string CaEpunjab1 { get; set; }
        public string CaDetail1 { get; set; }
        public string EvnCode1 { get; set; }
        public string EvnDetail1 { get; set; }
        public string Bag2 { get; set; }
        public string Bar2 { get; set; }
        public string Marks2 { get; set; }
        public string ExEpunjab2 { get; set; }
        public string ExDetail2 { get; set; }
        public string HeEpunjab2 { get; set; }
        public string HeDetail2 { get; set; }
        public string CaEpunjab2 { get; set; }
        public string CaDetail2 { get; set; }
        public string EvnCode2 { get; set; }
        public string EvnDetail2 { get; set; }
        public string Bag3 { get; set; }
        public string Bar3 { get; set; }
        public string Marks3 { get; set; }
        public string ExEpunjab3 { get; set; }
        public string ExDetail3 { get; set; }
        public string HeEpunjab3 { get; set; }
        public string HeDetail3 { get; set; }
        public string CaEpunjab3 { get; set; }
        public string CaDetail3 { get; set; }
        public string EvnCode3 { get; set; }
        public string EvnDetail3 { get; set; }
        public string OldResult { get; set; }
        public string NewResult { get; set; }
        public string Diff1 { get; set; }
        public string Diff2 { get; set; }
    }
}