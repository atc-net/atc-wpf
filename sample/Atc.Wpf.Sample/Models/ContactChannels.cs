namespace Atc.Wpf.Sample.Models;

[Flags]
public enum ContactChannels
{
    None = 0,
    Email = 1,
    Phone = 2,
    Sms = 4,
    Post = 8,
}