using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PSEBONLINE.Models
{
    public class LoginSessionSchl
    {        
        public string SCHL { get; set; }
        public string SENIOR { get; set; }
        public string OSENIOR { get; set; }
        public string MATRIC { get; set; }
        public string OMATRIC { get; set; }
        public string PRIMARY { get; set; }
        public string MIDDLE { get; set; }
      
    }


    public class LoginSession
    {
        public string CurrentSession { get; set; }
        public string STATUS { get; set; }
        public string DIST { get; set; }
        public string SCHL { get; set; }
        public string SCHLE { get; set; }
        public string PRINCIPAL { get; set; }
        //public string STATIONE { get; set; }
        //public string DISTE { get; set; }
        public string middle { get; set; }
        public string fifth { get; set; }
        public string Senior { get; set; }
        public string OSenior { get; set; }
        public string Matric { get; set; }
        public string OMATRIC { get; set; }
        public bool Approved { get; set; }
        public string MOBILE { get; set; }
        public string EMAILID { get; set; }
        public int LoginStatus { get; set; }
        public DateTime DateFirstLogin { get; set; }
        public string SCHLNME { get; set; }
        public string SCHLNMP { get; set; }
        // NEw ADded
        //public string EXAMCENTSCHLN { get; set; }
        public string EXAMCENT { get; set; }
        public string PRACCENT { get; set; }
        public string USERTYPE { get; set; }
        public string CLUSTERDETAILS { get; set; }
        public int IsMeritoriousSchool { get; set; }
        public int IsSupplementaryExam { get; set; }
        public int IsPrivateExam { get; set; }
        public int IsAllowPSTET { get; set; }

        public string DealingBranchContact { get; set; }
    }
    public class LoginModel
    {
        [Required]
        [Display(Name = "username")]
        public string username { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Required]
        [Display(Name = "Session")]
        public string Session { get; set; }
        public string SchoolId { get; set; }

        public string TollFreeNumber { get; set; }
        public string TollFreeEmail { get; set; }

        public string AdminEmployeeUserId { get; set; }
        public string AdminEmployeePassword { get; set; }

    }

    public class LoginModel2
    {
        [Required]
        [Display(Name = "username")]
        public string username { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "username")]
        public string admusername { get; set; }

        [Required]
        [Display(Name = "Session")]
        public string Session { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string admPassword { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }


    }
}