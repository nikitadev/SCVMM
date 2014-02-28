namespace VMMonitoringWebApplication.Infastracture
{
    public interface IRememberService
    {
        void Add<T>(string name, T model);
        T Get<T>(string name);
        bool Check(string name);
    }
}
