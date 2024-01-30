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
    public interface ISelfDeclarationRepository
    {
        //Task<SelfDeclarationLoginSession> CheckLogin(SelfDeclarationLoginModel LM) 
        Task<SelfDeclarationLoginSession> CheckLogin(SelfDeclarationLoginModel LM);
        Task<SelfDeclarations> GetDataByLoginDetails(SelfDeclarationLoginSession LM);


        // for Addition subject student

        Task<SelfDeclarationLoginSession> CheckLoginAdditionSubject(SelfDeclarationLoginModel LM);
        Task<SelfDeclarations> GetDataByLoginDetailsAdditionSubject(SelfDeclarationLoginSession LM);



    }
}