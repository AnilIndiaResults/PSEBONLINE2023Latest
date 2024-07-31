using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace PSEBONLINE.Models
{

    public class AssociateModel
    {
        public string EmpUserId { get; set; }
        public string ReceiptScannedCopy { get; set; }
        public List<RoomDetailsModel> RoomDetailsModelList { get; set; }
        public RoomDetailsModel RoomDetailsModel { get; set; }
        public AssociationDocumentDetailsModel AssociationDocumentDetailsModel { get; set; }
        public List<AssociationDocumentDetailsModel> AssociationDocumentDetailsModelList { get; set; }

        public List<StudentCountModel> StudentCountModelList { get; set; }
        public StudentCountModel StudentCountModel { get; set; }

        public List<AssociateSchoolModels> AssociateSchoolModelsList { get; set; }
        public AssociateSchoolModels AssociateSchoolModel { get; set; }

        public List<AssociateContinuationDashBoardViews> AssociateContinuationDashBoardViewsList { get; set; }
        public AssociateContinuationDashBoardViews AssociateContinuationDashBoardViews { get; set; }

        public List<AssociateContinuationBuildingSafty> AssociateContinuationBuildingSaftyList { get; set; }
        public AssociateContinuationBuildingSafty AssociateContinuationBuildingSafty { get; set; }
        public string ChallanCategoryName { get; set; }
        public DataSet StoreAllData { get; set; }
        public int ID { get; set; }
        public string SCHL { get; set; }
        public int RS10GTotal2017 { get; set; }
        public int RS10GPass2017 { get; set; }
        public int RS10GPercent2017 { get; set; }
        public int RS10GTotal2018 { get; set; }
        public int RS10GPass2018 { get; set; }
        public int RS10GPercent2018 { get; set; }
        public int RS12HTotal2017 { get; set; }
        public int RS12HPass2017 { get; set; }
        public int RS12HPercent2017 { get; set; }
        public int RS12HTotal2018 { get; set; }
        public int RS12HPass2018 { get; set; }
        public int RS12HPercent2018 { get; set; }
        public int RS12STotal2017 { get; set; }
        public int RS12SPass2017 { get; set; }
        public int RS12SPercent2017 { get; set; }
        public int RS12STotal2018 { get; set; }
        public int RS12SPass2018 { get; set; }
        public int RS12SPercent2018 { get; set; }
        public int RS12CTotal2017 { get; set; }
        public int RS12CPass2017 { get; set; }
        public int RS12CPercent2017 { get; set; }
        public int RS12CTotal2018 { get; set; }
        public int RS12CPass2018 { get; set; }
        public int RS12CPercent2018 { get; set; }
        public int RS12VTotal2017 { get; set; }
        public int RS12VPass2017 { get; set; }
        public int RS12VPercent2017 { get; set; }
        public int RS12VTotal2018 { get; set; }
        public int RS12VPass2018 { get; set; }
        public int RS12VPercent2018 { get; set; }
        //
        public int SF1TC2017 { get; set; }
        public int SF1TF2017 { get; set; }
        public int SF1TC2018 { get; set; }
        public int SF1TF2018 { get; set; }
        public int SF1Percent { get; set; }
        public int SF2TC2017 { get; set; }
        public int SF2TF2017 { get; set; }
        public int SF2TC2018 { get; set; }
        public int SF2TF2018 { get; set; }
        public int SF2Percent { get; set; }
        public int SF3TC2017 { get; set; }
        public int SF3TF2017 { get; set; }
        public int SF3TC2018 { get; set; }
        public int SF3TF2018 { get; set; }
        public int SF3Percent { get; set; }
        public int SF4TC2017 { get; set; }
        public int SF4TF2017 { get; set; }
        public int SF4TC2018 { get; set; }
        public int SF4TF2018 { get; set; }
        public int SF4Percent { get; set; }
        public int SF5TC2017 { get; set; }
        public int SF5TF2017 { get; set; }
        public int SF5TC2018 { get; set; }
        public int SF5TF2018 { get; set; }
        public int SF5Percent { get; set; }
        public int SF6TC2017 { get; set; }
        public int SF6TF2017 { get; set; }
        public int SF6TC2018 { get; set; }
        public int SF6TF2018 { get; set; }
        public int SF6Percent { get; set; }
        public int SF7TC2017 { get; set; }
        public int SF7TF2017 { get; set; }
        public int SF7TC2018 { get; set; }
        public int SF7TF2018 { get; set; }
        public int SF7Percent { get; set; }
        public int SF8TC2017 { get; set; }
        public int SF8TF2017 { get; set; }
        public int SF8TC2018 { get; set; }
        public int SF8TF2018 { get; set; }
        public int SF8Percent { get; set; }
        public int SF9TC2017 { get; set; }
        public int SF9TF2017 { get; set; }
        public int SF9TC2018 { get; set; }
        public int SF9TF2018 { get; set; }
        public int SF9Percent { get; set; }
        public int SF10TC2017 { get; set; }
        public int SF10TF2017 { get; set; }
        public int SF10TC2018 { get; set; }
        public int SF10TF2018 { get; set; }
        public int SF10Percent { get; set; }
        public int SF11HTC2017 { get; set; }
        public int SF11HTF2017 { get; set; }
        public int SF11HTC2018 { get; set; }
        public int SF11HTF2018 { get; set; }
        public int SF11HPercent { get; set; }
        public int SF11STC2017 { get; set; }
        public int SF11STF2017 { get; set; }
        public int SF11STC2018 { get; set; }
        public int SF11STF2018 { get; set; }
        public int SF11SPercent { get; set; }
        public int SF11CTC2017 { get; set; }
        public int SF11CTF2017 { get; set; }
        public int SF11CTC2018 { get; set; }
        public int SF11CTF2018 { get; set; }
        public int SF11CPercent { get; set; }
        public int SF11VTC2017 { get; set; }
        public int SF11VTF2017 { get; set; }
        public int SF11VTC2018 { get; set; }
        public int SF11VTF2018 { get; set; }
        public int SF11VPercent { get; set; }
        public int SF12HTC2017 { get; set; }
        public int SF12HTF2017 { get; set; }
        public int SF12HTC2018 { get; set; }
        public int SF12HTF2018 { get; set; }
        public int SF12HPercent { get; set; }
        public int SF12STC2017 { get; set; }
        public int SF12STF2017 { get; set; }
        public int SF12STC2018 { get; set; }
        public int SF12STF2018 { get; set; }
        public int SF12SPercent { get; set; }
        public int SF12CTC2017 { get; set; }
        public int SF12CTF2017 { get; set; }
        public int SF12CTC2018 { get; set; }
        public int SF12CTF2018 { get; set; }
        public int SF12CPercent { get; set; }
        public int SF12VTC2017 { get; set; }
        public int SF12VTF2017 { get; set; }
        public int SF12VTC2018 { get; set; }
        public int SF12VTF2018 { get; set; }
        public int SF12VPercent { get; set; }
        //
        public string BSDSES { get; set; }
        public decimal BSDTINC { get; set; }
        public decimal BSDTEXP { get; set; }
        public string BSDNAME { get; set; }
        public string BSDIDNO { get; set; }
        public string BSDADD { get; set; }
        public string BSDFILE { get; set; }
        //
        public string BSFROM { get; set; }
        public string BSTO { get; set; }
        public string BSIA { get; set; }
        public string BSMEMO { get; set; }
        public string BSIDATE { get; set; }//DateTime
        public string BSFILE { get; set; }

        public string FSFROM { get; set; }
        public string FSTO { get; set; }
        public string FSIA { get; set; }
        public string FSMEMO { get; set; }
        public string FSIDATE { get; set; }//DateTime
        public string FSFILE { get; set; }
        //
        public int BPTS { get; set; }
        public int BPBOOKPERCENT { get; set; }
        public decimal BPAMOUNT { get; set; }
        public string BPNAME { get; set; }
        public string BPBILL { get; set; }
        public string BPBILLDATE { get; set; }//DateTime
        public string BPFILE { get; set; }
        //
        public string ASDIST { get; set; }
        public string ASZONE { get; set; }
        public string ASNATIONAL { get; set; }
        public string ASINTER { get; set; }
        public string ASSTATE { get; set; }

        public string AOTH { get; set; }
        //
        public string OI1 { get; set; }
        public string OI2 { get; set; }
        public string OI3 { get; set; }
        public string OI4 { get; set; }
        public string OI5 { get; set; }
        public string OI6 { get; set; }
        public string OI7 { get; set; }
        public bool ISACTIVE { get; set; }
        public DateTime CREATEDDATE { get; set; }
        public DateTime UPDATEDDATE { get; set; }


        public string SCHLDIST { get; set; }
        public string SCHLDISTNM { get; set; }
        public string SCHLNM { get; set; }

        public int ChallanCategory { get; set; }
        public string ChallanId { get; set; }
        public string ChallanDt { get; set; }
        public int challanVerify { get; set; }

        public string OldRecieptNo { get; set; }
        public string oldChallanId { get; set; }
        public string OldDepositDate { get; set; }
        public int OldAmount { get; set; }

        public string CourtCaseFile { get; set; }

        public Int16 IsFormLock { get; set; }
        public string IsFormLockOn { get; set; }
        public string IsFormLockBy { get; set; }

        public int CurrentApplicationStatus { get; set; }
        public DateTime? CurrentApplicationStatusON { get; set; }
        public string ObjectionLetter { get; set; }

        public string InspectionReport { get; set; }
        public int UpdatedBy { get; set; }

        public string Remarks { get; set; }


        public string Sec1 { get; set; }
        public string Sec2 { get; set; }
        public string Sec3 { get; set; }
        public string Sec4 { get; set; }
        public string Sec5 { get; set; }
        public string Sec6 { get; set; }
        public string Sec7 { get; set; }
        public string Sec8 { get; set; }
        public string Sec9 { get; set; }
        public string Sec10 { get; set; }
        public string Sec11Hum { get; set; }
        public string Sec11Voc { get; set; }
        public string Sec11Comm { get; set; }
        public string Sec11Sci { get; set; }
        public string Sec12Hum { get; set; }
        public string Sec12Voc { get; set; }
        public string Sec12Comm { get; set; }
        public string Sec12Sci { get; set; }
        public string TotStu1 { get; set; }
        public string TotStu2 { get; set; }
        public string TotStu3 { get; set; }
        public string TotStu4 { get; set; }
        public string TotStu5 { get; set; }
        public string TotStu6 { get; set; }
        public string TotStu7 { get; set; }
        public string TotStu8 { get; set; }
        public string TotStu9 { get; set; }
        public string TotStu10 { get; set; }
        public string TotStu11Hum { get; set; }
        public string TotStu11Voc { get; set; }
        public string TotStu11Comm { get; set; }
        public string TotStu11Sci { get; set; }
        public string TotStu12Hum { get; set; }
        public string TotStu12Voc { get; set; }
        public string TotStu12Comm { get; set; }
        public string TotStu12Sci { get; set; }

        public string TAS { get; set; }
        public string CAS { get; set; }
        public string Playgl { get; set; }
        public string PlayGDS { get; set; }
        public string SBC { get; set; }
        public string DBC { get; set; }
        public string TotalNoOfcomputer { get; set; }
        public string smartClass { get; set; }
        public string ComputerLab { get; set; }
        public string PhysicsLab { get; set; }
        public string ChemistryLab { get; set; }
        public string liabrary { get; set; }
        public string Noofbooks { get; set; }
        public string typeinternet { get; set; }
        public string isTransport { get; set; }
        public string NoofPrinter { get; set; }
        public string pagePrintCapacity { get; set; }
        public string isCasePending { get; set; }
        public string isAccountCheque { get; set; }
        public string isRoAvail { get; set; }
        public string isToilet { get; set; }
        public string isDisplayBoard { get; set; }
        public string OtherActivities { get; set; }
        public string isPsebReg { get; set; }
        public string Associateschlcode { get; set; }
        public string AssociateLtrFile { get; set; }

        public string BuildingMapfile { get; set; }


    }
    public class AssociateContinuationDashBoardViews
        {
            [Key]
            public string SCHL { get; set; }
            public string SCHLICode { get; set; }
            public string ClassLevel { get; set; }
            public string UDISECode { get; set; }
            public string EastablishmentYear { get; set; }
            public string SCHLName { get; set; }
            public string DistrictName { get; set; }
            public string TehsilName { get; set; }
            public string Address { get; set; }
            public string PinCode { get; set; }
            public string PrincipalName { get; set; }
            public string DateofBirth { get; set; }
            public string Qualifications { get; set; }
            public string Experience { get; set; }
            public string DateofJoining { get; set; }
            public string Mobile { get; set; }
            public string StdCodephone { get; set; }
            public string OtherContactPerson { get; set; }
    }


    public class AssociateContinuationBuildingSafty
    {
        //public Int16 Id { get; set; }
        //public string buildingFrom { get; set; }
        //public string buildingUpto { get; set; }
        //public string buildingIssuingAuthority { get; set; }
        //public string buildingMemoDispatchNo { get; set; }
        //public string buildingIssuingDate { get; set; }
        //public string FireFrom { get; set; }
        //public string FireUpto { get; set; }
        //public string FireIssuingAuthority { get; set; }
        //public string FireMemoDispatchNo { get; set; }
        //public string FireIssuingDate { get; set; }


       


    }


    public class RoomDetailsModel
    {
        public Int32 Id { get; set; }
        public string SCHL { get; set; }
        public string RoomType { get; set; }
        public string FloorName { get; set; }
        public Int32 Height { get; set; }

        public Int32 width { get; set; }
        public Int32 Quantity { get; set; }
        public Int32 Area { get; set; }

      
    }

    public class StudentCountModel
    {
        public Int32 Id { get; set; }
        public string SCHL { get; set; }
        public string Sec1 { get; set; }
        public string Sec2 { get; set; }
        public string Sec3 { get; set; }
        public string Sec4 { get; set; }
        public string Sec5 { get; set; }
        public string Sec6 { get; set; }
        public string Sec7 { get; set; }
        public string Sec8 { get; set; }
        public string Sec9 { get; set; }
        public string Sec10 { get; set; }
        public string Sec11Hum { get; set; }
        public string Sec11Voc { get; set; }
        public string Sec11Comm { get; set; }
        public string Sec11Sci { get; set; }
        public string Sec12Hum { get; set; }
        public string Sec12Voc { get; set; }
        public string Sec12Comm { get; set; }
        public string Sec12Sci { get; set; }
        public string TotStu1 { get; set; }
        public string TotStu2 { get; set; }
        public string TotStu3 { get; set; }
        public string TotStu4 { get; set; }
        public string TotStu5 { get; set; }
        public string TotStu6 { get; set; }
        public string TotStu7 { get; set; }
        public string TotStu8 { get; set; }
        public string TotStu9 { get; set; }
        public string TotStu10 { get; set; }
        public string TotStu11Hum { get; set; }
        public string TotStu11Voc { get; set; }
        public string TotStu11Comm { get; set; }
        public string TotStu11Sci { get; set; }
        public string TotStu12Hum { get; set; }
        public string TotStu12Voc { get; set; }
        public string TotStu12Comm { get; set; }
        public string TotStu12Sci { get; set; }
        public int StudentCountId { get;  set; }
    }


    public class AssociationDocumentDetailsModel
    {
        public DataSet StoreAllData { get; set; }
        public int eDocId { get; set; }
        public string SCHL { get; set; }

        public int DocID { get; set; }

        public string DocFile { get; set; }
        public int IsActive { get; set; }
        public string CreatedDate { get; set; }


        public List<AssociationDocumentMaster> AssociationDocumentMasterList { get; set; }

      
    }

    public class AssociationDocumentMaster
    {
        public int DocID { get; set; }
        public string DocumentName { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompulsory { get; set; }

    }


    public class SchoolInfraModel
    {
        public int ID { get; set; }
        public string SCHL { get; set; }
        public string TAS { get; set; }
        public string CAS { get; set; }
        public string Playgl { get; set; }
        public string PlayGDS { get; set; }
        public string SBC { get; set; }
        public string DBC { get; set; }
        public string TotalNoOfcomputer { get; set; }
        public string smartClass { get; set; }
        public string ComputerLab { get; set; }
        public string PhysicsLab { get; set; }
        public string ChemistryLab { get; set; }
        public string liabrary { get; set; }
        public string Noofbooks { get; set; }
        public string typeinternet { get; set; }
        public string isTransport { get; set; }
        public string NoofPrinter { get; set; }
        public string pagePrintCapacity { get; set; }
        public string isCasePending { get; set; }
        public string isAccountCheque { get; set; }
        public string isRoAvail { get; set; }
        public string isToilet { get; set; }
        public string isDisplayBoard { get; set; }
        public string OtherActivities { get; set; }
     

    }

    public class AssociateSchoolModels
    {



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


}