namespace VMMonitoringWebApplication.Models
{
    using System;
    public class PerfomanceObject
    {
        public int[] History { get; set; }

        public DateTime[] TimeSamples { get; set; }
    }
}