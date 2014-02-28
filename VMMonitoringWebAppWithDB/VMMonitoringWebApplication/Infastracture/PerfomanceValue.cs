namespace VMMonitoringWebApplication.Infastracture
{
    public sealed class PerfomanceValue<T>
    {
        public T Value { get; private set; }

        public PerfomanceValue(T val)
        {
            Value = val;
        }
    }
}