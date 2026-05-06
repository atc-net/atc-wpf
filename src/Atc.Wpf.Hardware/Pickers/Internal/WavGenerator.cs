namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Generates a stereo 16-bit PCM WAV byte buffer for a sine test tone with
/// fade-in / fade-out at every channel-segment boundary. Three segments:
/// Left only → Right only → Both.
/// </summary>
internal static class WavGenerator
{
    private const int ChannelCount = 2;
    private const int BitsPerSample = 16;
    private const int BytesPerSample = BitsPerSample / 8;
    private const int BlockAlign = ChannelCount * BytesPerSample;
    private const int RiffHeaderBytes = 44;

    public static byte[] CreateStereoTestTone(
        int sampleRate,
        int frequencyHz,
        int segmentMilliseconds,
        int fadeMilliseconds,
        double amplitude)
    {
        var samplesPerSegment = sampleRate * segmentMilliseconds / 1000;
        var totalSamples = samplesPerSegment * 3;
        var fadeSamples = sampleRate * fadeMilliseconds / 1000;
        var dataBytes = totalSamples * BlockAlign;

        var buffer = new byte[RiffHeaderBytes + dataBytes];
        using var stream = new MemoryStream(buffer);
        using var writer = new BinaryWriter(stream);

        WriteRiffHeader(writer, sampleRate, dataBytes);
        WriteSamples(writer, sampleRate, frequencyHz, samplesPerSegment, totalSamples, fadeSamples, amplitude);

        return buffer;
    }

    private static void WriteRiffHeader(
        BinaryWriter writer,
        int sampleRate,
        int dataBytes)
    {
        var byteRate = sampleRate * BlockAlign;
        var fileBytes = RiffHeaderBytes + dataBytes;

        // RIFF header.
        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(fileBytes - 8);
        writer.Write(Encoding.ASCII.GetBytes("WAVE"));

        // fmt chunk.
        writer.Write(Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);  // PCM
        writer.Write((short)ChannelCount);
        writer.Write(sampleRate);
        writer.Write(byteRate);
        writer.Write((short)BlockAlign);
        writer.Write((short)BitsPerSample);

        // data chunk.
        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(dataBytes);
    }

    private static void WriteSamples(
        BinaryWriter writer,
        int sampleRate,
        int frequencyHz,
        int samplesPerSegment,
        int totalSamples,
        int fadeSamples,
        double amplitude)
    {
        var omega = 2.0 * global::System.Math.PI * frequencyHz / sampleRate;

        for (var i = 0; i < totalSamples; i++)
        {
            var segment = i / samplesPerSegment; // 0=Left, 1=Right, 2=Both
            var posInSegment = i - (segment * samplesPerSegment);

            var envelope = ComputeEnvelope(posInSegment, samplesPerSegment, fadeSamples);
            var sample = (short)(amplitude * envelope * short.MaxValue * global::System.Math.Sin(omega * i));

            short left = segment switch
            {
                0 => sample,           // Left only
                2 => sample,           // Both
                _ => 0,                // Right only → silence on left
            };

            short right = segment switch
            {
                1 => sample,           // Right only
                2 => sample,           // Both
                _ => 0,                // Left only → silence on right
            };

            writer.Write(left);
            writer.Write(right);
        }
    }

    private static double ComputeEnvelope(
        int positionInSegment,
        int segmentSamples,
        int fadeSamples)
    {
        if (fadeSamples <= 0)
        {
            return 1.0;
        }

        if (positionInSegment < fadeSamples)
        {
            return positionInSegment / (double)fadeSamples;
        }

        var samplesFromEnd = segmentSamples - positionInSegment;
        if (samplesFromEnd < fadeSamples)
        {
            return samplesFromEnd / (double)fadeSamples;
        }

        return 1.0;
    }
}