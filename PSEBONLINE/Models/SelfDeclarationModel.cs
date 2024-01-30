using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PSEBONLINE.Models
{
    public class SelfDeclarationLoginModel
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
    public class SelfDeclarationLoginSession
    {
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
        public int LoginStatus { get; set; }
    }
    public class SelfDeclarations
    {
        [Key]
        public int SelfDeclarationId { get; set; }
        public string CAT { get; set; }
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
        public string SelfDeclarationDocument { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyOn { get; set; }
        public int IsFinalSubmit { get; set; }
        public DateTime? IsFinalSubmitOn { get; set; }
    }
}