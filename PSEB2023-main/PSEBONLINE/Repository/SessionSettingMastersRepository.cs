using Microsoft.Practices.EnterpriseLibrary.Data;
using PSEBONLINE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PSEBONLINE.Repository
{

    public static class SessionSettingMastersRepository
    {
        private readonly static DBContext _context = new DBContext();

      
        public static SessionSettingMasters GetSessionSettingMasters()
        {                        
                SessionSettingMasters obj = _context.SessionSettingMasters.FirstOrDefault();
                return obj;
        }




    }

}