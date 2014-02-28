namespace VMMonitoringWebApplication.Models
{
    using System;

    public enum VMStatus
    {
        Normal,
        Warning,
        Error,
        NotFound
    }

    public class VMItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public VMStatus Status { get; set; }
    }
}