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
    public class SelfDeclarationRepository : ISelfDeclarationRepository
    {
        private DBContext db;
        public SelfDeclarationRepository()
        {
            db = new DBContext();
        }
        public Task<SelfDeclarationLoginSession> CheckLogin(SelfDeclarationLoginModel LM)  // Type 1=Regular, 2=Open
        {
            SelfDeclarationLoginSession loginSession = new SelfDeclarationLoginSession();
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SelfDeclarationLoginSP";
                cmd.Parameters.AddWithValue("@RP", LM.RP);
                cmd.Parameters.AddWithValue("@ROLL", LM.ROLL);
                cmd.Parameters.AddWithValue("@REGNO", LM.REGNO);
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        loginSession.CAT = DBNull.Value != reader["CAT"] ? (string)reader["CAT"] : default(string);
                        loginSession.CLASS = DBNull.Value != reader["CLASS"] ? (string)reader["CLASS"] : default(string);
                        loginSession.YEAR = DBNull.Value != reader["YEAR"] ? (string)reader["YEAR"] : default(string);
                        loginSession.MONTH = DBNull.Value != reader["MONTH"] ? (string)reader["MONTH"] : default(string);

                        loginSession.RP = DBNull.Value != reader["RP"] ? (string)reader["RP"] : default(string);
                        loginSession.ROLL = DBNull.Value != reader["ROLL"] ? (string)reader["ROLL"] : default(string);
                        loginSession.REGNO = DBNull.Value != reader["REGNO"] ? (string)reader["REGNO"] : default(string);


                        loginSession.NAME = DBNull.Value != reader["NAME"] ? (string)reader["NAME"] : default(string);
                        loginSession.FNAME = DBNull.Value != reader["FNAME"] ? (string)reader["FNAME"] : default(string);
                        loginSession.MNAME = DBNull.Value != reader["MNAME"] ? (string)reader["MNAME"] : default(string);

                        loginSession.RESULT = DBNull.Value != reader["RESULT"] ? (string)reader["RESULT"] : default(string);
                        loginSession.RESULTDTL = DBNull.Value != reader["RESULTDTL"] ? (string)reader["RESULTDTL"] : default(string);

                        loginSession.LoginStatus = DBNull.Value != reader["LoginStatus"] ? (int)reader["LoginStatus"] : default(int);
                    }
                }
                Thread.Sleep(2000);
            }
            catch (Exception)
            {
                loginSession = null;
            }
            return Task.FromResult(loginSession);
        }

        public Task<SelfDeclarations> GetDataByLoginDetails(SelfDeclarationLoginSession LM)  // Type 1=Regular, 2=Open
        {
            SelfDeclarations selfDeclarations = new SelfDeclarations();
            if (LM != null)
            {
                selfDeclarations = db.SelfDeclarations.SingleOrDefault(x => x.Roll.Trim() == LM.ROLL.Trim() && x.RegNo.Trim() == LM.REGNO.Trim());
            }
            Thread.Sleep(2000);
            return Task.FromResult(selfDeclarations);

        }


        public Task<SelfDeclarationLoginSession> CheckLoginAdditionSubject(SelfDeclarationLoginModel LM)  // Type 1=Regular, 2=Open
        {
            SelfDeclarationLoginSession loginSession = new SelfDeclarationLoginSession();
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SelfDeclarationAdditionSubjectLoginSP";
                cmd.Parameters.AddWithValue("@RP", LM.RP);
                cmd.Parameters.AddWithValue("@ROLL", LM.ROLL);
                cmd.Parameters.AddWithValue("@REGNO", LM.REGNO);
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        loginSession.CAT = DBNull.Value != reader["CAT"] ? (string)reader["CAT"] : default(string);
                        loginSession.CLASS = DBNull.Value != reader["CLASS"] ? (string)reader["CLASS"] : default(string);
                        loginSession.YEAR = DBNull.Value != reader["YEAR"] ? (string)reader["YEAR"] : default(string);
                        loginSession.MONTH = DBNull.Value != reader["MONTH"] ? (string)reader["MONTH"] : default(string);

                        loginSession.RP = DBNull.Value != reader["RP"] ? (string)reader["RP"] : default(string);
                        loginSession.ROLL = DBNull.Value != reader["ROLL"] ? (string)reader["ROLL"] : default(string);
                        //loginSession.REGNO = DBNull.Value != reader["REGNO"] ? (string)reader["REGNO"] : default(string);
                        loginSession.REGNO = DBNull.Value != reader["REFNO"] ? (string)reader["REFNO"] : default(string);


                        loginSession.NAME = DBNull.Value != reader["NAME"] ? (string)reader["NAME"] : default(string);
                        loginSession.FNAME = DBNull.Value != reader["FNAME"] ? (string)reader["FNAME"] : default(string);
                        loginSession.MNAME = DBNull.Value != reader["MNAME"] ? (string)reader["MNAME"] : default(string);

                        loginSession.RESULT = DBNull.Value != reader["RESULT"] ? (string)reader["RESULT"] : default(string);
                        loginSession.RESULTDTL = DBNull.Value != reader["RESULTDTL"] ? (string)reader["RESULTDTL"] : default(string);

                        loginSession.LoginStatus = DBNull.Value != reader["LoginStatus"] ? (int)reader["LoginStatus"] : default(int);
                    }
                }
                Thread.Sleep(2000);
            }
            catch (Exception)
            {
                loginSession = null;
            }
            return Task.FromResult(loginSession);
        }

        public Task<SelfDeclarations> GetDataByLoginDetailsAdditionSubject(SelfDeclarationLoginSession LM)  // Type 1=Regular, 2=Open
        {
            SelfDeclarations selfDeclarations = new SelfDeclarations();
            if (LM != null)
            {
                selfDeclarations = db.SelfDeclarations.SingleOrDefault(x => x.Roll.Trim() == LM.ROLL.Trim() && x.RegNo.Trim() == LM.REGNO.Trim());
            }
            Thread.Sleep(2000);
            return Task.FromResult(selfDeclarations);

        }

    }
}