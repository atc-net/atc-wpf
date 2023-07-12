namespace Atc.Wpf.Controls.XUnitTestTypes;

public class PrimitiveTypesModel
{
    public byte PropertyByte { get; set; }

    public byte? PropertyByteNullable { get; set; }

    public sbyte PropertyByteSigned { get; set; }

    public sbyte? PropertyByteNullableSigned { get; set; }

    public short PropertyShort { get; set; }

    public short? PropertyShortNullable { get; set; }

    public ushort PropertyShortSigned { get; set; }

    public ushort? PropertyShortNullableSigned { get; set; }

    public int PropertyInt { get; set; }

    public int? PropertyIntNullable { get; set; }

    public uint PropertyIntSigned { get; set; }

    public uint? PropertyIntNullableSigned { get; set; }

    public long PropertyLong { get; set; }

    public long? PropertyLongNullable { get; set; }

    public ulong PropertyLongSigned { get; set; }

    public ulong? PropertyLongNullableSigned { get; set; }

    public float PropertyFloat { get; set; }

    public float? PropertyFloatNullable { get; set; }

    public double PropertyDouble { get; set; }

    public double? PropertyDoubleNullable { get; set; }

    public decimal PropertyDecimal { get; set; }

    public decimal? PropertyDecimalNullable { get; set; }

    public bool PropertyBool { get; set; }

    public bool? PropertyBoolNullable { get; set; }

    public char PropertyChar { get; set; }

    public char? PropertyCharNullable { get; set; }

    public string PropertyString { get; set; } = string.Empty;

    public string? PropertyStringNullable { get; set; }

    public DayOfWeek PropertyEnum { get; set; } = DayOfWeek.Friday;

    public DayOfWeek? PropertyEnumNullable { get; set; }

    public DateTime PropertyDateTime { get; set; }

    public DateTime? PropertyDateTimeNullable { get; set; }

    public DateOnly PropertyDateOnly { get; set; }

    public DateOnly? PropertyDateOnlyNullable { get; set; }

    public TimeOnly PropertyTimeOnly { get; set; }

    public TimeOnly? PropertyTimeOnlyNullable { get; set; }

    public DateTimeOffset PropertyDateTimeOffset { get; set; }

    public DateTimeOffset? PropertyDateTimeOffsetNullable { get; set; }

    public CultureInfo PropertyCountry { get; set; } = GlobalizationConstants.DanishCultureInfo;

    public CultureInfo? PropertyCountryNullable { get; set; }

    public CultureInfo PropertyLanguage { get; set; } = GlobalizationConstants.DanishCultureInfo;

    public CultureInfo? PropertyLanguageNullable { get; set; }
}