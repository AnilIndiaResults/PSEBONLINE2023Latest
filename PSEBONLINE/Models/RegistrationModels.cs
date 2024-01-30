using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data;

namespace PSEBONLINE.Models
{

    public class RegnoErrorSummaryDownloadDataViews
    {
        public string Cls { get; set; }
        public string RP { get; set; }
        public string Form { get; set; }
        public string SET { get; set; }
        public string SCHL { get; set; }
        [Key]
        public string Std_id { get; set; }
        public string RegNo { get; set; }
        public string NAME { get; set; }
        public string FNAME { get; set; }
        public string MNAME { get; set; }
        public string SCHLNME { get; set; }
        public string DISTNM { get; set; }
        public string Mobile { get; set; }
        public string RegNoSatus { get; set; }
    }

    public class RegPhotoFromOldPathToNewPathViews
    {
        public string schl { set; get; }

        [Key]
        public long std_id { set; get; }
        public string std_Photo { set; get; }
        public string Old_std_Photo { set; get; }
        public string Old_std_Sign { set; get; }
        public string std_Sign { set; get; }
        public string form_name { set; get; }
        public string oldyear { set; get; }
        public string StudentUniqueIdNEW { set; get; }
        public string SCHLDIST { set; get; }
    }
    
    
    public class SeniorStudentMatricResultMarksViewsModelList
    {
        public DataSet StoreAllData { get; set; }
        public List<SeniorStudentMatricResultMarksViews> RegistrationSearchModel { get; set; }
    }
    #region SeniorStudentMatricResultMarksOPEN

    public class SeniorStudentMatricResultMarksOpenViews
    {
        [Key]
        public int Std_id { set; get; }
        public string Roll { set; get; }
        public string form_Name { set; get; }
        public string schl { set; get; }
        public string Admission_Date { set; get; }
        public string Candi_Name { set; get; }
        public string Father_Name { set; get; }
        public string Mother_Name { set; get; }
        public string DOB { set; get; }
        public int LOT { set; get; }
        public string aadhar_num { set; get; }
        public DateTime? CreatedDate { set; get; }
        public DateTime? UPDT { set; get; }
        public string SubjectList { set; get; }
        //
        public string REGNO { set; get; }
        public string StudentUniqueId { set; get; }
        public string schlDist { set; get; }
        //
        public long MRID { get; set; }
        //
        public string MAT_ROLL { get; set; }
        public string MAT_BOARD { get; set; }
        public string MAT_MONTH { get; set; }
        public string MAT_YEAR { get; set; }
        //
        public string SUB1 { get; set; }
        public string TOT1 { get; set; }
        public string MAX1 { get; set; }
        public string MIN1 { get; set; }
        public string SUB2 { get; set; }
        public string TOT2 { get; set; }
        public string MAX2 { get; set; }
        public string MIN2 { get; set; }
        public string SUB3 { get; set; }
        public string TOT3 { get; set; }
        public string MAX3 { get; set; }
        public string MIN3 { get; set; }
        public string SUB4 { get; set; }
        public string TOT4 { get; set; }
        public string MAX4 { get; set; }
        public string MIN4 { get; set; }
        public string SUB5 { get; set; }
        public string TOT5 { get; set; }
        public string MAX5 { get; set; }
        public string MIN5 { get; set; }
        public string SUB6 { get; set; }
        public string TOT6 { get; set; }
        public string MAX6 { get; set; }
        public string MIN6 { get; set; }
        public string SUB7 { get; set; }
        public string TOT7 { get; set; }
        public string MAX7 { get; set; }
        public string MIN7 { get; set; }
        public string SUB8 { get; set; }
        public string TOT8 { get; set; }
        public string MAX8 { get; set; }
        public string MIN8 { get; set; }
        public string SUB9 { get; set; }
        public string TOT9 { get; set; }
        public string MAX9 { get; set; }
        public string MIN9 { get; set; }
        public string MR_TOTAL { get; set; }
        public string MR_TOTMAX { get; set; }
        public string MR_RESULT { get; set; }
        public string ChangeStatus { get; set; }
        public string FilePath { get; set; }
        //
        public bool IsActive { get; set; }
        public DateTime SubmitOn { get; set; }
        public DateTime? ModifyOn { get; set; }
        public bool? IsFinalLock { get; set; }
        public DateTime? FinalSubmitOn { get; set; }
        public DateTime? CancelOn { get; set; }


