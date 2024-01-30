using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace PSEBONLINE.Models
{
    public class DBContext : DbContext
    {
        public DBContext() : base("name=myDBConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        
            Database.SetInitializer<DBContext>(null);
            base.OnModelCreating(modelBuilder);
        }


        // Master Setting Class

        public DbSet<ExamCentreConfidentialResources> ExamCentreConfidentialResources { get; set; }
        public DbSet<ExamCentreResources> ExamCentreResources { get; set; }
        public DbSet<LateAdmissionStatusMasters> LateAdmissionStatusMasters { get; set; }
        public DbSet<SessionSettingMasters> SessionSettingMasters { get; set; }



        public DbSet<RegnoErrorSummaryDownloadDataViews> RegnoErrorSummaryDownloadDataViews { get; set; }
        public DbSet<BindDeoCentreMasterViews> BindDeoCentreMasterViews { get; set; }


        public DbSet<ReExamTermStudents> ReExamTermStudents { get; set; }

        public DbSet<ExamCategoryMasters> ExamCategoryMasters { get; set; }

        //

        public DbSet<OnDemandCertificatesIndividuals> OnDemandCertificatesIndividuals { get; set; }
     
        public DbSet<OnDemandCertificates> OnDemandCertificates { get; set; }
        public DbSet<OnDemandCertificatesAllStudentViews> OnDemandCertificatesAllStudentViews { get; set; }
        public DbSet<OnDemandCertificatesVerifiedStudentCompleteDetailsViews> OnDemandCertificatesVerifiedStudentCompleteDetailsViews { get; set; }
        public DbSet<OnDemandCertificatesVerifiedStudentViews> OnDemandCertificatesVerifiedStudentViews { get; set; }
        public DbSet<OnDemandCertificatesViews> OnDemandCertificatesViews { get; set; }
        public DbSet<OnDemandCertificate_ChallanDetailsViews> OnDemandCertificate_ChallanDetailsViews { get; set; }


        //
        //OnlineCentreCreation
        public DbSet<SchoolMasterForOnlineCentreCreationViews> SchoolMasterForOnlineCentreCreationViews { get; set; }

        public DbSet<OnlineCentreCreations> OnlineCentreCreations { get; set; }
        public DbSet<OnlineCentreCreationsViews> OnlineCentreCreationsViews { get; set; }
        public DbSet<OnlineCentreCreationsChallanViews> OnlineCentreCreationsChallanViews { get; set; }

        // AdminEmployeeMasters
        public DbSet<RegPhotoFromOldPathToNewPathViews> RegPhotoFromOldPathToNewPathViews { get; set; }
        public DbSet<AdminEmployeeMasters> AdminEmployeeMasters { get; set; }
        public DbSet<EAffiliationDocumentMaster> EAffiliationDocumentMasters { get; set; }


        public DbSet<UndertakingOfQuestionPapers> UndertakingOfQuestionPapers { get; set; }
        public DbSet<UndertakingOfQuestionPapersViews> UndertakingOfQuestionPapersViews { get; set; }
        public DbSet<SeniorStudentMatricResultMarksOpenViews> SeniorStudentMatricResultMarksOpenViews { get; set; }
        public DbSet<SeniorStudentMatricResultMarksOpens> SeniorStudentMatricResultMarksOpens { get; set; }

        //SeniorStudentMatricResultMarks
        public DbSet<SeniorStudentMatricResultMarksViews> SeniorStudentMatricResultMarksViews { get; set; }
        public DbSet<SeniorStudentMatricResultMarks> SeniorStudentMatricResultMarks { get; set; }

        //StudentPreviousYearMarks
        public DbSet<StudentPreviousYearMarks> StudentPreviousYearMarks { get; set; }

        public DbSet<MagazineSchoolRequirements> MagazineSchoolRequirements { get; set; }
        public DbSet<MagazineSchoolRequirementsChallanViews> MagazineSchoolRequirementsChallanViews { get; set; }

        //NSQF MAster
        public DbSet<tblOtherBoardDocumentsByAdminUsers> tblOtherBoardDocumentsByAdminUsers { get; set; }
        public DbSet<tblOtherBoardDocumentsBySchool> tblOtherBoardDocumentsBySchool { get; set; }
        public DbSet<TblNsqfMaster> TblNsqfMaster { get; set; }
        //

        public DbSet<DocumentVerifyingEmployeeDetails> DocumentVerifyingEmployeeDetails { get; set; }


        //PrivateCandidateCategoryMasters
        public DbSet<PrivateCandidateCategoryMasters> PrivateCandidateCategoryMasters { get; set; }


        //AffiliationContinuation
        public DbSet<AffiliationContinuationDashBoardViews> AffiliationContinuationDashBoardViews { get; set; }

        //AdditionalSectionClassMasters
        public DbSet<AdditionalSectionClassMasters> AdditionalSectionClassMasters { get; set; }
        public DbSet<AdditionalSectionDashBoardViews> AdditionalSectionDashBoardViews { get; set; }
        public DbSet<AffObjectionLettersViews> AffObjectionLettersViews { get; set; }
        public DbSet<AdditionalSectionAllowByAdmins> AdditionalSectionAllowByAdmins { get; set; }


        //EAffiliationClassMasters
        public DbSet<AffObjectionListMasters> AffObjectionListMasters { get; set; }
        public DbSet<AffObjectionLetters> AffObjectionLetters { get; set; }
        public DbSet<EAffiliationClassMasters> EAffiliationClassMasters { get; set; }


        //StudentSchoolMigrations
        public DbSet<StudentSchoolMigrations> StudentSchoolMigrations { get; set; }


        // ExceptionHistoryMasters
        public DbSet<ExceptionHistoryMasters> ExceptionHistoryMasters { get; set; }

        // AtomSettlementTransactions
        public DbSet<AtomSettlementTransactions> AtomSettlementTransactions { get; set; }

        //  public DbSet<EAffiliationDashBoardViewModel> eAffiliationDashBoardViewModels { get; set; }

        //OPEN
        public DbSet<OpenUserLogin> OpenUserLogin { get; set; }
        public DbSet<tblsubjectopen> tblsubjectopen { get; set; }
        //public DbSet<OpenUserRegistration> OpenUserRegistration { get; set; }
        //public DbSet<OpenUserSubjects> OpenUserSubjects { get; set; }
        // public DbSet<FeeOpen> FeeOpen { get; set; }

        // Registration
        public DbSet<RegistrationClassFormWiseDocumentMasters> RegistrationClassFormWiseDocumentMasters { get; set; }


        //SelfDeclarations
        public DbSet<SelfDeclarations> SelfDeclarations { get; set; }

        // ParticularCorrectionStaffDetails
        public DbSet<ParticularCorrectionStaffDetails> ParticularCorrectionStaffDetails { get; set; }

        public DbSet<InfrasturePerformas> InfrasturePerformas { get; set; }

        public DbSet<tblSchUsers> tblSchUsers { get; set; }
        public DbSet<Tblifsccodes> Tblifsccodes { get; set; }

        public DbSet<InfrasturePerformasList> InfrasturePerformasList { get; set; }
        public DbSet<InfrasturePerformasListWithSchool> InfrasturePerformasListWithSchool { get; set; }
        public DbSet<ChallanModels> ChallanModels { get; set; }

		
		public DbSet<AttendanceMemoDetail> attendanceMemoDetail { get; set; }
        public DbSet<AttendanceAdminReport> AttendanceAdmin { get; set; }

        public DbSet<SubjectAttendanceSeniorRegular> subjectAttendanceSeniorRegular { get; set; }
		public DbSet<SubjectAttendanceEighthRegular> subjectAttendanceEighthRegular { get; set; }
		public DbSet<SubjectAttendanceMatricRegular> subjectAttendanceMatricRegular { get; set; }
		public DbSet<SubjectAttendanceFifthRegular> subjectAttendanceFifthRegular { get; set; }
		public DbSet<SubjectAttendancePrivate> subjectAttendancePrivate { get; set; }
		public DbSet<SubjectAttendanceOpen> subjectAttendanceOpen { get; set; }
		public DbSet<AttendenceSummaryDetail> attendenceSummaryDetail { get; set; }
	}
}