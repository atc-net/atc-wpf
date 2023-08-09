namespace Atc.Wpf.Sample.SamplesWpfTheming.InputSelector;

/// <summary>
/// Interaction logic for CalendarView.
/// </summary>
public partial class CalendarView
{
    public CalendarView()
    {
        InitializeComponent();

        Calendar2.DisplayDateStart = DateTime.Now.AddDays(-10);
        Calendar2.DisplayDateEnd = DateTime.Now.AddDays(10);

        var startDate = DateTime.Now.AddDays(-6);
        var endDate = startDate.AddDays(3);
        var calendarDateRange1 = new CalendarDateRange(startDate, endDate);

        Calendar2.BlackoutDates.Add(calendarDateRange1);
    }
}