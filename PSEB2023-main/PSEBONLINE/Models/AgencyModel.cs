using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace PSEBONLINE.Models
{  

    public class AgencyLoginSession
    {
        public string CurrentSession { get; set; }
        public int AgencyId { get; set; }
        public string UserName { get; set; }
        public string AgencyName { get; set; }
        public string Mobile { get; set; }
        public string EmailId { get; set; }
        public string SchoolAllows { get; set; }
        public bool IsActive { get; set; }
        public int LoginStatus { get; set; }

        public string AgencyInchargeName { get; set; }
        public string AllowClass { get; set; }
        public string AllowSubject { get; set; }
        public string UserType { get; set; }
    }

    public class TblNsqfMaster
    {
        [Key]
        public int AgencyId { get; set; }
        public string AgCode { get; set; }
        public string PWD { get; set; }
        public string Sector { get; set; }
        public string AgencyNM { get; set; }
        public string AgencyAdd { get; set; }
        public string AgencyMob { get; set; }
        public string AgencyEmail { get; set; }
        public bool IsActive { get; set; }


        public string AllowClass { get; set; }
        public string AllowSubject { get; set; }
        

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
        
        public DateTime? UPDDATE { get; set; }

        public string UserType { get; set; }
    }



    public class AgencyAllowClassModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class AgencyAllowSubjectModel
    {
        public string Sub { get; set; }
        public string SubNM { get; set; }
    }

    public class AgencySchoolModelList
    {
        public string AgencyAllowClass { get; set; }
        public string AgencyAllowSubject { get; set; }
        public List<AgencyAllowClassModel> AgencyAllowClassModels { set; get; }
        public List<AgencyAllowSubjectModel> AgencyAllowSubjectModels { set; get; }
        public List<AgencySchoolModel> AgencySchoolModels { set; get; }
    }
    public class AgencyModel
    {
        public DataSet StoreAllData { get; set; }
        public int AgencyId { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public string AgencyName { get; set; }
        public string Mobile { get; set; }
        public string EmailId { get; set; }
        public bool IsActive { get; set; }
        public string SchoolAllows { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class AgencySchoolModel
    {
        public string Schl { get; set; }
        public string SchlNME { get; set; }
        public string Udisecode { get; set; }
        public string MSET { get; set; }
        public string SSET { get; set; }
        public string Dist { get; set; }
        public string DistNM { get; set; }
        public string UserType { get; set; }     
        public string Matric { get; set; }
        public string Senior { get; set; }
        public int NOP { get; set; }

        public int NOMF { get; set; }
        public int IsMarkedFilled { get; set; }
        public DateTime LastDate { get; set; }
        public int IsActive { get; set; }
        public string FinalStatus { get; set; }
        public string FinalSubmitOn { get; set; }
        public string FinalSubmitLot { get; set; }
        public string FinalSubmitBy { get; set; }
        public string Mobile { get; set; }
        public string clsName { get; set; }
        public string SubCode { get; set; }

    }
    public class SchoolAllotedToAgency
    {
        public int Id { get; set; }
        public string AgencyId { get; set; }
        public string Schl { get; set; }
        public int IsActive { get; set; }
        public DateTime LastDate { get; set; }
        public int IsMarkedFilled { get; set; }
        public string FinalSubmitOn { get; set; }
        public string FinalSubmitLot { get; set; }
        public string FinalSubmitBy { get; set; }
        public string SCHLNME { get; set; }
    }

    public class SchoolAllowForMarksEntry
    {
        public DataSet StoreAllData { get; set; }
        public int? Id { get; set; }
        public string Cls { get; set; }
        public string Schl { get; set; }
        public int IsActive { get; set; }
        public string IsAllow { get; set; }
        public string LastDate { get; set; }
        public string AllowTo { get; set; }
        public string ReceiptNo { get; set; }
        public string DepositDate { get; set; }
        public int Amount { get; set; }
        public string UpdatedDate { get; set; }
        public string AllowedDate { get; set; }
        public string Panel { get; set; }

    }

}