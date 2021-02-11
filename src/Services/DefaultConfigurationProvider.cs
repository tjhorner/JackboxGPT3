namespace JackboxGPT3.Services
{
    public abstract class DefaultConfigurationProvider : IConfigurationProvider
    {
        public string EcastHost => "ecast.jackboxgames.com";
        
        public abstract string PlayerName { get; set; }
        public abstract string OpenAIEngine { get; set; }
        public abstract string RoomCode { get; set; }
        public abstract string LogLevel { get; set; }
    }
}
