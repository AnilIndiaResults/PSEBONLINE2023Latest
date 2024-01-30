using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PSEBONLINE.Models
{
    public class PayUVerifyResponseModel
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("transaction_details")]
        public PayUTransactionDetails transaction_details { get; set; }
    }    
   public class PayUTransactionDetails
    {
        [JsonProperty("mihpayid")]
        public string Mihpayid { get; set; }

        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        [JsonProperty("bank_ref_num")]
        public string BankRefNum { get; set; }

        [JsonProperty("amt")]
        public string Amt { get; set; }

        [JsonProperty("transaction_amount")]
        public string TransactionAmount { get; set; }

        [JsonProperty("txnid")]
        public string Txnid { get; set; }

        [JsonProperty("additional_charges")]
        public string AdditionalCharges { get; set; }

        [JsonProperty("productinfo")]
        public string Productinfo { get; set; }

        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        [JsonProperty("bankcode")]
        public string Bankcode { get; set; }

        [JsonProperty("udf1")]
        public string Udf1 { get; set; }

        [JsonProperty("udf3")]
        public string Udf3 { get; set; }

        [JsonProperty("udf4")]
        public string Udf4 { get; set; }

        [JsonProperty("udf5")]
        public string Udf5 { get; set; }

        [JsonProperty("field2")]
        public string Field2 { get; set; }

        [JsonProperty("field9")]
        public string Field9 { get; set; }

        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty("addedon")]
        public string Addedon { get; set; }

        [JsonProperty("payment_source")]
        public string PaymentSource { get; set; }

        [JsonProperty("card_type")]
        public string CardType { get; set; }

        [JsonProperty("error_Message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("net_amount_debit")]
        public string NetAmountDebit { get; set; }

        [JsonProperty("disc")]
        public string Disc { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("PG_TYPE")]
        public string PgType { get; set; }

        [JsonProperty("card_no")]
        public string CardNo { get; set; }

        [JsonProperty("udf2")]
        public string Udf2 { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("unmappedstatus")]
        public string Unmappedstatus { get; set; }

        [JsonProperty("Merchant_UTR")]
        public string MerchantUtr { get; set; }

        [JsonProperty("Settled_At")]
        public string SettledAt { get; set; }
    }
}