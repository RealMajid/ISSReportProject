using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISSReportProject.Models.Reports
{
    public class ScheduleDisbursement
    {
        public string LoanId { get; set; }
        public double Year { get; set; }
        public double Amount { get; set; }
    }
}