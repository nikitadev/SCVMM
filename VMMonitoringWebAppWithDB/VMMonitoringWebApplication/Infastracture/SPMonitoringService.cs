namespace VMMonitoringWebApplication.Infastracture
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class SPMonitoringService : IMonitoringService
    {
        private readonly IProvider<DapperCommand> provider;

        public SPMonitoringService(IProvider<DapperCommand> provider)
        {
            this.provider = provider;
        }

        public async Task<IEnumerable<PerfCounterModel>> GetPerformance(
            string name, 
            PerformanceCounterType performanceCounterType, 
            TimeframeType timeframeType)
        {
            var values = Enumerable.Empty<PerfCounterModel>();

            var pVMs = new DynamicParameters();
            pVMs.Add("ObjectType", 1);
            pVMs.Add("error", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            var command = new DapperCommand
            {
                Name = "[dbo].[prc_WLC_GetAllVMsWithChildrenByType]", 
                Parametrs = pVMs,
                Type = CommandType.StoredProcedure
            };

            var result = await this.provider.ExecuteAsync<VirtualMachineModel>(command);

            var virtualMachineModels = result as IList<VirtualMachineModel> ?? result.ToList();
            if (virtualMachineModels.Any())
            {
                foreach (var vm in virtualMachineModels)
                {
                    if (vm.Name.Equals(name))
                    {
                        switch (performanceCounterType)
                        {
                            case PerformanceCounterType.CPUUsage:
                                {
                                    var p = new DynamicParameters();
                                    p.Add("tieredPerfCounterID", vm.CPUTieredPerfCounterID, DbType.Guid, ParameterDirection.Input);
                                    p.Add("aggregationLevel", (int)timeframeType, DbType.Int32, ParameterDirection.Input);
                                    p.Add("error", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                                    var commandPerfCPU = new DapperCommand
                                    {
                                        Name = "[dbo].[prc_PCMT_PerfHistory_Get]",
                                        Parametrs = p,
                                        Type = CommandType.StoredProcedure
                                    };

                                    var cpuValues = await this.provider.ExecuteAsync<PerfCounterModel>(commandPerfCPU);

                                    values = cpuValues.OrderBy(t => t.Timestamp).ToList();
                                }
                                break;
                            case PerformanceCounterType.NetworkIOUsage:
                                {
                                    var readParams = new DynamicParameters();
                                    readParams.Add("tieredPerfCounterID", vm.NetworkBytesReadTieredPerfCounterID, DbType.Guid, ParameterDirection.Input);
                                    readParams.Add("aggregationLevel", (int)timeframeType, DbType.Int32, ParameterDirection.Input);
                                    readParams.Add("error", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                                    var commandPerfReadNet = new DapperCommand
                                    {
                                        Name = "[dbo].[prc_PCMT_PerfHistory_Get]",
                                        Parametrs = readParams,
                                        Type = CommandType.StoredProcedure
                                    };

                                    var readNetValues = await this.provider.ExecuteAsync<PerfCounterModel>(commandPerfReadNet);

                                    var writeParams = new DynamicParameters();
                                    writeParams.Add("tieredPerfCounterID", vm.NetworkBytesWriteTieredPerfCounterID, DbType.Guid, ParameterDirection.Input);
                                    writeParams.Add("aggregationLevel", (int)timeframeType, DbType.Int32, ParameterDirection.Input);
                                    writeParams.Add("error", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                                    var commandPerfWriteNet = new DapperCommand
                                    {
                                        Name = "[dbo].[prc_PCMT_PerfHistory_Get]",
                                        Parametrs = writeParams,
                                        Type = CommandType.StoredProcedure
                                    };

                                    var writeNetValues = await this.provider.ExecuteAsync<PerfCounterModel>(commandPerfWriteNet);

                                    values = readNetValues.Union(writeNetValues)
                                        //.Select(t => new PerfCounterModel { Value = t.Value / 1000, Timestamp = t.Timestamp })
                                        .OrderBy(t => t.Timestamp)
                                        .ToList();
                                }
                                break;
                            case PerformanceCounterType.StorageIOPSUsage:
                                {
                                    var readParams = new DynamicParameters();
                                    readParams.Add("tieredPerfCounterID", vm.DiskBytesReadTieredPerfCounterID, DbType.Guid, ParameterDirection.Input);
                                    readParams.Add("aggregationLevel", (int)timeframeType, DbType.Int32, ParameterDirection.Input);
                                    readParams.Add("error", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                                    var commandPerfReadStorage = new DapperCommand
                                    {
                                        Name = "[dbo].[prc_PCMT_PerfHistory_Get]",
                                        Parametrs = readParams,
                                        Type = CommandType.StoredProcedure
                                    };

                                    var readDiskValues = await this.provider.ExecuteAsync<PerfCounterModel>(commandPerfReadStorage);

                                    var writeParams = new DynamicParameters();
                                    writeParams.Add("tieredPerfCounterID", vm.DiskBytesWriteTieredPerfCounterID, DbType.Guid, ParameterDirection.Input);
                                    writeParams.Add("aggregationLevel", (int)timeframeType, DbType.Int32, ParameterDirection.Input);
                                    writeParams.Add("error", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                                    var commandPerfWriteStorage = new DapperCommand
                                    {
                                        Name = "[dbo].[prc_PCMT_PerfHistory_Get]",
                                        Parametrs = writeParams,
                                        Type = CommandType.StoredProcedure
                                    };

                                    var writeDiskValues = await this.provider.ExecuteAsync<PerfCounterModel>(commandPerfWriteStorage);

                                    values = readDiskValues.Union(writeDiskValues)
                                        //.Select(t => new PerfCounterModel { Value = t.Value / 10000, Timestamp = t.Timestamp })
                                        .OrderBy(t => t.Timestamp)
                                        .ToList();
                                }
                                break;
                        }

                        break;
                    }
                }
            }

            return values;
        }

        public Task<int> GetPerformanceMeanValue(string name, PerformanceCounterType performanceCounterType, TimeframeType timeframeType)
        {
            throw new NotImplementedException();
        }

        public Task RunPerformanceAsync(string name, 
            PerformanceCounterType performanceCounterType, 
            TimeframeType timeframeType, 
            IProgress<Tuple<PerformanceCounterType, PerfCounterModel>> progress)
        {
            throw new NotImplementedException();
        }
    }
}