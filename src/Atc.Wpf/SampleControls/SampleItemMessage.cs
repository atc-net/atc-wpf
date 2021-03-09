using Atc.Wpf.Messaging;

namespace Atc.Wpf.SampleControls
{
    public class SampleItemMessage : MessageBase
    {
        public SampleItemMessage(string? header, string? sampleItemPath)
        {
            this.Header = header;
            this.SampleItemPath = sampleItemPath;
        }

        public string? Header { get; }

        public string? SampleItemPath { get; }

        public override string ToString()
        {
            return $"{nameof(Header)}: {Header}, {nameof(SampleItemPath)}: {SampleItemPath}";
        }
    }
}