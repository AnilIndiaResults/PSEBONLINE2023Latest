using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PSEBONLINE.Models
{
    public class BankModels
    {
        public string ApprovalStatus { get; set; }
        public string DepositRemarks { get; set; }
        //Bank Master
        public DataSet BankMasterData { get; set; }
        public string Session { get; set; }
        public string BCODE { get; set; }
        public string BANK { get; set; }
        public string ACNO { get; set; }
        public string BRCODE { get; set; }
        public string BRANCH { get; set; }
        public string ADDRESS { get; set; }
        public string BANKNAME { get; set; }
        public string DISTRICT { get; set; }
        public string PINCODE { get; set; }
        public string MOBILE { get; set; }
        public string STD { get; set; }
        public string PHONE { get; set; }
        public string EMAILID1 { get; set; }
        public string EMAILID2 { get; set; }
        public string IFSC { get; set; }
        public string MICR { get; set; }
        public string buser_id { get; set; }
        public string password { get; set; }
        public string BRNCODE { get; set; }
        public string NODAL_BRANCH { get; set; }
        public string MNAGER_NM { get; set; }
        public string TECHNICAL_PERSON { get; set; }
        public string OTCONTACT { get; set; }
        public string MIS_FILENM { get; set; }
        public HttpPostedFileBase file { get; set; }

        public string OldPassword { get; set; }
        public string Newpassword { get; set; }
        public string ConfirmPassword { get; set; }

        //

        public int StatusNumber { get; set; }
        public string Action { get; set; }
        public string FeeStudentList { get; set; }
        public string SchoolCode { get; set; }
        public string TotalFeesInWords { get; set; }
        public string SchoolName { get; set; }
        public string DepositoryMobile { get; set; }


        //Challan Master
        public string CHALLANID { get; set; }
        public string CHLNDATE { get; set; }
        public string CHLNVDATE { get; set; }
        public string FEEMODE { get; set; }
        public string FEECODE { get; set; }
        public string FEECAT { get; set; }

        public float FEE { get; set; }
        public float BANKCHRG { get; set; }
        public float TOTFEE { get; set; }
        public string SCHLREGID { get; set; }
        public string DIST { get; set; }
        public string DISTNM { get; set; }
        public string SCHLCANDNM { get; set; }

        public string J_REF_NO { get; set; }
        public string DEPOSITDT { get; set; }
        public int VERIFIED { get; set; }
        public string VERIFYDATE { get; set; }
        public int DOWNLDFLG { get; set; }
        public float DOWNLDFLOT { get; set; }
        public string DOWNLDDATE { get; set; }
        public string APPNO { get; set; }
        public int ID { get; set; }
        public int addfee { get; set; }
        public int latefee { get; set; }
        public int prosfee { get; set; }
        public int addsubfee { get; set; }
        public int add_sub_count { get; set; }
        public int regfee { get; set; }
        public string type { get; set; }
        public int LOT { get; set; }
        public DateTime ChallanGDateN { get; set; }
        public DateTime ChallanVDateN { get; set; }
        public DateTime? VerifyDateN { get; set; }
        public DateTime? DownloadDateN { get; set; }
    }
    
    public class ErrorShowList
    {
        public DataSet StoreAllData { get; set; }
        public string ErrorList { get; set; }
        public DateTime Createdate { get; set; }
    }
}