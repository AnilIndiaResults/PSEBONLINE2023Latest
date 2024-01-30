using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Data.Entity;
using PSEBONLINE.Models;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PSEBONLINE.AbstractLayer
{
    public static class StaticDB
    {
        private static readonly DBContext _context = new DBContext();
  

        #region ExceptionHistoryMasters
        public static void AddExceptionInDB(Exception ex, string ctr, string actn, string lineno, string id, string type, string ExptNo)
        {
            using (DbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    ExceptionHistoryMasters obj = new ExceptionHistoryMasters()
                    {
                        ExceptionHistoryId = 0,
                        AddedDate = DateTime.Now,
                        ApplicationType = type, 
                        CaseNoOrTrackNo = id,
                        ErrorMessage = ex.Message,
                        ControllerName = ctr,
                        ActionName = actn,
                        LineNoCsFile = lineno,
                        ExecptionNo = ExptNo,
                    };

                    _context.ExceptionHistoryMasters.Add(obj);
                    int insertedRecords = _context.SaveChanges();
                    transaction.Commit();//transaction commit
                }
                catch (Exception ex1)
                {
                    transaction.Rollback();
                }
            }
        }
        #endregion

        public static string GenerateFileName(string context)
        {
            //return context + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N");
            return context + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public static string GenerateTxnId()
        {
            Random rnd = new Random();
            byte[] message = Encoding.UTF8.GetBytes(rnd.ToString() + DateTime.Now);
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            //string strHash = Generatehash512(rnd.ToString() + DateTime.Now);
            string strHash = hex;
            string txnid = strHash.ToString().Substring(0, 15);
            return txnid;
        }

        public static string GetDateFormatDDMMYYYY(string dateVal)
        {
            string dt = "";
            DateTime DobFormat;
            if (DateTime.TryParseExact(dateVal, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DobFormat))
            {
                dt = DobFormat.ToString("dd/MM/yyyy");
            }

            return dt;
        }



        public static string GetCurrentYear()
        {
            return "2023-2024";
        }


            //public static string getIPAddressAll(HttpRequestBase Request)
            //{

            //    string OutputFullIPAddress;

            //    string hostName = System.Net.Dns.GetHostName(); // Retrive the Name of HOST
            //    string localIP = System.Net.Dns.GetHostByName(hostName).AddressList[0].ToString();

            //    // External IP Address(get your external IP locally)
            //    UTF8Encoding utf8 = new UTF8Encoding();
            //    WebClient webClient = new WebClient();
            //    string externalIp2 = utf8.GetString(webClient.DownloadData("https://whatismyip.com/automation/n09230945.asp"));


            //    string externalIP = (new System.Net.WebClient()).DownloadString("https://checkip.dyndns.org/");

            //    string RemoteADDR = Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName;
            //    string RemoteHOST = Dns.GetHostEntry(Request.ServerVariables["REMOTE_HOST"]).HostName;


            //    // Remote IP Address (useful for getting user's IP from public site; run locally it just returns localhost)  
            //    string remoteIpAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //    if (string.IsNullOrEmpty(remoteIpAddress))
            //    {
            //        remoteIpAddress = Request.ServerVariables["REMOTE_ADDR"];
            //    }


            //    OutputFullIPAddress = "HostNM:" + RemoteHOST + ",LocalIP:" + localIP + ",PublicIP:" + externalIP + ",RemoteIP:" + remoteIpAddress + ",RemoteADDR:" + RemoteADDR;

            //    return OutputFullIPAddress;
            //}

            public static string GetFullIPAddress()
        {

            string OutputFullIPAddress;

            string hostName = System.Net.Dns.GetHostName(); // Retrive the Name of HOST
            string localIP = System.Net.Dns.GetHostByName(hostName).AddressList[0].ToString();

            string externalIP;
            try
            {
                externalIP = (new System.Net.WebClient()).DownloadString("https://checkip.dyndns.org/");
                externalIP = externalIP.Split(':')[1].Replace("</body></html>", "");
            }
            catch (Exception ex)
            {
                externalIP = "ExIP: " + ex.Message;
            }

            string RemoteHOST;
            try
            {
               RemoteHOST = Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["REMOTE_HOST"]).HostName;
            }
            catch (Exception ex)
            {
                RemoteHOST = "HOST: " + ex.Message;
            }


            // Remote IP Address (useful for getting user's IP from public site; run locally it just returns localhost)  
            string remoteIpAddress;
            try
            {
                
                remoteIpAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(remoteIpAddress))
                {
                    remoteIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch (Exception ex)
            {
                remoteIpAddress = "RmIP: " + ex.Message;
            }

            OutputFullIPAddress = "HostNM:" + RemoteHOST + ",LocalIP:" + localIP + ",PublicIP:" + externalIP + ",RemoteIp:" + remoteIpAddress;
            return OutputFullIPAddress;
        }


        public static string DataTableToNewtonsoftJSON(DataTable dt)
        {
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(dt);
            return JSONresult;
        }

        #region DataTableToJSON
        public static object DataTableToJSON(DataTable table)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = (Convert.ToString(row[col]));
                }
                list.Add(dict);
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(list);
        }
        #endregion DataTableToJSON


        public static List<T> DataTableToList<T>(this DataTable table) where T : new()
        {
            List<T> list = new List<T>();
            var typeProperties = typeof(T).GetProperties().Select(propertyInfo => new
            {
                PropertyInfo = propertyInfo,
                Type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType
            }).ToList();

            foreach (var row in table.Rows.Cast<DataRow>())
            {
                T obj = new T();
                foreach (var typeProperty in typeProperties)
                {
                    object value = row[typeProperty.PropertyInfo.Name];
                    object safeValue = value == null || DBNull.Value.Equals(value)
                        ? null
                        : Convert.ChangeType(value, typeProperty.Type);

                    typeProperty.PropertyInfo.SetValue(obj, safeValue, null);
                }
                list.Add(obj);
            }
            return list;
        }


        public static DataTable ConvertTo<T>(IList<T> Items)
        {
            return ConvertTo<T>(Items, false);
        }

        public static DataTable CreateTable<T>(bool IsStringOnlyDataType)
        {
            Type entityType = typeof(T);
            DataTable dataTable = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor property in properties)
            {
                Type dataColumnType = property.PropertyType;
                if (IsStringOnlyDataType)
                    dataColumnType = typeof(string);

                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    NullableConverter converter = new NullableConverter(property.PropertyType);
                    dataTable.Columns.Add(property.Name, converter.UnderlyingType);
                }
                else
                {
                    dataTable.Columns.Add(property.Name, dataColumnType);
                }
            }

            return dataTable;
        }

    
        public static DataTable ConvertTo<T>(IList<T> Items, bool IsStringOnlyDataType)
        {
            Type entityType = typeof(T);
            DataTable dataTable = CreateTable<T>(IsStringOnlyDataType);

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            #region iterate and add rows

            foreach (T item in Items)
            {
                DataRow newRow = dataTable.NewRow();

                foreach (PropertyDescriptor property in properties)
                {
                    newRow[property.Name] = property.GetValue(item);
                }

                dataTable.Rows.Add(newRow);
            }

            #endregion

            return dataTable;
        }

        public static DataTable ConvertListToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prp = props[i];
                table.Columns.Add(prp.Name, prp.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public static DataTable RemoveEmptyAndDuplicateRowFromDataTable(DataTable dt,string ColumnName)
        {
            if (dt.Rows.Count > 0)
            {
                DataTable dt1 = (DataTable)dt;
                // dt1 = dt.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();
                dt1 = dt.Rows.Cast<DataRow>().Where(s => s.Field<string>(ColumnName) != "").CopyToDataTable();
                dt1 = RemoveduplicateRow(dt1, ColumnName);
                return dt1;
            }
            return dt;
        }
        public static DataTable RemoveduplicateRow(DataTable dt, string ColumnName)
        {
            if (dt.Rows.Count > 0)
            {
                DataTable dt1 = dt.AsEnumerable()
                     .GroupBy(x => x.Field<string>(ColumnName))
                     .Select(y => y.First())
                     .CopyToDataTable();
                return dt1;
            }
            return dt;
        }



        public static string GetFirmName(string UserNM)
        {            
            switch (UserNM)
            {
                case "CREA": UserNM = "CIPL"; break;
                case "SAI": UserNM = "SAI"; break;
                case "DATA": UserNM = "DATA"; break;
                case "PERF": UserNM = "PERF"; break;
                default: UserNM = "ADMIN"; break;
            }
            return UserNM;
        }

        //    Get All Action and Controller
        //     Start********* Get all ActionName of all Controller by return type;
        public static string GetActionsOfController()
        {
            string actionname= "";
            var asm = Assembly.GetExecutingAssembly();
            var controllerTypes = from d in asm.GetExportedTypes() where typeof(System.Web.Mvc.IController).IsAssignableFrom(d) select d;
            foreach (var val in controllerTypes)
            {
                actionname = get_all_action(val);
            }
            return actionname;
        }   
       public static string get_all_action(Type controllerType)
        {
            string methods = "";
            MethodInfo[] mi = controllerType.GetMethods();
            foreach (MethodInfo m in mi)
            {
                if (m.IsPublic)
                    if (typeof(System.Web.Mvc.JsonResult).IsAssignableFrom(m.ReturnParameter.ParameterType))
                        methods = "'" + m.Name + "' ,'" + controllerType.Name + "'" + Environment.NewLine + methods;
            }
            return methods;
        }

        // 
        public  static int[] StringToIntArray(this string value, char separator)
        {           
            return Array.ConvertAll(value.Split(separator), s => int.Parse(s));
        }

        public static string[] StringToStringArray(this string value, char separator)
        {
            return Array.ConvertAll(value.Split(separator), s => Convert.ToString(s));
        }

        // Check value exists in comma string
        public static bool ContainsValue(this string str, string value)
        {
            return str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                //.Select(x => int.Parse(x.Trim()))
                .Select(x => x.Trim())
                .Contains(value);
        }

        public static string GetCurrentYear(string Year)
        {            
            return Year;
        }

        // Check Array Duplicate Value
        public static bool  CheckArrayDuplicates(string[] array)
        {
            if (array.Count() > 1)
            {
                //array = array.ToList().Where(x => x != null).ToArray();
                array = array.ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray();
                var duplicates = array
                 .GroupBy(p => p)
                 .Where(g => g.Count() > 1 && g != null)
                 .Select(g => g.Key);

                int dc = duplicates.Count();// Count duplicate records
                return (duplicates.Count() > 0);
            }
            else
            {
                return false;
            }
            
        }


        public static bool DateInFormatOld(string value)
        {
            bool datestatus = true;
            Regex regex = new Regex(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$");

            //Verify whether date entered in dd/MM/yyyy format.
            bool isValid = regex.IsMatch(value.Trim());

            //Verify whether entered date is Valid date.
            DateTime dt;
            isValid = DateTime.TryParseExact(value, "dd/MM/yyyy", new CultureInfo("en-GB"), DateTimeStyles.None, out dt);
            if (!isValid)
            {
                // MessageBox.Show("Invalid Date.");
                datestatus = false;
            }
            
             return datestatus;
        }
        public static bool DateInFormat(string dateValue)
        {           
            DateTime dt;
            string[] formats = { "dd/MM/yyyy" };
            if (!DateTime.TryParseExact(dateValue, formats,
                            System.Globalization.CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out dt))
            {
                //your condition fail code goes here
                return false;
            }
            else
            {
                //success code
                return true;
            }
          
        }




    }
}