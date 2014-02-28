namespace VMMonitoringWebApplication.Models
{
    public class PerfCounterRangeModel
    {
        public int Min { get; set; }

        public int MinMean { get; set; }

        public int MaxMean { get; set; }

        public int Max { get; set; }

        public VMStatus GetStatus(int value)
        {
            return value >= Min && value <= MinMean
                ? VMStatus.Normal
                : value > MinMean && value <= MaxMean ? VMStatus.Warning : VMStatus.Error;
        }
    }
}