        public string SUBNM1 { get; set; }
        public string SUBNM2 { get; set; }
        public string SUBNM3 { get; set; }
        public string SUBNM4 { get; set; }
        public string SUBNM5 { get; set; }
        public string SUBNM6 { get; set; }
        public string SUBNM7 { get; set; }
        public string SUBNM8 { get; set; }
        public string ChangeStatusNM { get; set; }
    }

    public class SeniorStudentMatricResultMarksOpens
    {
        [Key]
        public long MRID { get; set; }
        public long Std_Id { get; set; }
        public string SCHL { get; set; }
        //
        public string MAT_ROLL { get; set; }
        public string MAT_BOARD { get; set; }
        public string MAT_MONTH { get; set; }
        public string MAT_YEAR { get; set; }
        //
        public string SUB1 { get; set; }
        public string TOT1 { get; set; }
        public string MAX1 { get; set; }
        public string MIN1 { get; set; }
        public string SUB2 { get; set; }
        public string TOT2 { get; set; }
        public string MAX2 { get; set; }
        public string MIN2 { get; set; }
        public string SUB3 { get; set; }
        public string TOT3 { get; set; }
        public string MAX3 { get; set; }
        public string MIN3 { get; set; }
        public string SUB4 { get; set; }
        public string TOT4 { get; set; }
        public string MAX4 { get; set; }
        public string MIN4 { get; set; }
        public string SUB5 { get; set; }
        public string TOT5 { get; set; }
        public string MAX5 { get; set; }
        public string MIN5 { get; set; }
        public string SUB6 { get; set; }
        public string TOT6 { get; set; }
        public string MAX6 { get; set; }
        public string MIN6 { get; set; }
        public string SUB7 { get; set; }
        public string TOT7 { get; set; }
        public string MAX7 { get; set; }
        public string MIN7 { get; set; }
        public string SUB8 { get; set; }
        public string TOT8 { get; set; }
        public string MAX8 { get; set; }
        public string MIN8 { get; set; }
        public string SUB9 { get; set; }
        public string TOT9 { get; set; }
        public string MAX9 { get; set; }
        public string MIN9 { get; set; }
        public string MR_TOTAL { get; set; }
        public string MR_TOTMAX { get; set; }
        public string MR_RESULT { get; set; }
        public string ChangeStatus { get; set; }
        public string FilePath { get; set; }
        //
        public bool IsActive { get; set; }
        public DateTime SubmitOn { get; set; }
        public DateTime? ModifyOn { get; set; }
        public bool? IsFinalLock { get; set; }
        public DateTime? FinalSubmitOn { get; set; }
        public DateTime? CancelOn { get; set; }

        public string SUBNM1 { get; set; }
        public string SUBNM2 { get; set; }
        public string SUBNM3 { get; set; }
        public string SUBNM4 { get; set; }
        public string SUBNM5 { get; set; }
        public string SUBNM6 { get; set; }
        public string SUBNM7 { get; set; }
        public string SUBNM8 { get; set; }
    }
    #endregion SeniorStudentMatricResultMarksOPEN
    #region SeniorStudentMatricResultMarks

