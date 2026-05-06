namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// Native COM interface that returns the raw byte pointer of an
/// <see cref="Windows.Foundation.IMemoryBufferReference"/>. Required to access
/// the float samples behind <see cref="Windows.Media.AudioFrame"/> buffers.
/// </summary>
/// <remarks>
/// The naive <c>(IMemoryBufferByteAccess)reference</c> cast that's documented
/// in the UWP samples throws <see cref="InvalidCastException"/> under the
/// CsWinRT projection used by .NET 5+ / WPF. The supported alternative is
/// <c>WinRT.CastExtensions.As&lt;IMemoryBufferByteAccess&gt;(reference)</c>,
/// which goes through the WinRT runtime's COM-aggregation path and reaches
/// the underlying ABI object.
/// </remarks>
[ComImport]
[Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal unsafe interface IMemoryBufferByteAccess
{
    void GetBuffer(
        out byte* buffer,
        out uint capacity);
}