namespace Atc.Wpf.Controls.Tests.DataGrids;

public sealed class TestOrder
{
    public int Id { get; set; }

    public string? Customer { get; set; }

    public string Country { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public TestAddress? Address { get; set; }
}