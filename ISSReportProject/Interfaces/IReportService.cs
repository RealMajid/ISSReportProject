using ISSReportProject.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISSReportProject.Interfaces
{
    public interface IReportService
    {
        Task<ProfilProyekISS> GetRptProfilProyekISS();
    }
}
