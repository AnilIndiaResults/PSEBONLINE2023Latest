using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSEBONLINE.Models
{

    public class ExamCentreResources
    {
        [Key]
        public int id { get; set; }   
        public string schl { get; set; }
        public string internetAvailability { get; set; }
        public string printerAvailability { get; set; }
        public string perMinutePrintingSpeed { get; set; }
        public string powerBackup { get; set; }
        public string photostateAvailability { get; set; }
        public DateTime? submitOn { get; set; }
        public string submitBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public string modifiedBy { get; set; }
    }


    public class ExamCentreConfidentialResources
    {
        [Key]
        public int id { get; set; }
        public string schl { get; set; }
        public string principal { get; set; }
        public string mobile { get; set; }
        public string otp { get; set; }      
        public DateTime? submitOn { get; set; }
        public string submitBy { get; set; }
        public DateTime? modifiedOn { get; set; }
        public string modifiedBy { get; set; }
        public bool isdeleted { get; set; }
        public DateTime? deletedOn { get; set; }
        public string deletedBy { get; set; }
        public DateTime? downloadOn { get; set; }
        public int downloadCount { get; set; }

        [NotMapped]
        public String[] confidentialFiles { get; set; }
    }


    public class LateAdmissionStatusMasters
    {
        [Key]
        public int ID { get; set; }
        public int Statuscode { get; set; }
        public string StatusName { get; set; }
        public bool IsActive { get; set; }


    }
    public class CandSubjectReExamTermStudents
    {
        public string SUB { get; set; }
        public string OBTTEST1 { get; set; }
        public string MAXTEST1 { get; set; }
        public string OBTTEST2 { get; set; }
        public string MAXTEST2 { get; set; }
        public string OBTTEST3 { get; set; }
        public string MAXTEST3 { get; set; }
        public string OBTTEST4 { get; set; }
        public string MAXTEST4 { get; set; }
        public string OBTTEST5 { get; set; }
        public string MAXTEST5 { get; set; }
    }

    public partial class ReExamTermStudents
    {
        [NotMapped]
        public DataSet StoreAllData { get; set; }

        [NotMapped]
        public HttpPostedFileBase file { get; set; }

    }
    public partial class ReExamTermStudents
    {
        [Key]
        public long ReExamId { get; set; }
        public long Std_id { get; set; }
        public string Schl { get; set; }
        public int Cls { get; set; }
        public string Challanid { get; set; }
        public DateTime? Challandt { get; set; }
        public bool ChallanVerify { get; set; }
        public int Fee { get; set; }
        public int LateFee { get; set; }
        public int TotalFee { get; set; }
        public int IsPrinted { get; set; }
        public DateTime? PrintedOn { get; set; }
        public int IsActive { get; set; }
        public DateTime? SubmitOn { get; set; }
        public int IsChallanCancel { get; set; }
        public string SubmitBy { get; set; }

        public string EmpUserId { get; set; }
        public string AdminId { get; set; }

    }

    public class ReExamTermStudentsModelList
    {
        public DataSet StoreAllData { get; set; }
        public List<ReExamTermStudentsSearchModel> ReExamTermStudentsSearchModel { set; get; }
    }

    public class ReExamTermStudentsSearchModel
    {
        public long Std_id { set; get; }
        public string Roll { set; get; }
        public string SCHL { set; get; }
        public string AdmDate { set; get; }
        public string Class { set; get; }
        public string roll { set; get; }
        public string regno { set; get; }
        public string form { set; get; }
        public string Aadhar { set; get; }
        public string Dist { set; get; }
        public string Rp { set; get; }
        public string EXAM { set; get; }
        public string NSQF { set; get; }
        public string Category { set; get; }
        public string name { set; get; }
        public string pname { set; get; }
        public string fname { set; get; }
        public string pfname { set; get; }
        public string mname { set; get; }
        public string pmname { set; get; }
        public string Caste { set; get; }
        public string Gender { set; get; }
        public string DOB { set; get; }
        public string phy_chal { set; get; }
        public int IsExistsInReExamTermStudents { set; get; }
        public long ReExamId { set; get; }
        public int IsChallanCancel { set; get; }
        public string SubmitBy { get; set; }
    }

    public class ReExamTermStudents_ChallanDetailsViewsModelList
    {
        public DataSet StoreAllData { get; set; }
        public List<ReExamTermStudents_ChallanDetailsViews> ReExamTermStudents_ChallanDetailsViews { set; get; }
    }
    public class ReExamTermStudents_ChallanDetailsViews
    {
        [Key]
        public string ChallanId { set; get; }
        public int NOC { set; get; }
        public string Class { set; get; }
        public string RP { set; get; }
        public string feecode { set; get; }
        public int IsCancel { set; get; }
        public int LOT { set; get; }
        public string Bank { set; get; }
        public string BCODE { set; get; }

        public string CHLNDATE { set; get; }
        public string CHLNVDATE { set; get; }
        public string ChallanVerifiedOn { set; get; }
        public string DEPOSITDT { set; get; }
        public string SCHLREGID { set; get; }
        public float FEE { set; get; }
        public float TOTFEE { set; get; }
        public float LateFee { set; get; }
        public float TotalFee { set; get; }
        public string BRANCH { set; get; }
        public string Status { set; get; }
        public string StatusNumber { set; get; }
        public float verified { set; get; }
        public float downldflg { set; get; }
        public string ExpireVDate { set; get; }
        public string StudentList { set; get; }
        public string FeeDepositStatus { set; get; }
        public int FinalPrintStatus { set; get; }
        public string FinalPrintFilePath { set; get; }

    }


    public class OnlineCentreCreationsPaymentForm
    {

        public int OnlineCentreCreationId { get; set; }
        [Key]
        public string CentreAppNo { get; set; }
        public string USERTYPE { get; set; }
        public string IsNEW { get; set; }
        public string SCHL { get; set; }
        public int MATREG { get; set; }
        public int SSREG { get; set; }
        public int MIDREG { get; set; }
        public int Add_Count { get; set; }
        public int ExtraFees { get; set; }
        public int ContiFee { get; set; }
        public int fee { get; set; }
        public int latefee { get; set; }
        public int totfee { get; set; }
        public DateTime? sDate { get; set; }
        public DateTime? eDate { get; set; }
        public string TotalFeesWords { get; set; }
        public DateTime? BankLastdate { get; set; }
        public int FEECODE { get; set; }
        public string FEECAT { get; set; }
        public string AllowBanks { get; set; }

    }

    public class OnlineCentreCreationsChallanViews
    {
        [Key]
        public int OnlineCentreCreationId { get; set; }
        public string CentreAppNo { get; set; }
        public string SCHL { get; set; }
        public string CENT { get; set; }
        public string IsNew { get; set; }
        public string USERTYPE { get; set; }
        public int MATREG { get; set; }
        public int MATOPN { get; set; }
        public int SSREG { get; set; }
        public int SSOPN { get; set; }
        public int MATPVT { get; set; }
        public int SSPVT { get; set; }
        public int MIDREG { get; set; }
        public int? TotalRooms { get; set; }
        public int? TotalCapacity { get; set; }
        public int? TotalRoomsOther { get; set; }
        public int? SingleBenchFurniture { get; set; }
        public int? DoubleBenchFurniture { get; set; }
        public string SchoolCategoryMain { get; set; }
        public string Challanid { get; set; }
        public DateTime? Challandt { get; set; }
        public int ChallanVerify { get; set; }
        public int ContinuationFee { get; set; }
        public int Fee { get; set; }
        public int LateFee { get; set; }
        public int AddFee { get; set; }
        public int TotalFee { get; set; }
        public int AddCount { get; set; }
        public DateTime? SubmitOn { get; set; }
        public DateTime? ModifyOn { get; set; }
        public string ModifyBy { get; set; }
        public string Remarks { get; set; }
        public string EmpUserId { get; set; }
        //
        public string SCHLNME { get; set; }
        public string SCHLNMP { get; set; }
        public string DIST { get; set; }
        public string DISTNM { get; set; }
        public string DISTNMP { get; set; }
        public string MOBILE { get; set; }
        public string J_REF_NO { get; set; }
        public string DEPOSITDT { get; set; }

        public string BANK { get; set; }
        public int VERIFIED { get; set; }
        public string ChallanStatus { get; set; }
        public string ChallanShowStatus { get; set; }

        public string AppliedOn { get; set; }
        public string ScanCopy { get; set; }


    }


    public class OnlineCentreCreationsViews
    {
        [Key]
        public int OnlineCentreCreationId { get; set; }
        public string CentreAppNo { get; set; }
        public string SCHL { get; set; }
        public string CENT { get; set; }
        public string IsNew { get; set; }
        public string USERTYPE { get; set; }
        public int MATREG { get; set; }
        public int MATOPN { get; set; }
        public int SSREG { get; set; }
        public int SSOPN { get; set; }
        public int MATPVT { get; set; }
        public int SSPVT { get; set; }
        public int MIDREG { get; set; }
        public int? TotalRooms { get; set; }
        public int? TotalCapacity { get; set; }
        public int? TotalRoomsOther { get; set; }
        public int? SingleBenchFurniture { get; set; }
        public int? DoubleBenchFurniture { get; set; }
        public string Challanid { get; set; }
        public DateTime? Challandt { get; set; }
        public int ChallanVerify { get; set; }
        public int ContinuationFee { get; set; }
        public int Fee { get; set; }
        public int LateFee { get; set; }
        public int AddFee { get; set; }
        public int TotalFee { get; set; }
        public int AddCount { get; set; }
        public DateTime? SubmitOn { get; set; }
        public DateTime? ModifyOn { get; set; }
        public string ModifyBy { get; set; }
        public string Remarks { get; set; }
        public string EmpUserId { get; set; }
        //
        public string SCHLNME { get; set; }
        public string SCHLNMP { get; set; }
        public string DIST { get; set; }
        public string DISTNM { get; set; }
        public string DISTNMP { get; set; }
        public string MOBILE { get; set; }
        public string J_REF_NO { get; set; }
        public string DEPOSITDT { get; set; }
        public string AppliedOn { get; set; }
        public string ScanCopy { get; set; }

    }

    public class OnlineCentreCreations
    {
        [Key]
        public int OnlineCentreCreationId { get; set; }
        public string CentreAppNo { get; set; }
        public string SCHL { get; set; }
        public string CENT { get; set; }
        public string IsNew { get; set; }
        public string USERTYPE { get; set; }
        public int MATREG { get; set; }
        public int MATOPN { get; set; }
        public int SSREG { get; set; }
        public int SSOPN { get; set; }
        public int MATPVT { get; set; }
        public int SSPVT { get; set; }
        public int MIDREG { get; set; }
        public int? TotalRooms { get; set; }
        public int? TotalCapacity { get; set; }
        public int? TotalRoomsOther { get; set; }
        public int? SingleBenchFurniture { get; set; }
        public int? DoubleBenchFurniture { get; set; }
        public string Challanid { get; set; }
        public DateTime? Challandt { get; set; }
        public int ChallanVerify { get; set; }
        public int ContinuationFee { get; set; }
        public int Fee { get; set; }
        public int LateFee { get; set; }
        public int AddFee { get; set; }
        public int TotalFee { get; set; }
        public int AddCount { get; set; }
        public DateTime? SubmitOn { get; set; }
        public DateTime? ModifyOn { get; set; }
        public string ModifyBy { get; set; }
        public string Remarks { get; set; }
        public string EmpUserId { get; set; }
        public string SchoolCategoryMain { get; set; }
        public string ScanCopy { get; set; }
        public string Examination { get; set; }

        [NotMapped]
        public HttpPostedFileBase file { get; set; }
    }


    public partial class SchoolMasterForOnlineCentreCreationViews
    {
        [NotMapped]
        public List<OnlineCentreCreationsChallanViews> OnlineCentreCreationsChallanList { get; set; }
    }

    public partial class SchoolMasterForOnlineCentreCreationViews
    {
        [Key]
        public string SCHL { get; set; }
        public string SCHLE { get; set; }
        public string SCHLP { get; set; }
        public string SCHLNME { get; set; }
        public string SCHLNMP { get; set; }
        public string USERTYPE { get; set; }
        public string MOBILE { get; set; }
        public string DIST { get; set; }
        public string DISTNM { get; set; }
        public string DISTNMP { get; set; }



        public string EMAILID { get; set; }
        public int SENIOR_COUNT { get; set; }
        public int MATRIC_COUNT { get; set; }
        public int MIDDLE_COUNT { get; set; }
        public int IsExistsinCentre2017Master { get; set; }

        //
        public string CentreAppNo { get; set; }
        public int OnlineCentreCreationId { get; set; }
        public int? TotalRooms { get; set; }
        public int? TotalCapacity { get; set; }
        public int? TotalRoomsOther { get; set; }
        public int? SingleBenchFurniture { get; set; }
        public int? DoubleBenchFurniture { get; set; }
        public int? AppStatus { get; set; }
        public string SchoolCategoryMain { get; set; }

        public string ScanCopy { get; set; }
        public int IsExistsInOnlineCentreCreationsTERM1 { get; set; }
    }

    public class MagazineSchoolRequirementsChallanViews
    {

        [Key]
        public int MagazineId { get; set; }
        public string Schl { get; set; }
        public string RefNo { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public string RefNo2 { get; set; }
        public string FY { get; set; }
        public int FixedMagazine { get; set; }
        public int FixedRate { get; set; }
        public int FixedMonth { get; set; }
        public int FixedTotal { get; set; }
        public int ExtraMagazine { get; set; }
        public int ExtraRate { get; set; }
        public int ExtraMonth { get; set; }
        public int ExtraTotal { get; set; }
        public int TotalAmount { get; set; }
        public string ChallanId { get; set; }
        public DateTime? ChallanDT { get; set; }
        public int ChallanVerify { get; set; }
        public DateTime? ChallanPaidOn { get; set; }
        public DateTime SubmitOn { get; set; }

        public bool IsActive { get; set; }

        public string ChallanCategory { get; set; }
        public string OldReceiptNo { get; set; }
        public string oldChallanId { get; set; }
        public string OldDepositDate { get; set; }
        public int? OldAmount { get; set; }
        public string ReceiptScannedCopy { get; set; }
        public string IsApproved { get; set; }
        public string ApprovedOn { get; set; }
        public string ApprovedRemarks { get; set; }
        //
        public int VERIFIED { get; set; }
        public string ChallanStatus { get; set; }
        public string SCHL { get; set; }
        public string SCHLNME { get; set; }
        public string USERTYPE { get; set; }
        public string ChallanShowStatus { get; set; }
        public string ExpireVDate { get; set; }
        public int IsCancel { get; set; }




    }
    public partial class MagazineSchoolRequirements
    {
        [NotMapped]
        public string AllowBanks { get; set; }


        [NotMapped]
        public List<MagazineSchoolRequirementsChallanViews> MagazineSchoolRequirementsList { get; set; }
    }


    public partial class MagazineSchoolRequirements
    {
        [Key]
        public int MagazineId { get; set; }
        public string Schl { get; set; }
        public string RefNo { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public string RefNo2 { get; set; }
        public string FY { get; set; }
        public int FixedMagazine { get; set; }
        public int FixedRate { get; set; }
        public int FixedMonth { get; set; }
        public int FixedTotal { get; set; }
        public int ExtraMagazine { get; set; }
        public int ExtraRate { get; set; }
        public int ExtraMonth { get; set; }
        public int ExtraTotal { get; set; }
        public int TotalAmount { get; set; }
        public string ChallanId { get; set; }
        public DateTime? ChallanDT { get; set; }
        public int ChallanVerify { get; set; }
        public DateTime? ChallanPaidOn { get; set; }
        public DateTime SubmitOn { get; set; }
        public DateTime? ModifyOn { get; set; }
        public bool IsActive { get; set; }

        public string ChallanCategory { get; set; }
        public string OldReceiptNo { get; set; }
        public string oldChallanId { get; set; }
        public string OldDepositDate { get; set; }
        public int? OldAmount { get; set; }
        public string ReceiptScannedCopy { get; set; }
        public string IsApproved { get; set; }
        public string ApprovedOn { get; set; }
        public string ApprovedRemarks { get; set; }

    }

    public class tblOtherBoardDocumentsBySchool
    {
        [Key]
        public int Did { get; set; }
        public long Stdid { get; set; }
        public string Filepath { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public DateTime? SubmitOn { get; set; }
        public string SubmitBy { get; set; }

    }
    public class SchoolChangePasswordModel
    {
        public string SCHL { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string REMARKS { get; set; }
    }

    public class SchoolAccreditationCombinedModel
    {
        public SchoolModels schlmodel { get; set; }
        public SchoolAccreditationModel sam { get; set; }
    }

    public class FeeMasterAccreditation
    {
        public DataSet PaymentFormData { get; set; }
        public string cls { get; set; }
        public string Feecode { get; set; }
        public string Utype { get; set; }
        public string New { get; set; }
        public string ReNew { get; set; }
        public string latefee { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string BankEndDate { get; set; }
        public string BankAllow { get; set; }
        public string Id { get; set; }

    }

    public class SchoolAccreditationModel
    {
        public DataSet StoreAllData { get; set; }
        //Rohit
        public int id { get; set; }
        public string schl { get; set; }
        public string refno { get; set; }
        public string cls { get; set; }
        public string exam { get; set; }
        public string Acrtype { get; set; }

        public float fee { get; set; }
        public float latefee { get; set; }
        public float totfee { get; set; }

        public string challanid { get; set; }
        public int Challanverify { get; set; }
        public DateTime InsDate { get; set; }
        public int IsActive { get; set; }
        public int IsFinal { get; set; }
        public DateTime FinalDate { get; set; }
        public int Isdel { get; set; }
    }

    public class SchoolAllowForCCE
    {
        public string Panel { get; set; }
        public DataSet StoreAllData { get; set; }
        public int? Id { get; set; }
        public string Schl { get; set; }
        public string Cls { get; set; }
        public int IsActive { get; set; }
        public string IsAllow { get; set; }
        public string LastDate { get; set; }
        public string AllowTo { get; set; }
        public string ReceiptNo { get; set; }
        public string DepositDate { get; set; }
        public int Amount { get; set; }
        public string UpdatedDate { get; set; }
        public string AllowedDate { get; set; }
        public string AllowRemarks { get; set; }
        public string EmpUserId { get; set; }

        [NotMapped]
        public string EmpDetails { get; set; }
    }
    public class SelectListItemCheckBox
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }

    public class SchoolPremisesInformation
    {
        public string ChallanId { get; set; }
        public string ChallanDt { get; set; }
        public int challanVerify { get; set; }

        public DataSet StoreAllData { get; set; }
        public List<SelectListItemCheckBox> PropertyFloorList { get; set; }
        //public IList<SelectListItem> PropertyFloorList { get; set; }
        public int ID { get; set; }
        public string SCHL { get; set; }
        public string SSD1 { get; set; }
        public string SSD2 { get; set; }
        public int SSD3 { get; set; }
        public int SSD4 { get; set; }
        public float CB5 { get; set; }
        public float CB6 { get; set; }
        public int CB7 { get; set; }
        public int CB8 { get; set; }
        public int CB9 { get; set; }
        public int CB10 { get; set; }
        public int CB11 { get; set; }
        public int CB12 { get; set; }
        public int CB13 { get; set; }
        public int CB14 { get; set; }
        public int CB15 { get; set; }
        public string CB16 { get; set; }
        public string ECD17 { get; set; }
        public int ECD18 { get; set; }
        public int ECD19 { get; set; }
        public int ECD20 { get; set; }
        public int ECD21 { get; set; }
        public int ECD22 { get; set; }
        public int ECD23 { get; set; }
        public int ECD24 { get; set; }
        public string ECD25 { get; set; }
        public int CWS26 { get; set; }
        public int CWS27 { get; set; }
        public int CWS28 { get; set; }
        public int CWS29 { get; set; }
        public int CWS30 { get; set; }
        public int CWS31 { get; set; }
        public int CWS32 { get; set; }
        public int CWS33 { get; set; }
        public int CWS34 { get; set; }
        public int CWS35 { get; set; }
        public int CWS36 { get; set; }
        public int CWS37 { get; set; }
        public int CWS38 { get; set; }
        public int CWS39 { get; set; }
        public int CWS40 { get; set; }
        public int CWS41 { get; set; }
        public int CWS42 { get; set; }
        public int CWS43 { get; set; }
        public int CWS44 { get; set; }
        public int CSS45 { get; set; }
        public int CSS46 { get; set; }
        public int CSS47 { get; set; }
        public int CSS48 { get; set; }
        public float PG49 { get; set; }
        public int PG50 { get; set; }
        public string PG51 { get; set; }
        public int PG52 { get; set; }
        public string PG53 { get; set; }
        public int PG54 { get; set; }
        public float LIB55 { get; set; }
        public string LIB56 { get; set; }
        public int LIB57 { get; set; }
        public int LIB58 { get; set; }
        public int LIB59 { get; set; }
        public float LAB60 { get; set; }
        public int LAB61 { get; set; }
        public float LAB62 { get; set; }
        public int LAB63 { get; set; }
        public float LAB64 { get; set; }
        public int LAB65 { get; set; }
        public float LAB66 { get; set; }
        public int LAB67 { get; set; }
        public float CLAB68 { get; set; }
        public int CLAB69 { get; set; }
        public int CLAB70 { get; set; }
        public string CLAB71 { get; set; }
        public string OTH72 { get; set; }
        public string OTH73 { get; set; }
        public string OTH74 { get; set; }
        public string OTH75 { get; set; }
        public string OTH76 { get; set; }
        public string OTH77 { get; set; }
        public string OTH78 { get; set; }
        public string OTH79 { get; set; }
        public string OTH80 { get; set; }
        public string OTH81 { get; set; }
        public string OTH82 { get; set; }
        public string OTH83 { get; set; }
        public string UDISECODE { get; set; }
        public bool ISACTIVE { get; set; }
        public DateTime CREATEDDATE { get; set; }
        public DateTime UPDATEDDATE { get; set; }
        public int CREATEDBY { get; set; }
        public int UPDATEDBY { get; set; }
        public int IsFinalSubmit { get; set; }
        public DateTime FinalSubmitOn { get; set; }
    }

    public class BookRequest
    {
        public int BookId { get; set; }
        public int Class { get; set; }
        public string BookNM { get; set; }
        public string NOS { get; set; }
        public string Flag { get; set; }
    }
    public class BookAssessmentForm
    {
        public DataTable BookRequestDT { get; set; }
        public List<BookRequest> BookRequestList { get; set; }
        public DataSet StoreAllData { get; set; }
        public DataSet TotalCount { get; set; }
        public int id { get; set; }
        [Required(ErrorMessage = "*")]
        public string SCHL { get; set; }
        [Required(ErrorMessage = "*")]
        public string PrincipalName { get; set; }
        [Required(ErrorMessage = "*")]
        public string SchoolMobile { get; set; }
        public int Class { get; set; }
        public int BOOKID { get; set; }
        public string BOOK_NM { get; set; }
        public int TOT_BOOK { get; set; }
        public int TOT_STUD { get; set; }

    }
    public class RegistrationForTraining
    {
        public DataSet StoreAllData { get; set; }
        public DataSet TotalCount { get; set; }
        public int id { get; set; }
        [Required(ErrorMessage = "*")]
        public string schl { get; set; }
        [Required(ErrorMessage = "*")]
        public string PrincipalName { get; set; }
        [Required(ErrorMessage = "*")]
        public string SchoolMobile { get; set; }
        [Required(ErrorMessage = "*")]
        public string SchoolRepresentative { get; set; }
        [Required(ErrorMessage = "*")]
        public string Designation { get; set; }

        [RegularExpression(@"^\(?([5-9]{1})\)?([0-9]{2})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Incomplete Format.")]
        public string cpmobile { get; set; }

        [RegularExpression(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email ID.")]
        [DataType(DataType.EmailAddress)]
        public string cpemail { get; set; }

    }

    public class CallCenter
    {

        public int dist { get; set; }
        public DataSet StoreAllData { get; set; }
        public DataSet TotalCount { get; set; }
        public int ccfid { get; set; }
        [Required(ErrorMessage = "*")]
        public string schoolcode { get; set; }
        [Required(ErrorMessage = "*")]
        public string schoolname { get; set; }
        [Required(ErrorMessage = "*")]
        public string district { get; set; }
        [Required(ErrorMessage = "*")]
        public string classname { get; set; }
        [Required(ErrorMessage = "*")]
        public string formname { get; set; }
        [Required(ErrorMessage = "*")]
        public string cpname { get; set; }
        [RegularExpression(@"^\(?([5-9]{1})\)?([0-9]{2})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Incomplete Format.")]
        [Required(ErrorMessage = "*")]
        public string cpmobile { get; set; }
        [RegularExpression(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email ID.")]
        [Required(ErrorMessage = "*")]
        public string cpemail { get; set; }
        [Required(ErrorMessage = "*")]
        public string description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int status { get; set; }
        public string ticketNo { get; set; }
        public string remarks { get; set; }
        public string sessionyear { get; set; }
        public string photo { get; set; }
        public string pdf { get; set; }
        public HttpPostedFileBase[] files { get; set; }
        public List<String> CommaStringToList { get; set; }
    }
    //rohit
    public class CandSubject
    {
        public string SUB { get; set; }
        public string OBTMARKS { get; set; }
        public string MINMARKS { get; set; }
        public string MAXMARKS { get; set; }
    }

    //rohit
    public class CandSubjectPrac
    {
        public string CANDID { get; set; }
        public string SUB { get; set; }
        public string OBTMARKSP { get; set; }
        public string MINMARKSP { get; set; }
        public string MAXMARKSP { get; set; }
        public string ACCEPT { get; set; }
        public string PRACDATE { get; set; }
    }

    public class Printlist
    {
        public DataSet StoreAllData { get; set; }
        public DataTable dsCircular { get; set; }
        public DataTable dsMarque { get; set; }
        public string ExamCent { get; set; }
        public string ExamRoll { get; set; }
        [Required(ErrorMessage = "Enter Code")]
        [StringLength(11, ErrorMessage = "Name not be exceed 11 char")]
        public string udisecode { get; set; }
        public bool NSQF_flag { get; set; }
        public string MATRIC { get; set; }
        public string HUM { get; set; }
        public string SCI { get; set; }
        public string COMM { get; set; }
        public string VOC { get; set; }
        public string TECH { get; set; }
        public string AGRI { get; set; }
        public string numofhum { get; set; }
        public string numofsci { get; set; }
        public string numofcomm { get; set; }
        public string numofvoc { get; set; }
        public string numoftech { get; set; }
        public string numofregular { get; set; }
        public string numofnsqf { get; set; }
        public string numofagri { get; set; }
        public string Category { get; set; }
    }

    public class CandPracExaminer
    {
        public string SUB { get; set; }
        public string CENT { get; set; }
        public string EXAMINER { get; set; }
        public string SCHOOL { get; set; }
        public string TEACHER { get; set; }
        public string MOBILE { get; set; }
    }

    public class SchoolModels
    {
        //
        public SchoolAllotedToAgency schoolAllotedToAgency;
        public SchoolAllowForMarksEntry schoolAllowForMarksEntry;


        public string SCHLNME { get; set; }
        public string SCHLNMP { get; set; }
        public string APPNO { get; set; }
        public string SameAsSchl { get; set; }
        //fifth
        public string uclass { get; set; }
        public string lclass { get; set; }
        public string fifth { get; set; }
        public string FIF_YR { get; set; }
        public string FIF_S { get; set; }
        public string FIF_UTYPE { get; set; }
        public string FIF_NO { get; set; }

        //Rohit
        public string omattype { get; set; }
        public string ohumtype { get; set; }
        public string oscitype { get; set; }
        public string ocommtype { get; set; }
        public string acno { get; set; }
        public string confirmacno { get; set; }

        //
        public string LinkSchool { get; set; }
        public string OHID_YR { get; set; }
        public string OMID_YR { get; set; }
        public string OHYR { get; set; }
        public string OSYR { get; set; }
        public string OCYR { get; set; }
        public string OVYR { get; set; }
        public string OTYR { get; set; }
        public string OAYR { get; set; }

        public string HID_YR_SEC { get; set; }
        public string MID_YR_SEC { get; set; }
        public string HYR_SEC { get; set; }
        public string SYR_SEC { get; set; }
        public string CYR_SEC { get; set; }
        public string VYR_SEC { get; set; }
        public string TYR_SEC { get; set; }
        public string AYR_SEC { get; set; }

        public string OHID_YR_SEC { get; set; }
        public string OMID_YR_SEC { get; set; }
        public string OHYR_SEC { get; set; }
        public string OSYR_SEC { get; set; }
        public string OCYR_SEC { get; set; }
        public string OVYR_SEC { get; set; }
        public string OTYR_SEC { get; set; }
        public string OAYR_SEC { get; set; }

        //Ranjan
        public string hdnFlag { get; set; }
        public string category { get; set; }
        public string Exam_Type { get; set; }
        public string Edublock { get; set; }
        public string EduCluster { get; set; }
        public string SchlType { get; set; }
        public string SchlEstd { get; set; }
        public string Bank { get; set; }
        public string IFSC { get; set; }
        public string geoloc { get; set; }
        public string Phychall { get; set; }
        public string Quali { get; set; }
        //---------------------------------------------------
        //Rohit
        public string CandId { set; get; }
        //Ranjan
        public string ResultList { get; set; }
        public string Selsec { get; set; }
        // schl result 
        public string SelSet { get; set; }
        public string SearchByString { get; set; }
        public string SelList { get; set; }
        public string SelForm { get; set; }
        public string REGNO { get; set; }
        public string TotalSearchString { get; set; }
        //public int? ID { get; set; }
        public string SchlCode { get; set; }
        public string Candi_Name { get; set; }
        public string Father_Name { get; set; }
        public string Mother_Name { get; set; }
        public string DOB { get; set; }
        public string DOJ { get; set; }
        public string ExperienceYr { get; set; }
        public string PQualification { get; set; }
        public string Gender { get; set; }
        public string TotalMarks { get; set; }
        public string ObtainedMarks { get; set; }
        public string Result { get; set; }
        public string totMark { get; set; }
        public string reclock { get; set; }
        public string SearchResult { get; set; }
        public string FormName { get; set; }
        public string SdtID { get; set; }
        public string EXAM { get; set; }
        //
        //rohit
        public bool Agree { get; set; }

        public string ExamCent { get; set; }
        public string ExamSub { get; set; }
        public string ExamRoll { get; set; }
        public string udisecode { get; set; }
        public DataSet StoreAllData { get; set; }

        // School Master Table details
        public string status { get; set; }
        public string session { get; set; }
        public string dist { get; set; }
        //public string schl { get; set; }
        public string idno { get; set; }
        public string OCODE { get; set; }
        public string CLASS { get; set; }
        public string AREA { get; set; }
        public string SCHLP { get; set; }
        public string STATIONP { get; set; }
        public string SCHLE { get; set; }
        public string STATIONE { get; set; }
        public string DISTE { get; set; }
        public string DISTP { get; set; }
        public string DISTNM { get; set; }
        public string MATRIC { get; set; }
        public string HUM { get; set; }
        public string SCI { get; set; }
        public string COMM { get; set; }
        public string VOC { get; set; }
        public string TECH { get; set; }
        public string AGRI { get; set; }
        public string OMATRIC { get; set; }
        public string OHUM { get; set; }
        public string OSCI { get; set; }
        public string OCOMM { get; set; }
        public string OVOC { get; set; }
        public string OTECH { get; set; }
        public string OAGRI { get; set; }
        public string IDATE { get; set; }
        public string VALIDITY { get; set; }
        public string UDATE { get; set; }
        public string REMARKS { get; set; }
        public int id { get; set; }
        public string middle { get; set; }
        public string omiddle { get; set; }
        public string correctionno { get; set; }

        public string DISTNMPun { get; set; }
        public string username { get; set; }
        public string userip { get; set; }
        public string ImpschlOcode { get; set; }
        public string SSET { get; set; }
        public string MSET { get; set; }
        public string SOSET { get; set; }
        public string MOSET { get; set; }
        public string MID_CR { get; set; }
        public string MID_NO { get; set; }
        public string MID_YR { get; set; }
        public int MID_S { get; set; }
        public string MID_DNO { get; set; }

        public string HID_CR { get; set; }
        public string HID_NO { get; set; }
        public string HID_YR { get; set; }
        public int HID_S { get; set; }
        public string HID_DNO { get; set; }

        public string SID_CR { get; set; }
        public string SID_NO { get; set; }
        public string SID_DNO { get; set; }
        public string H { get; set; }
        public string HYR { get; set; }

        public int H_S { get; set; }
        public string C { get; set; }
        public string CYR { get; set; }
        public int C_S { get; set; }
        public string S { get; set; }
        public string SYR { get; set; }
        public int S_S { get; set; }

        public string A { get; set; }
        public string AYR { get; set; }
        public int A_S { get; set; }

        public string V { get; set; }
        public string VYR { get; set; }
        public int V_S { get; set; }

        public string T { get; set; }
        public string TYR { get; set; }
        public int T_S { get; set; }

        public string MID_UTYPE { get; set; }
        public string HID_UTYPE { get; set; }
        public string H_UTYPE { get; set; }
        public string S_UTYPE { get; set; }
        public string C_UTYPE { get; set; }
        public string V_UTYPE { get; set; }
        public string A_UTYPE { get; set; }
        public string T_UTYPE { get; set; }

        public string Tcode { get; set; }
        public string Tehsile { get; set; }
        public string Tehsilp { get; set; }

        //-tblschUser
        public string USER { get; set; }
        public string SCHL { get; set; }
        public string PASSWORD { get; set; }
        public string PRINCIPAL { get; set; }
        public string STDCODE { get; set; }
        public string PHONE { get; set; }
        public string MOBILE { get; set; }
        public string EMAILID { get; set; }
        public string CONTACTPER { get; set; }
        public string CPSTD { get; set; }
        public string CPPHONE { get; set; }
        public string OtContactno { get; set; }
        public string ACTIVE { get; set; }
        public string USERTYPE { get; set; }
        public string ADDRESSE { get; set; }
        public string ADDRESSP { get; set; }
        public string vflag { get; set; }
        public bool cflag { get; set; }
        public string DateFirstLogin { get; set; }
        public string Vcode { get; set; }
        public string Approved { get; set; }
        public bool schlInfoUpdFlag { get; set; }
        public string mobile2 { get; set; }
        public int PEND_RESULT { get; set; }
        public string NSQF_flag { get; set; }



        public string CorrectionNoOld { get; set; }
        public string RemarksOld { get; set; }
        public string RemarkDateOld { get; set; }
        public string CorrectionDetails { get; set; }
    }

    public class UploadImgndSignature
    {
        public HttpPostedFileBase std_Photo { get; set; }
        public HttpPostedFileBase std_Sign { get; set; }
    }

    public class CandSubjectPreBoard
    {
        public string SUB { get; set; }
        public string OBTMARKS { get; set; }
        public string MINMARKS { get; set; }
        public string MAXMARKS { get; set; }

        public string PROBTMARKS { get; set; }
        public string PRMINMARKS { get; set; }
        public string PRMAXMARKS { get; set; }

        public string INOBTMARKS { get; set; }
        public string INMINMARKS { get; set; }
        public string INMAXMARKS { get; set; }
    }

    public partial class ChallanModels
    {
        [Key]
        public string CHALLANID { get; set; }
        public string CHLNDATE { get; set; }
        public string CHLNVDATE { get; set; }
        public string FEEMODE { get; set; }
        public string FEECODE { get; set; }
        public string FEECAT { get; set; }
        public string BCODE { get; set; }
        public string BANK { get; set; }
        public string ACNO { get; set; }
        public string FEE { get; set; }
        public string BANKCHRG { get; set; }
        public string TOTFEE { get; set; }
        public string SCHLREGID { get; set; }
        public string DIST { get; set; }
        public string DISTNM { get; set; }
        public string SCHLCANDNM { get; set; }
        public string BRCODE { get; set; }
        public string BRANCH { get; set; }
        public string J_REF_NO { get; set; }
        public string DEPOSITDT { get; set; }
        public string VERIFIED { get; set; }
        public string VERIFYDATE { get; set; }
        public string DOWNLDFLG { get; set; }
        public string DOWNLDFLOT { get; set; }
        public string DOWNLDDATE { get; set; }
        public string APPNO { get; set; }
        public string ID { get; set; }
        public string addfee { get; set; }
        public string latefee { get; set; }
        public string prosfee { get; set; }
        public string addsubfee { get; set; }
        public string add_sub_count { get; set; }
        public string regfee { get; set; }
        public string type { get; set; }
        public string LOT { get; set; }
        public string ChallanGDateN { get; set; }
        public string ChallanVDateN { get; set; }
        public string VerifyDateN { get; set; }
        public string DownloadDateN { get; set; }
        public string SchoolCode { get; set; }
        public string ReChallanFlag { get; set; }
        public string Mobile { get; set; }
        public string IsCancel { get; set; }
        public string ActionRemarks { get; set; }
        public string CancelDate { get; set; }
        public string LumsumFine { get; set; }
        public string LSFRemarks { get; set; }
        public string RP { get; set; }
        public string StudentList { get; set; }
        public string DMReceiveNo { get; set; }
        public string DMReceiveDate { get; set; }
        public string BRCODECAND { get; set; }
        public string BRANCHCAND { get; set; }
        public string J_REF_NOCAND { get; set; }
        public string DEPOSITDTCAND { get; set; }
        public string SUBMITCAND { get; set; }
        public string OpenExamFee { get; set; }
        public string OpenLateFee { get; set; }
        public string OpenTotalFee { get; set; }
        public string SettlementDate { get; set; }
        public string RefundStatus { get; set; }
        public string RefundRefno { get; set; }
        public string RefundDate { get; set; }
        public string VerifyRemarks { get; set; }
        public string CancelRemarks { get; set; }
        public string EmpUserId { get; set; }
        public string FeeDistrict { get; set; }

    }
    public partial class InfrasturePerformasListWithSchool
    {
        public int StaffCount { get; set; }
        [Key]
        public int ID { get; set; }
        public string Bank { get; set; }
        public string BankAddress { get; set; }
        public string IFSC { get; set; }
        public string SOLID { get; set; }
        public string DISTE { get; set; }
        public string SCHLE { get; set; }
        public string EstablishmentYear { get; set; }
        public string SchoolName { get; set; }
        public string SCHL { get; set; }
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
        public string Col5 { get; set; }
        public string Col6 { get; set; }
        public string Col7 { get; set; }
        public string Col8 { get; set; }
        public string Col9 { get; set; }
        public string Col10 { get; set; }
        public string Col11 { get; set; }
        public string Col12 { get; set; }
        public string Col13 { get; set; }
        public string Col14 { get; set; }
        public string Col15 { get; set; }
        public string Col16 { get; set; }
        public string Col17 { get; set; }
        public string Col18 { get; set; }
        public string Col19 { get; set; }
        public string Col20 { get; set; }
        public string Col21 { get; set; }
        public string Col22 { get; set; }
        public string Col23 { get; set; }
        public string Col24 { get; set; }
        public string Col25A { get; set; }
        public string Col25B { get; set; }
        public string Col25C { get; set; }
        public string Col29 { get; set; }
        public string Col30 { get; set; }
        public string Bank1 { get; set; }
        public string BankBranch1 { get; set; }
        public string IFSC1 { get; set; }
        public string Bank2 { get; set; }
        public string BankBranch2 { get; set; }
        public string IFSC2 { get; set; }
        public string Bank3 { get; set; }
        public string BankBranch3 { get; set; }
        public string IFSC3 { get; set; }
        public int FinalSubmitStatus { get; set; }
        public string FinalSubmitDate { get; set; }
        public string DIST { get; set; }

        public SchoolAllotedToAgency schoolAllotedToAgency;
        public SchoolAllowForMarksEntry schoolAllowForMarksEntry;


        //fifth
        public string uclass { get; set; }
        public string lclass { get; set; }
        public string fifth { get; set; }
        public string FIF_YR { get; set; }
        public string FIF_S { get; set; }
        public string FIF_UTYPE { get; set; }
        public string FIF_NO { get; set; }

        //Rohit
        public string omattype { get; set; }
        public string ohumtype { get; set; }
        public string oscitype { get; set; }
        public string ocommtype { get; set; }
        //
        public string Edublock { get; set; }
        public string EduCluster { get; set; }
        public string SchlType { get; set; }
        public string SchlEstd { get; set; }

        //public int? ID { get; set; }
        public string SchlCode { get; set; }
        public string udisecode { get; set; }

        // School Master Table details
        public string status { get; set; }
        public string session { get; set; }
        public string dist { get; set; }
        //public string schl { get; set; }
        public string idno { get; set; }
        public string OCODE { get; set; }
        public string CLASS { get; set; }
        public string AREA { get; set; }
        public string SCHLP { get; set; }
        public string STATIONP { get; set; }

        public string STATIONE { get; set; }
        public string DISTP { get; set; }
        public string DISTNM { get; set; }
        public string MATRIC { get; set; }
        public string HUM { get; set; }
        public string SCI { get; set; }
        public string COMM { get; set; }
        public string VOC { get; set; }
        public string TECH { get; set; }
        public string AGRI { get; set; }
        public string OMATRIC { get; set; }
        public string OHUM { get; set; }
        public string OSCI { get; set; }
        public string OCOMM { get; set; }
        public string OVOC { get; set; }
        public string OTECH { get; set; }
        public string OAGRI { get; set; }
        public string IDATE { get; set; }
        public string VALIDITY { get; set; }
        public string UDATE { get; set; }
        public string REMARKS { get; set; }
        public int id { get; set; }
        public string middle { get; set; }
        public string omiddle { get; set; }
        public string correctionno { get; set; }

        public string DISTNMPun { get; set; }
        public string username { get; set; }
        public string userip { get; set; }
        public string ImpschlOcode { get; set; }
        public string SSET { get; set; }
        public string MSET { get; set; }
        public string SOSET { get; set; }
        public string MOSET { get; set; }
        public string MID_CR { get; set; }
        public string MID_NO { get; set; }
        public string MID_YR { get; set; }
        public int MID_S { get; set; }
        public string MID_DNO { get; set; }

        public string HID_CR { get; set; }
        public string HID_NO { get; set; }
        public string HID_YR { get; set; }
        public int HID_S { get; set; }
        public string HID_DNO { get; set; }

        public string SID_CR { get; set; }
        public string SID_NO { get; set; }
        public string SID_DNO { get; set; }
        public string H { get; set; }
        public string HYR { get; set; }

        public int H_S { get; set; }
        public string C { get; set; }
        public string CYR { get; set; }
        public int C_S { get; set; }
        public string S { get; set; }
        public string SYR { get; set; }
        public int S_S { get; set; }

        public string A { get; set; }
        public string AYR { get; set; }
        public int A_S { get; set; }

        public string V { get; set; }
        public string VYR { get; set; }
        public int V_S { get; set; }

        public string T { get; set; }
        public string TYR { get; set; }
        public int T_S { get; set; }

        public string MID_UTYPE { get; set; }
        public string HID_UTYPE { get; set; }
        public string H_UTYPE { get; set; }
        public string S_UTYPE { get; set; }
        public string C_UTYPE { get; set; }
        public string V_UTYPE { get; set; }
        public string A_UTYPE { get; set; }
        public string T_UTYPE { get; set; }

        public string Tcode { get; set; }
        public string Tehsile { get; set; }
        public string Tehsilp { get; set; }
        public string DISTNMP { get; set; }
        public string SCHLEfull { get; set; }
        public string SCHLPfull { get; set; }
        public string ADDRESSEfull { get; set; }
        public string ADDRESSPfull { get; set; }
        public string MID_UTYPEfull { get; set; }
        public string HID_UTYPEFull { get; set; }
        public string H_UTYPEFull { get; set; }
        public string S_UTYPEFull { get; set; }
        public string C_UTYPEFull { get; set; }
        public string V_UTYPEFull { get; set; }
        public string A_UTYPEFull { get; set; }
        public string T_UTYPEFull { get; set; }
        public string USER { get; set; }
        public string PASSWORD { get; set; }
        public string PRINCIPAL { get; set; }
        public string DOB { get; set; }
        public string DOJ { get; set; }
        public string ExperienceYr { get; set; }
        public string PQualification { get; set; }
        public string STDCODE { get; set; }
        public string PHONE { get; set; }
        public string MOBILE { get; set; }
        public string EMAILID { get; set; }
        public string CONTACTPER { get; set; }
        public string CPSTD { get; set; }
        public string CPPHONE { get; set; }
        public string OtContactno { get; set; }
        public string ACTIVE { get; set; }
        public string USERTYPE { get; set; }
        public string ADDRESSE { get; set; }
        public string ADDRESSP { get; set; }
        public string vflag { get; set; }
        public string cflag { get; set; }
        public string DateFirstLogin { get; set; }
        public string Vcode { get; set; }
        public string Approved { get; set; }
        public string schlInfoUpdFlag { get; set; }
        public string mobile2 { get; set; }
        public string PEND_RESULT { get; set; }
        public string NSQF_flag { get; set; }
        public string SchoolArea { get; set; }
        public string ClassID { get; set; }
        public string SchoolTypeID { get; set; }
        public string IsVerified { get; set; }
        public string IsApproved { get; set; }
        public string acno { get; set; }
        public string FIF_UTYPEFull { get; set; }
        public string GeoUdise { get; set; }
        public string GeoLong { get; set; }
        public string GeoLat { get; set; }
        public string NoOfPrimary { get; set; }
        public string NoOfMiddle { get; set; }
        public string IsEAffilicationCLU_Allowed { get; set; }
        public string IsAffiliationMiddle { get; set; }
        public string LoginStatus { get; set; }

    }

    //}
    //public partial class InfrasturePerformasListWithSchool
    //{
    //    public int StaffCount { get; set; }
    //    [Key]
    //    public int ID { get; set; }
    //    public string Bank { get; set; }
    //    public string BankAddress { get; set; }
    //    public string IFSC { get; set; }
    //    public string SOLID { get; set; }
    //    public string DISTE { get; set; }
    //    public string SCHLE { get; set; }
    //    public string EstablishmentYear { get; set; }
    //    public string SchoolName { get; set; }
    //    public string SCHL { get; set; }
    //    public string Col1 { get; set; }
    //    public string Col2 { get; set; }
    //    public string Col3 { get; set; }
    //    public string Col4 { get; set; }
    //    public string Col5 { get; set; }
    //    public string Col6 { get; set; }
    //    public string Col7 { get; set; }
    //    public string Col8 { get; set; }
    //    public string Col9 { get; set; }
    //    public string Col10 { get; set; }
    //    public string Col11 { get; set; }
    //    public string Col12 { get; set; }
    //    public string Col13 { get; set; }
    //    public string Col14 { get; set; }
    //    public string Col15 { get; set; }
    //    public string Col16 { get; set; }
    //    public string Col17 { get; set; }
    //    public string Col18 { get; set; }
    //    public string Col19 { get; set; }
    //    public string Col20 { get; set; }
    //    public string Col21 { get; set; }
    //    public string Col22 { get; set; }
    //    public string Col23 { get; set; }
    //    public string Col24 { get; set; }
    //    public string Col25A { get; set; }
    //    public string Col25B { get; set; }
    //    public string Col25C { get; set; }
    //    public string Col29 { get; set; }
    //    public string Col30 { get; set; }
    //    public string Bank1 { get; set; }
    //    public string BankBranch1 { get; set; }
    //    public string IFSC1 { get; set; }
    //    public string Bank2 { get; set; }
    //    public string BankBranch2 { get; set; }
    //    public string IFSC2 { get; set; }
    //    public string Bank3 { get; set; }
    //    public string BankBranch3 { get; set; }
    //    public string IFSC3 { get; set; }
    //    public int FinalSubmitStatus { get; set; }
    //    public string FinalSubmitDate { get; set; }
    //    public string DIST { get; set; }

    //    public SchoolAllotedToAgency schoolAllotedToAgency;
    //    public SchoolAllowForMarksEntry schoolAllowForMarksEntry;


    //    public string SCHLNME { get; set; }
    //    public string SCHLNMP { get; set; }
    //    public string APPNO { get; set; }
    //    public string SameAsSchl { get; set; }
    //    //fifth
    //    public string uclass { get; set; }
    //    public string lclass { get; set; }
    //    public string fifth { get; set; }
    //    public string FIF_YR { get; set; }
    //    public string FIF_S { get; set; }
    //    public string FIF_UTYPE { get; set; }
    //    public string FIF_NO { get; set; }

    //    //Rohit
    //    public string omattype { get; set; }
    //    public string ohumtype { get; set; }
    //    public string oscitype { get; set; }
    //    public string ocommtype { get; set; }
    //    public string acno { get; set; }
    //    public string confirmacno { get; set; }

    //    //
    //    public string LinkSchool { get; set; }
    //    public string OHID_YR { get; set; }
    //    public string OMID_YR { get; set; }
    //    public string OHYR { get; set; }
    //    public string OSYR { get; set; }
    //    public string OCYR { get; set; }
    //    public string OVYR { get; set; }
    //    public string OTYR { get; set; }
    //    public string OAYR { get; set; }

    //    public string HID_YR_SEC { get; set; }
    //    public string MID_YR_SEC { get; set; }
    //    public string HYR_SEC { get; set; }
    //    public string SYR_SEC { get; set; }
    //    public string CYR_SEC { get; set; }
    //    public string VYR_SEC { get; set; }
    //    public string TYR_SEC { get; set; }
    //    public string AYR_SEC { get; set; }

    //    public string OHID_YR_SEC { get; set; }
    //    public string OMID_YR_SEC { get; set; }
    //    public string OHYR_SEC { get; set; }
    //    public string OSYR_SEC { get; set; }
    //    public string OCYR_SEC { get; set; }
    //    public string OVYR_SEC { get; set; }
    //    public string OTYR_SEC { get; set; }
    //    public string OAYR_SEC { get; set; }

    //    //Ranjan
    //    public string hdnFlag { get; set; }
    //    public string category { get; set; }
    //    public string Exam_Type { get; set; }
    //    public string Edublock { get; set; }
    //    public string EduCluster { get; set; }
    //    public string SchlType { get; set; }
    //    public string SchlEstd { get; set; }
    //    public string geoloc { get; set; }
    //    public string Phychall { get; set; }
    //    public string Quali { get; set; }
    //    //---------------------------------------------------
    //    //Rohit
    //    public string CandId { set; get; }
    //    //Ranjan
    //    public string ResultList { get; set; }
    //    public string Selsec { get; set; }
    //    // schl result 
    //    public string SelSet { get; set; }
    //    public string SearchByString { get; set; }
    //    public string SelList { get; set; }
    //    public string SelForm { get; set; }
    //    public string REGNO { get; set; }
    //    public string TotalSearchString { get; set; }
    //    //public int? ID { get; set; }
    //    public string SchlCode { get; set; }
    //    public string Candi_Name { get; set; }
    //    public string Father_Name { get; set; }
    //    public string Mother_Name { get; set; }
    //    public string DOB { get; set; }
    //    public string DOJ { get; set; }
    //    public string ExperienceYr { get; set; }
    //    public string PQualification { get; set; }
    //    public string Gender { get; set; }
    //    public string TotalMarks { get; set; }
    //    public string ObtainedMarks { get; set; }
    //    public string Result { get; set; }
    //    public string totMark { get; set; }
    //    public string reclock { get; set; }
    //    public string SearchResult { get; set; }
    //    public string FormName { get; set; }
    //    public string SdtID { get; set; }
    //    public string EXAM { get; set; }
    //    //
    //    //rohit
    //    public bool Agree { get; set; }

    //    public string ExamCent { get; set; }
    //    public string ExamSub { get; set; }
    //    public string ExamRoll { get; set; }
    //    public string udisecode { get; set; }

    //    // School Master Table details
    //    public string status { get; set; }
    //    public string session { get; set; }
    //    public string dist { get; set; }
    //    //public string schl { get; set; }
    //    public string idno { get; set; }
    //    public string OCODE { get; set; }
    //    public string CLASS { get; set; }
    //    public string AREA { get; set; }
    //    public string SCHLP { get; set; }
    //    public string STATIONP { get; set; }

    //    public string STATIONE { get; set; }
    //    public string DISTP { get; set; }
    //    public string DISTNM { get; set; }
    //    public string MATRIC { get; set; }
    //    public string HUM { get; set; }
    //    public string SCI { get; set; }
    //    public string COMM { get; set; }
    //    public string VOC { get; set; }
    //    public string TECH { get; set; }
    //    public string AGRI { get; set; }
    //    public string OMATRIC { get; set; }
    //    public string OHUM { get; set; }
    //    public string OSCI { get; set; }
    //    public string OCOMM { get; set; }
    //    public string OVOC { get; set; }
    //    public string OTECH { get; set; }
    //    public string OAGRI { get; set; }
    //    public string IDATE { get; set; }
    //    public string VALIDITY { get; set; }
    //    public string UDATE { get; set; }
    //    public string REMARKS { get; set; }
    //    public int id { get; set; }
    //    public string middle { get; set; }
    //    public string omiddle { get; set; }
    //    public string correctionno { get; set; }

    //    public string DISTNMPun { get; set; }
    //    public string username { get; set; }
    //    public string userip { get; set; }
    //    public string ImpschlOcode { get; set; }
    //    public string SSET { get; set; }
    //    public string MSET { get; set; }
    //    public string SOSET { get; set; }
    //    public string MOSET { get; set; }
    //    public string MID_CR { get; set; }
    //    public string MID_NO { get; set; }
    //    public string MID_YR { get; set; }
    //    public int MID_S { get; set; }
    //    public string MID_DNO { get; set; }

    //    public string HID_CR { get; set; }
    //    public string HID_NO { get; set; }
    //    public string HID_YR { get; set; }
    //    public int HID_S { get; set; }
    //    public string HID_DNO { get; set; }

    //    public string SID_CR { get; set; }
    //    public string SID_NO { get; set; }
    //    public string SID_DNO { get; set; }
    //    public string H { get; set; }
    //    public string HYR { get; set; }

    //    public int H_S { get; set; }
    //    public string C { get; set; }
    //    public string CYR { get; set; }
    //    public int C_S { get; set; }
    //    public string S { get; set; }
    //    public string SYR { get; set; }
    //    public int S_S { get; set; }

    //    public string A { get; set; }
    //    public string AYR { get; set; }
    //    public int A_S { get; set; }

    //    public string V { get; set; }
    //    public string VYR { get; set; }
    //    public int V_S { get; set; }

    //    public string T { get; set; }
    //    public string TYR { get; set; }
    //    public int T_S { get; set; }

    //    public string MID_UTYPE { get; set; }
    //    public string HID_UTYPE { get; set; }
    //    public string H_UTYPE { get; set; }
    //    public string S_UTYPE { get; set; }
    //    public string C_UTYPE { get; set; }
    //    public string V_UTYPE { get; set; }
    //    public string A_UTYPE { get; set; }
    //    public string T_UTYPE { get; set; }

    //    public string Tcode { get; set; }
    //    public string Tehsile { get; set; }
    //    public string Tehsilp { get; set; }

    //    //-tblschUser
    //    public string USER { get; set; }
    //    public string PASSWORD { get; set; }
    //    public string PRINCIPAL { get; set; }
    //    public string STDCODE { get; set; }
    //    public string PHONE { get; set; }
    //    public string MOBILE { get; set; }
    //    public string EMAILID { get; set; }
    //    public string CONTACTPER { get; set; }
    //    public string CPSTD { get; set; }
    //    public string CPPHONE { get; set; }
    //    public string OtContactno { get; set; }
    //    public string ACTIVE { get; set; }
    //    public string USERTYPE { get; set; }
    //    public string ADDRESSE { get; set; }
    //    public string ADDRESSP { get; set; }
    //    public string vflag { get; set; }
    //    public bool cflag { get; set; }
    //    public string DateFirstLogin { get; set; }
    //    public string Vcode { get; set; }
    //    public string Approved { get; set; }
    //    public bool schlInfoUpdFlag { get; set; }
    //    public string mobile2 { get; set; }
    //    public int PEND_RESULT { get; set; }
    //    public string NSQF_flag { get; set; }
    //    public string CorrectionNoOld { get; set; }
    //    public string RemarksOld { get; set; }
    //    public string RemarkDateOld { get; set; }
    //    public string CorrectionDetails { get; set; }
    //}
    public partial class InfrasturePerformasList
    {
        [Key]

        public int StaffCount { get; set; }
        public int ID { get; set; }
        public string Bank { get; set; }
        public string BankAddress { get; set; }
        public string IFSC { get; set; }
        public string SOLID { get; set; }
        public string DISTE { get; set; }
        public string SCHLE { get; set; }
        public string EstablishmentYear { get; set; }
        public string SchoolName { get; set; }
        public string SCHL { get; set; }
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
        public string Col5 { get; set; }
        public string Col6 { get; set; }
        public string Col7 { get; set; }
        public string Col8 { get; set; }
        public string Col9 { get; set; }
        public string Col10 { get; set; }
        public string Col11 { get; set; }
        public string Col12 { get; set; }
        public string Col13 { get; set; }
        public string Col14 { get; set; }
        public string Col15 { get; set; }
        public string Col16 { get; set; }
        public string Col17 { get; set; }
        public string Col18 { get; set; }
        public string Col19 { get; set; }
        public string Col20 { get; set; }
        public string Col21 { get; set; }
        public string Col22 { get; set; }
        public string Col23 { get; set; }
        public string Col24 { get; set; }
        public string Col25A { get; set; }
        public string Col25B { get; set; }
        public string Col25C { get; set; }
        public string Col29 { get; set; }
        public string Col30 { get; set; }
        public string Bank1 { get; set; }
        public string BankBranch1 { get; set; }
        public string IFSC1 { get; set; }
        public string Bank2 { get; set; }
        public string BankBranch2 { get; set; }
        public string IFSC2 { get; set; }
        public string Bank3 { get; set; }
        public string BankBranch3 { get; set; }
        public string IFSC3 { get; set; }
        public int FinalSubmitStatus { get; set; }
        public string FinalSubmitDate { get; set; }
        public string DIST { get; set; }


    }
    public class InfrasturePerformasviewModel
    {
        public SchoolModels schlmodel { get; set; }
        public InfrasturePerformasList ipf { get; set; }
    }
    public partial class Tblifsccodes
    {
        public double ID { get; set; }
        public string BANK { get; set; }
        public string IFSC { get; set; }
    }
    public partial class InfrasturePerformas
    {
        [Key]
        public int ID { get; set; }
        public string SCHL { get; set; }
        public string Bank1 { get; set; }
        public string BankBranch1 { get; set; }
        public string IFSC1 { get; set; }
        public string Bank2 { get; set; }
        public string BankBranch2 { get; set; }
        public string IFSC2 { get; set; }
        public string Bank3 { get; set; }
        public string BankBranch3 { get; set; }
        public string IFSC3 { get; set; }

        public string Bank { get; set; }
        public string BankAddress { get; set; }
        public string IFSC { get; set; }
        public string SOLID { get; set; }
        public string Col1 { get; set; }
        public string Col2 { get; set; }
        public string Col3 { get; set; }
        public string Col4 { get; set; }
        public string Col5 { get; set; }
        public string Col6 { get; set; }
        public string Col7 { get; set; }
        public string Col8 { get; set; }
        public string Col9 { get; set; }
        public string Col10 { get; set; }
        public string Col11 { get; set; }
        public string Col12 { get; set; }
        public string Col13 { get; set; }
        public string Col14 { get; set; }
        public string Col15 { get; set; }
        public string Col16 { get; set; }
        public string Col17 { get; set; }
        public string Col18 { get; set; }
        public string Col19 { get; set; }
        public string Col20 { get; set; }
        public string Col21 { get; set; }
        public string Col22 { get; set; }
        public string Col23 { get; set; }
        public string Col24 { get; set; }
        public string Col25A { get; set; }
        public string Col25B { get; set; }
        public string Col25C { get; set; }
        public string Col29 { get; set; }
        public string Col30 { get; set; }
        public int FinalSubmitStatus { get; set; }
        public string FinalSubmitDate { get; set; }
    }
}