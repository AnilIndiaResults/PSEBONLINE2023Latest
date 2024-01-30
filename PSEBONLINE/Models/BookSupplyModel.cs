using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web.Mvc;


namespace PSEBONLINE.Models
{  

    public class PrinterModel
    {  
        public DataSet StoreAllData { get; set; }
        public int Id { get; set; }
        public string PrinterId { get; set; }
        public string  UserId { get; set; }
        public string UserType { get; set; }
        
        public string Password { get; set; }
        public string PrinterName { get; set; }
        public string Address { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pin { get; set; }
        public string ContactPerson1 { get; set; }
        public string MobileCP1 { get; set; }
        public string ContactPerson2 { get; set; }
        public string MobileCP2 { get; set; }
        public string TAN { get; set; }
        public string PAN { get; set; }
        public string GSTN { get; set; }
        public string EMAILID { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        //
        public string OldPassword { get; set; }
        public string Newpassword { get; set; }

    }

    public class BookPrintMaster
    {
        public DataSet StoreAllData { get; set; }
        public int Id { get; set; }
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string Color { get; set; }
        public string Class { get; set; }
        public string Medium { get; set; }
        public string Size { get; set; }
        public int NOPB { get; set; }
        public int Sale { get; set; }
        public int Ssadwo { get; set; }
        public string RNN { get; set; }
        public string TextPaper { get; set; }
        public string CoverPaper { get; set; }
        public int Numberofbookstobeprinted { get; set; }
        public int IsActive { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }


    
    public class AssignmentofBooksforprinting
    {
        public DataSet StoreAllData { get; set; }
        public int Id { get; set; }
        public string TransId { get; set; }
        public string PrinterId { get; set; }
        public string BookId { get; set; }
        public int QtyofBooksforprinting { get; set; }
        public string DateStamp { get; set; }
        public string Remarks { get; set; }
        public int Lot { get; set; }
        public string Typeofbooks { get; set; }
        public int IsActive { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }

    public class SupplyofBooks
    {
        public DataSet StoreAllData { get; set; }
        public int SupplyId { get; set; }
        public string DepotUserId { get; set; }
        public string TransId { get; set; }
        public string PrinterId { get; set; }
        public int NumberofSuppliedbooks { get; set; }
        public string DateStamp { get; set; }
        public string Remarks { get; set; }
        public int SupplyLot { get; set; }
        public string Type { get; set; }
        public int IsActive { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }

}