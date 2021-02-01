using JackboxGPT3.Games.Common;
using JackboxGPT3.Games.Common.Models;
using JackboxGPT3.Games.Fibbage3.Models;
using JackboxGPT3.Services;
using Serilog;

namespace JackboxGPT3.Games.Fibbage3
{
    public class Fibbage3Client : BcSerializedClient<Fibbage3Room, Fibbage3Player>
    {
        public Fibbage3Client(IConfigurationProvider configuration, ILogger logger) : base(configuration, logger) { }

        public void ChooseCategory(int index)
        {
            var req = new ChooseRequest<int>(index);
            ClientSend(req);
        }
        
        public void ChooseTruth(int index, string text)
        {
            var req = new ChooseRequest<TruthChoice>(new TruthChoice
            {
                Order = index,
                Text = text
            });

            ClientSend(req);
        }

        public void SubmitLie(string lie)
        {
            var req = new SendEntryRequest(lie);
            ClientSend(req);
        }
    }
}