    public class SeniorStudentMatricResultMarksViews
    {
        [Key]
        public int Std_id { set; get; }
        public string Roll { set; get; }
        public string form_Name { set; get; }
        public string schl { set; get; }
        public string Admission_Date { set; get; }
        public string Candi_Name { set; get; }
        public string Father_Name { set; get; }
        public string Mother_Name { set; get; }
        public string DOB { set; get; }
        public int LOT { set; get; }
        public string aadhar_num { set; get; }
        public DateTime? CreatedDate { set; get; }
        public DateTime? UPDT { set; get; }
        public string SubjectList { set; get; }
        //
        public string REGNO { set; get; }
        public string StudentUniqueId { set; get; }
        public string schlDist { set; get; }
        //
        public long MRID { get; set; }
        //
        public string MAT_ROLL { get; set; }
        public string MAT_BOARD { get; set; }
        public string MAT_MONTH { get; set; }
        public string MAT_YEAR { get; set; }
        //
        public string SUB1 { get; set; }
        public string TOT1 { get; set; }
        public string MAX1 { get; set; }
        public string MIN1 { get; set; }
        public string SUB2 { get; set; }
        public string TOT2 { get; set; }
        public string MAX2 { get; set; }
        public string MIN2 { get; set; }
        public string SUB3 { get; set; }
        public string TOT3 { get; set; }
        public string MAX3 { get; set; }
        public string MIN3 { get; set; }
        public string SUB4 { get; set; }
        public string TOT4 { get; set; }
        public string MAX4 { get; set; }
        public string MIN4 { get; set; }
        public string SUB5 { get; set; }
        public string TOT5 { get; set; }
        public string MAX5 { get; set; }
        public string MIN5 { get; set; }
        public string SUB6 { get; set; }
        public string TOT6 { get; set; }
        public string MAX6 { get; set; }
        public string MIN6 { get; set; }
        public string SUB7 { get; set; }
        public string TOT7 { get; set; }
        public string MAX7 { get; set; }
        public string MIN7 { get; set; }
        public string SUB8 { get; set; }
        public string TOT8 { get; set; }
        public string MAX8 { get; set; }
        public string MIN8 { get; set; }
        public string SUB9 { get; set; }
        public string TOT9 { get; set; }
        public string MAX9 { get; set; }
        public string MIN9 { get; set; }
        public string MR_TOTAL { get; set; }
        public string MR_TOTMAX { get; set; }
        public string MR_RESULT { get; set; }
        public string ChangeStatus { get; set; }
        public string FilePath { get; set; }
        //
        public bool IsActive { get; set; }
        public DateTime SubmitOn { get; set; }
        public DateTime? ModifyOn { get; set; }
        public bool? IsFinalLock { get; set; }
        public DateTime? FinalSubmitOn { get; set; }
        public DateTime? CancelOn { get; set; }


        public string SUBNM1 { get; set; }
        public string SUBNM2 { get; set; }
        public string SUBNM3 { get; set; }
        public string SUBNM4 { get; set; }
        public string SUBNM5 { get; set; }
        public string SUBNM6 { get; set; }
        public string SUBNM7 { get; set; }
        public string SUBNM8 { get; set; }
        public string ChangeStatusNM { get; set; }
    }

    public class SeniorStudentMatricResultMarks
    {
        [Key]
        public long MRID { get; set; }
        public long Std_Id { get; set; }
        public string SCHL { get; set; }
        //
        public string MAT_ROLL { get; set; }
        public string MAT_BOARD { get; set; }
        public string MAT_MONTH { get; set; }
        public string MAT_YEAR { get; set; }
        //
        public string SUB1 { get; set; }
        public string TOT1 { get; set; }
        public string MAX1 { get; set; }
        public string MIN1 { get; set; }
        public string SUB2 { get; set; }
        public string TOT2 { get; set; }
        public string MAX2 { get; set; }
        public string MIN2 { get; set; }
        public string SUB3 { get; set; }
        public string TOT3 { get; set; }
        public string MAX3 { get; set; }
        public string MIN3 { get; set; }
        public string SUB4 { get; set; }
        public string TOT4 { get; set; }
        public string MAX4 { get; set; }
        public string MIN4 { get; set; }
        public string SUB5 { get; set; }
        public string TOT5 { get; set; }
        public string MAX5 { get; set; }
        public string MIN5 { get; set; }
        public string SUB6 { get; set; }
        public string TOT6 { get; set; }
        public string MAX6 { get; set; }
        public string MIN6 { get; set; }
        public string SUB7 { get; set; }
        public string TOT7 { get; set; }
        public string MAX7 { get; set; }
        public string MIN7 { get; set; }
        public string SUB8 { get; set; }
        public string TOT8 { get; set; }
        public string MAX8 { get; set; }
        public string MIN8 { get; set; }
        public string SUB9 { get; set; }
        public string TOT9 { get; set; }
        public string MAX9 { get; set; }
        public string MIN9 { get; set; }
        public string MR_TOTAL { get; set; }
        public string MR_TOTMAX { get; set; }
        public string MR_RESULT { get; set; }
        public string ChangeStatus { get; set; }
        public string FilePath { get; set; }
        //
        public bool IsActive { get; set; }
        public DateTime SubmitOn { get; set; }
        public DateTime? ModifyOn { get; set; }
        public bool? IsFinalLock { get; set; }
        public DateTime? FinalSubmitOn { get; set; }
        public DateTime? CancelOn { get; set; }

