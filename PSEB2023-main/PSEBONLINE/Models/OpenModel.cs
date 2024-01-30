using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;

namespace PSEBONLINE.Models
{

    public class OpenUserLogin
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public long APPNO { get; set; }
        [DefaultValue("")]
        public string CLASS { get; set; }
        [DefaultValue("")]
        public string FORM { get; set; }
        [DefaultValue("")]
        public string REGDATE { get; set; }
        [DefaultValue("")]
        public string DIST { get; set; }
        [DefaultValue("")]
        public string SCHL { get; set; }
        [DefaultValue("")]
        [Required]
        [RegularExpression(@"^[a-z,A-Z,' ']{3,50}$", ErrorMessage = "Please Use letters only")]
        public string NAME { get; set; }
        [DefaultValue("")]
        public string PNAME { get; set; }
        [DefaultValue("")]
        [Required]
        [RegularExpression(@"^[6-9][0-9]{9}$", ErrorMessage = "Please enter valid mobile number")]
        public string MOBILENO { get; set; }
        [DefaultValue(null)]
        [Required]
        public string DOB { get; set; }
        [DefaultValue("")]
        public string DISTNME { get; set; }
        [DefaultValue("")]
        public string SCHOOLE { get; set; }        
       
        [DefaultValue("")]
        [Required]
        [Display(Name = "Email Id")]
        [DataType(DataType.EmailAddress)]
        //  [Remote("IsSchlEmailExists", "EAffiliation", ErrorMessage = "Duplicate EMAIL ID")]

        public string EMAILID { get; set; }



        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [DefaultValue("")]
        public string PWD { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Remote("IsPasswordSame", "Open", AdditionalFields = "PWD", ErrorMessage = "Both Password Are Not Matched.")]

        public string RepeatPassword { get; set; }



