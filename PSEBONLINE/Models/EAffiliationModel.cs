using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSEBONLINE.Models
{

    public class EAffiliationExamGroupApplyView
    {
        public string APPNO { get; set; }      
        public string SCHL { get; set; }
        public string NEW_PRIMARY { get; set; }
        public string NEW_MIDDLE { get; set; }
        public string NEW_MATRIC { get; set; }
        public string NEW_HUM { get; set; }
        public string NEW_COMM { get; set; }
        public string NEW_SCI { get; set; }
    }
    public class EAffiliationSchoolMaster
    {    
        
        public string AppType { get; set; }
        public string AppNo { get; set; }
        public string SchlCat { get; set; }
        public string SCHL { get; set; }
        public string SCHLE { get; set; }
        public string SCHLP { get; set; }
        public string STATIONE { get; set; }
        public string STATIONP { get; set; }

        public string REMARKS { get; set; }

    }

    public class EAffiliationClassMasters
    {
        [Key]
        public int Id { get; set; }
        public string ClassValue { get; set; }
        public string ClassText { get; set; }
        public DateTime? LastDate { get; set; }
        public bool IsActive { get; set; }

    }
    public class EAffiliationApplicationStatusMaster
    {
        [Key]
        public int eApplicationStatusID { get; set; }
        public string eApplicationStatusName { get; set; }
        public bool IsActive { get; set; }

        public string eApplicationBranch { get; set; }

    }

    public class EAffiliationDashBoardViewModel
    {
        public List<AffObjectionLettersViews> affObjectionLettersViewList { get; set; }
        public string APPNO { get; set; }
        public string SCHLNAME { get; set; }
        public string CREATEDDATE { get; set; }
        public string FeePaidStatus { get; set; }
        public string ForwardedToPSEB { get; set; }
        public string ForwardedToInspection { get; set; }
        public string Objection { get; set; }
        public string ObjectionLetter { get; set; }
        public string FormUnlocked { get; set; }
        public string ForwardForApproval { get; set; }
        public string ApprovedStatus { get; set; }
        public string appliedForClass { get; set; }
        public string eaffCategory { get; set; }
        public string EAffSession { get; set; }

    
    }


    public class EAffiliationFee
    {
        public DataSet PaymentFormData { get; set; }
        public int Class { get; set; }
        public string FORM { get; set; }
        public string sDate { get; set; }
        public string eDate { get; set; }
        public int Fee { get; set; }
        public int LateFee { get; set; }
        public int TotFee { get; set; }
        public int FeeCode { get; set; }
        public string FeeCat { get; set; }
        public string BankLastDate { get; set; }
        public string Type { get; set; }
        public int IsActive { get; set; }
        public int ID { get; set; }
        public string AllowBanks { get; set; }
        public string RP { get; set; }
    }

    public class EAffiliationModel
    {
        public string EmpUserId { get; set; }
        public string IsCCTV { get; set; }
        public string EAffSession { get; set; }
        public string EAffClass { get; set; }

        public int IsCancelEAffiliation { get; set; }
        public string CancelEAffiliationRemarks { get; set; }
        public DataSet dataSetPrevious { get; set; }
        public DataSet StoreAllData { get; set; }

        [Required]
        public string EAffType { get; set; }
        public int ID { get; set; }
        public string APPNO { get; set; }

        [Required(ErrorMessage = "Required")]
        public string SCHLNAME { get; set; }

       
        public string SCHLMOBILE { get; set; }

        [Required(ErrorMessage = "Required")]
        [Remote("IsSchlEmailExists", "EAffiliation", ErrorMessage = "Duplicate EMAIL ID")]
        public string SCHLEMAIL { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string PWD { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Remote("IsPasswordSame", "EAffiliation", AdditionalFields = "PWD", ErrorMessage = "Both Password Are Not Matched.")]
       
        public string RepeatPassword { get; set; }
        //TESTED
        //[Remote("IsSchlExists", "EAffiliation",  ErrorMessage = "Invalid School Code.")]

        public string SCHL { get; set; }
        public string DIST { get; set; }
        public string UDISECODE { get; set; }
        public string SCHLNME { get; set; }
        public string SCHLNMP { get; set; }
        public string STATIONE { get; set; }
        public string STATIONP { get; set; }
        public string ADDRESSE { get; set; }
        public string ADDRESSP { get; set; }
        public string DISTNME { get; set; }
        public string PINCODE { get; set; }
        public string TehsilCode { get; set; }
        public string PostOfficeCode { get; set; }
        public string EducationType { get; set; }
        public string Area { get; set; }
        public string PrincipalName { get; set; }
        public string Qualification { get; set; }
        public string DOJ { get; set; }
        public string OtherContactPerson { get; set; }
        public string DOB { get; set; }
        public string Experience { get; set; }
        public string StdCode { get; set; }
        public string PHONE { get; set; }

        public string PrincipalMobileNo { get; set; }
        public string SocietyName { get; set; }
        public string SocietyRegNo { get; set; }
        public string SocietyRegDate { get; set; }//datetime
        public string SocietyNOM { get; set; }
        public string SocietyChairmanName { get; set; }
        public string SocietyChairmanMobile { get; set; }
        public string SocietyFile { get; set; }

        public string BSFROM { get; set; }
        public string BSTO { get; set; }
        public string BSIA { get; set; }
        public string BSMEMO { get; set; }
        public string BSIDATE { get; set; }//datetime
        public string BSFILE { get; set; }
        public string FSFROM { get; set; }
        public string FSTO { get; set; }
        public string FSIA { get; set; }
        public string FSMEMO { get; set; }
        public string FSIDATE { get; set; }//datetime
        public string FSFILE { get; set; }
        public string MAPNAME { get; set; }
        public string MAPREGNO { get; set; }
        public string MAPAUTH { get; set; }
        public string MAPMEMO { get; set; }
        public string MAPIDATE { get; set; }//datetime
        public string MAPFILE { get; set; }

        // NEW CLU Details
        public string CLUAUTH { get; set; }
        public string CLUMEMO { get; set; }
        public string CLUIDATE { get; set; }//datetime
        public string CLUFILE { get; set; }

        public int C1B { get; set; }
        public int C1G { get; set; }
        public int C1T { get; set; }
        public int C2B { get; set; }
        public int C2G { get; set; }
        public int C2T { get; set; }
        public int C3B { get; set; }
        public int C3G { get; set; }
        public int C3T { get; set; }
        public int C4B { get; set; }
        public int C4G { get; set; }
        public int C4T { get; set; }
        public int C5B { get; set; }
        public int C5G { get; set; }
        public int C5T { get; set; }
        public int C6B { get; set; }
        public int C6G { get; set; }
        public int C6T { get; set; }
        public int C7B { get; set; }
        public int C7G { get; set; }
        public int C7T { get; set; }
        public int C8B { get; set; }
        public int C8G { get; set; }
        public int C8T { get; set; }
        public int C9B { get; set; }
        public int C9G { get; set; }
        public int C9T { get; set; }
        public int C10B { get; set; }
        public int C10G { get; set; }
        public int C10T { get; set; }
        public int C11HB { get; set; }
        public int C11HG { get; set; }
        public int C11HT { get; set; }
        public int C11SB { get; set; }
        public int C11SG { get; set; }
        public int C11ST { get; set; }
        public int C11CB { get; set; }
        public int C11CG { get; set; }
        public int C11CT { get; set; }
        public int C11VB { get; set; }
        public int C11VG { get; set; }
        public int C11VT { get; set; }
        public int C12HB { get; set; }
        public int C12HG { get; set; }
        public int C12HT { get; set; }
        public int C12SB { get; set; }
        public int C12SG { get; set; }
        public int C12ST { get; set; }
        public int C12CB { get; set; }
        public int C12CG { get; set; }
        public int C12CT { get; set; }
        public int C12VB { get; set; }
        public int C12VG { get; set; }
        public int C12VT { get; set; }

        public int TOTALBOYS { get; set; }
        public int TOTALGIRLS { get; set; }
        public int TOTALSTUDENTS { get; set; }
        

        public string OSTUDYMEDIUM { get; set; }
        public string OLANDTYPE { get; set; }
        public string OTOTALAREA { get; set; }
        public string OCOVAREA { get; set; }
        public string OPLAYGROUNDSIZE { get; set; }
        public string OCOURT { get; set; }
        public string OTRANSPORT { get; set; }
        public string OSALARYEMP { get; set; }
        public string OROWATER { get; set; }
        public string OTOILET { get; set; }
        public string OPLAYGROUND { get; set; }
        public string OBOARDSPLCAND { get; set; }
        public string OCOMPLAB { get; set; }
        public string OSCILAB { get; set; }
        public string OINTERNET { get; set; }
        public string OSMARTCLS { get; set; }
        public string OFIRESAFE { get; set; }
        public string OFURNITURE { get; set; }
        public string ONOCLASSROOMS { get; set; }
        public string OActivitiesAchievements { get; set; }
        public bool ISACTIVE { get; set; }
        public DateTime CREATEDDATE { get; set; }
        public DateTime UPDATEDDATE { get; set; }
        public int UpdatedBy { get; set; }

        public string Remarks { get; set; }


        public string Islibrary { get; set; }
        public string IsPhysics { get; set; }
        public string IsChemistry { get; set; }
        public string IsBiology { get; set; }

        public string PrincipalExperienceFile { get; set; }

        public int IsFormLock { get; set; }
        public string IsFormLockOn { get; set; }
        public string IsFormLockBy { get; set; }

        public int CurrentApplicationStatus { get; set; }
        public DateTime? CurrentApplicationStatusON { get; set; }
        public string ObjectionLetter { get; set; }

        public string InspectionReport { get; set; }

    }


    public class EAffiliationPaymentDetailsModel
    {
        public DataSet StoreAllData { get; set; }
        //Rohit
        public int id { get; set; }
        public string APPNO { get; set; }
        public string refno { get; set; }
        public string cls { get; set; }
        public string exam { get; set; }
       

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

    public class EAffiliationStaffDetailsModel
    {
        public DataSet StoreAllData { get; set; }
        //Rohit
        public int eStaffId { get; set; }
        public string APPNO { get; set; }
        public string Name { get; set; }
        public string FName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string AadharNo { get; set; }
        public string Qualification { get; set; }

        public string Cadre { get; set; }
        public string Subject { get; set; }
        public string ExpYear { get; set; }

        public string ExpMonth { get; set; }
        public string MOBILENO { get; set; }
        public string Salary { get; set; }
        public string SalaryMode { get; set; }
        public int IsActive { get; set; }     
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }


        public string StaffFile { get; set; }

    }

    public class EAffiliationDocumentMaster
    {
        [Key]
        public int DocID { get; set; }
        public string DocumentName { get ; set; }
        public string DocCode { get ; set; }
        public bool IsActive { get; set; }
        public bool IsCompulsory { get; set; }

    }

    public class EAffiliationDocumentDetailsModel
    {
        public DataSet StoreAllData { get; set; }
        //Rohit
        public int eDocId { get; set; }
        public string APPNO { get; set; }

        public int DocID { get; set; }

        public string DocFile { get; set; }
        public string DocCode { get; set; }


        public int IsActive { get; set; }
        public string CreatedDate { get; set; }

        public List<EAffiliationDocumentMaster> EAffiliationDocumentMasterList { get; set; }


    }
}