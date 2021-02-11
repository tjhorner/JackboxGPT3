// ReSharper disable InconsistentNaming
namespace JackboxGPT3.Services
{
    public interface IConfigurationProvider
    {
        public string EcastHost { get; }
        public string PlayerName { get; }
        public string RoomCode { get; }
        public string LogLevel { get; }
        
        public string OpenAIEngine { get; }
    }
}
