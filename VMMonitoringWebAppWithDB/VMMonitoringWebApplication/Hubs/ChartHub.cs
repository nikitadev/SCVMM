namespace VMMonitoringWebApplication.Hubs
{
    using Models;
    using Microsoft.AspNet.SignalR;
    using System;
    using System.Threading.Tasks;
    using Infastracture;

    public class ChartHub : Hub
    {
        private readonly Progress<Tuple<PerformanceCounterType, PerfCounterModel>> progress;
        private readonly IMonitoringService monitoringService;

        public ChartHub(IMonitoringService monitoringService)
        {
            this.progress = new Progress<Tuple<PerformanceCounterType, PerfCounterModel>>();
            this.monitoringService = monitoringService;
        }

        public async Task Refresh(string nameMachine, int type = 0, int timeframe = 0)
        {
            var numbers = await monitoringService.GetPerformance(nameMachine, (PerformanceCounterType)type, (TimeframeType)timeframe);

            Clients.All.Init(type, numbers);
        }

        public async Task RunDraw(string nameMachine, int type = 0, int timeframe = 0)
        {
            this.progress.ProgressChanged += ProgressOnProgressChanged;

            await monitoringService.RunPerformanceAsync(nameMachine, (PerformanceCounterType)type, (TimeframeType)timeframe, progress);

            this.progress.ProgressChanged -= ProgressOnProgressChanged;
        }

        private void ProgressOnProgressChanged(object sender, Tuple<PerformanceCounterType, PerfCounterModel> tuple)
        {
            Clients.All.UpdateChart(tuple.Item1, tuple.Item2);
        }
    }

}