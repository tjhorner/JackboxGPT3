namespace JackboxGPT3.Services
{
    public abstract class DefaultConfigurationProvider : IConfigurationProvider
    {
        public string EcastHost => "ecast.jackboxgames.com";
        public string PlayerName => "GPT-3";
        
        public abstract string OpenAIEngine { get; set; }
        public abstract string RoomCode { get; set; }
        public abstract string LogLevel { get; set; }
    }
}
