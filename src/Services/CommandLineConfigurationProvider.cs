using System;

namespace JackboxGPT3.Services
{
    public class CommandLineConfigurationProvider : DefaultConfigurationProvider
    {
        public override string RoomCode => Environment.GetCommandLineArgs()[1];
    }
}
