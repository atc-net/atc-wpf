using Atc.Wpf.Messaging;

namespace Atc.Wpf.SampleControls
{
    public class SampleItemMessage : MessageBase
    {
        public SampleItemMessage(string? sampleItemPath)
        {
            this.SampleItemPath = sampleItemPath;
        }

        public string? SampleItemPath { get; }
    }
}