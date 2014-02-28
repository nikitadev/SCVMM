namespace VMMonitoringWebApplication.Infastracture
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public enum PerformanceCounterType
    {
        CPUUsage = 0,
        NetworkIOUsage = 1,
        StorageIOPSUsage = 2
    }

    public enum TimeframeType
    {
        Hour,
        Day,
        Month
    }

    public interface IMonitoringService
    {
        Task<IEnumerable<PerfCounterModel>> GetPerformance(string name, PerformanceCounterType performanceCounterType, TimeframeType timeframeType);

        Task<int> GetPerformanceMeanValue(string name, PerformanceCounterType performanceCounterType, TimeframeType timeframeType);

        Task RunPerformanceAsync(string name, PerformanceCounterType performanceCounterType, TimeframeType timeframeType, IProgress<Tuple<PerformanceCounterType, PerfCounterModel>> progress);
    }
}
