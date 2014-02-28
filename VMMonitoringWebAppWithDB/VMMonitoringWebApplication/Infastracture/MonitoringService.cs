namespace VMMonitoringWebApplication.Infastracture
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Threading.Tasks;

    public sealed class MonitoringService : IMonitoringService
    {
        private readonly string mainCommandTemplate;

        private readonly IProvider<PSCommand> psProvider;

        public MonitoringService(IProvider<PSCommand> psProvider)
        {
            this.psProvider = psProvider;

            this.mainCommandTemplate = "$VM = Get-SCVirtualMachine -Name \"{0}\"";
        }

        public async Task<IEnumerable<VMItem>> GetStatus(string name)
        {
            string command = String.Concat(String.Format(this.mainCommandTemplate, name), Environment.NewLine, "$VM.DynamicMemoryStatus");
            
            var mapper = new PSMapper();
            var objects = await this.psProvider.ExecuteAsync<PSObject>(new PSCommand { ScriptCommand = command });

            return objects.Select(mapper.Map);
        }

        public async Task<IEnumerable<PerfCounterModel>> GetPerformance(string name, PerformanceCounterType performanceCounterType, TimeframeType timeframeType)
        {
            string command = String
                .Concat(String.Format(this.mainCommandTemplate, name), Environment.NewLine, "Get-SCPerformanceData -VM $VM -PerformanceCounter {0} -Timeframe {1}");

            command = String.Format(command, performanceCounterType, timeframeType);

            var mapper = new ChartMapper();
            var objects = await this.psProvider.ExecuteAsync<PSObject>(new PSCommand { ScriptCommand = command });
            var listValues = objects.Select(mapper.Map).ToList();

            if (listValues.Any())
            {
                var perfCounter = listValues.ToList()[0];

                var timestamps = perfCounter.TimeSamples;
                var values = perfCounter.History;

                return values.Select((t, i) => new PerfCounterModel { Value = t, Timestamp = timestamps[i] }).ToList();
            }

            return Enumerable.Empty<PerfCounterModel>();
        }

        public async Task<int> GetPerformanceMeanValue(string name, PerformanceCounterType performanceCounterType, TimeframeType timeframeType)
        {
            string command = String
                .Concat(String.Format(this.mainCommandTemplate, name), Environment.NewLine, "Get-SCPerformanceData -VM $VM -PerformanceCounter {0} -Timeframe {1}");

            command = String.Format(command, performanceCounterType, timeframeType);

            var mapper = new ChartMapper();
            var objects = await this.psProvider.ExecuteAsync<PSObject>(new PSCommand { ScriptCommand = command });
            var listValues = objects.Select(mapper.Map).ToList();

            int meanValue = 0;
            if (listValues.Any())
            {
                var result = listValues.ToList()[0].History;

                meanValue = result.Sum() / result.Length;
            }

            return meanValue > 0 ? meanValue / 100 : 0;
        }

        public async Task RunPerformanceAsync(string name, PerformanceCounterType performanceCounterType, TimeframeType timeframeType, IProgress<Tuple<PerformanceCounterType, PerfCounterModel>> progress)
        {
            await Task.Run(async () => 
            {
                while (true)
                {
                    await Task.Yield();

                    var value = await GetPerformanceMeanValue(name, performanceCounterType, timeframeType);

                    progress.Report(
                        new Tuple<PerformanceCounterType, PerfCounterModel>(performanceCounterType, new PerfCounterModel { Timestamp = DateTime.Now, Value = value }));

                    await Task.Delay(250);
                }
            }).ConfigureAwait(false);
        }
    }
}