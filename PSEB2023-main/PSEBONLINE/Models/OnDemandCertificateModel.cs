using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSEBONLINE.Models
{

    public class OnDemandCertificatesLoginModel
    {
        [Required]
        [Display(Name = "ROLL")]
        public string ROLL { get; set; }

        [Required]
        [Display(Name = "REGNO")]
        public string REGNO { get; set; }

        [Display(Name = "RP")]
        public string RP { get; set; }

        [Required]
        [Display(Name = "Session")]
        public string Session { get; set; }


    }
    public class OnDemandCertificatesLoginSession
    {
        public string AppliedSession { get; set; }
        public string CurrentSession { get; set; }
        public string ROLL { get; set; }
        public string REGNO { get; set; }
        public string RP { get; set; }
        public string CLASS { get; set; }
        public string YEAR { get; set; }
        public string MONTH { get; set; }

        public string NAME { get; set; }
        public string FNAME { get; set; }
        public string MNAME { get; set; }
        public string CAT { get; set; }
        public string RESULT { get; set; }

        public string RESULTDTL { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public int LoginStatus { get; set; }

        public long std_id { get; set; }
        public string Schl { get; set; }
    }


    public partial class OnDemandCertificatesIndividuals
    {
        [NotMapped]
        public string AllowBanks { get; set; }
        [NotMapped]
        public DataSet StoreAllData { get; set; }
      
    }
    public partial class OnDemandCertificatesIndividuals
    {
        [Key]
        public int OnDemandCertificatesIndividualId { get; set; }

        public long CandId { get; set; }
        public string Schl { get; set; }
        //  public string CAT { get; set; }
        public string Class { get; set; }
        public string ApplyYear { get; set; }
        public string ApplyMonth { get; set; }
        public string RP { get; set; }
        public string Roll { get; set; }
        public string RegNo { get; set; }

        public string Name { get; set; }
        public string FName { get; set; }
        public string MName { get; set; }
        public string Result { get; set; }
        public string Resultdtl { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string FilePath { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public int IsFinalSubmit { get; set; }
        public DateTime? IsFinalSubmitOn { get; set; }
        //
        public int? IsCancel { get; set; }
        public string CancelBy { get; set; }
        public string CancelRemarks { get; set; }

        public DateTime? CancelOn { get; set; }
        public string ApplySession { get; set; }

    }

    public class OnDemandCertificatesViews
    {
        [Key]
        public long DemandId { get; set; }
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
        public string RegistryNumber { get; set; }
        //

        public string Roll { set; get; }
        public string AdmDate { set; get; }
        //public string roll { set; get; }
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
    }


    public class OnDemandCertificatesAllStudentViews
    {
        [Key]
        public long DemandId { get; set; }
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

        public string RegistryNumber { get; set; }
        public int? DOWNLOADLOT { get; set; }
        public DateTime? DOWNLOADON { get; set; }
        public DateTime? RegistryON { get; set; }
        public string CertNo { get; set; }
        public string EmpUserId { get; set; }
        public string AdminId { get; set; }
        //

        public string Roll { set; get; }
        public string AdmDate { set; get; }
        
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
    }


    public class OnDemandCertificatesVerifiedStudentViews
    {
        [Key]
        public long DemandId { get; set; }
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

        public string RegistryNumber { get; set; }
        public int? DOWNLOADLOT { get; set; }
        public DateTime? DOWNLOADON { get; set; }
        public DateTime? RegistryON { get; set; }
        public string CertNo { get; set; }
        public string EmpUserId { get; set; }
        public string AdminId { get; set; }
        //

        public string Roll { set; get; }
        public string AdmDate { set; get; }
        
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
    }


    public partial class OnDemandCertificates
    {      
        [NotMapped]
        public DataSet StoreAllData { get; set; }

        [NotMapped]
        public HttpPostedFileBase file { get; set; }

    }
    public partial class OnDemandCertificates
    {
        [Key]
        public long DemandId { get; set; }
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
        /// <summary>
        /// 
        /// </summary>
        public string RegistryNumber { get; set; }
        public int? DOWNLOADLOT { get; set; }
        public DateTime? DOWNLOADON { get; set; }
        public DateTime? RegistryON { get; set; }
        public string CertNo { get; set; }
        public string EmpUserId { get; set; }
        public string AdminId { get; set; }
        public int RegistryLot { get; set; }
    }
    public class OnDemandCertificateModelList
    {
        public DataSet StoreAllData { get; set; }
        public List<OnDemandCertificateSearchModel> OnDemandCertificateSearchModel { set; get; }
    }

    public class OnDemandCertificateSearchModel
    {
        public string OnDemandCertificatesStatus { set; get; }
        public string IsHardCopyCertificate { set; get; }
        public long Std_id { set; get; }
        public string Roll { set; get; }
        public string SCHL { set; get; }
        public string AdmDate { set; get; }
        public string Class { set; get; }
        
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
        public int IsExistsInOnDemandCertificates { set; get; }
        public long DemandId { set; get; }
        public int IsChallanCancel { set; get; }
        public string SubmitBy { get; set; }
    }


    public class OnDemandCertificate_ChallanDetailsViewsModelList
    {
        public DataSet StoreAllData { get; set; }
        public List<OnDemandCertificate_ChallanDetailsViews> OnDemandCertificate_ChallanDetailsViews { set; get; }
    }
    public class OnDemandCertificate_ChallanDetailsViews
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
        public string RegistryNumber { set; get; }
        public string RegistryNumberStatus { set; get; }
    }

    public class OnDemandCertificatesVerifiedStudentCompleteDetailsViewsModelList
    {
        public DataSet StoreAllData { get; set; }
        public List<OnDemandCertificatesVerifiedStudentCompleteDetailsViews> OnDemandCertificatesVerifiedStudentCompleteDetailsViewsSearchModel { set; get; }
    }

    public class OnDemandCertificatesVerifiedStudentCompleteDetailsViews
    {
              

        public string SCHL { set; get; }
        public string Class { set; get; }
        public long Std_id { set; get; }
        public string roll { set; get; }
        public string regno { set; get; }
        public string form { set; get; }
        public string Dist { set; get; }
        public string RP { set; get; }
        public string EXAM { set; get; }
        public string Category { set; get; }
        public string name { set; get; }
        public string pname { set; get; }
        public string fname { set; get; }
        public string pfname { set; get; }
        public string mname { set; get; }
        public string pmname { set; get; }
        public string Aadhar { set; get; }
        public string phy_chal { set; get; }
        public string Caste { set; get; }
        public string Gender { set; get; }
        public string DOB { set; get; }

        [Key]
        public long DemandId { set; get; }
        public string Challanid { set; get; }
        public DateTime? Challandt { set; get; }
        public bool ChallanVerify { set; get; }
        public int? Fee { set; get; }
        public int? LateFee { set; get; }
        public int? TotalFee { set; get; }
        public int? IsPrinted { set; get; }
        
        public DateTime? PrintedOn { set; get; }
        public DateTime? SubmitOn { set; get; }
        public string RegistryNumber { set; get; }
        public DateTime? RegistryON { set; get; }
        public string CertNo { set; get; }
        public int? DOWNLOADLOT { set; get; }
        public DateTime? DOWNLOADON { set; get; }
        public string Status { set; get; }
    }


}