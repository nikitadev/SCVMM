namespace DataAccessLayer.Infastracture
{
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public static class Connection
    {
        public async static Task<SqlConnection> GetOpenConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();

            return connection;
        }
    }
}
