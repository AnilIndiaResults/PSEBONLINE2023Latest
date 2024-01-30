using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
namespace PSEBONLINE.Models
{

    public class BindDeoCentreMasterViews
    {
        public string Month { get; set; }
        public string Year { get; set; }
        [Key]
        public string ExamMonth { get; set; }
        public string ExamMonthNM { get; set; }
        public string DeoSessionMonthYear { get; set; }
    }


    public class DEOModel
    {
        public string DeoMonthYear { get; set; }
        //--------------------------------------Login Start------
        [Required]
        [Display(Name = "username")]
        public string username { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        //--------------------------------------Login End---------
        //---------------------------------------------------STAFF START------------
        public string CrtNo { get; set; }
        public string AgeGroup { get; set; }
        public string Session { get; set; }
        public string Positions { get; set; }

        
        

        public string RollNo { get; set; }
        public string UdiseCode { get; set; }
        public string schlType { get; set; }
        public string schlMGMT { get; set; }
        public string SCHLCAT { get; set; }
        public string DOB { get; set; }
        public string centreDate { get; set; }
        public string Mobile { get; set; }
        public HttpPostedFileBase file { get; set; }       
        public string ClusterCode { get; set; }
        public string StaffName { get; set; }
        public string StaffFatherName { get; set; }
        public string designation { get; set; }
        public string desi { get; set; }
        public string typeDuty { get; set; }
        public string schlcode { get; set; }
        public string teacherepunjabid { get; set; }
        public string adharno { get; set; }
        public string cadre { get; set; }
        public string physicallydisablity { get; set; }
        public string disablitypercentage { get; set; }
        public string bankname { get; set; }
        public string bankaccno { get; set; }
        public string ifsccode { get; set; }
        public string homeaddress { get; set; }
        public string homedist { get; set; }
        public string hometehsil { get; set; }
        public string homestate { get; set; }
        public string homepincode { get; set; }
        public string expmonth { get; set; }       
        public string experience { get; set; }
        public string gender { get; set; }
        public string Selschool { get; set; }
        //------------------------------------------STAFF END--------------
        //-------------------Importdata--------------
        public string SelDist { get; set; }
        public string Edublock { get; set; }
        public string EduCluster { get; set; }
        public string SearchBy { get; set; }        
        public string Otherschl { get; set; }        
        public string Category { get; set; }
        public string SearchString { get; set; }
        public DataSet StoreAllData { get; set; }
        public DataSet StoreAllData2 { get; set; }
        public DataSet StoreAllData3 { get; set; }
        public DataSet StoreAllStaffData { get; set; }
        public DataSet TotalCount { get; set; }
        public List<CntreIDModel> chkidList { get; set; }
        public string SelLot { get; set; }
        //------------------------------------------------------Add Profile------------
        public string NAME { get; set; }
        //public string EMAILID { get; set; }
        //public string MOBILE { get; set; }
        public string STD { get; set; }
        public string PHONE { get; set; }

        public string PNAME1 { get; set; }
        public string PDESI1 { get; set; }
        public string PMOBILE1 { get; set; }

        public string PNAME2 { get; set; }
        public string PDESI2 { get; set; }
        public string PMOBILE2 { get; set; }

        public string PNAME3 { get; set; }
        public string PDESI3 { get; set; }
        public string PMOBILE3 { get; set; }

        public string PNAME4 { get; set; }
        public string PDESI4 { get; set; }
        public string PMOBILE4 { get; set; }

        public string PNAME5 { get; set; }
        public string PDESI5 { get; set; }
        public string PMOBILE5 { get; set; }
        public int ProfileFlag { get; set; }

        //--------------------------------------------------------End Profile------------
        //------------------------Change Password---------------------//
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        //------------------------End Cahnge password--------------------//
        //------------------------------------------------------Add Examiner------------
        public string DOR { get; set; }
        public int Exid { get; set; }
        public string Class { get; set; }
        public string Subjects { get; set; }
        //public string lstSubjects { get; set; }
        public string Quali { get; set; }
        public string mailid { get; set; }
        public string teachingexp { get; set; }
        public string Evaluationexp { get; set; }
        public string remarks { get; set; }

        public List<string> lstSubjects { get; set; }
        //public List<string> sportslist { get; set; }
        //public string[] SelectedSports { get; set; }
        //--------------------------------------------------------End Examiner---------
    }
    public class CntreIDModel
    {
        public string stores { get; set; }
        public bool Selected { get; set; }
        public string id { get; set; }
        public string Name { get; set; }
    }

    public class DeoMonthYearModel
    {
        public string Month { get; set; }
        public string Year { get; set; }
        public string DisplayValue { get; set; }
        public string DisplayText { get; set; }
    }
}