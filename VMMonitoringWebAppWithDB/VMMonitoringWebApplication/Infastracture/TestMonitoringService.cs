namespace VMMonitoringWebApplication.Infastracture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;

    public sealed class TestMonitoringService : IMonitoringService
    {
        private const int startPoint = 50;
        private const int minPoint = 0;
        private const int maxPoint = 99;

        public async Task RunPerformanceAsync(string name, PerformanceCounterType performanceCounterType, TimeframeType timeframeType, IProgress<Tuple<PerformanceCounterType, PerfCounterModel>> progress)
        {
            await Task.Run(async () =>
            {
                var random = new Random();
                int prevPoint = 0;

                while (true)
                {
                    await Task.Yield();

                    int point = prevPoint > 0 ? prevPoint : startPoint;

                    // Update the point price by a random factor of the range percent
                    int number = (point + (int)(random.NextDouble() * random.Next(0, 30))) - random.Next(0, 50);

                    if (number < minPoint)
                    {
                        number = random.Next(0, 10);
                    }
                    else if (number > maxPoint)
                    {
                        number = 100;
                    }

                    progress.Report(new Tuple<PerformanceCounterType, PerfCounterModel>(performanceCounterType, new PerfCounterModel { Timestamp = DateTime.Now, Value = number }));

                    await Task.Delay(250);

                    prevPoint = number;
                }
            }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PerfCounterModel>> GetPerformance(string name, PerformanceCounterType performanceCounterType, TimeframeType timeframeType)
        {
            var random = new Random();
            random.NextDouble();

            var numbers = new List<PerfCounterModel>();
            for (int i = 0; i < 2; i++)
            {
                await Task.Yield();

                int number = performanceCounterType == PerformanceCounterType.CPUUsage 
                    ? random.Next(0, 100) : performanceCounterType == PerformanceCounterType.NetworkIOUsage 
                    ? random.Next(0, 5000) : random.Next(0, 1000000);

                numbers.Insert(i, new PerfCounterModel
                {
                    Timestamp = DateTime.Now,
                    Value = number
                });
            }

            return numbers;
        }

        public async Task<int> GetPerformanceMeanValue(string name, PerformanceCounterType performanceCounterType, TimeframeType timeframeType)
        {
            var random = new Random();
            random.NextDouble();

            var numbers = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                await Task.Yield();

                int number = random.Next(0, 100);
                numbers.Insert(i, number);
            }

            return numbers.Sum() / numbers.Count;
        }
    }
}