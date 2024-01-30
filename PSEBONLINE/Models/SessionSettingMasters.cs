using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSEBONLINE.Models
{
    public class SessionSettingMasters
    {
        [Key]
        public int SessionId { get; set; }
        public string  SessionStartYear { get; set; }
        public string SessionEndYear { get; set; }
        public string SessionFullYear { get; set; }
        public string SessionShortYear { get; set; }
        public string ChallanYear { get; set; }
        public string FileUploadFullPath { get; set; }
        public string FileUploadFolderName { get; set; }
        public string CurrentDBName { get; set; }
        public string LastYearDBName { get; set; }
        public string RegTableFullNM { get; set; }
        public string SessionLastYear { get; set; }
    }

}