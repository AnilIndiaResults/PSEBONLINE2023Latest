using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;

namespace PSEBONLINE.Models
{
    public class MigrateSchoolModels
    {
        // by Rohit

        public IList<SelectListItem> ErrorList { get; set; }
        //
        public DataSet StoreAllData;
        public string SchlCode { get; set; }                
        public string idno { get; set; }
        public string SchlName { get; set; }
        public string DistName { get; set; }
        public string DistNameP { get; set; }
        public string Status { get; set; }
        public string SelDist { get; set; }
        public string SelList { get; set; }
        public string RegEntry { get; set; }
        public string SearchString { get; set; }

        // RegEntryview

        public string Sno { get; set; }
        public string RegNo { get; set; }
        public string FormName { get; set; }
        public string Candi_Name { get; set; }
        public string Father_Name { get; set; }
        public string Mother_Name { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }                
        public string RegDate { get; set; }
        public string AdmDate { get; set; }
        public string Fee { get; set; }
        public string Lot { get; set; }
        public string SearchFilter { get; set; }

        // Migration Form

        public string MigrateNo { get; set; }
        public string StdId { get; set; }
        public string SchlCodeNew { get; set; }
        public string rdoDD { get; set; }
        public string rdoBrdRcpt { get; set; }
        public string Migrationfee { get; set; }
        public string DDRcptNo { get; set; }
        public string Amount { get; set; }
        public string DepositDt { get; set; }
        public string BankName { get; set; }

        public string DiryOrderNo { get; set; }
        public string OrderDt { get; set; }
        public string OrderBy { get; set; }
        public string Remark { get; set; }
        public string schlName { get; set; }

        public string newSchlDetail { get; set; }
        public string OldSchlDetail { get; set; }
        public string newForm { get; set; }
        public string Std_Sub { get; set; }
        public string Std_SubOld { get; set; }

        public string Pname { get; set; }
        public string PFname { get; set; }
        public string PMname { get; set; }

        public string SelForm { get; set; }
        public string SelLot { get; set; }
        public string SelFilter { get; set; }

        public string UserName { get; set; }
        public string Migrationdate { get; set; }
        public string RegSet { get; set; }
        
    }
}