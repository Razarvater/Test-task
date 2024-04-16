namespace WorldWord.Config
{
    public class DatabaseConfig
    {
        public string ConnectionString { get; private set; } = string.Empty;
        public string DatabaseName { get; private set; } = string.Empty;

        public DatabaseConfig()
        {

        }

        public DatabaseConfig(string connectionString, string databaseName)
        {
            this.ConnectionString = connectionString;
            this.DatabaseName = databaseName;
        }
    }
}
