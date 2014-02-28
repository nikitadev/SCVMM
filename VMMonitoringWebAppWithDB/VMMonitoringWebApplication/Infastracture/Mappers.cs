namespace VMMonitoringWebApplication.Infastracture
{
    using System.IO;
    using System;
    using System.Management.Automation;
    using Models;

    public sealed class TestMapper : IMapper<PSObject, VMItem>
    {
        public VMItem Map(PSObject obj)
        {
            var status = VMStatus.NotFound;
            var dynamicMemoryStatus = obj.Properties["DynamicMemoryStatus"];
            if (dynamicMemoryStatus != null)
            {
                var memoryStatus = (string)dynamicMemoryStatus.Value;
                status = memoryStatus == null || memoryStatus.ToLower().Equals("ok")
                    ? VMStatus.Normal
                    : memoryStatus.ToLower().Equals("low")
                        ? VMStatus.Warning
                        : VMStatus.Error;
            }

            var vmName = "Field not found";
            var name = obj.Properties["Name"];
            if (name != null)
            {
                vmName = (string) name.Value;
            }

            return new VMItem { Name = vmName, Status = status };
        }
    }

    public sealed class PSMapper : IMapper<PSObject, VMItem>
    {
        public VMItem Map(PSObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            var status = VMStatus.NotFound;
            var dynamicMemoryStatus = obj.Properties["DynamicMemoryStatus"];
            if (dynamicMemoryStatus != null)
            {
                var memoryStatus = (string)dynamicMemoryStatus.Value;
                status = memoryStatus == null || memoryStatus.ToLower().Equals("ok")
                    ? VMStatus.Normal
                    : memoryStatus.ToLower().Equals("low")
                        ? VMStatus.Warning
                        : VMStatus.Error;
            }

            var vmName = "Field not found";
            var name = obj.Properties["Name"];
            if (name != null)
            {
                vmName = (string)name.Value;
            }

            var id = Guid.Empty;
            var vmId = obj.Properties["VMId"];
            if (vmId != null)
            {
                var val = vmId.Value;
                if (val != null)
                {
                    Guid.TryParse((string) val, out id);
                }
            }

            return new VMItem { Id = id, Name = vmName, Status = status };
        }
    }

    public sealed class ChartMapper : IMapper<PSObject, PerfomanceObject>
    {
        public PerfomanceObject Map(PSObject obj)
        {
            var history = (PSObject)obj.Properties["PerformanceHistory"].Value;
            var times = (PSObject)obj.Properties["TimeSamples"].Value;

            dynamic hArray = history.BaseObject;
            var historyNumbers = (int[])hArray.ToArray(typeof(int));

            dynamic tArray = times.BaseObject;
            var timeNumbers = (DateTime[])tArray.ToArray(typeof(DateTime));

            return new PerfomanceObject { History = historyNumbers, TimeSamples = timeNumbers };
        }
    }
}