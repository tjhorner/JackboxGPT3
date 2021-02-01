using System;
using JackboxGPT3.Clients.Models.Quiplash3;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Clients
{
    public class Quiplash3Client : BaseJackboxClient<Quiplash3Room, Quiplash3Player>
    {
        public Quiplash3Client(IConfigurationProvider configuration, ILogger logger) : base(configuration, logger) { }
    }
}
