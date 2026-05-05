namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Bridges WinRT <c>AudioFrame</c> data buffers to managed <c>Span&lt;float&gt;</c>.
/// Uses manual <c>QueryInterface</c> + vtable invocation against
/// <c>IMemoryBufferByteAccess</c> instead of a managed <c>[ComImport]</c> cast — the
/// cast pattern that works in UWP throws <see cref="InvalidCastException"/> under the
/// CsWinRT projection used by .NET 5+ / WPF.
/// </summary>
internal static class AudioBufferAccess
{
    private static readonly Guid IidMemoryBufferByteAccess
        = new("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D");

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
    /// Writes <paramref name="source"/> floats into <paramref name="frame"/>'s buffer
    /// and sets <c>AudioBuffer.Length</c> to the byte count written. The Length must be
    /// set explicitly under CsWinRT — its default value of 0 makes <c>AddFrame</c>
    /// throw <c>ArgumentException("Length must be greater than zero")</c> — and it must
    /// be set <em>before</em> the <c>IMemoryBufferReference</c> is opened, otherwise
    /// the property setter doesn't propagate back to the underlying WinRT buffer.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Audio buffer access must not crash the tester on transient errors.")]
    public static void WriteFloatSamples(
        Windows.Media.AudioFrame frame,
        ReadOnlySpan<float> source)
    {
        try
        {
            using var buffer = frame.LockBuffer(Windows.Media.AudioBufferAccessMode.Write);

            var capacity = buffer.Capacity;
            var bytesToWrite = (uint)(source.Length * sizeof(float));
            if (bytesToWrite > capacity)
            {
                bytesToWrite = capacity;
            }

            // Set Length BEFORE creating the IMemoryBufferReference. The order matters
            // under CsWinRT: setting Length while a reference is open is a no-op.
            buffer.Length = bytesToWrite;

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
    /// Manually <c>QueryInterface</c>s the WinRT memory-buffer reference for the
    /// <c>IMemoryBufferByteAccess</c> COM interface and invokes <c>GetBuffer</c>
    /// through the vtable. The naive <c>(IMemoryBufferByteAccess)reference</c> cast
    /// throws <see cref="InvalidCastException"/> under CsWinRT.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Buffer access must not crash on transient COM failures.")]
    private static unsafe bool TryGetBufferPointer(
        Windows.Foundation.IMemoryBufferReference reference,
        out byte* dataInBytes,
        out uint capacity)
    {
        dataInBytes = null;
        capacity = 0;

        var unkPtr = Marshal.GetIUnknownForObject(reference);
        if (unkPtr == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            var iid = IidMemoryBufferByteAccess;
            var hr = Marshal.QueryInterface(unkPtr, in iid, out var byteAccessPtr);
            if (hr < 0 || byteAccessPtr == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                // Vtable layout (inherits IUnknown):
                //  [0] QueryInterface
                //  [1] AddRef
                //  [2] Release
                //  [3] GetBuffer(out byte** buffer, out uint* capacity)  -> HRESULT
                var vtable = *(IntPtr**)byteAccessPtr;
                var getBufferFn =
                    (delegate* unmanaged[Stdcall]<IntPtr, byte**, uint*, int>)vtable[3];

                byte* outBuffer;
                uint outCapacity;
                var callHr = getBufferFn(byteAccessPtr, &outBuffer, &outCapacity);
                if (callHr < 0)
                {
                    return false;
                }

                dataInBytes = outBuffer;
                capacity = outCapacity;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                Marshal.Release(byteAccessPtr);
            }
        }
        finally
        {
            Marshal.Release(unkPtr);
        }
    }
}