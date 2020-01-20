using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ISSReportProject.Models
{
    public class LNKLOANContext : DbContext
    {
        public LNKLOANContext() : base(new OracleConnection("DATA SOURCE=LNKLOAN; PASSWORD=4dm1nlnkloan;USER ID=LNKLOAN"), true)
        {
        }

        public DbSet<PARTS> PARTS { get; set; }
    }

    [Table("DMFAS.PARTS")]
    public class PARTS
    {
        [Key]
        public double PA_ID { get; set; }
        public string NAME { get; set; }
    }
}