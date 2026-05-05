namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// COM interface used to access the raw byte pointer behind a WinRT
/// <c>Windows.Foundation.IMemoryBufferReference</c>. Required for the audio capture / render
/// pipeline since <c>AudioBuffer</c> does not expose a <c>CopyToBuffer(IBuffer)</c> overload.
/// </summary>
[ComImport]
[Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal unsafe interface IMemoryBufferByteAccess
{
    void GetBuffer(
        out byte* buffer,
        out uint capacity);
}