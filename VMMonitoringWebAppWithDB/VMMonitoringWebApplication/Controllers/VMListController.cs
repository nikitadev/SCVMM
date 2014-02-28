namespace VMMonitoringWebApplication.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Infastracture;
    using Models;
    using Elmah.Contrib.WebApi;
    using System.Diagnostics;

    [ElmahHandleErrorApi]
    public sealed class VMListController : ApiController
    {
        private readonly IHostService hostService;
        private readonly IMonitoringService monitoringService;
        private readonly IRememberService rememberService;

        public VMListController(
            IHostService hostService, 
            IMonitoringService monitoringService, 
            IRememberService rememberService)
        {
            this.hostService = hostService;
            this.monitoringService = monitoringService;
            this.rememberService = rememberService;
        }

        // GET api/vmlist
        public async Task<IEnumerable<VMItemDto>> Get()
        {
            var model = this.rememberService.Get<ServerModel>(DefaultNames.ServerCookieName);

            if (!String.IsNullOrEmpty(model.Name))
            {
                var result = await this.hostService.GetVMListAsync(model.Name);
                var vmItems = result as VMItem[] ?? result.ToArray();

                if (vmItems.Any())
                {
                    var list = new List<VMItemDto>();
                    foreach (var item in vmItems)
                    {
                        await Task.Yield();

                        var vm = new VMItemDto(item);

                        try
                        {
                            var cpu =
                            (await
                                monitoringService.GetPerformance(item.Name,
                                    Infastracture.PerformanceCounterType.CPUUsage,
                                    TimeframeType.Hour)).ToList();
                            var network =
                                (await
                                    monitoringService.GetPerformance(item.Name,
                                        Infastracture.PerformanceCounterType.NetworkIOUsage,
                                        TimeframeType.Hour)).ToList();
                            var storage =
                                (await
                                    monitoringService.GetPerformance(item.Name,
                                        Infastracture.PerformanceCounterType.StorageIOPSUsage,
                                        TimeframeType.Hour)).ToList();

                            if (cpu.Count > 0)
                            {
                                vm.CPU = cpu.Sum(t => t.Value) / cpu.Count;
                            }

                            if (network.Count > 0)
                            {
                                vm.Network = network.Sum(t => t.Value) / network.Count;
                            }

                            if (storage.Count > 0)
                            {
                                vm.Storage = storage.Sum(t => t.Value) / storage.Count;
                            }

                            var cpuRange = new PerfCounterRangeModel { Min = 0, MinMean = 30, MaxMean = 60, Max = 100 };
                            var networkRange = new PerfCounterRangeModel { Min = 0, MinMean = 1500, MaxMean = 3000, Max = 5000 };
                            var storageRange = new PerfCounterRangeModel { Min = 0, MinMean = 300000, MaxMean = 600000, Max = 1000000 };

                            vm.RecalculateStatus(cpuRange, networkRange, storageRange);
                        }
                        catch (Exception ex)
                        {
                            vm.SetStatus();

                            Trace.TraceError(ex.Message);
                        }

                        list.Add(vm);
                    }

                    return list;
                }
            }

            return Enumerable.Empty<VMItemDto>();
        }
    }
}
