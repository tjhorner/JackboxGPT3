using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace JackboxGPT3.Games.SurviveTheInternet
{
    public class ImageDescriptionProvider
    {
        private readonly Dictionary<string, string> _imageDescriptions;
        
        public ImageDescriptionProvider(string descriptionPath)
        {
            var file = File.ReadAllText(descriptionPath);
            _imageDescriptions = JsonConvert.DeserializeObject<Dictionary<string, string>>(file);
        }
        
        public string ProvideDescriptionForImageId(string id)
        {
            return _imageDescriptions[id];
        }
    }
}