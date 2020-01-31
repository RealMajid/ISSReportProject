using ISSReportProject.Interfaces;
using ISSReportProject.Models.Reports;
using ISSReportProject.Services;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ISSReportProject.Controllers
{
    public class ReportController : Controller
    {
        IReportService reportService;
        private IEnumerable<ProfilProyekISS.Activitiy> activitiys;
        private IEnumerable<ProfilProyekISS.FundingSource> fundingSources;
        private IEnumerable<ProfilProyekISS.KomponenDana> komponenDanas;
        private ProfilProyekISS.TermsAndConditions termsAndConditions;
        private ProfilProyekISS.DisbursementPlan disbursementPlans;

        public ReportController() : this(new ReportService())
        {

        }
        public ReportController(IReportService reportService)
        {
            this.reportService = reportService;
        }

        [HttpGet]
        public async Task<ActionResult> RptProfilProyekISS(double proyekId, string format)
        {
            //Get Data
            var profilProyek = await reportService.GetRptProfilProyekISS(proyekId);
            activitiys = profilProyek.ActivitiyData != null ? profilProyek.ActivitiyData : Enumerable.Empty<ProfilProyekISS.Activitiy>();
            fundingSources = profilProyek.FundingSourceData != null ? profilProyek.FundingSourceData : Enumerable.Empty<ProfilProyekISS.FundingSource>();
            komponenDanas = profilProyek.KomponenDanaData != null ? profilProyek.KomponenDanaData : Enumerable.Empty<ProfilProyekISS.KomponenDana>();
            termsAndConditions = profilProyek.TermsAndConditionsData != null ? profilProyek.TermsAndConditionsData : Activator.CreateInstance<ProfilProyekISS.TermsAndConditions>();
            disbursementPlans = profilProyek.DisbursementPlanData != null ? profilProyek.DisbursementPlanData : Activator.CreateInstance<ProfilProyekISS.DisbursementPlan>(); ;

            if (string.IsNullOrEmpty(format) || string.IsNullOrWhiteSpace(format))
            {
                format = "PDF";
            }

            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            var reportDataSourceBluebook = profilProyek.BluebookProfileData != null ? GetReportDataSourceSingle(profilProyek.BluebookProfileData, "DataSet1") : GetReportDataSourceSingle(Activator.CreateInstance<ProfilProyekISS.BluebookProfile>(), "DataSet1");
            var reportDataSourceGreenbook = profilProyek.GreenbookProfileData != null ? GetReportDataSourceSingle(profilProyek.GreenbookProfileData, "DataSet2") : GetReportDataSourceSingle(Activator.CreateInstance<ProfilProyekISS.GreenbookProfile>(), "DataSet2");
            var reportDataSourceDaftarKegiatan = profilProyek.DaftarKegiatanData != null ? GetReportDataSourceSingle(profilProyek.DaftarKegiatanData, "DataSet3") : GetReportDataSourceSingle(Activator.CreateInstance<ProfilProyekISS.DaftarKegiatan>(), "DataSet3");
            var reportDataSourceLoanAgreement = profilProyek.LoanAgreementData != null ? GetReportDataSourceSingle(profilProyek.LoanAgreementData, "DataSet4") : GetReportDataSourceSingle(Activator.CreateInstance<ProfilProyekISS.LoanAgreement>(), "DataSet4");
            var reportDataSourceRegisterInformation = profilProyek.RegisterInformationData != null ? GetReportDataSourceSingle(profilProyek.RegisterInformationData, "DataSet5") : GetReportDataSourceSingle(Activator.CreateInstance<ProfilProyekISS.RegisterInformation>(), "DataSet5");
            var reportDataSourceAlokasiAPBN = profilProyek.AlokasiAPBNData != null ? GetReportDataSourceList(profilProyek.AlokasiAPBNData, "DataSet6") : GetReportDataSourceList(Enumerable.Empty<ProfilProyekISS.AlokasiAPBN>(), "DataSet6");

            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = "Report/RptProfilProyekISS.rdlc";
            viewer.LocalReport.EnableExternalImages = true;
            viewer.LocalReport.DataSources.Add(reportDataSourceBluebook);
            viewer.LocalReport.DataSources.Add(reportDataSourceGreenbook);
            viewer.LocalReport.DataSources.Add(reportDataSourceDaftarKegiatan);
            viewer.LocalReport.DataSources.Add(reportDataSourceLoanAgreement);
            viewer.LocalReport.DataSources.Add(reportDataSourceRegisterInformation);
            viewer.LocalReport.DataSources.Add(reportDataSourceAlokasiAPBN);
            viewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
            byte[] bytes = viewer.LocalReport.Render(format, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            if (format == "PDF")
            {
                mimeType = "Application/" + format;
                format = "";
            }
            else
            {
                Response.AddHeader("content-disposition", "attachment; filename=Laporan_1." + extension);
            }
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;

            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            Response.End();
            return View();
        }

        void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            if (e.ReportPath == "SubProfilProyek_Activities")
            {
                e.DataSources.Add(GetReportDataSourceList(activitiys, "DataSet1"));
            }
            else if (e.ReportPath == "SubProfilProyek_FundingSources")
            {
                e.DataSources.Add(GetReportDataSourceList(fundingSources, "DataSet1"));
            }
            else if (e.ReportPath == "SubProfilProyek_DisbursementPlan")
            {
                e.DataSources.Add(GetReportDataSourceSingle(disbursementPlans, "DataSet1"));
            }
            else if (e.ReportPath == "SubProfilProyek_PendanaanProyek")
            {
                e.DataSources.Add(GetReportDataSourceList(komponenDanas, "DataSet1"));
            }
            else if (e.ReportPath == "SubProfilProyek_TermsAndConditions")
            {
                e.DataSources.Add(GetReportDataSourceSingle(termsAndConditions, "DataSet1"));
            }
        }

        private ReportDataSource GetReportDataSourceSingle<T>(T data,string datasetName)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(prop => !prop.GetIndexParameters().Any()).ToArray();
            foreach (PropertyInfo prop in props)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ??
                    prop.PropertyType);
            }

            var values = new object[props.Length];
            for (int x = 0; x < props.Length; x++)
            {
                values[x] = props[x].GetValue(data, null);
            }
            dataTable.Rows.Add(values);

            ReportDataSource reportDataSource = new ReportDataSource(datasetName, dataTable);

            return reportDataSource;
        }

        private ReportDataSource GetReportDataSourceList<T>(IEnumerable<T> dataList, string datasetName) 
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ??
                    prop.PropertyType);
            }

            foreach (var item in dataList)
            {
                var values = new object[props.Length];
                for (int x = 0; x < props.Length; x++)
                {
                    values[x] = props[x].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            
            ReportDataSource reportDataSource = new ReportDataSource(datasetName, dataTable);

            return reportDataSource;
        }
    }
}
