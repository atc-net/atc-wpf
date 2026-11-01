namespace Atc.Wpf.Forms.Tests.XUnitTestTypes;

[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "The extractor must see List<T> properties.")]
[SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "The extractor must see array properties.")]
[SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "The writer must be able to set the collections.")]
public sealed class MultiSelectModel
{
    [Required]
    public AccessRights Rights { get; set; }

    public AccessRights? OptionalRights { get; set; }

    [MaxLength(3)]
    public List<DayOfWeek> WorkDays { get; set; } = [];

    public DayOfWeek[]? DaysOff { get; set; }

    public IEnumerable<DayOfWeek>? MeetingDays { get; set; }

    [Required]
    public List<string> Tags { get; set; } = [];

    public string[]? Aliases { get; set; }

    [Length(1, 2)]
    public ICollection<string>? Labels { get; set; }

    public IList<string> Notes { get; } = new List<string>();
}