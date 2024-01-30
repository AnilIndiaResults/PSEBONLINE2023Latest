using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PSEBONLINE.Models
{

    public class ParticularCorrectionStaffDetails
    {
        [Key]
        public int PCSID { get; set; }
        public int StudentId { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string StaffAadharNumber { get; set; }
        public string StaffMobile { get; set; }
        public string StaffOTP { get; set; }
        public DateTime? CreatedOn { get; set; }

    }
}