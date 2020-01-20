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

        public ReportController() : this(new ReportService())
        {

        }
        public ReportController(IReportService reportService)
        {
            this.reportService = reportService;
        }

        [HttpGet]
        public async Task<ActionResult> RptProfilProyekISS(double pbbId, string format)
        {
            //Get Data
            var profilProyek = await reportService.GetRptProfilProyekISS();

            if (string.IsNullOrEmpty(format) || string.IsNullOrWhiteSpace(format))
            {
                format = "PDF";
            }

            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            var reportDataSourceBluebook = GetReportDataSourceSingle(profilProyek.BluebookProfileData, "DataSet1");
            var reportDataSourceGreenbook = GetReportDataSourceSingle(profilProyek.GreenbookProfileData, "DataSet2");
            var reportDataSourceActivities = GetReportDataSourceList(profilProyek.ActivitiyData, "DataSet3");

            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = "Report/ProfilProyek.rdlc";
            viewer.LocalReport.EnableExternalImages = true;
            viewer.LocalReport.DataSources.Add(reportDataSourceBluebook);
            viewer.LocalReport.DataSources.Add(reportDataSourceGreenbook);
            viewer.LocalReport.DataSources.Add(reportDataSourceActivities);
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

        private ReportDataSource GetReportDataSourceSingle<T>(T data,string datasetName)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
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