        public string SUBNM1 { get; set; }
        public string SUBNM2 { get; set; }
        public string SUBNM3 { get; set; }
        public string SUBNM4 { get; set; }
        public string SUBNM5 { get; set; }
        public string SUBNM6 { get; set; }
        public string SUBNM7 { get; set; }
        public string SUBNM8 { get; set; }
    }
    #endregion SeniorStudentMatricResultMarks

    #region StudentPreviousYearMarks
    public class RegistrationSearchStudentPreviousYearMarksModelList
    {
        public List<SelectListItem> MyMatricBoard { get; set; }
        public List<SelectListItem> MyBoard { get; set; }
        public List<SelectListItem> MatricYearList { get; set; }
        public List<SelectListItem> YearList { get; set; }
        public List<SelectListItem> MonthList { get; set; }
        public List<SelectListItem> ResultList { get; set; }
        public DataSet StoreAllData { get; set; }
        public List<RegistrationSearchStudentPreviousYearMarksModel> RegistrationSearchModel { get; set; }
    }
    public class RegistrationSearchStudentPreviousYearMarksModel
    {
        public int Std_id { set; get; }
        public string Roll { set; get; }
        public string form_Name { set; get; }
        public string schl { set; get; }
        public string Admission_Date { set; get; }
        public string Candi_Name { set; get; }
        public string Father_Name { set; get; }
        public string Mother_Name { set; get; }
        public string DOB { set; get; }
        public int LOT { set; get; }
        public string aadhar_num { set; get; }
        public DateTime? CreatedDate { set; get; }
        public DateTime? UPDT { set; get; }
        public string SubjectList { set; get; }
        //
        public string REGNO { set; get; }
        public string ProofCertificate { set; get; }
        public string ProofNRICandidates { set; get; }
        public string StudentUniqueId { set; get; }
        public string schlDist { set; get; }
        public long PYID { get; set; }
        public int MAT_OBTMARKS { get; set; }
        public int MAT_MAXMARKS { get; set; }
        public int MAT_PERCENTAGE { get; set; }
        public string MAT_RESULT { get; set; }
        public int ELV_OBTMARKS { get; set; }
        public int ELV_MAXMARKS { get; set; }
        public int ELV_PERCENTAGE { get; set; }
        public string ELV_RESULT { get; set; }
        public bool IsActive { get; set; }
        public DateTime? SubmitOn { get; set; }
        public DateTime? ModifyOn { get; set; }
        public bool IsFinalLock { get; set; }
        public DateTime? FinalSubmitOn { get; set; }
        //
        //
        public string MAT_ROLL { get; set; }
        public string MAT_BOARD { get; set; }
        public string MAT_MONTH { get; set; }
        public string MAT_YEAR { get; set; }
        public string ELV_ROLL { get; set; }
        public string ELV_BOARD { get; set; }
        public string ELV_MONTH { get; set; }
        public string ELV_YEAR { get; set; }
    }

    public class StudentPreviousYearMarks
    {
        [Key]
        public long PYID { get; set; }
        public long Std_Id { get; set; }
        public string SCHL { get; set; }
        public int MAT_OBTMARKS { get; set; }
        public int MAT_MAXMARKS { get; set; }
        public int MAT_PERCENTAGE { get; set; }
        public string MAT_RESULT { get; set; }
        public int ELV_OBTMARKS { get; set; }
        public int ELV_MAXMARKS { get; set; }
        public int ELV_PERCENTAGE { get; set; }
        public string ELV_RESULT { get; set; }
        public bool IsActive { get; set; }
        public DateTime SubmitOn { get; set; }
        public DateTime? ModifyOn { get; set; }
        public bool IsFinalLock { get; set; }
        public DateTime? FinalSubmitOn { get; set; }
        //
        public string MAT_ROLL { get; set; }
        public string MAT_BOARD { get; set; }
        public string MAT_MONTH { get; set; }
        public string MAT_YEAR { get; set; }
        public string ELV_ROLL { get; set; }
        public string ELV_BOARD { get; set; }
        public string ELV_MONTH { get; set; }
        public string ELV_YEAR { get; set; }
    }
    #endregion StudentPreviousYearMarks

