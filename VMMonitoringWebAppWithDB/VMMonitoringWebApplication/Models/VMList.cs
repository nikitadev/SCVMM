namespace VMMonitoringWebApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class VMList
    {
        public virtual List<VMItem> VMs { get; set; }
    }
}