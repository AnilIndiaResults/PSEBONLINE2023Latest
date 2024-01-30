using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace PSEBONLINE.Models
{
    public class ExamCategoryMasters
    {

        [Key]
        public int ExamCategoryId { get; set; }
        public string ExamCategoryName { get; set; }
        public string ExamCategoryMonth { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExamCategoryLastDate { get; set; }
    }
    public class ReceiptUpdateManualModel
    {
        [Key]
        public string Schl { get; set; }
        public string challanid { get; set; }
        public string challancategory { get; set; }
        public string ReceiptUpdate { get; set; }
        public string appno { get; set; }
        public string feecode { get; set; }
        public string ReceiptScannedCopy { get; set; }
    }



    public class AdminEmployeeAPIModel
    {
      //  public string userid { get; set; }
        public string id { get; set; }
        public string Name { get; set; }
        public string FName { get; set; }
        public string Post { get; set; }
        public string Type { get; set; }
        public string employeeImg { get; set; }
        public string HouseNo { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City_Village { get; set; }
        public string Tehsil { get; set; }
        public string Pin { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string MobileUniqueId { get; set; }
        public string IsApp { get; set; }
        public string role { get; set; }
        public string pan { get; set; }
        public string dob { get; set; }
        public string dor { get; set; }
        public string doj { get; set; }
        public string Bcode { get; set; }
        public string postcode { get; set; }
        public string BranchName { get; set; }
        public string Status { get; set; }
        public string remarks { get; set; }
        public string AttVerifier { get; set; }
        public string LastUpdateDate { get; set; }
        public string LastUpdatedBy { get; set; }
    }

    public class AdminEmployeeMasters
    {
        [Key]
        public string Userid { get; set; }
        public string id { get; set; }
        public string Name { get; set; }
        public string FName { get; set; }
        public string pwd { get; set; }
        public string Post { get; set; }
        public string Type { get; set; }
        public string employeeImg { get; set; }
        public string HouseNo { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City_Village { get; set; }
        public string Tehsil { get; set; }
        public string Pin { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string MobileUniqueId { get; set; }
        public string IsApp { get; set; }
        public string role { get; set; }
        public string pan { get; set; }
        public string dob { get; set; }
        public string dor { get; set; }
        public string doj { get; set; }
        public string MenuHeads { get; set; }
        public string SubMenus { get; set; }
        public string Actions { get; set; }
        public string transPassword { get; set; }
        public string Bcode { get; set; }
        public string postcode { get; set; }
        public string BranchName { get; set; }
        public string Status { get; set; }
        public string remarks { get; set; }
        public string AttVerifier { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdatedBy { get; set; }
    }


    public class AffObjectionLettersViewModel
    {
        public HttpPostedFileBase ObjectionFile { get; set; }
        public string[] selObjCode { get; set; }
        public List<AffObjectionListMasters> affObjectionListMastersList { get; set; }
        public AffObjectionLetters AffObjectionLetters { get; set; }
        public DataSet StoreAllData { get; set; }
    }
    public class AffObjectionListMasters
    {
        [Key]
        public int ID { get; set; }
        public string objcode { get; set; }
        public string objection { get; set; }
        public int IsActive { get; set; }
    }
    public class AffObjectionLettersResponseModel
    {
        [Key]
        public int OLID { get; set; }
        public string AppType { get; set; }
        public string AppNo { get; set; }
        public string ObjCode { get; set; }
        public string ObjectionSchoolReply { get; set; }
        public string ObjectionAttachment { get; set; }   
    }


    public class AffObjectionLetters
    {
        [Key]
        public int OLID { get; set; }
        public string AppType { get; set; }
        public string AppNo { get; set; }
        public string ObjCode { get; set; }
        public string Objection { get; set; }
        public string ObjectionFile { get; set; }
        public DateTime? ObjDate { get; set; }
        public string ObjStatus { get; set; }
        public string SubmitBy { get; set; }
        public DateTime? ClearDate { get; set; }
        public string Attachment { get; set; }
        public string SchoolReply { get; set; }
        public DateTime? SchoolReplyOn { get; set; }

        public string ApprovalStatus { get; set; }
        public string ApprovalRemarks { get; set; }
        public DateTime? ApprovalOn { get; set; }
        public string ApprovalBy { get; set; }
        public string ApprovalIP { get; set; }
        public string EmpUserId { get; set; }
    }

    public class AffObjectionLettersViews
    {
        [Key]
        public int OLID { get; set; }
        public string AppType { get; set; }
        public string AppNo { get; set; }
        public string ObjCode { get; set; }
        public string Objection { get; set; }
        public string ObjectionFile { get; set; }
        public DateTime? ObjDate { get; set; }
        public string ObjStatus { get; set; }
        public string SubmitBy { get; set; }
        public DateTime? ClearDate { get; set; }
        public string Attachment { get; set; }
        public string SchoolReply { get; set; }
        public DateTime? SchoolReplyOn { get; set; }
        public string ObjName { get; set; }


        public string ApprovalStatus { get; set; }
        public DateTime? ApprovalOn { get; set; }
        public string ApprovalBy { get; set; }
        public string ApprovalIP { get; set; }
        public string ApprovalRemarks { get; set; }
    }

    public class tblOtherBoardDocumentsByAdminUsers
    {
        [Key]
        public int Did { get; set; }
        public long Stdid { get; set; }
        public string Filepath { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public DateTime? SubmitOn { get; set; }
        public string SubmitBy { get; set; }
        public string EmpUserId { get; set; }

    }
    public class AdminLoginSession
    {
        public string CurrentSession { get; set; }
        public int AdminId { get; set; }
        public string USER { get; set; }
        public string AdminType { get; set; }
        public string USERNAME { get; set; }
        public string PAccessRight { get; set; }
        public string Dist_Allow { get; set; }
        public string ActionRight { get; set; }
        public int LoginStatus { get; set; }
        public string RoleType { get; set; }
        public string ClassAssign { get; set; }

        //Employee Details
        public string AdminEmployeeUserId { get; set; }
        public string AdminEmployeePassword { get; set; }
        public string AdminEmployeeName { get; set; }
        public string AdminEmployeePost { get; set; }

    }
    public class CircularModels
    {       
        public HttpPostedFileBase file { get; set; }
        public DataSet StoreAllData { get; set; }
        public int? Type { get; set; }
        public int? ID { get; set; }
        public string CircularNo { get; set; }
        public string Session { get; set; }
        public string Title { get; set; }
        public string Attachment { get; set; }
        public string UrlLink { get; set; }
        public string Category { get; set; }
        public string UploadDate { get; set; }
        public string ExpiryDate { get; set; }
        public int? IsMarque { get; set; }
        public int IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public List<CircularTypeMaster> CircularTypeMasterList { get; set; }
        public string SelectedCircularTypes { get; set; }
        public string CircularRemarks { get; set; }

    }

    public class CircularTypeMaster
    {
        public int Id { get; set; }
        public string CircularType { get; set; }
        public bool IsSelected { get; set; }
    }

    public class DuplicateCertificate
    {
        // Result
        public string Result { get; set; }
        public string ResultDt { get; set; }
        public string MaxMarks { get; set; }
        public string ObtMarks { get; set; }
        public string DOB { get; set; }
        public string REGNO { get; set; }
        //

        public DataSet StoreAllData { get; set; }
        public int Adminid { get; set; }
        public int id { get; set; }
        public int DNo { get; set; }
        public string DairyNo { get; set; }
        public DateTime? DairyDate { get; set; }
        public int Class { get; set; }
        public string Session { get; set; }
        public string Roll { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Dist { get; set; }
        public string CertNo { get; set; }
        public DateTime? CertDate { get; set; }
        public string DispNo { get; set; }
        public DateTime? DispDate { get; set; }
        public string Remarks { get; set; }
        public string Address { get; set; }
        public int Year { get; set; }
        public string IsSameAsRecord { get; set; }
        public string TypeOf { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public string FNAME { get; set; }
        public string MNAME { get; set; }
        public string ReceiptNo { get; set; }
        public string ReceiptDate { get; set; }
        public string ObjectionLetter { get; set; }
        public int FeeAmount { get; set; }
        public string ScanFile { get; set; }
        public HttpPostedFileBase file { get; set; }
        public int IsForward { get; set; }
        public List<SameAsRecord> SameAsRecordList { get; set; }
        public DataTable SameAsRecordDT { get; set; }
        public string UserDist { get; set; }
        public int IsType { get; set; }
        public string PrevCert { get; set; }

        public string OrderBy { get; set; }
        public string OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
    }

    public class SameAsRecord
    {
        public string TypeOf { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
        public bool IsSelected { get; set; }
    }

    public class SiteMenu
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public int ParentMenuID { get; set; }
        public int IsMenu { get; set; }
        public bool IsSelected { get; set; }
        public DataSet StoreAllData;
    }

    public class FileDetail
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
    }

    public class InboxModel
    {
        public IList<SelectListItem> AdminList { get; set; }       
        public DataSet StoreAllData;
        public int id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachments { get; set; }
        public HttpPostedFileBase[] files { get; set; }
        public FileDetail fileDetails { get; set; }
        public List<String> CommaStringToList { get; set; }

        public string Reply { get; set; }
        public HttpPostedFileBase[] Replyfiles { get; set; }
        public List<MailReplyMaster> MailReplyMasterToList { get; set; }
        public List<String> ReplyfilesToList { get; set; }
    }

    public class MailReplyMaster
    {
        public int Rid { get; set; }
        public int MId { get; set; }
        public string Reply { get; set; }
        public string ReplyFile { get; set; }
        public int ReplyBy { get; set; }
        public int ReplyTo { get; set; }
        public string ReplyOn { get; set; }
        public int IsReplyRead { get; set; }
        public string ReplyReadOn { get; set; }        
    }

    public class AdminUserModel 
    {
        public string Set_Allow { get; set; }
        public IList<SelectListItem> SetList { get; set; }
        public IList<SelectListItem>AdminList { get; set; }
        public IList<SelectListItem> DistList { get; set; }
        public List<SiteMenu> SiteMenuModel { get; set; }
        public DataSet StoreAllData;
        public int id { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public string PAccessRight { get; set; }
        public string Usertype { get; set; }
        public DateTime CreatedDt { get; set; }
        public string Dist_Allow { get; set; }    
        public string LastActivityDate { get; set; }
        public string User_fullnm { get; set; }
        public string Designation { get; set; }
        public string Branch { get; set; }
        public string Mobno { get; set; }
        public string Remarks { get; set; }
        public int STATUS { get; set; }
        public string EmailID { get; set; }
        public string SAccessRight { get; set; }
        public string ActionRight { get; set; }
        public int utype { get; set; }
        public IList<string> listOfActionRight { get; set; }
    }

   
    public class AdminModels
    {

        public string exammonth { get; set; }
        public string name { get; set; }
        public string set { get; set; }
        public string requestID { get; set; }
        public bool IsLateAdm { get; set; }
        public string Admission_Date { get; set; }
        public string CertNo { get; set; }
        public string CertDate { get; set; }
        public string Remarks { get; set; }

        public string SelYear { get; set; }
        public string SelExamDist { get; set; }
        public string ROLLexam { get; set; }
        public string ERRcode { get; set; }
        public HttpPostedFileBase file { get; set; }

        public string CorrectionFromDate { get; set; }
        public string CorrectionToDate { get; set; }
        public string CorrectionRecieptNo { get; set; }
        public string CorrectionRecieptDate { get; set; }
        public string CorrectionNoCapproved { get; set; }
        public string CorrectionAmount { get; set; }

        public string CorrectionUserId { get; set; }
        public string CorrectionOldPwd { get; set; }
        public string CorrectionNewPwd { get; set; }

        public string CorrectionType { get; set; }
        public string CorrectionLot { get; set; }
        public string REGNO { get; set; }
        public string TotalSearchString { get; set; }
        public DataSet StoreAllData;
        public int? ID { get; set; }
        public string SchlCode { get; set; }
        public string Candi_Name { get; set; }
        public string Father_Name { get; set; }
        public string Mother_Name { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string TotalMarks { get; set; }
        public string ObtainedMarks { get; set; }
        public string Result { get; set; }
        public string totMark { get; set; }
        public string reclock { get; set; }
        public string UPTREMARKS { get; set; }
        public string SearchResult { get; set; }
        public string FormName { get; set; }
        public string SdtID { get; set; }
        public string EXAM { get; set; }
        #region Challan Master

        public string ChallanID { get; set; }
        public string SchlReffAppRll { get; set; }
        public string Challan_Date { get; set; }
        public string Challan_V_Date { get; set; }
        public string FeeType { get; set; }
        public string BankName { get; set; }
        public string Fee { get; set; }
        public string Journal_No { get; set; }
        public string Branch { get; set; }
        public string Challan_Depst_Date { get; set; }
        public string Challan_Dwld_Stats { get; set; }
        public string Challan_Verify_Stats { get; set; }
        #endregion Challan Master
    }

    public class FeeModels
    {
        public HttpPostedFileBase file { get; set; }
        public DataSet StoreAllData { get; set; }
        public string AllowBanks { get; set; }
        public string RP { get; set; }
        public int? ID { get; set; }       
        public string Type { get; set; }
        public string FORM { get; set; }
        public string FeeCat { get; set; }
        public int? Class { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string BankLastDate { get; set; }
        public int? Fee { get; set; }
        public int? LateFee { get; set; }
        public int? FeeCode { get; set; }
        public int IsActive { get; set; }
    }


    public class FinalResultModels
    {
        public DataSet StoreAllData { get; set; }
        public string ROLL { get; set; }
        public string CENT { get; set; }
        public string REGNO { get; set; }    
        public int? ID { get; set; }
        public string Schl { get; set; }
        public string SchoolName { get; set; }
        public string Candi_Name { get; set; }
        public string Father_Name { get; set; }
        public string Mother_Name { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string TotalMarks { get; set; }
        public string ObtainedMarks { get; set; }
        public string Result { get; set; }
        public string Category { get; set; }
        public string reclock { get; set; }
        public string SearchResult { get; set; }
        public string FormName { get; set; }
        public string StdId { get; set; }
        public string EXAM { get; set; }
        public string SET { get; set; }
        public string PHONE { get; set; }
        public string MOBILE { get; set; }
        public string DIST { get; set; }
        public string DISTNM { get; set; }
    }
 
}