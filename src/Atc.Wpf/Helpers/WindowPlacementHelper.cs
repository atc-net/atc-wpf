namespace Atc.Wpf.Helpers;

public static class WindowPlacementHelper
{
    private const int ShowNormal = 1;
    private const int ShowMinimized = 2;
    private static readonly Encoding Encoding = new UTF8Encoding();
    private static readonly XmlSerializer Serializer = new(typeof(WINDOWPLACEMENT));

    [SuppressMessage("Security", "CA5369:Use XmlReader for 'XmlSerializer.Deserialize()'", Justification = "OK.")]
    public static void SetPlacement(IntPtr windowHandle, string placementXml)
    {
        if (string.IsNullOrEmpty(placementXml))
        {
            return;
        }

        var xmlBytes = Encoding.GetBytes(placementXml);

        try
        {
            using var memoryStream = new MemoryStream(xmlBytes);
            var placement = (WINDOWPLACEMENT)Serializer.Deserialize(memoryStream)!;

            placement.length = Marshal.SizeOf<WINDOWPLACEMENT>();
            placement.flags = 0;
            placement.showCmd = placement.showCmd == ShowMinimized
                ? ShowNormal
                : placement.showCmd;

            NativeMethods.SetWindowPlacement(windowHandle, ref placement);
        }
        catch (InvalidOperationException)
        {
            // Parsing placement XML failed. Fail silently.
        }
    }

    public static string GetPlacement(IntPtr windowHandle)
    {
        NativeMethods.GetWindowPlacement(windowHandle, out var placement);

        using var memoryStream = new MemoryStream();
        using var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
        Serializer.Serialize(xmlTextWriter, placement);
        var xmlBytes = memoryStream.ToArray();
        return Encoding.GetString(xmlBytes);
    }
}