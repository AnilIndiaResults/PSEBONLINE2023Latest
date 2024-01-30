using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;

namespace PSEBONLINE.Models
{
    public class ReportModel
    {
        public DataSet StoreAllData { get; set; }
        public string feecode { get; set; }

        public string Dist { get; set; }
        public IList<SelectListItem> DistList { get; set; }
    }

    public class ClusterReportModel
    {
        public DataSet StoreAllData { get; set; }
        public string ccode { get; set; }
        public string sub { get; set; }
        public string Dist { get; set; }
        public IList<ClusterModel> ClusterList { get; set; }
        public IList<SubjectModel> SubList { get; set; }
        public IList<SelectListItem> DistList { get; set; }
    }

    public class ClusterModel
    {
        public string dist { get; set; }
        public string ccode { get; set; }
        public string clusternm { get; set; }
    }
    public class SubjectModel
    {
        public string sub { get; set; }
        public string subnm { get; set; }
    }

    public class CentreExamDateModel
    {
        [Key]
        public int ID { get; set; }
        public string Class { get; set; }
        public string ExamDate { get; set; }
        public string cschl { get; set; }
        public string Ecentre { get; set; }
        public string Schoole { get; set; }
        public string Mobileno { get; set; }
        public string Attendance { get; set; }
        public string Status { get; set; }
        public DataSet StoreAllData { get; set; }
    }


}