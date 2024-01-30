using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace PSEBONLINE.Models
{
    public class Import
    {
        public string Session1 { get; set; }
        public string SelList { get; set; }
        public DataSet StoreAllData { get; set; }
        public DataSet TotalCount { get; set; }
        public string schoolcode { get; set; }
        public string regno { get; set; }
        public string name { get; set; }
        public string fname { get; set; }
        public string mname { get; set; }
        public string dob { get; set; }
        public string result { get; set; }
        public List<ImportIDModel> chkidList { get; set; }        
    } 

    public class ImportIDModel
    {
        public string stores { get; set; }
        public bool Selected { get; set; }
        public string id { get; set; }
        public string Name { get; set; }
    }
}