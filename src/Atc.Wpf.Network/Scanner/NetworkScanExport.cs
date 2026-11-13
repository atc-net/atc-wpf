namespace Atc.Wpf.Network.Scanner;

/// <summary>
/// Writes network scanner rows as CSV or JSON.
/// </summary>
/// <remarks>
/// The column names are English and fixed, so exported files read the same in every UI language.
/// Cells that start with <c>=</c>, <c>+</c>, <c>-</c>, <c>@</c>, tab or CR get an apostrophe prefix,
/// so spreadsheets read them as text and not as a formula.
/// </remarks>
public static class NetworkScanExport
{
    private static readonly string[] CsvHeader =
    [
        "IP address",
        "Status",
        "Device name",
        "Host name",
        "Workgroup",
        "MAC address",
        "MAC vendor",
        "Model",
        "Open ports",
        "Services",
        "ONVIF",
    ];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    /// <summary>
    /// Writes the hosts as CSV with a header line.
    /// </summary>
    /// <param name="hosts">The hosts.</param>
    /// <returns>The CSV text.</returns>
    public static string ToCsv(IEnumerable<NetworkHostViewModel> hosts)
    {
        ArgumentNullException.ThrowIfNull(hosts);

        var sb = new StringBuilder();
        sb.AppendLine(string.Join(',', CsvHeader));
        foreach (var host in hosts)
        {
            string?[] values =
            [
                host.IpAddress.ToString(),
                host.PingStatus?.Status.ToString(),
                host.DeviceName,
                host.Hostname,
                host.Workgroup,
                host.MacAddress,
                host.MacVendor,
                host.Model,
                host.OpenPortNumbersAsCommaList,
                host.ServicesAsCommaList,
                host.OnvifDeviceServiceUrl?.ToString(),
            ];

            sb.AppendLine(string.Join(',', values.Select(EscapeCsv)));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Writes the hosts as a JSON array.
    /// </summary>
    /// <param name="hosts">The hosts.</param>
    /// <returns>The JSON text.</returns>
    public static string ToJson(IEnumerable<NetworkHostViewModel> hosts)
    {
        ArgumentNullException.ThrowIfNull(hosts);

        var items = hosts.Select(host => new
        {
            IpAddress = host.IpAddress.ToString(),
            Status = host.PingStatus?.Status.ToString(),
            host.DeviceName,
            host.Hostname,
            host.Workgroup,
            host.MacAddress,
            host.MacVendor,
            host.Model,
            OpenPorts = host.OpenPortNumbers.ToArray(),
            host.Services,
            OnvifDeviceServiceUrl = host.OnvifDeviceServiceUrl?.ToString(),
        });

        return JsonSerializer.Serialize(items, JsonOptions);
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        // Device names come from the devices themselves; a leading formula character would run
        // in a spreadsheet, so the cell is marked as text with an apostrophe.
        var isFormulaLike = value[0] is '=' or '+' or '-' or '@' or '\t' or '\r';
        if (isFormulaLike)
        {
            value = "'" + value;
        }

        return isFormulaLike || value.AsSpan().IndexOfAny(",\"\r\n") >= 0
            ? $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : value;
    }
}