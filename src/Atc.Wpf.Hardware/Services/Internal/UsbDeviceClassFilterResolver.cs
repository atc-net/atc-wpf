namespace Atc.Wpf.Hardware.Services.Internal;

internal static class UsbDeviceClassFilterResolver
{
    internal const string UsbDeviceInterfaceGuid = "{a5dcbf10-6530-11d2-901f-00c04fb951ed}";
    internal const string HidInterfaceGuid = "{4d1e55b2-f16f-11cf-88cb-001111000030}";
    internal const string ImagingInterfaceGuid = "{6bdd1fc6-810f-11d0-bec7-08002be2092f}";
    internal const string AudioInterfaceGuid = "{6994ad04-93ef-11d0-a3cc-00a0c9223196}";
    internal const string PrinterInterfaceGuid = "{0ecef634-6ef0-472a-8085-5ad023ecbccd}";
    internal const string MassStorageInterfaceGuid = "{53f56307-b6bf-11d0-94f2-00a0c91efb8b}";
    internal const string ComPortInterfaceGuid = "{86e0d1e0-8089-11d0-9ce4-08003e301f73}";

    public static string ToAqs(UsbDeviceClassFilter filter)
    {
        if (filter is UsbDeviceClassFilter.None)
        {
            return $"System.Devices.InterfaceClassGuid:=\"{UsbDeviceInterfaceGuid}\"";
        }

        var clauses = new List<string>(capacity: 6);

        if (filter.HasFlag(UsbDeviceClassFilter.Hid))
        {
            clauses.Add($"System.Devices.InterfaceClassGuid:=\"{HidInterfaceGuid}\"");
        }

        if (filter.HasFlag(UsbDeviceClassFilter.Imaging))
        {
            clauses.Add($"System.Devices.InterfaceClassGuid:=\"{ImagingInterfaceGuid}\"");
        }

        if (filter.HasFlag(UsbDeviceClassFilter.Audio))
        {
            clauses.Add($"System.Devices.InterfaceClassGuid:=\"{AudioInterfaceGuid}\"");
        }

        if (filter.HasFlag(UsbDeviceClassFilter.Printer))
        {
            clauses.Add($"System.Devices.InterfaceClassGuid:=\"{PrinterInterfaceGuid}\"");
        }

        if (filter.HasFlag(UsbDeviceClassFilter.MassStorage))
        {
            clauses.Add($"System.Devices.InterfaceClassGuid:=\"{MassStorageInterfaceGuid}\"");
        }

        if (filter.HasFlag(UsbDeviceClassFilter.Communication))
        {
            clauses.Add($"System.Devices.InterfaceClassGuid:=\"{ComPortInterfaceGuid}\"");
        }

        return string.Join(" OR ", clauses);
    }
}