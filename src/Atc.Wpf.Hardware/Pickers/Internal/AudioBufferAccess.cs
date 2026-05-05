namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Bridges WinRT <c>AudioFrame</c> data buffers to managed <c>float[]</c> via the
/// <c>IMemoryBufferByteAccess</c> COM interface. Only place in the assembly that uses
/// <c>unsafe</c>; everything else stays in safe code.
/// </summary>
internal static class AudioBufferAccess
{
    /// <summary>
    /// Copies the float-encoded samples from <paramref name="frame"/> into <paramref name="destination"/>
    /// and returns the number of floats actually copied (capped at the destination length).
    /// Returns 0 on any failure.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Audio buffer access must not crash the picker on transient errors.")]
    public static int ReadFloatSamples(
        Windows.Media.AudioFrame frame,
        Windows.Media.AudioBufferAccessMode mode,
        Span<float> destination)
    {
        try
        {
            using var buffer = frame.LockBuffer(mode);
            using var reference = buffer.CreateReference();
            return CopyBytes(reference, destination);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    /// <summary>
    /// Writes <paramref name="source"/> floats into <paramref name="frame"/>'s buffer.
    /// Mirrors the official Microsoft AudioCreation Scenario3_FrameInputNode pattern:
    /// the buffer is written via the COM byte access and committed when the lock is
    /// released. <c>AudioBuffer.Length</c> is not set explicitly — the WinRT engine
    /// uses the AudioFrame's allocation capacity as the data length when in Write mode.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Audio buffer access must not crash the tester on transient errors.")]
    public static void WriteFloatSamples(
        Windows.Media.AudioFrame frame,
        ReadOnlySpan<float> source)
    {
        try
        {
            using var buffer = frame.LockBuffer(Windows.Media.AudioBufferAccessMode.Write);
            using var reference = buffer.CreateReference();
            WriteBytes(reference, source);
        }
        catch (Exception)
        {
            // Skip this frame.
        }
    }

    [SuppressMessage("Reliability", "CA2020:Prevent behavioral change", Justification = "Native pointer arithmetic.")]
    private static unsafe int CopyBytes(
        Windows.Foundation.IMemoryBufferReference reference,
        Span<float> destination)
    {
        ((IMemoryBufferByteAccess)reference).GetBuffer(out var dataInBytes, out var capacity);

        var floatCount = (int)(capacity / sizeof(float));
        if (floatCount > destination.Length)
        {
            floatCount = destination.Length;
        }

        var floatSpan = new ReadOnlySpan<float>(dataInBytes, floatCount);
        floatSpan.CopyTo(destination);
        return floatCount;
    }

    [SuppressMessage("Reliability", "CA2020:Prevent behavioral change", Justification = "Native pointer arithmetic.")]
    private static unsafe int WriteBytes(
        Windows.Foundation.IMemoryBufferReference reference,
        ReadOnlySpan<float> source)
    {
        ((IMemoryBufferByteAccess)reference).GetBuffer(out var dataInBytes, out var capacity);

        var maxFloats = (int)(capacity / sizeof(float));
        var count = source.Length;
        if (count > maxFloats)
        {
            count = maxFloats;
        }

        var dest = new Span<float>(dataInBytes, count);
        source[..count].CopyTo(dest);
        return count * sizeof(float);
    }
}