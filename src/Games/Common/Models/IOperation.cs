namespace JackboxGPT3.Games.Common.Models
{
    public interface IOperation
    {
        public int From { get; }
        public string Key { get; }
        public string Value { get; }
        public int Version { get; }
    }
}