    public class RegistrationSearchModelList
    {
        public DataSet StoreAllData { get; set; }
        public List<RegistrationSearchModel> RegistrationSearchModel { set; get; }
    }

    public class RegistrationSearchModel
    {
        public int Std_id { set; get; }
        public string Roll { set; get; }
        public string form_Name { set; get; }
        public string schl { set; get; }
        public string Admission_Date { set; get; }
        public string Candi_Name { set; get; }
        public string Father_Name { set; get; }
        public string Mother_Name { set; get; }
        public string DOB { set; get; }
        public int LOT { set; get; }
        public string aadhar_num { set; get; }
        public DateTime? CreatedDate { set; get; }
        public DateTime? UPDT { set; get; }
        public string SubjectList { set; get; }
        //
        public string REGNO { set; get; }
        public string ProofCertificate { set; get; }
        public string ProofNRICandidates { set; get; }
        public string StudentUniqueId { set; get; }
        public string schlDist { set; get; }
        //NEw

        public string Exam { set; get; }
        public string Group_Name { set; get; }
        public string E_punjab_Std_id { set; get; }
        public string Caste { set; get; }
        public string Differently_Abled { set; get; }
        //
        public string Gender { set; get; }
        public string Belongs_BPL { set; get; }
        public string Religion { set; get; }



    }

    public class DocumentVerifyingEmployeeDetails
    {
        [Key]
        public int VID { get; set; }
        public int Stdid { get; set; }
        public string Form { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }

