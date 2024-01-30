using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace PSEBONLINE.Models
{

    public partial class UndertakingOfQuestionPapers
    {
        [NotMapped]
        public string CentNM { get; set; }
        [NotMapped]
        public string CentMobile { get; set; }
        [NotMapped]
        public string CentHeadNM { get; set; }
        [NotMapped]
        public string QP_MonthName { get; set; }
        [NotMapped]
        public List<SelectListItem> StatusList { get; set; }
        [NotMapped]
        public List<SelectListItem> ClassList { get; set; }
        [NotMapped]
        public List<SelectListItem> MonthList { get; set; }
        [NotMapped]
        public List<SelectListItem> YearList { get; set; }
        [NotMapped]
        public List<UndertakingOfQuestionPapers> UndertakingOfQuestionPapersList { get; set; }
    }

    public partial class UndertakingOfQuestionPapers
    {
        [Key]
        public int QPID { get; set; }
        public string CentCode { get; set; }
        public string Refno { get; set; }
        public string QP_Class { get; set; }
        public string QP_Month { get; set; }
        public string QP_Year { get; set; }
        public string QP_Description1 { get; set; }
        public string QP_Status1 { get; set; }
        public string QP_Remarks1 { get; set; }
        public string QP_Description2 { get; set; }
        public string QP_Status2 { get; set; }
        public string QP_Remarks2 { get; set; }
        public string QP_Description3 { get; set; }
        public string QP_Status3 { get; set; }
        public string QP_Remarks3 { get; set; }
        public bool IsFinalLock { get; set; }
        public bool IsActive { get; set; }
        public string SubmitBy { get; set; }
        public DateTime SubmitOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? FinalSubmitOn { get; set; }

    }

    public partial class UndertakingOfQuestionPapersViews
    {
        [Key]
        public int QPID { get; set; }
        public string CentCode { get; set; }
        public string Refno { get; set; }
        public string QP_Class { get; set; }
        public string QP_Month { get; set; }
        public string QP_Year { get; set; }
        public string QP_Description1 { get; set; }
        public string QP_Status1 { get; set; }
        public string QP_Remarks1 { get; set; }
        public string QP_Description2 { get; set; }
        public string QP_Status2 { get; set; }
        public string QP_Remarks2 { get; set; }
        public string QP_Description3 { get; set; }
        public string QP_Status3 { get; set; }
        public string QP_Remarks3 { get; set; }
        public bool IsFinalLock { get; set; }
        public bool IsActive { get; set; }
        public string SubmitBy { get; set; }
        public DateTime SubmitOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? FinalSubmitOn { get; set; }

        public string CentNM { get; set; }
        public string CentMobile { get; set; }
        public string CentHeadNM { get; set; }

    }

    public class HomeModels
    {
       // public DataSet StoreAllData { get; set; }
    }

    public class BankListModel
    {
        public string BCode { get; set; }
        public string BankName { get; set; }
        public string Img { get; set; }
    }

   
    public class FeeHomeViewModel
    {
        public string selectedClass { get; set; }
        public string BankCode { get; set; }

        public string AllowBanks { get; set; }

        public string OfflineLastDate { get; set; }
        public string schl { get; set; }
        public string dateSelected { get; set; }
        public string formSelected { get; set; }
        public string insertDate { get; set; }

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
        public string Type { get; set; }
        // public DateTime BankLastDate { get; set; }
    }

    public class PaymentformViewModel
    {
        public string OfflineLastDate { get; set; }
        public string IsOnline { get; set; }
        public List<BankListModel> bankList { get; set; }        
        public string AllowBanks { get; set; }
        public string ExamForm { get; set; }
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
        public string FeeDate { get; set; }
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
        public string StartDate { get; set; }

        public DataTable StudentwiseFeesDT { get; set; }
    }

    public class ChallanMasterModel
    {
        // Open Exam Fee Column
        public string EmpUserId { get; set; }
        public float OpenExamFee { get; set; }
        public float OpenLateFee { get; set; }
        public float OpenTotalFee { get; set; }
        //
        public string FormType { get; set; }
        public int Class { get; set; }
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

    }

    public class ChallanDepositDetailsModel
    {
        public string challanremarks { get; set; }
        //
        public DataSet dsData { get; set; }   
        public string CHALLANID { get; set; }
        public string SCHLREGID { get; set; }
        public string APPNO { get; set; }
        public string SCHLCANDNM { get; set; }
        public string CHLNDATE { get; set; }
        public string CHLVDATE { get; set; }
        public string BANK { get; set; }
        public string FEE { get; set; }
        public int LOT { get; set; }       
        public string DOWNLDFLOT { get; set; }
        public string DOWNLDDATE { get; set; }


        public string BRCODECAND { get; set; }
        public string BRANCHCAND { get; set; }
        public string J_REF_NOCAND { get; set; }
        public string DEPOSITDTCAND { get; set; }
        public float DOWNLDFLG { get; set; }
    }
}