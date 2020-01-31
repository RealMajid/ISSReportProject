using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISSReportProject.Models.Reports
{
    public class LoanTranches
    {
        public string LoanId { get; set; }
        public DateTime? DateEffective { get; set; }
        public DateTime? PrincipalLastPayment { get; set; }
        public DateTime? InterestLastPayment { get; set; }
        public string GracePeriod { get; set; }
        public double? InterestRate { get; set; }
        public string LoanCommissionFeeType { get; set; }
        public double? CommitmentRate { get; set; }
    }
}