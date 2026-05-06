namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Bridges WinRT <c>AudioFrame</c> data buffers to managed <c>Span&lt;float&gt;</c>.
/// Uses <c>WinRT.CastExtensions.As&lt;IMemoryBufferByteAccess&gt;()</c> from CsWinRT —
/// the direct <c>(IMemoryBufferByteAccess)reference</c> cast that's documented in the
/// UWP samples throws <see cref="InvalidCastException"/> under the CsWinRT projection
/// used by .NET 5+ / WPF, and a manual <c>QueryInterface</c> on the CCW returned by
/// <c>Marshal.GetIUnknownForObject</c> cannot reach the underlying ABI object on
/// read-side frames.
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
    /// The <see cref="Windows.Media.AudioFrame"/> must be allocated at exactly
    /// <c>source.Length * sizeof(float)</c> bytes — that allocation size <em>is</em>
    /// the buffer length the audio engine will read. Do NOT set
    /// <c>AudioBuffer.Length</c> after the fact; the setter is unreliable under
    /// CsWinRT and the canonical UWP sample never touches it.
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

    private static unsafe int CopyBytes(
        Windows.Foundation.IMemoryBufferReference reference,
        Span<float> destination)
    {
        if (!TryGetBufferPointer(reference, out var dataInBytes, out var capacity))
        {
            return 0;
        }

        var floatCount = (int)(capacity / sizeof(float));
        if (floatCount > destination.Length)
        {
            floatCount = destination.Length;
        }

        var floatSpan = new ReadOnlySpan<float>(dataInBytes, floatCount);
        floatSpan.CopyTo(destination);
        return floatCount;
    }

    private static unsafe void WriteBytes(
        Windows.Foundation.IMemoryBufferReference reference,
        ReadOnlySpan<float> source)
    {
        if (!TryGetBufferPointer(reference, out var dataInBytes, out var capacity))
        {
            return;
        }

        var maxFloats = (int)(capacity / sizeof(float));
        var count = source.Length;
        if (count > maxFloats)
        {
            count = maxFloats;
        }

        var dest = new Span<float>(dataInBytes, count);
        source[..count].CopyTo(dest);
    }

    /// <summary>
    /// Casts the WinRT memory-buffer reference to <see cref="IMemoryBufferByteAccess"/>
    /// via <see cref="WinRT.CastExtensions.As{TInterface}"/> and invokes <c>GetBuffer</c>.
    /// CsWinRT routes the call through the underlying ABI object, which the naive
    /// managed cast and the <c>Marshal.GetIUnknownForObject</c> + <c>QueryInterface</c>
    /// route cannot reach for read-mode frames.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Buffer access must not crash on transient COM failures.")]
    private static unsafe bool TryGetBufferPointer(
        Windows.Foundation.IMemoryBufferReference reference,
        out byte* dataInBytes,
        out uint capacity)
    {
        dataInBytes = null;
        capacity = 0;

        try
        {
            var byteAccess = WinRT.CastExtensions.As<IMemoryBufferByteAccess>(reference);
            byteAccess.GetBuffer(out var outBuffer, out var outCapacity);
            dataInBytes = outBuffer;
            capacity = outCapacity;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}