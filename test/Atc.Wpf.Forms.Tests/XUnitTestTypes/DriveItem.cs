namespace Atc.Wpf.Forms.Tests.XUnitTestTypes;

public sealed class DriveItem
{
    [Required]
    [MinLength(2)]
    public string? Name { get; set; }

    [Required]
    public DirectoryInfo? Directory { get; set; }

    [Required]
    public FileInfo? File { get; set; }

    public override string ToString()
        => $"{nameof(Name)}: {Name}, {nameof(Directory)}: {Directory?.FullName}, {nameof(File)}: {File?.FullName}";
}