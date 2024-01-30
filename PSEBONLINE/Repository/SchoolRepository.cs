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
    public interface ISchoolRepository
    {
      
          

        #region PrivateStudents Signature Chart and Confidential List

        DataSet SignatureChartSP_PrivateStudents(int type, string cls, string SCHL, string cent);
        DataSet GetSignatureChartSP_PrivateStudents(SchoolModels sm);
        DataSet GetConfidentialListSP_PrivateStudents(SchoolModels sm);
        #endregion PrivateStudents Signature Chart and Confidential List 

        #region PhyChlMarksEntry Marks Entry Panel      

        DataSet GetPhyChlMarksEntryMarksDataBySCHL(string search, string schl, int pageNumber, string class1, int action1);
        string AllotPhyChlMarksEntryMarks(string submitby, string stdid, DataTable dtSub, string cls, out int OutStatus);
        DataSet PhyChlMarksEntryMarksEntryReport(string CenterId, int reporttype, string search, string schl, string cls, out string OutError);
        #endregion PhyChlMarksEntry Entry Panel


        //#region ReAppear Marks Entry Panel      

        //DataSet GetReAppearCCEMarksDataBySCHL(string search, string schl, int pageNumber, string class1, int action1);
        //string AllotReAppearCCEMarks(string submitby, string stdid, DataTable dtSub, string cls, out int OutStatus);
        //DataSet ReAppearCCEMarksEntryReport(string CenterId, int reporttype, string search, string schl, string cls, out string OutError);
        //#endregion ReAppear Entry Panel

        #region SchoolBasedExams Marks Entry Panel      

        DataSet GetSchoolBasedExamsMarksDataBySCHL(string search, string schl, int pageNumber, string class1, int action1);
        string AllotSchoolBasedExamsMarks(string submitby, string stdid, DataTable dtSub, string cls, out int OutStatus);
        DataSet SchoolBasedExamsMarksEntryReport(string CenterId, int reporttype, string search, string schl, string cls, out string OutError);


        DataSet GetSchoolBasedExamsMarksDataBySCHLOpen(string search, string schl, int pageNumber, string class1, int action1);
        string AllotSchoolBasedExamsMarksOpen(string submitby, string stdid, DataTable dtSub, string cls, out int OutStatus);
        DataSet SchoolBasedExamsMarksEntryReportOpen(string CenterId, int reporttype, string search, string schl, string cls, out string OutError);
        #endregion  SchoolBasedExams Marks Entry Panel     
    }

}