using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using PSEBONLINE.Models;
using System.IO;
using System.Web.Mvc;
using Ionic.Zip;
using System.Web.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PSEBONLINE.AbstractLayer
{
    public class Gloabl
    {
        
        public static string PathPSEBAPI { get { return GetAppKeyValue("PSEBAPI", true); } }
        public static string PathAWSKey { get { return GetAppKeyValue("AWSKey", true); } }
        public static string PathAWSValue { get { return GetAppKeyValue("AWSValue", true); } }
        public static string PathExamImage { get { return GetAppKeyValue("PathExam", true); } }





        static string GetAppKeyValue(string appKey, bool isCreateFolder = false)
        {
            string keyValue = "";
            try
            {
                keyValue = ConfigurationManager.AppSettings.GetValues(appKey)[0];
                if (isCreateFolder)
                {
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(keyValue)))
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(keyValue));
                }
                keyValue = keyValue.Replace("~", "");
            }
            catch (Exception ex)
            {
                //new ClsException("Global.cs", "GetAppConfigPath", ex);
            }

            return keyValue;

        }



    }
}