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
    public class LNKLPContext : DbContext
    {
        public LNKLPContext() : base(new OracleConnection("DATA SOURCE=LNKLP; PASSWORD=ADMINLNKLP;USER ID=lnklp"), true)
        {
        }

        public DbSet<BLUEBOOK> BLUEBOOK { get; set; }
        public DbSet<BLUEBOOK_PROJECT> BLUEBOOK_PROJECT { get; set; }
        public DbSet<PROJECT> PROJECT { get; set; }
        public DbSet<PROJECT_ACTIVITY> PROJECT_ACTIVITY { get; set; }
        public DbSet<PROJECT_DANA> PROJECT_DANA { get; set; }
        public DbSet<VW_PROJECT_DANA> VW_PROJECT_DANA { get; set; }
        public DbSet<VW_PROJECT_ACTIVITY> VW_PROJECT_ACTIVITY { get; set; }
    }

    [Table("LNKLP.BLUEBOOK")]
    public class BLUEBOOK : Generic
    {
        public BLUEBOOK()
        {
            BLUEBOOK_PROJECTS = new HashSet<BLUEBOOK_PROJECT>();
        }

        [Key]
        public double BB_ID { get; set; }
        public string NO_BB { get; set; }
        public string REVISI { get; set; }
        public double CG_KLASIFIKASI { get; set; }
        public string CD_KLASIFIKASI { get; set; }
        public string TAHUN { get; set; }
        public string NAMA_FILE { get; set; }
        public string NAMA_FILE_CLIENT { get; set; }
        public virtual ICollection<BLUEBOOK_PROJECT> BLUEBOOK_PROJECTS { get; set; }

    }

    [Table("LNKLP.BLUEBOOK_PROJECT")]
    public class BLUEBOOK_PROJECT : Generic
    {
        [Key]
        public double PBB_ID { get; set; }
        public double PROJECT_ID { get; set; }
        public double BB_ID { get; set; }
        [ForeignKey("PROJECT_ID")]
        public virtual PROJECT PROJECT { get; set; }
        [ForeignKey("BB_ID")]
        public virtual BLUEBOOK BLUEBOOK { get; set; }
    }

    [Table("LNKLP.PROJECT")]
    public class PROJECT : Generic
    {
        [Key]
        public double PROJECT_ID { get; set; }
        public string NAME { get; set; }
        public string GOAL { get; set; }
        public string TARGET { get; set; }
        public string SCOPEOFWORK { get; set; }
        public double CG_BB { get; set; }
        public string CD_BB { get; set; }
        public double CG_GB { get; set; }
        public string CD_GB { get; set; }
        public double CG_EA { get; set; }
        public string CD_EA { get; set; }
        public double CG_SECTOR { get; set; }
        public string CD_SECTOR { get; set; }
        public double CG_SOURCE { get; set; }
        public string CD_SOURCE { get; set; }
        public double CG_TYPE { get; set; }
        public string CD_TYPE { get; set; }
        public string DURATION { get; set; }
        public double CG_ORIGIN { get; set; }
        public string CD_ORIGIN { get; set; }
        public double CG_PROGRAM { get; set; }
        public string CD_PROGRAM { get; set; }
        public string SHORTNAME { get; set; }
        public string REMARK { get; set; }
    }

    [Table("LNKLP.PROJECT_ACTIVITY")]
    public class PROJECT_ACTIVITY : Generic
    {
        [Key]
        public double PROJECT_ACTIVITY_ID { get; set; }
        public double PROJECT_ID { get; set; }
        public DateTime ACTIVITY_DATE { get; set; }
        public virtual double CG_TAHAPAN { get; set; }
        public string CD_TAHAPAN { get; set; }
        public virtual double CG_ACTIVITY { get; set; }
        public string CD_ACTIVITY { get; set; }
        public string REMARK { get; set; }
        public virtual double CG_STATUS { get; set; }
        public string CD_STATUS { get; set; }
        public string NAMA_FILE { get; set; }
        public string NAMA_FILE_CLIENT { get; set; }
    }

    [Table("LNKLP.PROJECT_DANA")]
    public class PROJECT_DANA : Generic
    {
        [Key]
        public double PD_ID { get; set; }
        public double CG_CATEGORY { get; set; }
        public string CD_CATEGORY { get; set; }
        public double CG_JENIS { get; set; }
        public string CD_JENIS { get; set; }
        public double CG_CURRENCY { get; set; }
        public string CD_CURRENCY { get; set; }
        public double JUMLAH { get; set; }
        public double PROJECT_ID { get; set; }
    }

    [Table("LNKLP.PROJECT_IA")]
    public class PROJECT_IA : Generic
    {
        public double PROJECT_IA_ID { get; set; }
        public double PROJECT_ID { get; set; }
        public double CG_KL { get; set; }
        public string CD_KL { get; set; }
        public double BIAYA { get; set; }
        public string LINGKUP { get; set; }
        public string REMARK { get; set; }
    }

    [Table("LNKLP.VW_PROJECT_ACTIVITY")]
    public class VW_PROJECT_ACTIVITY : PROJECT_ACTIVITY
    {
        public string TAHAPAN { get; set; }
        public string ACTIVITY { get; set; }
        public string UD1_TAHAPAN { get; set; }

    }

    [Table("LNKLP.VW_PROJECT_DANA")]
    public class VW_PROJECT_DANA : PROJECT_DANA
    {
        public string CATEGORY { get; set; }
        public string JENIS { get; set; }
        public string CURRENCY { get; set; }
    }

    [Table("LNKLP.VW_PROJECT_IA")]
    public class VW_PROJECT_IA : PROJECT_IA
    {
        public string KL { get; set; }
    }

    public class Generic
    {
        public DateTime CREATE_DATE { get; set; }
        public string CREATE_BY { get; set; }
        public DateTime MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
    }
}