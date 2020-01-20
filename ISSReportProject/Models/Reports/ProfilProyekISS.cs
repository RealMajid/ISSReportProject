using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISSReportProject.Models.Reports
{
    public class ProfilProyekISS
    {
        public BluebookProfile BluebookProfileData { get; set; }
        public GreenbookProfile GreenbookProfileData { get; set; }
        public IEnumerable<Activitiy> ActivitiyData { get; set; }
        public IEnumerable<FundingSource> FundingSourceData { get; set; }
        public DaftarKegiatan DaftarKegiatanData { get; set; }
        public LoanAgreement LoanAgreementData { get; set; }
        public IEnumerable<KomponenDana> KomponenDanaData { get; set; }
        public TermsAndConditions TermsAndConditionsData { get; set; }
        public class BluebookProfile
        {
            public string BbNo { get; set; }
            public string ProjectTitle { get; set; }
            public string ProgramTitle { get; set; }
            public string ExecutingAgency { get; set; }
            public string ImplementingAgency { get; set; }
            public string Duration { get; set; }
            public string Location { get; set; }
            public string ScopeOfWork { get; set; }
            public string Outputs { get; set; }
            public double Loan { get; set; }
            public double Grant { get; set; }
            public double CentralGovernment { get; set; }
            public double RegionalGovernment { get; set; }
            public double StateOwnedEnterprise { get; set; }
            public double Others { get; set; }
        }

        public class GreenbookProfile
        {
            public string GbNo { get; set; }
            public string ExecutingAgency { get; set; }
            public string ImplementingAgency { get; set; }
            public string Duration { get; set; }
            public string Location { get; set; }
            public string Objectives { get; set; }
            public string ScopeOfWork { get; set; }
        }

        public class Activitiy
        {
            public string Activity { get; set; }
            public string ImplementationLocations { get; set; }
            public string ProjectImplementationUnits { get; set; }
        }

        public class FundingSource
        {
            public string ImplementingAgency { get; set; }
            public double LoanAmount { get; set; }
            public string Source { get; set; }
        }

        public class DaftarKegiatan 
        {
            public string NomorSuratDaftarKegiatan { get; set; }
            public DateTime TglSuratDaftarKegiatan { get; set; }
            public string ImplementingAgency { get; set; }
        }

        public class LoanAgreement
        {
            public string ImplementingAgency { get; set; }
            public string ImplementasiProyek { get; set; }
            public string KomponenProyek { get; set; }
            public string Output { get; set; }
            public string Lokasi { get; set; }
            public double Kontrak { get; set; }
        }

        public class KomponenDana
        {
            public string JenisKomponen { get; set; }
            public double Jumlah { get; set; }
            public string Keterangan { get; set; }
        }

        public class TermsAndConditions
        {
            public double LoanAmount { get; set; }
            public string Tenor { get; set; }
            public string GracePeriod { get; set; }
            public string RepaymentPeriod { get; set; }
            public string InterestRate { get; set; }
            public string ManagementFee { get; set; }
            public string CommitmentFee { get; set; }
        }
    }
}