namespace VMMonitoringWebApplication.Infastracture
{
    using Dapper;
    using DataAccessLayer.Infastracture;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class DapperProvider : IProvider<DapperCommand>
    {
        private readonly string connectionString;

        public string Error { get; private set; }

        public DapperProvider(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<T>> ExecuteAsync<T>(DapperCommand command)
        {
            using (var connection = await Connection.GetOpenConnection(this.connectionString))
            {
                var grid = connection.QueryMultiple(command.Name, command.Parametrs, commandType: command.Type);

                var result = grid.Read<T>().ToList();

                return result;
            }
        }
    }
}