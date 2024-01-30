using Microsoft.Practices.EnterpriseLibrary.Data;
using PSEBONLINE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace PSEBONLINE.Repository
{

    public interface IAgencyRepository
    {
        int ChangePassword(string UserId, string CurrentPassword, string NewPassword);
        Task<AgencyLoginSession> CheckAgencyLogin(LoginModel LM);

        List<AgencySchoolModel> AgencyMasterSP(int type, int id, string cls, string sub, string Search, out DataSet ds1);
     

        List<TblNsqfMaster> TblNsqfMasterSP(int type, int id, string Search);

        //DataSet CheckSchlAllowToAgency(int type, string centerid, string schl, string Search);
        DataSet GetNSQFAssesmentDataFormat(int type, string username, string cls, string Search);

        DataSet NSQFPracExamMarksPendingSchoolList(int type, string Search);
        #region NSQF Marks Entry Panel 



        // SchoolAllowForMarksEntry SchoolAllowForMarksEntry(string SCHL, string cls);
        DataSet GetNSQFMarksEntryDataBySCHL(string search, string sub, string pcent, int pageNumber, string class1, int SelectedAction, string schl);
        string AllotNSQFMarksEntry(string submitby, string RP, DataTable dtSub, string class1, out int OutStatus, out string OutError);
        string RemoveNSQFPracMarks(string submitby, string RP, DataTable dtSub, string class1, out int OutStatus, out string OutError);

        DataSet ClassSubjectByAgencyId(int type, string agencyId, string cls, string sub, string Search);
        DataSet ViewNSQFMarksEntryFinalSubmit(string class1, string rp, string cent, string Search, int SelectedAction, int pageNumber, string sub, string schl); 
        DataSet NSQFMarksEntryReport(string CenterId, int reporttype, string search, string schl, string cls, out string OutError);
        //string NSQFPracExamFinalSubmit(string ExamCent, string class1, string RP, string cent, string sub, string schl, DataTable dtSub, out int OutStatus, out string OutError);


            //Final Submit 
        string NSQFPracExamAllSchoolsFinalSubmit(string class1, string cent, string sub,  out string OutError);
        #endregion NSQF Marks Entry Panel


    }

}