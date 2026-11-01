namespace Atc.Wpf.Forms.Tests.XUnitTestTypes;

public sealed class DatesModel
{
    public DateTime DateTimeValue { get; set; }

    public DateTime? DateTimeNullable { get; set; }

    public DateOnly DateOnlyValue { get; set; }

    public DateOnly? DateOnlyNullable { get; set; }

    public TimeOnly TimeOnlyValue { get; set; }

    public TimeOnly? TimeOnlyNullable { get; set; }

    public DateTimeOffset DateTimeOffsetValue { get; set; }

    public DateTimeOffset? DateTimeOffsetNullable { get; set; }
}