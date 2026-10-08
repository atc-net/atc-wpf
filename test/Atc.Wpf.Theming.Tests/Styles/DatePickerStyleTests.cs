namespace Atc.Wpf.Theming.Tests.Styles;

public sealed class DatePickerStyleTests
{
    public DatePickerStyleTests()
    {
        // The style uses SvgImage from Atc.Wpf, and pack://application URIs need the Application type initialized.
        _ = typeof(Atc.Wpf.Controls.Media.SvgImage).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    [StaFact]
    public void CalendarButton_IconIsNotMirroredInRightToLeft()
    {
        var resources = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Theming;component/Styles/BaseControls/DatePicker.xaml"),
        };
        var button = new Button { Style = (Style)resources["AtcApps.Styles.DropDownButtonStyle"] };
        _ = new Border { FlowDirection = FlowDirection.RightToLeft, Child = button };

        button.ApplyTemplate();

        var icon = (FrameworkElement)button.Template.FindName("CalenderImage", button);
        Assert.Equal(FlowDirection.LeftToRight, icon.FlowDirection);
    }
}