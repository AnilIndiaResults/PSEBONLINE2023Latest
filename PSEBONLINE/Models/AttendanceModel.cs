using PSEBONLINE.AbstractLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Services.Protocols;

namespace PSEBONLINE.Models
{
	public class AttendanceResponse
	{
		public string returnmessage { get; set; }
		public string returncode { get; set; }
		public string memonumber { get; set; }
	}

	public class AttendanceMemoDetail
	{
		[Key]
		public int id { get; set; }
		public string status { get; set; }

		public string subCode { get; set; }
		public string schl { get; set; }
		public string cls { get; set; }
		public string rp { get; set; }
		public string examDate { get; set; }
		public string examBatch { get; set; }
		public string memoNumber { get; set; }
		public string CreatedBy { get; set; }
		public DateTime? CreatedOn { get; set; }
		public string ModifiedBy { get; set; }
		public DateTime? ModifiedOn { get; set; }
		public string EmpUserId { get; set; }
		public DateTime? finalsubmiton { get; set; }
		public string finalsubmitby { get; set; }
		public string statusNM { get; set; }
		public string schlnme { get; set; }
		public string schlnmp { get; set; }
		public string centrecode { get; set; }
		public string centernm { get; set; }
		public string clsName { get; set; }
		public string SubNM { get; set; }
		public string FinalSubmitOnPrint { get; set; }
		public string RPname { get; set; }
        public string schle { get; set; }
        public string edublock { get; set; }
    }

	public class AttendenceSummaryDetail
	{
		[Key]
		public int id { get; set; }
		public string center { get; set; }
		public string cls { get; set; }
		public string category { get; set; }
		public string subcode { get; set; }
		public string subname { get; set; }
		public string examdate { get; set; }
		public int total { get; set; }
		public int present { get; set; }
		public int absent { get; set; }
		public int cancel { get; set; }
		public int umc { get; set; }
		public string memonumber { get; set; }
		public string status { get; set; }
        public string remarks { get; set; }

    }
	public class vmAttendanceModel
	{
		public AttendanceMemoDetail attendanceMemoDetail { get; set; }
		public List<SubjectAttendanceOpen> subjectAttendanceOpens { get; set; }
		public List<SubjectAttendancePrivate> subjectAttendancePrivates { get; set; }
		public List<SubjectAttendanceFifthRegular> subjectAttendanceFifthRegulars { get; set; }
		public List<SubjectAttendanceMatricRegular> subjectAttendanceMatricRegulars { get; set; }
		public List<SubjectAttendanceEighthRegular> subjectAttendanceEighthRegulars { get; set; }
		public List<SubjectAttendanceSeniorRegular> subjectAttendanceSeniorRegulars { get; set; }

		public List<SubjectAttendance> attendanceList { get; set; }

		public List<SubjectAttendance> attendanceListPresent { get; set; }
		public List<SubjectAttendance> attendanceListAbsent { get; set; }
		public List<SubjectAttendance> attendanceListCancel { get; set; }
		public List<SubjectAttendance> attendanceListUMC { get; set; }
		public string status { get; set; }
		public string memmoNumber { get; set; }
		public string centrecode { get; set; }
		public string cid { get; set; }
	}

	public class AttendanceAdminReport
	{
		[Key]
		public int r { get; set; }
		public int Total { get; set; }
        public int Absent { get; set; }
        public int Present { get; set; }
        public int UMC { get; set; }
        public int Cancel { get; set; }
        public string cls { get; set; }
        public string rp { get; set; }
        public string centrecode { get; set; }
        public string clsName { get; set; }
        public string RPname { get; set; }
        public string subcode { get; set; }
        public string examBatch { get; set; }
        public string Exmdate { get; set; }
        public string memoNumber { get; set; }
        public string statusNM { get; set; }
        public string SubNM { get; set; }
        public string remarks { get; set; }


    }