        public string RegNo { get; set; }
        public string VerifyingPerson { get; set; }     
         public string Remarks { get; set; }
        public DateTime? SubmitOn { get; set; }

    }
    public class RegistrationClassFormWiseDocumentMasters
    {
        [Key]
        public int RegDocId { get; set; }
        public int Class { get; set; }
        public string FormName { get; set; }
        public string RegDocumentName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class RegistrationModels
    {

        public string PhotoExist { get; set; } 
        public string SignExist { get; set; } 
        public string old_std_Photo { get; set; } 
        public string old_std_sign { get; set; } 
        public int OTID { get; set; } 
        public string Imptblname { get; set; } = "";
        public string IsSmartPhone { get; set; } = "";
        public string NSQFPattern { get; set; } = "";
        public string IsCorrectionInParticular { get; set; } = "";
        public string SchoolHeadEPunjabId { get; set; }


        public string IsRegNoExists { get; set; } = "";
        public string IsImportedStudent { get; set; }
        //new add in 2023-24       
        public string IsUnlockForm { get; set; }
        public bool IsNRICandidate { get; set; }


        public string ssWEL { get; set; }
        public string smWEL { get; set; }

        //
        public HttpPostedFileBase DocProofCertificate { get; set; }
        public HttpPostedFileBase DocProofNRICandidates { get; set; }

        public string ProofCertificate { get; set; }
        public string ProofNRICandidates { get; set; }
        public List<RegistrationClassFormWiseDocumentMasters> registrationClassFormWiseDocumentMastersList { get; set; }
        public string requestID { get; set; }
        public bool IsLateAdm { get; set; }
        public string NADID { set; get; }
        public string Father_MobNo { set; get; }
        public string Father_Occup { set; get; }
        public string Mother_MobNo { set; get; }
        public string Mother_Occup { set; get; }
        public string MatricResult { set; get; }
        public string oldyear { set; get; }// for Import
        public string DP { set; get; } //Disability Percentage
        public string CandId { set; get; }
        public string ExamRoll { set; get; }
		//-------\\\       
		//-----------------------Start Correction Suject Begin--------
		public Byte[] QRCode { get; set; }
		public string schlCorConDetails { set; get; }
        public string SubType { set; get; }
        public string NewStream { set; get; }
        public string Stream { set; get; }
        public string SelList { set; get; }
        public string SelClass { set; get; }
        public string OpenRegularType { get; set; }
        public string Oldsubcode { get; set; }
        public string Newsubcode { get; set; }
        public string Newsubmedium { get; set; }
        public string Correctiontype { get; set; }
        public string CorrectionLot { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string OldSub { get; set; }
        public string NewSub { get; set; }
        public string NewMedium { get; set; }
        //-------------------End Correction Subject End-----------------------

        //-----------------------Start Correction Performa Begin--------
        public string Grouplist { get; set; }
        public string CorrectionSet { get; set; }
        public string CorrectionBranch { get; set; }
        public string CorrectionBarCode { get; set; }
        public string CorrectionId { get; set; }
        public string CorrectionInsertDt { get; set; }
        public string CorrectionFinalSubmitDt { get; set; }
        public string CorrectionVerifyDt { get; set; }
        public string Class { get; set; }
        //public string CorrectionLot { get; set; }
        //public string CorrectionType { get; set; }
        public string Remark { get; set; }
        public string SelListField { get; set; }
        public string oldVal { get; set; }
        public string newVal { get; set; }
        public string newValP { get; set; }
        public string newValDOB { get; set; }
        public string schlCorNameE { get; set; }
        public string schlCorNameP { get; set; }
        public DataSet Correctiondata { get; set; }
        public DataSet Correctiondatafinal { get; set; }

        //-----------------------END Correction Performa END--------


        //----
        
        
        public string CandiMedium { get; set; }
        public string MetricBoard { get; set; }
        public string checkCategory { get; set; }
        public int IsImported { get; set; }
        public DataSet StoreAllData { get; set; }
        public bool Permanent { get; set; }
        public bool Provisional { get; set; }
        public string Metric_Roll_Num { get; set; }
        public string MetricMonth { get; set; }
        public string MetricYear { get; set; }
        public string scribeWriter { get; set; }
        public string DA { get; set; }
        public string Tgroup { get; set; }

        //-------------SSE Sience Model------
        public string coms1 { get; set; }
        public string comm1 { get; set; }
        public string coms2 { get; set; }
        public string comm2 { get; set; }
        public string coms3 { get; set; }
        public string comm3 { get; set; }
        public string coms4 { get; set; }
        public string coms4sci { get; set; }
        public string comm4 { get; set; }
        public string ss5 { get; set; }
        public string sm5 { get; set; }
        public string ss6 { get; set; }
        public string sm6 { get; set; }
        public string ss7 { get; set; }
        public string sm7 { get; set; }
        public string ss8 { get; set; }
        public string sm8 { get; set; }

        //-------------SSE Commerce Model------

        public string comcm1 { get; set; }
        public string comcs2 { get; set; }
        public string comcm2 { get; set; }
        public string comcm3 { get; set; }
        public string comcm4 { get; set; }

        public string coms4comm { get; set; }

        //public string coms3 { get; set; }
        //public string comm3 { get; set; }
        //public string coms4 { get; set; }
        //public string comm4 { get; set; }
        public string coms5 { get; set; }
        public string comm5 { get; set; }
        public string coms6 { get; set; }
        public string comm6 { get; set; }
        public string coms7 { get; set; }
        public string comm7 { get; set; }
        public string coms8 { get; set; }
        public string comm8 { get; set; }
        public string coms9 { get; set; }
        public string comm9 { get; set; }

        //-------------SSE Humanity Model------

        public string hums1 { get; set; }
        public string humm1 { get; set; }
        public string hums2 { get; set; }
        public string humm2 { get; set; }
        public string hums3 { get; set; }
        public string humm3 { get; set; }
        public string hums4 { get; set; }
        public string humm4 { get; set; }
        public string hums5 { get; set; }
        public string humm5 { get; set; }
        public string hums6 { get; set; }
        public string humm6 { get; set; }
        public string hums7 { get; set; }
        public string humm7 { get; set; }
        public string hums8 { get; set; }
        public string humm8 { get; set; }
        //-------------SSE vocational Model------
        public string groupsel { get; set; }
        public string grouptr { get; set; }
        public string TCODE { get; set; }
        public string vocs1 { get; set; }
        public string vocm1 { get; set; }
        public string vocs2 { get; set; }
        public string vocm2 { get; set; }
        public string vocs3 { get; set; }
        public string vocm3 { get; set; }
        public string vocs4 { get; set; }
        public string vocm4 { get; set; }
        public string vocs5 { get; set; }
        public string vocm5 { get; set; }
        public string vocs6 { get; set; }
        public string vocm6 { get; set; }
        public string vocs7 { get; set; }
        public string vocm7 { get; set; }
        public string vocs8 { get; set; }
        public string vocm8 { get; set; }
        public string vocs9 { get; set; }
        public string vocm9 { get; set; }
        public string vocs10 { get; set; }
        public string vocm10 { get; set; }
        public string vocs11 { get; set; }
        public string vocm11 { get; set; }
        //-------------SSE technical Model------

        public string tecs1 { get; set; }
        public string tecm1 { get; set; }
        public string tecs2 { get; set; }
        public string tecm2 { get; set; }
        public string tecs3 { get; set; }
        public string tecm3 { get; set; }
        public string tecs4 { get; set; }
        public string tecm4 { get; set; }
        public string tecs5 { get; set; }
        public string tecm5 { get; set; }
        public string tecs6 { get; set; }
        public string tecm6 { get; set; }
        public string tecs7 { get; set; }
        public string tecm7 { get; set; }
        public string tecs8 { get; set; }
        public string tecm8 { get; set; }
        //public string vocs9 { get; set; }
        //public string vocm9 { get; set; }

        //-------------SSE Agriculture Model------
        public string agrs1 { get; set; }
        public string agrm1 { get; set; }
        public string agrs2 { get; set; }
        public string agrm2 { get; set; }
        public string agrs3 { get; set; }
        public string agrm3 { get; set; }
        public string agrs4 { get; set; }
        public string agrm4 { get; set; }
        public string agrs5 { get; set; }
        public string agrm5 { get; set; }
        public string agrs6 { get; set; }
        public string agrm6 { get; set; }
        public string agrs7 { get; set; }
        public string agrm7 { get; set; }
        public string agrs8 { get; set; }
        public string agrm8 { get; set; }
        //public string vocs9 { get; set; }
        //public string vocm9 { get; set; }



        //-------------Subject Details-----------
        public int sub_id { get; set; }
        public string sub_code { get; set; }
        public string subS1 { get; set; }
        public string subm1 { get; set; }
        public string subS2 { get; set; }
        public string subM2 { get; set; }
        public string subS3 { get; set; }
        public string subm3 { get; set; }
        public string subS4 { get; set; }
        public string subM4 { get; set; }
        public string subS5 { get; set; }
        public string subM5 { get; set; }
        public string subS6 { get; set; }
        public string subM6 { get; set; }
        public string subS7 { get; set; }
        public string subM7 { get; set; }
        public string subS8 { get; set; }
        public string subM8 { get; set; }
        public string s9 { get; set; }
        public string m9 { get; set; }
        public string s10 { get; set; }
        public string s110 { get; set; }
        public string m10 { get; set; }
        public string s11 { get; set; }
        public string m11 { get; set; }
        public string s12 { get; set; }
        public string m12 { get; set; }
        public string PreNSQF { get; set; }
        public string NSQF { get; set; }
        public string NsqfsubS6Upd { get; set; }
        public string NsqfsubS6 { get; set; }
        public string ns10 { get; set; }

        public string PreNSQFsci { get; set; }
        public string NsqfsubS6sci { get; set; }

        public string PreNSQFcomm { get; set; }
        public string NsqfsubS6comm { get; set; }

        public string PreNSQFvoc { get; set; }
        public string NsqfsubS6voc { get; set; }
        //---------------Subjects For Blind-----------
        //subbS1,bm1,subbS2,bm2,subbS3,bm3,subbS4,bm4,subbS5,bm5,subbS6,bm6
        public string subbS1 { get; set; }
        public string bm1 { get; set; }
        public string subbS2 { get; set; }
        public string bm2 { get; set; }
        public string subbS3 { get; set; }
        public string bm3 { get; set; }
        public string subbS4 { get; set; }
        public string bm4 { get; set; }
        public string subbS5 { get; set; }
        public string bm5 { get; set; }
        public string subbS6 { get; set; }
        public string bm6 { get; set; }
        public string subbS7 { get; set; }
        public string bm7 { get; set; }
        public string subbS8 { get; set; }
        public string bm8 { get; set; }
        public string subbS9 { get; set; }
        public string bm9 { get; set; }
        public string subbS10 { get; set; }
        public string bm10 { get; set; }

        public string subbS11 { get; set; }
        public string bm11 { get; set; }
        //--------------N1-N2-N3-forms Registarion ------------
        // public string PhotoString { get; set; }
        public int DIST { get; set; }
        public int Std_id { get; set; }
        public bool Agree { get; set; }
        public string form_Name { get; set; }
        public string Category { get; set; }
        public string Board { get; set; }
        public string Other_Board { get; set; }
        public string Board_Roll_Num { get; set; }
        public string Prev_School_Name { get; set; }
        public string Registration_num { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string AWRegisterNo { get; set; }
        public string Admission_Num { get; set; }
        public string Admission_Date { get; set; }
        public string Class_Roll_Num_Section { get; set; }
        //-----personal Information
        public int PI_Id { get; set; }
        public string Candi_Name { get; set; }
        public string Candi_Name_P { get; set; }
        public string Father_Name { get; set; }
        public string Father_Name_P { get; set; }
        public string Mother_Name { get; set; }
        public string Mother_Name_P { get; set; }
        public string Caste { get; set; }
        public string Gender { get; set; }
        public string Differently_Abled { get; set; }
        public string Religion { get; set; }
        public string DOB { get; set; }
        public string Belongs_BPL { get; set; }
        public string Mobile { get; set; }
        public string AadharEnroll { get; set; }
        public string Aadhar_num { get; set; }
        public string E_punjab_Std_id { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string Block { get; set; }
        public int Tehsil { get; set; }
        public int District { get; set; }
        public bool IsPrevSchoolSelf { get; set; }
        public bool IsPSEBRegNum { get; set; }
        public char Section { get; set; }
        public string MYTehsil { get; set; }
        public string MyDistrict { get; set; }

        public string PinCode { get; set; }
        public HttpPostedFileBase std_Photo { get; set; }
        public HttpPostedFileBase std_Sign { get; set; }
        public HttpPostedFileBase file { get; set; }
        public HttpPostedFileBase fileM { get; set; }
        public DateTime EnterDate { get; set; }
        //[Required(ErrorMessage = "Enter the Issued date.")]
        //[DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }
        public string fname { get; set; }
        public string SCHL { get; set; }
        public string SESSION { get; set; }
        public string LOT { get; set; }
        public string IDNO { get; set; }
        public string MyGroup { get; set; }

    }

    public class ShowModel
    {
        public int Id { get; set; }
        public string Year { get; set; }
        public string Lot { get; set; }
        public string form { get; set; }
        public string RegDate { get; set; }
        public string Exam { get; set; }
        public string SCHL { get; set; }
        public string regno { get; set; }
        public string Name { get; set; }
        public string FNAME { get; set; }
        public string MNAME { get; set; }
        public string DOB { get; set; }
        public string sex { get; set; }
        public string caste { get; set; }
        public int pagec { get; set; }
    }
    public class ValidateFileAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int MaxContentLength = 1024 * 1024 * 3; //3 MB
            string[] AllowedFileExtensions = new string[] { ".jpeg", ".jpg" };

            var file = value as HttpPostedFileBase;

            if (file == null)
                return false;
            else if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
            {
                ErrorMessage = "Please upload Your Photo of type: " + string.Join(", ", AllowedFileExtensions);
                return false;
            }
            else if (file.ContentLength > MaxContentLength)
            {
                ErrorMessage = "Your Photo is too large, maximum allowed size is : " + (MaxContentLength / 1024).ToString() + "MB";
                return false;
            }
            else
                return true;
        }
    }
}