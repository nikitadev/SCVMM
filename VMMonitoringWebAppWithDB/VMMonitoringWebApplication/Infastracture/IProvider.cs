namespace VMMonitoringWebApplication.Infastracture
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProvider<in TCmd>
    {
        string Error { get; }

        Task<IEnumerable<T>> ExecuteAsync<T>(TCmd command);
    }
}