namespace Atc.Wpf.Hardware.Tests.Pickers.Internal;

public sealed class SineToneEnvelopeTests
{
    private const long PerSegment = 48_000;     // 1 second @ 48 kHz
    private const long Total = PerSegment * 3;  // L → R → Both
    private const long Fade = 1_440;            // 30 ms @ 48 kHz
    private const double Target = 0.3;

    [Fact]
    public void Past_end_is_silent()
    {
        Assert.Equal(0.0, Compute(Total));
        Assert.Equal(0.0, Compute(Total + 100));
    }

    [Fact]
    public void Negative_position_is_silent()
    {
        Assert.Equal(0.0, Compute(-1));
    }

    [Fact]
    public void At_segment_start_amplitude_starts_at_zero()
    {
        Assert.Equal(0.0, Compute(0), precision: 12);
        Assert.Equal(0.0, Compute(PerSegment), precision: 12);
        Assert.Equal(0.0, Compute(PerSegment * 2), precision: 12);
    }

    [Fact]
    public void At_middle_of_segment_amplitude_reaches_target()
    {
        Assert.Equal(Target, Compute(PerSegment / 2), precision: 12);
        Assert.Equal(Target, Compute(PerSegment + (PerSegment / 2)), precision: 12);
    }

    [Fact]
    public void Approaching_fade_in_endpoint_returns_target()
    {
        Assert.Equal(Target, Compute(Fade), precision: 12);
    }

    [Fact]
    public void Inside_fade_in_amplitude_is_linear()
    {
        var halfwayIn = Fade / 2;
        var halfTarget = Target / 2;
        Assert.Equal(halfTarget, Compute(halfwayIn), precision: 6);
    }

    [Fact]
    public void Inside_fade_out_amplitude_decays_linearly()
    {
        // At one sample before the end of a segment we are 1 sample from the end of the
        // fade-out, so amplitude = Target * (1 / Fade).
        var oneSampleBeforeEnd = PerSegment - 1;
        var amplitude = Compute(oneSampleBeforeEnd);

        Assert.Equal(Target / Fade, amplitude, precision: 6);
    }

    [Fact]
    public void Zero_segment_size_returns_target_directly()
    {
        var amp = SineToneEnvelope.ComputeAmplitude(
            position: 100,
            perSegmentSamples: 0,
            totalSamples: 1000,
            fadeSamples: 10,
            targetAmplitude: Target);

        Assert.Equal(Target, amp);
    }

    private static double Compute(long position)
        => SineToneEnvelope.ComputeAmplitude(
            position: position,
            perSegmentSamples: PerSegment,
            totalSamples: Total,
            fadeSamples: Fade,
            targetAmplitude: Target);
}