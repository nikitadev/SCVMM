namespace VMMonitoringWebApplication.Infastracture
{
    using System.Data;
    using Dapper;

    public class DapperCommand
    {
        public string Name { get; set; }
        public DynamicParameters Parametrs { get; set; }

        public CommandType Type { get; set; }
    }
}