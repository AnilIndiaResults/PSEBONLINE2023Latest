using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace PSEBONLINE.Models
{
    public class RecheckModels
    {

        public string Recheckevaluation { get; internal set; }
        public bool IsReEvaluation { get; internal set; }
        public bool Agree { get; internal set; }
        public bool IsRecheck { get; internal set; }
        public bool IsRTI { get; internal set; }

        public string SubList { get; internal set; }
        public string ROLL { get; internal set; }
        public string Other_Board { get; internal set; }
        public string MatricMarks { get; internal set; }
        public string Board { get; internal set; }
        public string batch { get; internal set; }
        public string batchYear { get; internal set; }
        public string MatricSub { get; internal set; }
        public string MatricSub2 { get; internal set; }
        public string TwelveSub { get; internal set; }
        public string TwelveSub2 { get; internal set; }

        public int ID { get; set; }
        public DataSet StoreAllData { get; set; }
        public string SearchResult { get; internal set; }
        public string Candi_Name { get; internal set; }
        public string Father_Name { get; internal set; }
        public string Mother_Name { get; internal set; }
        public string Pname { get; internal set; }
        public string PFname { get; internal set; }
        public string PMname { get; internal set; }
        public string EPname { get; internal set; }
        public string EPFname { get; internal set; }
        public string EPMname { get; internal set; }
        public string DOB { get; internal set; }
        public string Gender { get; internal set; }
        public string SdtID { get; internal set; }
        public string FormName { get; internal set; }
        //public string regno { get; internal set; }
        public string SCHL { get; internal set; }

        public string DEPOSITDT { get; set; }
        public string BANK { get; set; }

        public string BRANCH { get; set; }

        public string dispatchNo { get; internal set; }
        public string attendanceTot { get; internal set; }
        public string attendancePresnt { get; internal set; }
        public string struckOff { get; internal set; }
        public string reasonFrSchoolLeav { get; internal set; }


        public string SelExamDist { get; internal set; }
        public string SelDist { get; internal set; }
        public string tehsil { get; internal set; }
        public string SelList { get; internal set; }
        public string SearchString { get; internal set; }
        public string SearchBy { get; internal set; }


        public string SelForm { get; internal set; }
        public string SelLot { get; internal set; }
        public string SelFilter { get; internal set; }
        public string SchlCode { get; internal set; }


        public string idno { get; internal set; }
        public string Sno { get; internal set; }
        public string RegNo { get; internal set; }
        public string AdmDate { get; internal set; }
        public string Fee { get; internal set; }
        public string Lot { get; internal set; }
        public string Std_Sub { get; internal set; }


        public string distCode { get; internal set; }
        public string distName { get; internal set; }
        public string distNameP { get; internal set; }
        public string SCHLType { get; internal set; }
        public string SCHLfullNM_P { get; internal set; }
        public string SCHLfullNM_E { get; internal set; }

        public string Nation { get; internal set; }
        public string admno { get; internal set; }

        public string OROLL { get; internal set; }
        public string struckOffdt { get; internal set; }

        public string Result { get; internal set; }
        public string obtmark { get; internal set; }
        public string Totmark { get; internal set; }
        public string aadhar { get; internal set; }

        //---------------------------


        public string refNo { get; internal set; }
        public string centrCode { get; internal set; }
        public string setNo { get; internal set; }
        public string sheetNo { get; internal set; }
        public string category { get; internal set; }
        public string phyChal { get; internal set; }
        public string emailID { get; internal set; }
        public string mobileNo { get; internal set; }
        public string phoneNo { get; internal set; }
        public string address { get; internal set; }
        public string addressfull { get; internal set; }
        public string challanNo { get; internal set; }
        public string std_SignE { get; internal set; }
        public string std_SignP { get; internal set; }
        public string sub1 { get; internal set; }
        public string sub2 { get; internal set; }
        public string sub3 { get; internal set; }
        public string sub4 { get; internal set; }
        public string sub5 { get; internal set; }
        public string sub6 { get; internal set; }
        public string sub7 { get; internal set; }
        public string sub8 { get; internal set; }
        public string sub9 { get; internal set; }
        public string sub10 { get; internal set; }

        public string sub1code { get; internal set; }
        public string sub2code { get; internal set; }
        public string sub3code { get; internal set; }
        public string sub4code { get; internal set; }
        public string sub5code { get; internal set; }
        public string sub6code { get; internal set; }
        public string sub7code { get; internal set; }
        public string sub8code { get; internal set; }
        public string sub9code { get; internal set; }
        public string sub10code { get; internal set; }

        public string declaration { get; internal set; }
        //public string address { get; internal set; }
        public string block { get; internal set; }
        public string landmark { get; internal set; }
        public string pinCode { get; internal set; }
        //public string mobileNo { get; internal set; }



        public string Exam_Type { get; internal set; }
        public string Candidate_Type { get; internal set; }
        public string Class { get; internal set; }
        public string ClassNM { get; internal set; }
        //public string SelMonth { get; internal set; }
        //public string SelYear { get; internal set; }
        //public string SelMonth { get; internal set; }
        //public string SelYear { get; internal set; }
        //public string SelMonth { get; internal set; }

        public string SelYear { get; internal set; }
        public string SelMonth { get; internal set; }
        public string Session { get; internal set; }

        public string capcha { get; internal set; }
        public string IsphysicalChall { get; internal set; }
        public string rdoWantWriter { get; internal set; }
        public string Choice1 { get; internal set; }
        public string Choice2 { get; internal set; }

        public HttpPostedFileBase std_Photo { get; set; }
        public HttpPostedFileBase std_Sign { get; set; }
        public HttpPostedFileBase file { get; set; }

        public string PathPhoto { get; set; }
        public string PathSign { get; set; }

        public string imgPhoto { get; set; }
        public string imgSign { get; set; }

        public string CastList { get; set; }
        public string Area { get; set; }
        public string Relist { get; set; }

        public string IsPracExam { get; set; }
        public string FormStatus { get; set; }
    }
    public class RecheckFeeHomeViewModel
    {
        public DataSet StoreAllData { get; set; }
        public int ID { get; set; }
        public string FORM { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Fee { get; set; }
        public int LateFee { get; set; }
        public int StudentFee { get; set; }
        public int TotalFees { get; set; }
        public int CountStudent { get; set; }
        public string FeeDate { get; set; }
        public string TotalFeesInWords { get; set; }
        // public DateTime BankLastDate { get; set; }
    }

    public class RecheckPaymentformViewModel
    {
        public DateTime BankLastDate { get; internal set; }
        public string roll { get; internal set; }
        public string Class { get; internal set; }
        public string ExamType { get; internal set; }
        public string category { get; internal set; }
        public string Name { get; internal set; }
        public string RegNo { get; internal set; }
        public string RefNo { get; internal set; }

        public DataSet PaymentFormData { get; set; }
        public int PrintLot { get; set; }
        public int SCHL { get; set; }
        public int LOTNo { get; set; }
        public string Dist { get; set; }
        public string District { get; set; }
        public string DistrictFull { get; set; }
        //public int SchoolCode { get; set; }
        public string SchoolCode { get; set; }
        public string SchoolName { get; set; }
        public int TotalCandidates { get; set; }
        public int TotalFees { get; set; }
        public int TotalLateFees { get; set; }
        public int TotalFinalFees { get; set; }
        //public string FeeDate { get; set; }
        public DateTime FeeDate { get; set; }
        // public int TotalFeesAmount { get; set; }
        public string TotalFeesInWords { get; set; }
        public string BankName { get; set; }
        public int BankCharges { get; set; }
        public string FeeDescription { get; set; }
        public string FeeCode { get; set; }
        public string FeeMode { get; set; }
        public string FeeCategory { get; set; }
        public string BankCode { get; set; }
        //public string ChallanGenerationDate { get; set; }
        //public string ChallanID { get; set; }
        //public string ChallanValidDate { get; set; }    
        public int totaddfee { get; set; }
        public int totlatefee { get; set; }
        public int totpracfee { get; set; }
        public int totaddsubfee { get; set; }
        public int totadd_sub_count { get; set; }
        public int totprac_sub_count { get; set; }
        public int totregfee { get; set; }
        public int totfee { get; set; }
        public int StudentId { get; set; }
        public DataTable StudentwiseFeesDT { get; set; }
    }
    public class RecheckChallanMasterModel
    {
        public string RefNo { get; internal set; }
        public string roll { get; internal set; }
        public int StatusNumber { get; set; }
        public string Action { get; set; }
        public string FeeStudentList { get; set; }
        public DataSet ChallanMasterData { get; set; }
        public string SchoolCode { get; set; }
        public string TotalFeesInWords { get; set; }
        public string SchoolName { get; set; }
        public string DepositoryMobile { get; set; }
        //
        public string CHALLANID { get; set; }
        public string CHLNDATE { get; set; }
        public string CHLNVDATE { get; set; }
        public string FEEMODE { get; set; }
        public string FEECODE { get; set; }
        public string FEECAT { get; set; }
        public string BCODE { get; set; }
        public string BANK { get; set; }
        public string ACNO { get; set; }
        public float FEE { get; set; }
        public float BANKCHRG { get; set; }
        public float TOTFEE { get; set; }
        public string SCHLREGID { get; set; }
        public string DIST { get; set; }
        public string DISTNM { get; set; }
        public string SCHLCANDNM { get; set; }
        public string BRCODE { get; set; }
        public string BRANCH { get; set; }
        public string J_REF_NO { get; set; }
        public string DEPOSITDT { get; set; }
        public float VERIFIED { get; set; }
        public string VERIFYDATE { get; set; }
        public float DOWNLDFLG { get; set; }
        public float DOWNLDFLOT { get; set; }
        public string DOWNLDDATE { get; set; }
        public string APPNO { get; set; }
        public int ID { get; set; }
        public int addfee { get; set; }
        public int latefee { get; set; }
        public int pracfee { get; set; }
        public int prosfee { get; set; }
        public int addsubfee { get; set; }
        public int add_sub_count { get; set; }
        public int prac_sub_count { get; set; }
        public int regfee { get; set; }
        public string type { get; set; }
        public int LOT { get; set; }
        public DateTime ChallanGDateN { get; set; }
        public DateTime ChallanVDateN { get; set; }
        public DateTime? VerifyDateN { get; set; }
        public DateTime? DownloadDateN { get; set; }
        public float? LumsumFine { get; set; }
        public string LSFRemarks { get; set; }
        public string JChallanVDateN { get; set; }
    }
}
