namespace PSEBONLINE.Models
{
    public class CcAvenueViewModel
    {
        public CcAvenueViewModel(string encryptionRequest, string accessCode, string checkoutUrl)
        {
            EncryptionRequest = encryptionRequest;
            AccessCode = accessCode;
            CheckoutUrl = checkoutUrl;
        }

        public string EncryptionRequest { get; set; }
        public string AccessCode { get; set; }
        public string CheckoutUrl { get; set; }
    }

    public class PaymentSuccessModel
    {
        public string order_id { get; set; }
        public string tracking_id { get; set; }
        public string amount { get; set; }
        public string trans_date { get; set; }
        public string bank_ref_no { get; set; }
        public string order_status { get; set; }
        public string payment_mode { get; set; }
        public string merchant_param1 { get; set; }
        public string bankname { get; set; }
        public string bankcode { get; set; }



        public string FEECODE { get; set; }
        public string APPNO { get; set; }
        public string SCHLREGID { get; set; }
    }
}