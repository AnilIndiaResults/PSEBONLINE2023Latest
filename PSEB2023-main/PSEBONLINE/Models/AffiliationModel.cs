using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace PSEBONLINE.Models
{

    public class AffiliationFee
    {
        public string ReceiptScannedCopy { get; set; }
        public DataSet StoreAllData { get; set; }
        public string SCHL { get; set; }
        public string Class { get; set; }
        public string Form { get; set; }
        public string sDate { get; set; }
        public string eDate { get; set; }
        public int fee { get; set; }
        public int latefee { get; set; }
        public int totfee { get; set; }
        public int FEECODE { get; set; }
        public string FEECAT { get; set; }
        public string BankLastdate { get; set; }
        public string Type { get; set; }
        public string AllowBanks { get; set; }
        public string TotalFeesInWords { get; set; }
        //
        public string BankCode { get; set; }
        public int ChallanCategory { get; set; }
        public string OldRecieptNo { get; set; }
        public string oldChallanId { get; set; }
        public string OldDepositDate { get; set; }
        public int OldAmount { get; set; }
    }


    public class AffiliationContinuationDashBoardViews
    {
        [Key]
        public string SCHL { get; set; }
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
    }

    public class AffiliationModel 
    {
        public string EmpUserId { get; set; }
        public string ReceiptScannedCopy { get; set; }
        public List<AffObjectionLettersViews> affObjectionLettersViewList { get; set; }
        public AffiliationContinuationDashBoardViews affiliationContinuationDashBoardViews { get; set; }


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

        public int IsFormLock { get; set; }
        public string IsFormLockOn { get; set; }
        public string IsFormLockBy { get; set; }

        public int CurrentApplicationStatus { get; set; }
        public DateTime? CurrentApplicationStatusON { get; set; }
        public string ObjectionLetter { get; set; }

        public string InspectionReport { get; set; }
        public int UpdatedBy { get; set; }

        public string Remarks { get; set; }
    }


    public class AffiliationDocumentMaster
    {
        public int DocID { get; set; }
        public string DocumentName { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompulsory { get; set; }

    }

    public class AffiliationDocumentDetailsModel
    {
        public DataSet StoreAllData { get; set; }
        //Rohit
        public int eDocId { get; set; }
        public string SCHL { get; set; }

        public int DocID { get; set; }

        public string DocFile { get; set; }
        public int IsActive { get; set; }
        public string CreatedDate { get; set; }

        public List<AffiliationDocumentMaster> AffiliationDocumentMasterList { get; set; }


    }
}