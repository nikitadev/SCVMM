namespace VMMonitoringWebApplication.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class VirtualMachineModel
    {
        public string Name { get; set; }

        public Guid CPUTieredPerfCounterID { get; set; }
        public Guid DiskBytesReadTieredPerfCounterID { get; set; }
        public Guid DiskBytesWriteTieredPerfCounterID { get; set; }

        public Guid NetworkBytesReadTieredPerfCounterID { get; set; }
        public Guid NetworkBytesWriteTieredPerfCounterID { get; set; }
    }
}