using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
namespace PSEBONLINE.Models
{
    public class DMModel
    {
        public DataSet StoreAllData { get; set; }

        public string DairyNo { get; set; }
        public string DairyDate { get; set; }
        
        public string SCHL { get; set; }
        public string Class { get; set; }
        public string RNo { get; set; }
        public string RDate { get; set; }
        public string Remarks { get; set; }
        public string RType { get; set; }
        public string CreatedDate { get; set; }

        public string UserName  { get; set; }
        public string DistName { get; set; }
        public string OfficeName { get; set; }
    }
}