using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
namespace PSEBONLINE.Models
{
    public class Challan
    {
        public DataSet StoreAllData { get; set; }
        public double ID { get; set; }
        public string Bank { get; set; }
        public string challanid { get; set;}        
        public string CHLNDATE { get; set; }
        public string CHLNVDATE { get; set; }
        public string FEECAT { get; set;}
        public double SCHLREGID { get; set;}
        public double FEE { get; set;}        
        public double DOWNLDFLG { get; set; }
        public double DOWNLDFLOT { get; set;}
        public string DOWNLDDATE { get; set;}
        public double VERIFIED { get; set;}
        public double J_REF_NO { get; set;}
        public double APPNO { get; set;}
        public string SCHLCANDNM { get; set;}
        public string type { get; set;}
        public string BANK { get; set;} 
        public double ACNO { get; set;}
        public string BRANCH { get; set;}
        public DateTime DEPOSITDT { get; set;}
        public string LOT { get; set;}
        public string VERIFYDATE { get; set;}
    }

    public class ShiftChallan
    {
        public double ID { get; set; }
        public string Bank { get; set; }
        public string challanid { get; set; }
        public string CHLNDATE { get; set; }
        public string CHLNVDATE { get; set; }
        public string FEECAT { get; set; }
        public string SCHLREGID { get; set; }
        public string DIST { get; set; }
        public string DISTNM { get; set; }
        public double FEE { get; set; }
        public double BANKCHRG { get; set; }
        public double TOTFEE { get; set; }        
       
        public double DOWNLDFLG { get; set; }
        public double DOWNLDFLOT { get; set; }
        public string DOWNLDDATE { get; set; }
        public string VERIFIED { get; set; }
        public string J_REF_NO { get; set; }
        public string APPNO { get; set; }
        public string SCHLCANDNM { get; set; }
        public string type { get; set; }
        public string BANK { get; set; }
        public string ACNO { get; set; }
        public string BRANCH { get; set; }
        public string DEPOSITDT { get; set; }
        public string LOT { get; set; }
        public string VERIFYDATE { get; set; }
        public string FEECODE { get; set; }
        public string BCODE { get; set; }      
        public string BRCODE { get; set; }    

        public int addfee { get; set; }
        public int latefee { get; set; }
        public int prosfee { get; set; }
        public int addsubfee { get; set; }
        public int add_sub_count { get; set; }
        public int regfee { get; set; }

        public string Mobile { get; set; }
        public int IsCancel { get; set; }
        public string CancelRemarks { get; set; }
        public string CancelDate { get; set; }
        public float LumsumFine { get; set; }
        public string LSFRemarks { get; set; }
        public string RP { get; set; }
        public string StudentList { get; set; }
        public string BoardBranch { get; set; }
        
    }

     public class ShiftChallanDetails
    {
        public DataSet StoreAllData { get; set; }
        public int ShiftId { get; set; }
        public string WrongChallan { get; set; }
        public string CorrectChallan { get; set; }
        public int AdminId { get; set; }
        public int ActionType { get; set; }
        
        public string ShiftFile { get; set; }
        public HttpPostedFileBase file { get; set; }
        public string ShiftRemarks { get; set; }
        
        public List<ShiftChallan> challanList { get; set; }
    }
}