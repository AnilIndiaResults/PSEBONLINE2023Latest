using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSEBONLINE.Models
{
    public class ServiceResponce<T>
    {
        public T Data { get; set; }
        public List<T> DataList { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        //public int TotalPage { get; set; }
        public int TotalRecord { get; set; }
      //  public int PageSize { get; set; }
    }
    public class ServiceResponceReport
    {
        public string Received { get; set; }
        public string Dispached { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class StatusResponse
    {
        [JsonProperty(PropertyName = "Status Code")]
        public string StatusCode { get; set; }
        [JsonProperty(PropertyName = "Status Message")]
        public string StatusMessage { get; set; }
    }


    public class ATOMServiceResponce<T>
    {
        public StatusResponse StatusResponse { get; set; }
        public T Data { get; set; }
        public List<T> DataList { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }        
        public int TotalRecord { get; set; }        
    }
}