namespace VMMonitoringWebApplication.Infastracture
{
    public interface IMapper<in T, out TResult>
    {
        TResult Map(T obj);
    }
}
