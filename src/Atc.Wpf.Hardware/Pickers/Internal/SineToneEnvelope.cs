namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Computes the per-sample amplitude envelope for the speaker test tone:
/// silent past the end, target amplitude in the middle of each segment,
/// and a linear fade in/out of <c>fadeSamples</c> at every segment boundary
/// to suppress click artefacts.
/// </summary>
internal static class SineToneEnvelope
{
    public static double ComputeAmplitude(
        long position,
        long perSegmentSamples,
        long totalSamples,
        long fadeSamples,
        double targetAmplitude)
    {
        if (position < 0 || position >= totalSamples)
        {
            return 0.0;
        }

        if (perSegmentSamples <= 0 || fadeSamples <= 0)
        {
            return targetAmplitude;
        }

        var posInSegment = position % perSegmentSamples;
        var samplesFromStart = posInSegment;
        var samplesFromEnd = perSegmentSamples - posInSegment;

        if (samplesFromStart < fadeSamples)
        {
            return targetAmplitude * (samplesFromStart / (double)fadeSamples);
        }

        if (samplesFromEnd < fadeSamples)
        {
            return targetAmplitude * (samplesFromEnd / (double)fadeSamples);
        }

        return targetAmplitude;
    }
}