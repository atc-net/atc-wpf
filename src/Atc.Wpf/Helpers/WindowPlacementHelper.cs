namespace Atc.Wpf.Helpers;

public static class WindowPlacementHelper
{
    private const int ShowNormal = 1;
    private const int ShowMinimized = 2;
    private static readonly Encoding Encoding = new UTF8Encoding();
    private static readonly XmlSerializer Serializer = new(typeof(WINDOWPLACEMENT));

    public static void SetPlacement(
        IntPtr windowHandle,
        string placementXml)
    {
        if (string.IsNullOrEmpty(placementXml))
        {
            return;
        }

        var xmlBytes = Encoding.GetBytes(placementXml);

        try
        {
            using var memoryStream = new MemoryStream(xmlBytes);
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
            };
            using var reader = XmlReader.Create(
                memoryStream,
                settings);
            var placement = (WINDOWPLACEMENT)Serializer.Deserialize(reader)!;

            placement.length = Marshal.SizeOf<WINDOWPLACEMENT>();
            placement.flags = 0;
            placement.showCmd = placement.showCmd == ShowMinimized
                ? ShowNormal
                : placement.showCmd;

            NativeMethods.SetWindowPlacement(
                windowHandle,
                ref placement);
        }
        catch (InvalidOperationException)
        {
            // Parsing placement XML failed. Fail silently.
        }
    }

    public static string GetPlacement(IntPtr windowHandle)
    {
        NativeMethods.GetWindowPlacement(
            windowHandle,
            out var placement);

        using var memoryStream = new MemoryStream();
        using var xmlTextWriter = new XmlTextWriter(
            memoryStream,
            Encoding.UTF8);
        Serializer.Serialize(
            xmlTextWriter,
            placement);
        var xmlBytes = memoryStream.ToArray();
        return Encoding.GetString(xmlBytes);
    }
}