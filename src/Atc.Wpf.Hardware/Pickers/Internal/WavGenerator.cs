using System.IO;
using System.Text;

namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Generates a stereo 16-bit PCM WAV byte buffer for a sine test tone with
/// fade-in / fade-out at every channel-segment boundary. Three segments:
/// Left only → Right only → Both.
/// </summary>
internal static class WavGenerator
{
    public static byte[] CreateStereoTestTone(
        int sampleRate,
        int frequencyHz,
        int segmentMilliseconds,
        int fadeMilliseconds,
        double amplitude)
    {
        const int channelCount = 2;
        const int bitsPerSample = 16;
        const int bytesPerSample = bitsPerSample / 8;
        const int blockAlign = channelCount * bytesPerSample;
        var byteRate = sampleRate * blockAlign;

        var samplesPerSegment = sampleRate * segmentMilliseconds / 1000;
        var totalSamples = samplesPerSegment * 3;
        var fadeSamples = sampleRate * fadeMilliseconds / 1000;
        var dataBytes = totalSamples * blockAlign;
        var fileBytes = 44 + dataBytes;

        var buffer = new byte[fileBytes];
        using var stream = new MemoryStream(buffer);
        using var writer = new BinaryWriter(stream);

        // RIFF header.
        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(fileBytes - 8);
        writer.Write(Encoding.ASCII.GetBytes("WAVE"));

        // fmt chunk.
        writer.Write(Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);  // PCM
        writer.Write((short)channelCount);
        writer.Write(sampleRate);
        writer.Write(byteRate);
        writer.Write((short)blockAlign);
        writer.Write((short)bitsPerSample);

        // data chunk.
        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(dataBytes);

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

        return buffer;
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