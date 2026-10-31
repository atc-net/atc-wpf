namespace Atc.Wpf.Forms.Tests.XUnitTestTypes;

public sealed class IntegersModel
{
    public long LongValue { get; set; }

    public long? LongNullable { get; set; }

    public ulong UnsignedLongValue { get; set; }

    public uint UnsignedIntValue { get; set; }

    public ushort UnsignedShortValue { get; set; }

    public short ShortValue { get; set; }

    public DayOfWeek DayValue { get; set; } = DayOfWeek.Monday;

    public DayOfWeek? DayNullable { get; set; }
}