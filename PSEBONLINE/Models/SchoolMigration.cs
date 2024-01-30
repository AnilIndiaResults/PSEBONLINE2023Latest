using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace PSEBONLINE.Models
{
    public class StudentSchoolMigrationViewModel
    {
        //public DataSet StoreAllData;

        public int MigrationId { get; set; }
        public string StdId { get; set; }
        public string RegNo { get; set; }
        public string CurrentSCHL { get; set; }
        public string NewSCHL { get; set; }
        public string OldStream { get; set; }
        public string NewStream { get; set; }
        public string StudentMigrationLetter { get; set; }
        public int MigrationFlag { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string userName { get; set; }
        public string UserIP { get; set; }
        public string AppLevel { get; set; }
        public int MigrationStatusCode { get; set; }
        public int IsAppBySchl { get; set; }
        public int IsAppByHOD { get; set; }
        public int IsCancel { get; set; }
        public DateTime? CancelOn { get; set; }
        public string CancelRemarks { get; set; }
        public string CancelBy { get; set; }
        public string Name { get; set; }
        public string FName { get; set; }
        public string MName { get; set; }

        public string PName { get; set; }
        public string PFName { get; set; }
        public string PMName { get; set; }
        public string DOB { get; set; }
        public DateTime? LastDate { get; set; }
        public string AppLevelByLastDate { get; set; }
        public string Registration_num { get; set; }
        public string Aadhar_num { get; set; }
        public string NewSchlDetails { get; set; }
        public string OldSchlDetails { get; set; }

        public string NewSchlDetailsP { get; set; }
        public string OldSchlDetailsP { get; set; }
        public int CurrentMigrationStatusCode { get; set; }
        public string CurrentMigrationStatus { get; set; }
        public string CurrentMigrationStatusABBR { get; set; }

        public string OldSchlMigStatus { get; set; }

        public float fee { get; set; }
        public float latefee { get; set; }
        public float totfee { get; set; }
        public string ChallanId { get; set; }
        public int Challanverify { get; set; }

        public string ChallanDate { get; set; }
        public string J_REF_NO { get; set; }
        public string DEPOSITDT { get; set; }


        public string DIST { get; set; }
        public string DISTNM { get; set; }
        public string DISTNMP { get; set; }

        public string OLDSCHLMOBILE { get; set; }
        public string NEWSCHLMOBILE { get; set; }

        public string Form_Name { get; set; }
        public string cls { get; set; }
        public string MigrationCertificateNumber { get; set; }
    }

    public class StudentSchoolMigrations
    {
        [Key]
        public int MigrationId { get; set; }
        public string StdId { get; set; }
        public string RegNo { get; set; }
        public string CurrentSCHL { get; set; }
        public string NewSCHL { get; set; }
        public string OldStream { get; set; }
        public string NewStream { get; set; }
        public string StudentMigrationLetter { get; set; }
        public int MigrationFlag { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string userName { get; set; }
        public string UserIP { get; set; }

        public string AppLevel { get; set; }
        public int MigrationStatusCode { get; set; }
        public int IsAppBySchl { get; set; }
        public int IsAppByHOD { get; set; }


        public int IsCancel { get; set; }
        public DateTime? CancelOn { get; set; }
        public string CancelRemarks { get; set; }
        public string CancelBy { get; set; }

        public string OldSchlMigStatus { get; set; }

        public float fee { get; set; }
        public float latefee { get; set; }
        public float totfee { get; set; }
        public string ChallanId { get; set; }
        public int Challanverify { get; set; }

        public string MigrationReason { get; set; }

        // public string OldSchoolMobile { get; set; }
        public string MigrationCertificateNumber { get; set; }

    }


  
}