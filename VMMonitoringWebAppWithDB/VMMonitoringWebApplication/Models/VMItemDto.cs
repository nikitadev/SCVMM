namespace VMMonitoringWebApplication.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    internal static class VMStatusColor
    {
        public const string NormalKey = "#008000";     // Green
        public const string WarningKey = "#FFFF00";    // Yellow
        public const string ErrorKey = "#FF0000";      // Red
        public const string NotFoundKey = "#0000FF";   // Blue
    }

    public class VMItemDto
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "VM name")]
        public string Name { get; set; }

        public string Memory { get; set; }

        public int CPU { get; set; }

        public int Network { get; set; }

        public int Storage { get; set; }

        public string Status { get; set; }

        public VMStatus SysStatus { get; set; }

        public VMItemDto() { }

        public VMItemDto(VMItem vmItem)
        {
            Id = vmItem.Id;
            Name = vmItem.Name;

            SysStatus = vmItem.Status;
            Memory = SysStatus.ToString();
        }

        public void RecalculateStatus(
            PerfCounterRangeModel cpuPerfCounterRangeModel, 
            PerfCounterRangeModel networkPerfCounterRangeModel, 
            PerfCounterRangeModel storagePerfCounterRangeModel)
        {
            if (SysStatus == VMStatus.NotFound)
            {
                Status = VMStatusColor.NotFoundKey;
                return;
            }

            var cpuStatus = cpuPerfCounterRangeModel.GetStatus(CPU);
            var networkStatus = networkPerfCounterRangeModel.GetStatus(Network);
            var storageStatus = storagePerfCounterRangeModel.GetStatus(Storage);

            if (SysStatus == VMStatus.Normal
                && cpuStatus == VMStatus.Normal 
                && networkStatus == VMStatus.Normal 
                && storageStatus == VMStatus.Normal)
            {
                Status = VMStatusColor.NormalKey;
            }
            else if (SysStatus == VMStatus.Warning
                     || cpuStatus == VMStatus.Warning
                     || networkStatus == VMStatus.Warning
                     || storageStatus == VMStatus.Warning)
            {
                Status = VMStatusColor.WarningKey;
            }
            else
            {
                Status = VMStatusColor.ErrorKey;
            }
        }

        public void SetStatus()
        {
            Status = (SysStatus == VMStatus.Normal) ? VMStatusColor.NormalKey : (SysStatus == VMStatus.Warning) ? VMStatusColor.WarningKey : VMStatusColor.ErrorKey;
        }
    }
}