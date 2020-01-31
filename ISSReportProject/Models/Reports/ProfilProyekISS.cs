using System;
using System.Collections.Generic;
using System.Globalization;
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
        public DisbursementPlan DisbursementPlanData { get; set; }
        public DaftarKegiatan DaftarKegiatanData { get; set; }
        public LoanAgreement LoanAgreementData { get; set; }
        public IEnumerable<KomponenDana> KomponenDanaData { get; set; }
        public TermsAndConditions TermsAndConditionsData { get; set; }
        public RegisterInformation RegisterInformationData { get; set; }
        public IEnumerable<AlokasiAPBN> AlokasiAPBNData { get; set; }
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

        public class DisbursementPlan
        {
            public double? First { get; set; }
            public double? Second { get; set; }
            public double? Third { get; set; }
            public double? Fourth { get; set; }
            public double? Fifth { get; set; }

            
            public double? this[int index]
            {
                get
                {
                    switch (index) {
                        case 0:
                            return First;
                        case 1:
                            return Second;
                        case 2:
                            return Third;
                        case 3:
                            return Fourth;
                        default:
                            return Fifth;
                    }
                }

                set
                {
                    switch (index)
                    {
                        case 0:
                            First = value;
                            break;
                        case 1:
                            Second = value;
                            break;
                        case 2:
                            Third = value;
                            break;
                        case 3:
                            Fourth = value;
                            break;
                        default:
                            Fifth = value;
                            break;
                    }
                }
            }
        }

        public class DaftarKegiatan 
        {
            public string NomorSuratDaftarKegiatan { get; set; }
            public DateTime TglSuratDaftarKegiatan { get; set; }
            public string ImplementingAgency { get; set; }
            public string TanggalSuratDaftarKegiatan
            {
                get { return TglSuratDaftarKegiatan.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("id")); }
            }
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
            public double Amount { get; set; }
            public string MataUang { get; set; }
            public string Keterangan { get; set; }
            public string Jumlah { 
                get 
                {
                    return $@"{MataUang} {Amount.ToString("N", CultureInfo.InvariantCulture)}";        
                }
            }
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
        
        public class RegisterInformation
        {
            public string LoanNo { get; set; }
            public string RegisterNo { get; set; }
        }

        public class AlokasiAPBN
        {
            public string NmDept { get; set; }
            public double PaguTahun1 { get; set; }
            public double RealisasiTahun1 { get; set; }
            public double PaguTahun2 { get; set; }
            public double RealisasiTahun2 { get; set; }
            public double PaguTahun3 { get; set; }
            public double RealisasiTahun3 { get; set; }
            public double PaguTahun4 { get; set; }
            public double RealisasiTahun4 { get; set; }
            public string LabelTahun { get; set; }
        }
    }
}