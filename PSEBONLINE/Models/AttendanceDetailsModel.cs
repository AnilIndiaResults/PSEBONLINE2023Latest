using PSEBONLINE.AbstractLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace PSEBONLINE.Models
{
	public class AttendanceDetailsModel
    {
       public List<AttendanceAdminDtl> AttendanceAdminDtl { get; set; }
        public List<AttendanceAdminDetailsReport> AttendanceAdminDetailsReport { get; set; }
        public List<AttendanceReportDetailsCount> AttendanceReportDetailsCount { get; set; }
        public List<AttendanceReportCandidateDetails> AttendanceReportCandidateDetails { get; set; }
        
    }


    public class AttendanceAdminDtl
    {
       
        public string rollNo { get; set; }
        public string studentId { get; set; }
        public string candidateName { get; set; }
        public string motherName { get; set; }
        public string fatherName { get; set; }
        public string dob { get; set; }
        public string differentlyAbled { get; set; }
        public string attendanceStatus { get; set; }
        public string cls { get; set; }

        public DataSet StoreAllData { get; set; }





    }

    public class AttendanceAdminDetailsReport
	{	
		public int r { get; set; }
		public int Total { get; set; }
        public int Absent { get; set; }
        public int Present { get; set; }
        public int UMC { get; set; }
        public int Cancel { get; set; }
        public string cls { get; set; }
        public string rp { get; set; }
        public string centrecode { get; set; }
        public string distnm { get; set; }
        public string clsName { get; set; }
        public string RPname { get; set; }
        public string subcode { get; set; }
        public string examBatch { get; set; }
        public string Exmdate { get; set; }
        public string memoNumber { get; set; }
        public string statusNM { get; set; }
        public string SubNM { get; set; }
        public string remarks { get; set; }

        public string schl { get; set; }
        public string schle { get; set; }
        
        public DataSet StoreAllData { get; set; }


    }


    public class AttendanceReportDetailsCount
    {
       
        public int Total { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Cancel { get; set; }
        public int UMC { get; set; }

       





    }

    public class AttendanceReportCandidateDetails
    {

        public string roll { get; set; }
        public string Candi_Name { get; set; }

        public string Father_Name { get; set; }





    }
   
}