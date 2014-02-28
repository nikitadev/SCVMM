namespace VMMonitoringWebApplication.Infastracture
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IHostService
    {
        Task<IEnumerable<VMItem>> GetVMListAsync(string name);
    }
}
