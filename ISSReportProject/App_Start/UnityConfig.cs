using ISSReportProject.Interfaces;
using ISSReportProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Unity;

namespace ISSReportProject.App_Start
{
    public class UnityConfig
    {
        public static IUnityContainer GetConfiguredContainer()
        {
            var container = new UnityContainer();

            // Register Interface
            container.RegisterType<IReportService, ReportService>();

            return container;
        }
    }
}