using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSEBONLINE.ModelVM
{ 
    public class CommonResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public ErrorModel error { get; set; }
        public List<int> failedPacketIds { get; set; }

        public CommonResponse()
        {
            status = ResponseCode.Failed;
            message = string.Empty;
            error = new ErrorModel();
            failedPacketIds= new List<int>();
        }
    }

	public class ResponseCode
	{
		public const int Success = 1;
		public const int Failed = 0;
		public const int Pending = 2;
	}

	public class ErrorModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public ErrorModel()
        {
            //code 0 means unknown
            code = 0;
            message = details = string.Empty;
        }
    }


}