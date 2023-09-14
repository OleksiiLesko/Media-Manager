using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace MediaManager.Repository
{
    /// <summary>
    /// Configuration for work with database
    /// </summary>
    public class DbConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DbConfiguration> _logger;
        /// <summary>
        /// Initializes a new instance of the DbConfiguration
        /// </summary>
        /// <param name="configuration"></param>
        public DbConfiguration(IConfiguration configuration, ILogger<DbConfiguration> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        /// <summary>
        /// Creates connection to database
        /// </summary>
        /// <returns></returns>
        public string CreateConnectionString()
        {
            string server = _configuration["DbHostName:Server"];
            string database = _configuration["DbHostName:Database"];

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                TrustServerCertificate = true,
                IntegratedSecurity = true,
                ConnectRetryCount = 1
            };

            return builder.ConnectionString;
        }
        /// <summary>
        /// Works with database
        /// </summary>
        public void OpenConnection()
        {
            string connectionString = CreateConnectionString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Opened connection to database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while working with database: " + ex.Message);
            }
        }
    }
}
