namespace WorldWord.Config
{
    public class WorldWordConfiguration
    {
        public string AllowedAccessToApi { get; private set; } = string.Empty;
        public DatabaseConfig MongoConfig { get; private set; }

        public WorldWordConfiguration()
        {
            
        }

        public WorldWordConfiguration(string AllowedAccessToApi, DatabaseConfig dbConfig)
        {
            this.AllowedAccessToApi = AllowedAccessToApi;
            this.MongoConfig = dbConfig;
        }
    }
}
