using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSEBONLINE.Models
{
    public class AtomViewModel
    {
        public AtomViewModel(string checkoutUrl)
        {
            CheckoutUrl = checkoutUrl;
        }
        public string CheckoutUrl { get; set; }
    }


    public class AtomSettlementTransactions
    {
        [Key]
        public long AtomId { get; set; }

        public string srNo { get; set; }

        public string merchantName { get; set; }

        public string merchantID { get; set; }

        public string atomTxnID { get; set; }

        public string txnState { get; set; }

        public string txnDate { get; set; }

        public string clientCode { get; set; }

        public string merchantTxnID { get; set; }

        public string product { get; set; }

        public string discriminator { get; set; }

        public string bankCardName { get; set; }

        public string bankRefNo { get; set; }

        public string refundRefNo { get; set; }

        public string grossTxnAmount { get; set; }

        public string txnCharges { get; set; }

        public string serviceTax { get; set; }

        public string sbCess { get; set; }

        public string krishiKalyanCess { get; set; }

        public string totalChargeable { get; set; }

        public string netAmountToBePaid { get; set; }

        public string paymentStatus { get; set; }

        public string settlementDate { get; set; }

        public string refundStatus { get; set; }

        
    }
}