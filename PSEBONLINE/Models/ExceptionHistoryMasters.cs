using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSEBONLINE.Models
{
    public class ExceptionHistoryMasters
    {
        [Key]
        public long ExceptionHistoryId { get; set; }
        public DateTime AddedDate { get; set; }
        public string ApplicationType { get; set; }
        public string CaseNoOrTrackNo { get; set; }
        public string ErrorMessage { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string LineNoCsFile { get; set; }
        public string ExecptionNo { get; set; }
    }
}