    public class SubjectAttendanceSeniorRegular
	{
		[Key]
		public string studentId { get; set; }
		public string subCode { get; set; }
		public string subName { get; set; }
		public string candidateName { get; set; }
		public string fatherName { get; set; }
		public string motherName { get; set; }
		public string dob { get; set; }
		public string differentlyAbled { get; set; }
		public string rollNo { get; set; }
		public string cent { get; set; }
		public string schl { get; set; }
		public string cls { get; set; }
		public string attendanceStatus { get; set; }
		public string rp { get; set; }
		public string attendance { get; set; }
		public string memonumber { get; set; }
	}
	public class SubjectAttendanceEighthRegular
	{
		[Key]
		public string studentId { get; set; }
		public string subCode { get; set; }
		public string subName { get; set; }
		public string candidateName { get; set; }
		public string fatherName { get; set; }
		public string motherName { get; set; }
		public string dob { get; set; }
		public string differentlyAbled { get; set; }
		public string rollNo { get; set; }
		public string cent { get; set; }
		public string schl { get; set; }
		public string cls { get; set; }
		public string attendanceStatus { get; set; }
		public string rp { get; set; }
		public string attendance { get; set; }
		public string memonumber { get; set; }
	}
	public class SubjectAttendanceMatricRegular
	{
		[Key]
		public string studentId { get; set; }
		public string subCode { get; set; }
		public string subName { get; set; }
		public string candidateName { get; set; }
		public string fatherName { get; set; }
		public string motherName { get; set; }
		public string dob { get; set; }
		public string differentlyAbled { get; set; }
		public string rollNo { get; set; }
		public string cent { get; set; }
		public string schl { get; set; }
		public string cls { get; set; }
		public string attendanceStatus { get; set; }
		public string rp { get; set; }
		public string attendance { get; set; }
		public string memonumber { get; set; }
	}
	public class SubjectAttendanceFifthRegular
	{
		[Key]
		public string studentId { get; set; }
		public string subCode { get; set; }
		public string subName { get; set; }
		public string candidateName { get; set; }
		public string fatherName { get; set; }
		public string motherName { get; set; }
		public string dob { get; set; }
		public string differentlyAbled { get; set; }
		public string rollNo { get; set; }
		public string cent { get; set; }
		public string schl { get; set; }
		public string cls { get; set; }
		public string attendanceStatus { get; set; }
		public string rp { get; set; }
		public string attendance { get; set; }
		public string memonumber { get; set; }
	}
	public class SubjectAttendancePrivate
	{
		[Key]
		public string studentId { get; set; }
		public string subCode { get; set; }
		public string subName { get; set; }
		public string candidateName { get; set; }
		public string fatherName { get; set; }
		public string motherName { get; set; }
		public string dob { get; set; }
		public string differentlyAbled { get; set; }
		public string rollNo { get; set; }
		public string cent { get; set; }
		public string schl { get; set; }
		public string cls { get; set; }
		public string attendanceStatus { get; set; }
		public string rp { get; set; }
		public string attendance { get; set; }
		public string memonumber { get; set; }
	}
	public class SubjectAttendanceOpen
	{
		[Key]
		public string studentId { get; set; }
		public string subCode { get; set; }
		public string subName { get; set; }
		public string candidateName { get; set; }
		public string fatherName { get; set; }
		public string motherName { get; set; }
		public string dob { get; set; }
		public string differentlyAbled { get; set; }
		public string rollNo { get; set; }
		public string cent { get; set; }
		public string schl { get; set; }
		public string cls { get; set; }
		public string attendanceStatus { get; set; }
		public string rp { get; set; }
		public string attendance { get; set; }
		public string memonumber { get; set; }
	}

	public class SubjectAttendance
	{
		[Key]
		public string studentId { get; set; }
		public string subCode { get; set; }
		public string subName { get; set; }
		public string candidateName { get; set; }
		public string fatherName { get; set; }
		public string motherName { get; set; }
		public string dob { get; set; }
		public string differentlyAbled { get; set; }
		public string rollNo { get; set; }
		public string cent { get; set; }
		public string schl { get; set; }
		public string cls { get; set; }
		public string attendanceStatus { get; set; }
		public string rp { get; set; }
		public string attendance { get; set; }
		public string memonumber { get; set; }
	}
}