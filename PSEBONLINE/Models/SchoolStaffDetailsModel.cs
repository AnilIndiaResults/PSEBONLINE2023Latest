using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
namespace PSEBONLINE.Models
{
    public class SchoolStaffDetailsModel
    {
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string Category { get; set; }
        public string tehsil { get; set; }
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
        public DataSet StoreAllData { get; set; }
        public DataSet TotalCount { get; set; }
        [Required(ErrorMessage = "*")]
        public int id { get; set; }
        [Required(ErrorMessage = "*")]
        public string Name { get; set; }
        [Required(ErrorMessage = "*")]
        public string FName { get; set; }
        [Required(ErrorMessage = "*")]
        public string DOB { get; set; }
        [Required(ErrorMessage = "*")]
        public string Gender { get; set; }
        [RegularExpression(@"^(\d{12})$", ErrorMessage = "Invalid/Incomplete Aadhar No")]
        public string AadharNo { get; set; }
        [RegularExpression(@"^\(?([5-9]{1})\)?([0-9]{2})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Incomplete Format.")]
        [Required(ErrorMessage = "*")]
        public string MobileNo { get; set; }

        [RegularExpression(@"^(\d{4}|\d{5})$", ErrorMessage = "Invalid/Incomplete StdCode")]
        public string stdCode { get; set; }
        [RegularExpression(@"^(\d{6}|\d{7})$", ErrorMessage = "Invalid/Incomplete PhoneNo")]
        public string PhoneNo { get; set; }
        [RegularExpression(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email ID.")]
        [Required(ErrorMessage = "*")]
        public string Email { get; set; }
        [Required(ErrorMessage = "*")]
        public string appointmentDate { get; set; }
        [Required(ErrorMessage = "*")]
        public string joiningDate { get; set; }
        [Required(ErrorMessage = "*")]
        public string Cadreid { get; set; }
        public string cadrename { get; set; }
        [Required(ErrorMessage = "*")]
        public string Subjectid { get; set; }
        public string subjectname { get; set; }
        [Required(ErrorMessage = "*")]
        public string HouseFlatNo { get; set; }
        [Required(ErrorMessage = "*")]
        public string VillWardCity { get; set; }
        [Required(ErrorMessage = "*")]
        public string LandMark { get; set; }
        [Required(ErrorMessage = "*")]
        public int DistrictId { get; set; }
        public string otherdistrict { get; set; }
        [Required(ErrorMessage = "*")]
        public string State { get; set; }
        public string otherstate { get; set; }
        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^\d{6}(-\d{4})?$", ErrorMessage = "Invalid/Incomplete PinCode")]
        public string PinCode { get; set; }
        [Required(ErrorMessage = "*")]
        public string DistanceFromSchool { get; set; }
        public string schoolcode { get; set; }
        public string photo { get; set; }
        [Required(ErrorMessage = "*")]
        [ValidateFile]
        [Display(Name = "Upload Photo1")]
        public HttpPostedFileBase file1 { get; set; }
    }

    public class District
    {

        public int DIST { get; set; }
        public string DISTNM { get; set; }
    }

}