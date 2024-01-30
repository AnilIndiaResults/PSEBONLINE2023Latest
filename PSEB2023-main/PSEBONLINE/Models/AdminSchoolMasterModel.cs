using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace PSEBONLINE.Models
{
    public class AdminSchoolMasterModel
    {
        public int orgID { get; set; }
        public string orgName { get; set; }
        public DataSet StoreAllData { get; set; }

        public string status { get; set; }
        public string session { get; set; }
        public string dist { get; set; }
        public string schl { get; set; }
        public string idno { get; set; }
        public string OCODE { get; set; }
        public string CLASS { get; set; }
        public string AREA { get; set; }
        public string SCHLP { get; set; }
        public string STATIONP { get; set; }
        public string SCHLE { get; set; }
        public string STATIONE { get; set; }
        public string DISTE { get; set; }
        public string DISTP { get; set; }
        public string DISTNM { get; set; }
        public string MATRIC { get; set; }
        public string HUM { get; set; }
        public string SCI { get; set; }
        public string COMM { get; set; }
        public string VOC { get; set; }
        public string TECH { get; set; }
        public string AGRI { get; set; }
        public string OMATRIC { get; set; }
        public string OHUM { get; set; }
        public string OSCI { get; set; }
        public string OCOMM { get; set; }
        public string OVOC { get; set; }
        public string OTECH { get; set; }
        public string OAGRI { get; set; }
        public string IDATE { get; set; }
        public string VALIDITY { get; set; }
        public string UDATE { get; set; }
        public string REMARKS { get; set; }
        public int id { get; set; }
        public string middle { get; set; }
        public string omiddle { get; set; }
        public string correctionno { get; set; }
        public string DISTNMPun { get; set; }
        public string username { get; set; }
        public string userip { get; set; }
        public string ImpschlOcode { get; set; }
        public string SSET { get; set; }
        public string MSET { get; set; }
        public string SOSET { get; set; }
        public string MOSET { get; set; }
        public string MID_CR { get; set; }
        public string MID_NO { get; set; }
        public string MID_YR { get; set; }
        public int MID_S { get; set; }

        public string MID_DNO { get; set; }
        public string HID_CR { get; set; }
        public string HID_NO { get; set; }
        public string HID_YR { get; set; }
        public int HID_S { get; set; }

        public string HID_DNO { get; set; }
        public string SID_CR { get; set; }
        public string SID_NO { get; set; }
        public string SID_DNO { get; set; }
        public string H { get; set; }
        public string HYR { get; set; }

        public int H_S { get; set; }
        public string C { get; set; }
        public string CYR { get; set; }
        public int C_S { get; set; }
        public string S { get; set; }
        public string SYR { get; set; }
        public int S_S { get; set; }

        public string A { get; set; }
        public string AYR { get; set; }
        public int A_S { get; set; }

        public string V { get; set; }
        public string VYR { get; set; }
        public int V_S { get; set; }

        public string T { get; set; }
        public string TYR { get; set; }
        public int T_S { get; set; }

        public string MID_UTYPE { get; set; }
        public string HID_UTYPE { get; set; }
        public string H_UTYPE { get; set; }
        public string S_UTYPE { get; set; }
        public string C_UTYPE { get; set; }
        public string V_UTYPE { get; set; }
        public string A_UTYPE { get; set; }
        public string T_UTYPE { get; set; }

        public string Tcode { get; set; }
        public string Tehsile { get; set; }
        public string Tehsilp { get; set; }

        //-tblschUser
        public string USER { get; set; }
        public string SCHL { get; set; }
        public string PASSWORD { get; set; }
        public string PRINCIPAL { get; set; }
        public string STDCODE { get; set; }
        public string PHONE { get; set; }
        public string MOBILE { get; set; }
        public string EMAILID { get; set; }
        public string CONTACTPER { get; set; }
        public string CPSTD { get; set; }
        public string CPPHONE { get; set; }
        public string OtContactno { get; set; }
        public string ACTIVE { get; set; }
        public string USERTYPE { get; set; }
        public string ADDRESSE { get; set; }
        public string ADDRESSP { get; set; }
        public bool vflag { get; set; }
        public bool cflag { get; set; }
        public string DateFirstLogin { get; set; }
        public string Vcode { get; set; }
        public string Approved { get; set; }
        public bool schlInfoUpdFlag { get; set; }
        public string mobile2 { get; set; }
        public int PEND_RESULT { get; set; }
        public bool NSQF_flag { get; set; }

    }
}