        [DefaultValue(0)]
        [Range(0,1)]
        public int ISSTEP1 { get; set; }
        [DefaultValue(0)]
        [Range(0, 1)]
        public int ISSTEP2 { get; set; }
        [DefaultValue(0)]
        [Range(0, 1)]
        public int ISCOMPLETE { get; set; }
        [DefaultValue(0)]
        [Range(0, 1)]
        public int CHALLANFLA { get; set; }
        [DefaultValue(null)]
        public DateTime CHALLANDT { get; set; }
        [DefaultValue(null)]
        public DateTime ISSTEP1DT { get; set; }
        [DefaultValue(null)]
        public DateTime ISSTEP2DT { get; set; }
        [DefaultValue(null)]
        public DateTime INSERTDT { get; set; }
        [DefaultValue(null)]
        public DateTime UPDT { get; set; }
        [DefaultValue("")]
        public string STREAM { get; set; }
        [DefaultValue("")]
        public string STREAMCODE { get; set; }
        [DefaultValue("")]
        public string IMG_RAND { get; set; }
        [DefaultValue("")]
        public string IMGSIGN_RA { get; set; }
        [DefaultValue("")]
        public string DOC_A_RAND { get; set; }
        [DefaultValue("")]
        public string DOC_B_RAND { get; set; }
        [DefaultValue("")]
        public string DOC_C_RAND { get; set; }
        [DefaultValue(0)]
        [Range(0, 1)]
        public int ISSTEP2B { get; set; }
        [DefaultValue(0)]
        [Range(0, 1)]
        public int ISSUBJECT { get; set; }
        [DefaultValue("")]
        [Required]
        [RegularExpression(@"^[1-9][0-9]{11}$", ErrorMessage = "Please enter valid AADHAR number")]
        public string AADHAR_NO { get; set; }
        [DefaultValue(0)]
        [Range(0, 1)]
        public int ISSCHLCHOO { get; set; }
        public float RECEIVEFLA { get; set; }
        [DefaultValue("")]
        public string RDATE { get; set; }
        [DefaultValue("")]
        public string TOKENNO { get; set; }
        public float ADMINUSER { get; set; }
        [DefaultValue("")]
        [Required]
        public string ADDRESS { get; set; }
        [DefaultValue("")]
        public string LANDMARK { get; set; }
        [DefaultValue("")]
        public string BLOCK { get; set; }
        [DefaultValue("")]
        [Required]
        public string TEHSIL { get; set; }
        [DefaultValue("")]
        [Required]
        [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "Please enter valid pincode")]
        public string PINCODE { get; set; }
        [DefaultValue("")]
        public string FLG_DIST { get; set; }
        [DefaultValue("")]
        public string REMARK { get; set; }
        [DefaultValue("")]
        public string MODIFYBY { get; set; }
        [DefaultValue("")]
        public string MODIFYDT { get; set; }
        [DefaultValue("")]
        [Required]
        public string CATEGORY { get; set; }
        [DefaultValue(0)]
        [Range(0, 1)]
        public int DOWNLOADFL { get; set; }
        [DefaultValue(null)]
        public DateTime DOWNLOADDA { get; set; }
        [DefaultValue("")]
        [Required]
        public string HOMEDIST { get; set; }
        [DefaultValue("")]
        [Required]
        public string HOMEDISTNM { get; set; }
        [DefaultValue("")]
        public string correctionid { get; set; }
        [DefaultValue(null)]
        public DateTime correction_dt { get; set; }
        [DefaultValue("")]
        public string SubjectList { get; set; }


        public int IsCancel { get; set; }
        public string CancelRemarks { get; set; }
        

    }

    public class OpenUserRegistration 
    {

        [Key]

        public float ID { get; set; }
        // By Rohit ( New in 2018-19)

        public string IsREGNO { get; set; }
        public string AppearingYear { get; set; }

        public DataSet StoreAllData { get; set; }
        public string Month { get; set; }
        //public string Year { get; set; }
        public string STREAM { get; set; }
        [DefaultValue("")]
        public string CATEGORY { get; set; }
        //[DefaultValue(0)]
        //[Range(0, 1)]
       
        [Required]
        [RegularExpression(@"^[1-9][0-9]{11}$", ErrorMessage = "Please enter valid AADHAR number")]
        public string AADHAR_NO { get; set; }
        [Required]
        public string APPNO{get; set;}
        public string YEAR{get; set;}
        public string SET{get; set;}
        public string CLASS{get; set;}
        public string FORM{get; set;}
        public string DIST{get; set;}
        public string RP{get; set;}
        public string EXAM{get; set;}
        public string SCHL{get; set;}
        public string REGNO{get; set; }
        [Required]
        [RegularExpression(@"^[a-z,A-Z,' ']{3,50}$", ErrorMessage = "Please Use letters only")]
        public string NAME{get; set;}
        public string PNAME{get; set; }
        [Required]
        [RegularExpression(@"^[a-z,A-Z,' ']{3,50}$", ErrorMessage = "Please Use letters only")]
        public string FNAME{get; set;}
        public string PFNAME{get; set; }
        [Required]
        [RegularExpression(@"^[a-z,A-Z,' ']{3,50}$", ErrorMessage = "Please Use letters only")]
        public string MNAME{get; set;}
        public string PMNAME{get; set;}
        public string DOB{get; set;}
        public string PHY_CHAL{get; set; }
        public int DisabilityPercent { get; set; }
        [Required]
        public string SEX{get; set;}
        public string CASTE{get; set;}
        public string RELIGION{get; set;}
        public string NATION{get; set; }
        [Required]
        public string CAT{get; set;}
        public string BOARD{get; set;}
        public string OROLL{get; set;}
        public string OSESSION{get; set;}
        public string OSCHOOL{get; set;}
        public string SCHOOLE{get; set;}
        public DateTime INSERTDT{get; set;}
        public DateTime UPDT{get; set;}
        public string SCHL2{get; set;}
        public string SCHL3{get; set;}
        public string SCHL1{get; set;}
        public DateTime SCHLUPD_DT{get; set;}
        public string FLG_DIST{get; set;}
        public float PRINTLOT{get; set;}
        public float PRINTSTATU{get; set;}
        public float FEE_EXMPT{get; set;}
        public string SUBJ{get; set;}
        public string FORMNO{get; set;}
        public string TEMPREGNO{get; set;}
        public string REGNO1{get; set;}
        public string REGNOOLD{get; set;}
        public int emr17flag{get; set;}
        public string CandStudyMedium{get; set;}
        public string correctionid{get; set;}
        public DateTime correction_dt{get; set;}

        public string IsSmartPhone { get; set; }
        public string IsHardCopyCertificate { get; set; }
    }

    public class OpenUserSubjects
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string BOARD { get; set; }
        public string CATEGORY { get; set; }
        public string MONTH { get; set; }
        public string YEAR { get; set; }


        [Required]
        public string APPNO { get; set; }
        public string CLASS { get; set; }
        public string SCHL { get; set; }
        [Required]
        public string SUB { get; set; }
        public string SUBNM { get; set; }
        public string SUBABBR { get; set; }
        public string MEDIUM { get; set; }
        public string SUBCAT { get; set; }
        public string OBTMARKS { get; set; }
        public string MINMARKS { get; set; }
        public string MAXMARKS { get; set; }
        public string OBTMARKSP { get; set; }
        public string MINMARKSP { get; set; }
        public string MAXMARKSP { get; set; }
        public string STREAM { get; set; }
        public string STREAMCODE { get; set; }
        public string GROUP { get; set; }
        public string GROUPCODE { get; set; }
        public string TRADE { get; set; }
        public string TRADECODE { get; set; }
        public DateTime INSERTDT { get; set; }
        public DateTime UPDT { get; set; }
        public float SUB_SEQ { get; set; }
        public string OBTMARKSCC { get; set; }
        public string MINMARKSCC { get; set; }
        public string MAXMARKSCC { get; set; }
        public string correctionid { get; set; }
        public DateTime correction_dt { get; set; }
    }

    public class FeeOpen
    {
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        // Exam Fee
        public string ExamStartDate { get; set; }
        public string ExamEndDate { get; set; }
        public DateTime ExamBankLastDate { get; set; }
        public int ExamRegFee { get; set; }
        public int ExamLateFee { get; set; }
        public int ExamTotalFee { get; set; }
        public int ExamAddSubFee { get; set; }
        public int ExamPrSubFee { get; set; }
        public int ExamNOAS { get; set; }
        public int ExamNOPS { get; set; }
        //
        public string BankCode { get; set; }
        public string TotalFeesInWords { get; set; }        
        public DataSet StoreAllData { get; set; }
        public string FeeCode { get; set; }
        public string FeeCat { get; set; }
        public string AppNo { get; set; }
        public string SCHL { get; set; }

      
        public string FORM { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public DateTime BankLastDate { get; set; }
        public int RegFee { get; set; }
        public int LateFee { get; set; }
        public int ProsFee { get; set; }
        public int RegConti { get; set; }
        public string RegContiCat { get; set; }
        public int AdmissionFee { get; set; }
        public string FeeCategory { get; set; }
        public int AddSubFee { get; set; }
        public int ADDFEE { get; set; }        
        public int NoAddSub { get; set; }
        public int TotalFee { get; set; }
        public int HardCopyCertificateFee { get; set; }
    }



    public class tblsubjectopen
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string APPNO { get; set; }            
        [Required]
        public string SUB { get; set; }       
        public string MEDIUM { get; set; }
        public string SUBCAT { get; set; }
        public float SUB_SEQ { get; set; }
        public string OBTMARKS { get; set; }
        public string OBTMARKSP { get; set; }
        public string OBTMARKSCC { get; set; }        
        public DateTime INSERTDT { get; set; }
        public DateTime UPDT { get; set; }
        public string correctionid { get; set; }
        public DateTime correction_dt { get; set; }

    }

    public class OpenUserRegistrationViewModel 
    {
        public OpenUserLogin openUserLogin { get; set; }
        public OpenUserRegistration openUserRegistration { get; set; }
    }




}