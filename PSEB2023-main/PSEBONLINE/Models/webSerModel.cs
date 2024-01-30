using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data;

namespace PSEBONLINE.Models
{
    public class webSerModel
    {
        public string ExamCent { get; set; }
        public string UdiseCode { get; set; }
        public string importBy { get; set; }
        public string SearchString { get; set; }
        public DataSet StoreAllData { get; set; }
        public DataTable StoreAllDataTable { get; set; }
    }
}

