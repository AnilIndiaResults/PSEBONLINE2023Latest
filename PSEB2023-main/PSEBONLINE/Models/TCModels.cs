using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace PSEBONLINE.Models
{
    public class TCModels
    {
        public string SelYear { get; internal set; }
        public string AWRegisterNo { get; internal set; }
        public string SelRec { get; internal set; }
        public int ID { get; set; }
        public DataSet StoreAllData { get; set; }
        public string SearchResult { get; internal set; }
        public string Candi_Name { get; internal set; }
        public string Father_Name { get; internal set; }
        public string Mother_Name { get; internal set; }
        public string DOB { get; internal set; }
        public string Gender { get; internal set; }
        public string SdtID { get; internal set; }
        public string FormName { get; internal set; }
        public string regno { get; internal set; }
        public string SCHL { get; internal set; }
        public string NSQF_flag { get; internal set; }
        public string Group_Name { get; internal set; }
        public string Section { get; internal set; }


        public string dispatchNo { get; internal set; }
        public string attendanceTot { get; internal set; }
        public string attendancePresnt { get; internal set; }
        public string struckOff { get; internal set; }
        public string reasonFrSchoolLeav { get; internal set; }

        public string SelDist { get; internal set; }
        public string SelList { get; internal set; }
        public string SearchString { get; internal set; }

        public string SelForm { get; internal set; }
        public string SelLot { get; internal set; }
        public string SelFilter { get; internal set; }
        public string SchlCode { get; internal set; }


        public string idno { get; internal set; }
        public string Sno { get; internal set; }
        public string RegNo { get; internal set; }
        public string AdmDate { get; internal set; }
        public string Fee { get; internal set; }
        public string Lot { get; internal set; }
        public string Std_Sub { get; internal set; }

        public string TCrefno { get; internal set; }

        public string Pname { get; internal set; }
        public string PFname { get; internal set; }
        public string PMname { get; internal set; }

        public string TCdate { get; internal set; }

        public string distCode { get; internal set; }
        public string distName { get; internal set; }
        public string distNameP { get; internal set; }
        public string SCHLType { get; internal set; }
        public string SCHLfullNM_P { get; internal set; }
        public string SCHLfullNM_E { get; internal set; }
        public string Caste { get; internal set; }

        public string Religion { get; internal set; }
        public string Nation { get; internal set; }
        public string admno { get; internal set; }

        public string OROLL { get; internal set; }
        public string struckOffdt { get; internal set; }

        public string Result { get; internal set; }
        public string obtmark { get; internal set; }
        public string Totmark { get; internal set; }
        public string aadhar { get; internal set; }
        
        

    }
}