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

        public async Task<ProfilProyekISS> GetRptProfilProyekISS()
        {
            var profilProyek = new ProfilProyekISS();

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
                        WHERE PBB_ID = 142)
                        GROUP BY NO_BB, NAME, PROGRAMLOAN, EANAME, KL, DURATION, Location, SCOPEOFWORK, TARGET";

            profilProyek.BluebookProfileData = await Task.Run(() => {
                return db.Database.SqlQuery<ProfilProyekISS.BluebookProfile>(sqlBluebookProfile).FirstOrDefault(); ;
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
	                                        WHERE b.PROJECT_ID = 288";

            profilProyek.GreenbookProfileData = await Task.Run(() =>
            {
                return db.Database.SqlQuery<ProfilProyekISS.GreenbookProfile>(sqlGreenbookProfile).FirstOrDefault(); ;
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
                WHERE a.PROJECT_ID = 302
                ORDER BY a.PROJECT_ID
            ";
            profilProyek.ActivitiyData = await Task.Run(() => {
                return db.Database.SqlQuery<ProfilProyekISS.Activitiy>(sqlActivity).AsEnumerable();
            });

            var sqlFundingSource = $@"      
                SELECT KL ImplementingAgency, BIAYA LoanAmount, LINGKUP Source FROM VW_PROJECT_IA
                WHERE PROJECT_ID = 295
            ";

            profilProyek.FundingSourceData = await Task.Run(() =>
            {
                return db.Database.SqlQuery<ProfilProyekISS.FundingSource>(sqlFundingSource).AsEnumerable();
            });

            var sqlDaftarKegiatan = $@"
                SELECT c.NO_DOKUMEN NomorSuratDaftarKegiatan, c.TGL_DOKUMEN TglSuratDaftarKegiatan, a.EANAME ImplementingAgency FROM VW_PROJECT a
                INNER JOIN DAFTARKEGIATAN_PROJECT b
                ON a.PROJECT_ID = b.PROJECT_ID
                INNER JOIN DAFTARKEGIATAN c
                ON b.DK_ID = c.DK_ID
                WHERE a.PROJECT_ID = 1
            ";

            profilProyek.DaftarKegiatanData = await Task.Run(() =>
            {
                return db.Database.SqlQuery<ProfilProyekISS.DaftarKegiatan>(sqlDaftarKegiatan).FirstOrDefault(); ;
            });

            var sqlKomponenDana = $@"
                    SELECT CATEGORY JenisKomponen, JUMLAH Jumlah, JENIS Keterangan FROM VW_PROJECT_DANA
                    WHERE PROJECT_ID = 295
                ";

            profilProyek.KomponenDanaData = await Task.Run(() =>
            {
                return db.Database.SqlQuery<ProfilProyekISS.KomponenDana>(sqlKomponenDana).AsEnumerable();
            });

            var sqlTermsAndConditions = $@"SELECT 1000 LoanAmount, '15 Years' Tenor, '5 Years' GracePeriod, '10 Years' RepaymentPeriod, '3% p.a.' InterestRate,
                '0.2% flat' ManagementFee, '0.2% p.a.' CommitmentFee FROM DUAL";

            profilProyek.TermsAndConditionsData = await Task.Run(() =>
            {
                return dbLoan.Database.SqlQuery<ProfilProyekISS.TermsAndConditions>(sqlTermsAndConditions).FirstOrDefault();
            });

            return profilProyek;
        }
    }
}