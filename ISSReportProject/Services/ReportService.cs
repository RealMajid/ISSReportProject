using ISSReportProject.Interfaces;
using ISSReportProject.Models;
using ISSReportProject.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ISSReportProject.Services
{
    public class ReportService : IReportService
    {
        private LNKLPContext db = new LNKLPContext();
        private LNKLOANContext dbLoan = new LNKLOANContext();

        public async Task<ProfilProyekISS> GetRptProfilProyekISS(double proyekId)
        {
            var profilProyek = new ProfilProyekISS();

            var loanId = await Task.Run(() =>
            {
                return db.Database.SqlQuery<string>("SELECT LOAN_ID FROM PROJECT_LOAN WHERE PROJECT_ID = :proyekId", proyekId).FirstOrDefault();
            });

            var sqlBluebookProfile = $@"SELECT NO_BB BbNo, NAME ProjectTitle, PROGRAMLOAN ProgramTitle, EANAME ExecutingAgency, KL ImplementingAgency, DURATION Duration, Location, SCOPEOFWORK ScopeOfWork, TARGET Outputs, SUM(PINJAMAN) Loan, SUM(HIBAH) ""Grant"",
                        SUM(PEMERINTAH_PUSAT) CentralGovernment, SUM(PEMERINTAH_DAERAH) RegionalGovernment, SUM(BADAN_USAHA) StateOwnedEnterprise, SUM(LAINNYA) Others FROM(
                        SELECT a.NO_BB, c.NAME, c.PROGRAMLOAN, c.EANAME, d.KL, c.DURATION, Location , c.SCOPEOFWORK, c.TARGET,
                        CASE WHEN f.CD_COST = '001' THEN f.AMOUNT ELSE 0 END PINJAMAN,
                        CASE WHEN f.CD_COST = '002' THEN f.AMOUNT ELSE 0 END HIBAH,
                        CASE WHEN f.CD_COST = '003' THEN f.AMOUNT ELSE 0 END PEMERINTAH_PUSAT,
                        CASE WHEN f.CD_COST = '004' THEN f.AMOUNT ELSE 0 END PEMERINTAH_DAERAH,
                        CASE WHEN f.CD_COST = '005' THEN f.AMOUNT ELSE 0 END BADAN_USAHA,
                        CASE WHEN f.CD_COST = '006' THEN f.AMOUNT ELSE 0 END LAINNYA
                        FROM VW_BLUEBOOK a
                        INNER JOIN VW_BLUEBOOK_PROJECT b ON a.BB_ID = b.BB_ID
                        INNER JOIN VW_PROJECT c ON b.PROJECT_ID = c.PROJECT_ID
                        LEFT JOIN(
                                                        SELECT PROJECT_ID,
                                                        LISTAGG(CD_KL,',') WITHIN GROUP(ORDER BY CD_KL) AS CD_KL,
                                                        LISTAGG(KL, ', ') WITHIN GROUP(ORDER BY KL) AS KL

                                                        FROM VW_PROJECT_IA

                                                        GROUP BY PROJECT_ID
												) d ON b.PROJECT_ID = d.PROJECT_ID
                        LEFT JOIN(
                            SELECT PROJECT_ID,
                            LISTAGG(CD_PROVINSI, ',') WITHIN GROUP(ORDER BY CD_PROVINSI) AS CD_PROVINSI,
                            LISTAGG(CD_KOTA, ',') WITHIN GROUP(ORDER BY CD_KOTA) AS CD_KOTA,
                            LISTAGG(
	                        CASE WHEN KOTA IS NOT NULL THEN KOTA || ' - ' || PROVINSI ELSE PROVINSI END
                            , ', ') WITHIN GROUP(ORDER BY CD_PROVINSI) AS LOCATION

                            FROM VW_PROJECT_LOCATION

                            GROUP BY PROJECT_ID
                        ) e ON b.PROJECT_ID = e.PROJECT_ID
                        LEFT JOIN VW_PROJECT_COST f ON b.PROJECT_ID = f.PROJECT_ID
                        WHERE c.PROJECT_ID = :proyekId)
                        GROUP BY NO_BB, NAME, PROGRAMLOAN, EANAME, KL, DURATION, Location, SCOPEOFWORK, TARGET";

            profilProyek.BluebookProfileData = await Task.Run(() => {
                return db.Database.SqlQuery<ProfilProyekISS.BluebookProfile>(sqlBluebookProfile, proyekId).FirstOrDefault(); ;
            });

            var sqlGreenbookProfile = $@"SELECT a.NO_GB GbNo, b.EANAME ExecutingAgency, d.KL ImplementingAgency, c.DURATION Duration, Location, c.TARGET Objectives, c.SCOPEOFWORK ScopeOfWork
                                        FROM VW_GREENBOOK a
                                        INNER JOIN VW_GREENBOOK_PROJECT b ON a.GB_ID = b.GB_ID
                                        INNER JOIN VW_PROJECT c ON b.PROJECT_ID = c.PROJECT_ID
                                        LEFT JOIN (
                                            SELECT PROJECT_ID, 
                                            LISTAGG(CD_KL,',') WITHIN GROUP(ORDER BY CD_KL) AS CD_PROVINSI,
                                            LISTAGG(KL,', ') WITHIN GROUP(ORDER BY KL) AS KL
                                            FROM VW_PROJECT_IA
                                            GROUP BY PROJECT_ID
                                        ) d ON b.PROJECT_ID = d.PROJECT_ID
                                        LEFT JOIN (
	                                        SELECT PROJECT_ID, 
	                                        LISTAGG(CD_PROVINSI, ',') WITHIN GROUP (ORDER BY CD_PROVINSI) AS CD_PROVINSI,
	                                        LISTAGG(CD_KOTA,',') WITHIN GROUP (ORDER BY CD_KOTA) AS CD_KOTA,
	                                        LISTAGG(
	                                        CASE WHEN KOTA IS NOT NULL THEN KOTA || ' - ' || PROVINSI ELSE PROVINSI END
	                                        , ', ') WITHIN GROUP (ORDER BY CD_PROVINSI) AS LOCATION
	                                        FROM VW_PROJECT_LOCATION
	                                        GROUP BY PROJECT_ID
                                        ) e ON b.PROJECT_ID = e.PROJECT_ID
	                                        WHERE b.PROJECT_ID = :proyekId";

            profilProyek.GreenbookProfileData = await Task.Run(() =>
            {
                return db.Database.SqlQuery<ProfilProyekISS.GreenbookProfile>(sqlGreenbookProfile, proyekId).FirstOrDefault(); ;
            });

            var sqlActivity = $@"
                SELECT a.ACTIVITY Activity, b.LOCATION ImplementationLocations, c.KL ProjectImplementationUnits  FROM VW_PROJECT_ACTIVITY a
                LEFT JOIN(
                    SELECT PROJECT_ID,
                    LISTAGG(CD_PROVINSI, ',') WITHIN GROUP(ORDER BY CD_PROVINSI) AS CD_PROVINSI,
                   LISTAGG(CD_KOTA, ',') WITHIN GROUP(ORDER BY CD_KOTA) AS CD_KOTA,
                   LISTAGG(
	                CASE WHEN KOTA IS NOT NULL THEN KOTA || ' - ' || PROVINSI ELSE PROVINSI END
                   , ', ') WITHIN GROUP(ORDER BY CD_PROVINSI) AS LOCATION

                    FROM VW_PROJECT_LOCATION

                    GROUP BY PROJECT_ID
                ) b ON a.PROJECT_ID = b.PROJECT_ID
                LEFT JOIN(
                    SELECT PROJECT_ID,
                    LISTAGG(CD_KL,',') WITHIN GROUP(ORDER BY CD_KL) AS CD_PROVINSI,
                    LISTAGG(KL, ', ') WITHIN GROUP(ORDER BY KL) AS KL

                    FROM VW_PROJECT_IA

                    GROUP BY PROJECT_ID
                ) c ON a.PROJECT_ID = c.PROJECT_ID
                WHERE a.PROJECT_ID = :proyekId
                ORDER BY a.PROJECT_ID
            ";
            profilProyek.ActivitiyData = await Task.Run(() => {
                return db.Database.SqlQuery<ProfilProyekISS.Activitiy>(sqlActivity, proyekId).AsEnumerable();
            });

            var sqlFundingSource = $@"      
                SELECT KL ImplementingAgency, BIAYA LoanAmount, LINGKUP Source FROM VW_PROJECT_IA
                WHERE PROJECT_ID = :proyekId
            ";

            profilProyek.FundingSourceData = await Task.Run(() =>
            {
                return db.Database.SqlQuery<ProfilProyekISS.FundingSource>(sqlFundingSource, proyekId).AsEnumerable();
            });

            var sqlDisbursementPlan = $@"
                SELECT LO_NO LoanId, YEAR Year, SUM(AMT) Amount FROM (
                SELECT LO_NO, EXTRACT(YEAR FROM D_SCH) YEAR, AMT FROM SCH_DSBS WHERE LO_NO = :loanId AND 
                EXTRACT(YEAR FROM D_SCH) BETWEEN EXTRACT(YEAR FROM SYSDATE) + 1 AND EXTRACT(YEAR FROM SYSDATE) + 5)
                GROUP BY LO_NO, YEAR
                ORDER BY YEAR
            ";

            profilProyek.DisbursementPlanData = await Task.Run(() => {
                var scheduleDisbursements = dbLoan.Database.SqlQuery<ScheduleDisbursement>(sqlDisbursementPlan, loanId).ToList();
                var disbursementPlan = new ProfilProyekISS.DisbursementPlan();

                for (var i = 0; i < scheduleDisbursements.Count; i++)
                {
                    disbursementPlan[i] = scheduleDisbursements[i].Amount;
                }

                return disbursementPlan;
            });

            var sqlDaftarKegiatan = $@"
                SELECT c.NO_DOKUMEN NomorSuratDaftarKegiatan, c.TGL_DOKUMEN TglSuratDaftarKegiatan, a.EANAME ImplementingAgency FROM VW_PROJECT a
                INNER JOIN DAFTARKEGIATAN_PROJECT b
                ON a.PROJECT_ID = b.PROJECT_ID
                INNER JOIN DAFTARKEGIATAN c
                ON b.DK_ID = c.DK_ID
                WHERE a.PROJECT_ID = :proyekId
            ";

            profilProyek.DaftarKegiatanData = await Task.Run(() =>
            {
                return db.Database.SqlQuery<ProfilProyekISS.DaftarKegiatan>(sqlDaftarKegiatan, proyekId).FirstOrDefault(); ;
            });

            var sqlLoanAgreement = $@"
                SELECT b.KL ImplementingAgency, a.DURATION ImplementasiProyek, a.TARGET KomponenProyek, a.TARGET Output, c.LOCATION Lokasi FROM VW_PROJECT a 
                LEFT JOIN(
		                SELECT PROJECT_ID,
		                LISTAGG(CD_KL,',') WITHIN GROUP(ORDER BY CD_KL) AS CD_KL,
		                LISTAGG(KL, ', ') WITHIN GROUP(ORDER BY KL) AS KL

		                FROM VW_PROJECT_IA

		                GROUP BY PROJECT_ID
                ) b ON a.PROJECT_ID = b.PROJECT_ID
                 LEFT JOIN(
		                SELECT PROJECT_ID,
		                LISTAGG(CD_PROVINSI, ',') WITHIN GROUP(ORDER BY CD_PROVINSI) AS CD_PROVINSI,
		                LISTAGG(CD_KOTA, ',') WITHIN GROUP(ORDER BY CD_KOTA) AS CD_KOTA,
		                LISTAGG(
	                CASE WHEN KOTA IS NOT NULL THEN KOTA || ' - ' || PROVINSI ELSE PROVINSI END
		                , ', ') WITHIN GROUP(ORDER BY CD_PROVINSI) AS LOCATION

		                FROM VW_PROJECT_LOCATION

		                GROUP BY PROJECT_ID
                ) c ON b.PROJECT_ID = c.PROJECT_ID
                WHERE a.PROJECT_ID = :proyekId
            ";

            profilProyek.LoanAgreementData = await Task.Run(() => {
                return db.Database.SqlQuery<ProfilProyekISS.LoanAgreement>(sqlLoanAgreement, proyekId).FirstOrDefault();
            });

            var sqlKomponenDana = $@"
                    SELECT CATEGORY JenisKomponen, JUMLAH Amount, CURRENCY MataUang, JENIS Keterangan FROM VW_PROJECT_DANA
                    WHERE PROJECT_ID = :proyekId
                ";

            profilProyek.KomponenDanaData = await Task.Run(() =>
            {
                return db.Database.SqlQuery<ProfilProyekISS.KomponenDana>(sqlKomponenDana, proyekId).AsEnumerable();
            });

            //var sqlTermsAndConditions = $@"SELECT 1000 LoanAmount, '15 Years' Tenor, '5 Years' GracePeriod, '10 Years' RepaymentPeriod, '3% p.a.' InterestRate,
            //    '0.2% flat' ManagementFee, '0.2% p.a.' CommitmentFee FROM DUAL";
            
            if (loanId != null)
            {
                profilProyek.TermsAndConditionsData = await Task.Run(() =>
                {
                    var termsAndConditions = new ProfilProyekISS.TermsAndConditions();

                    var loanTranches = dbLoan.Database.SqlQuery<LoanTranches>("SELECT DATE_EFFECTIVE DateEffective, PRINCIPAL_LAST_PMT PrincipalLastPayment, INTEREST_LAST_PMT InterestLastPayment, GRACE_PERIOD GracePeriod, INTEREST_RATE InterestRate, LOAN_COMMISSION_FEE_TYPE LoanCommissionFeeType, COMMITMENT_RATE CommitmentRate FROM DMFASVWS.LOAN_TRANCHES WHERE LOAN_ID = :loanId", loanId).FirstOrDefault();

                    termsAndConditions.LoanAmount = dbLoan.Database.SqlQuery<double>("SELECT AMT FROM DMFAS.LOANS WHERE LO_NO = :loanId", loanId).FirstOrDefault();

                    if (loanTranches != null)
                    {
                        if (loanTranches.InterestLastPayment == null && loanTranches.PrincipalLastPayment != null)
                        {
                            termsAndConditions.Tenor = ((loanTranches.PrincipalLastPayment - loanTranches.DateEffective).Value.Days / 365).ToString();
                            termsAndConditions.Tenor += Convert.ToInt32(termsAndConditions.Tenor) > 1 ? " Years" : " Year";
                        }
                        else if (loanTranches.PrincipalLastPayment == null && loanTranches.InterestLastPayment != null)
                        {
                            termsAndConditions.Tenor = ((loanTranches.InterestLastPayment - loanTranches.DateEffective).Value.Days / 365).ToString();
                            termsAndConditions.Tenor += Convert.ToInt32(termsAndConditions.Tenor) > 1 ? " Years" : " Year";
                        }
                        else
                        {
                            termsAndConditions.Tenor = null;
                        }

                        var gracePeriod = loanTranches.GracePeriod.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        termsAndConditions.GracePeriod = $@"{gracePeriod[0]?[0].ToString()} Year(s)  {gracePeriod[1]?[0].ToString()} Month(s)  {gracePeriod[2]?[0].ToString()} Day(s)";

                        termsAndConditions.InterestRate = loanTranches.InterestRate.ToString() + " p.a";
                        if (loanTranches.LoanCommissionFeeType == "COMMITMENT FEE")
                        {
                            termsAndConditions.CommitmentFee = loanTranches.CommitmentRate.ToString();
                        }
                        else if (loanTranches.LoanCommissionFeeType == "MANAGEMENT FEE")
                        {
                            termsAndConditions.ManagementFee = loanTranches.CommitmentRate.ToString();
                        }
                    }

                    return termsAndConditions;
                });

                var sqlRegisterInformation = $@"SELECT LO_NO LoanNo, UD1 RegisterNo FROM DMFAS.LOANS WHERE LO_NO = :loanId";
                profilProyek.RegisterInformationData = await Task.Run(() =>
                {
                    return dbLoan.Database.SqlQuery<ProfilProyekISS.RegisterInformation>(sqlRegisterInformation, loanId).FirstOrDefault();
                });

                profilProyek.AlokasiAPBNData = await Task.Run(() => {
                    var lengthTahunAPBN = 4;
                    var sqlYear = string.Empty;

                    var tahun = DateTime.Now.Year;
                    var startTahun = tahun - lengthTahunAPBN;
                    var endTahun = tahun - 1;
                    var labelTahun = string.Empty;

                    for (var i = 0; i < lengthTahunAPBN; i++)
                    {
                        var tahunAPBN = startTahun + i;
                        sqlYear += $@", CASE WHEN TA = {tahunAPBN} THEN PAGU ELSE 0 END {tahunAPBN}";
                        if (tahunAPBN == endTahun)
                        {
                            labelTahun += tahunAPBN.ToString();
                        }
                        else
                        {
                            labelTahun += $@"{tahunAPBN.ToString()},";
                        }
                    }

                    var sqlAlokasiAPBN = $@"
                        SELECT NMDEPT, LabelTahun, 
                        SUM(PAGUTAHUN1) PAGUTAHUN1,
                        SUM(REALISASITAHUN1) REALISASITAHUN1,
                        SUM(PAGUTAHUN2) PAGUTAHUN2,
                        SUM(REALISASITAHUN2) REALISASITAHUN2,
                        SUM(PAGUTAHUN3) PAGUTAHUN3,
                        SUM(REALISASITAHUN3) REALISASITAHUN3,
                        SUM(PAGUTAHUN4) PAGUTAHUN4,
                        SUM(REALISASITAHUN4) REALISASITAHUN4 FROM (
                        SELECT NMDEPT, '{labelTahun}' LabelTahun,
                        CASE WHEN TA = EXTRACT(YEAR FROM SYSDATE) - 4 THEN PAGU ELSE 0 END PAGUTAHUN1,
                        CASE WHEN TA = EXTRACT(YEAR FROM SYSDATE) - 4 THEN REALISASI ELSE 0 END REALISASITAHUN1,
                        CASE WHEN TA = EXTRACT(YEAR FROM SYSDATE) - 3 THEN PAGU ELSE 0 END PAGUTAHUN2,
                        CASE WHEN TA = EXTRACT(YEAR FROM SYSDATE) - 3 THEN REALISASI ELSE 0 END REALISASITAHUN2,
                        CASE WHEN TA = EXTRACT(YEAR FROM SYSDATE) - 2 THEN PAGU ELSE 0 END PAGUTAHUN3,
                        CASE WHEN TA = EXTRACT(YEAR FROM SYSDATE) - 2 THEN REALISASI ELSE 0 END REALISASITAHUN3,
                        CASE WHEN TA = EXTRACT(YEAR FROM SYSDATE) - 1 THEN PAGU ELSE 0 END PAGUTAHUN4,
                        CASE WHEN TA = EXTRACT(YEAR FROM SYSDATE) - 1 THEN REALISASI ELSE 0 END REALISASITAHUN4
                        FROM (
                        SELECT NMDEPT, REGISTER, SUM(PAGU) PAGU, SUM(RPHREAL) REALISASI, TA FROM (
                        SELECT b.NMDEPT, a.REGISTER, a.PAGU, c.RPHREAL, a.TA  FROM 
                        LNKEXTERNAL.DJA_PAGU a
                        INNER JOIN LNKEXTERNAL.DJA_T_DEPT b
                        ON a.KDDEPT = b.KDDEPT
                        INNER JOIN LNKEXTERNAL.DJPBN_T_BELANJA c
                        ON a.REGISTER = c.REGISTER AND a.KDSATKER = c.KDSATKER AND a.KDGIAT = c.KEGIATAN AND a.KDPROGRAM = c.PROGRAM AND a.TA = c.TA
                        WHERE a.REGISTER = :RegisterNo AND a.TA BETWEEN EXTRACT(YEAR FROM SYSDATE) - 4 AND EXTRACT(YEAR FROM SYSDATE) - 1)
                        GROUP BY NMDEPT, REGISTER, TA
                        )
                        ) GROUP BY NMDEPT, LabelTahun
                    ";
                    return db.Database.SqlQuery<ProfilProyekISS.AlokasiAPBN>(sqlAlokasiAPBN, profilProyek.RegisterInformationData.RegisterNo).AsEnumerable();
                });
            } else
            {
                var lengthTahunAPBN = 4;
                var labelTahun = string.Empty;
                var tahun = DateTime.Now.Year;
                var startTahun = tahun - lengthTahunAPBN;
                var endTahun = tahun - 1;

                for (var i = 0; i < lengthTahunAPBN; i++)
                {
                    var tahunAPBN = startTahun + i;
                    if (tahunAPBN == endTahun)
                    {
                        labelTahun += tahunAPBN.ToString();
                    }
                    else
                    {
                        labelTahun += $@"{tahunAPBN.ToString()},";
                    }
                }
                profilProyek.AlokasiAPBNData = new List<ProfilProyekISS.AlokasiAPBN>
                {
                    new ProfilProyekISS.AlokasiAPBN
                    {
                        LabelTahun = labelTahun
                    }
                };
            }

            return profilProyek;
        }
    }
}