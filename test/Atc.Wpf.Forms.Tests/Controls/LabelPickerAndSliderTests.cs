namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelPickerAndSliderTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void LabelSlider_IsValid_WithAValidValue_IsValid()
    {
        var sut = new LabelSlider { Value = 5 };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void LabelDatePicker_Text_Changed_RaisesTextChangedWithOldAndNewValue()
    {
        var sut = new LabelDatePicker { Text = "01-02-2026" };
        var textBefore = sut.Text;
        var raised = new List<RoutedPropertyChangedEventArgs<string>>();
        sut.AddHandler(
            LabelDatePicker.TextChangedEvent,
            new RoutedPropertyChangedEventHandler<string>((_, e) => raised.Add(e)));

        sut.Text = "03-04-2026";

        // The picker may re-format the parsed date afterwards, which is a further (legitimate) text change.
        Assert.NotEmpty(raised);
        Assert.Equal(textBefore, raised[0].OldValue);
        Assert.Equal("03-04-2026", raised[0].NewValue);
    }

    [StaFact]
    public void LabelTimePicker_Text_Changed_RaisesTextChangedWithOldAndNewValue()
    {
        // en-US re-formats "09:30" to "9:30 AM", so the run doesn't depend on the machine's culture.
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        var sut = new LabelTimePicker { Text = "08:00" };
        var textBefore = sut.Text;
        var raised = new List<RoutedPropertyChangedEventArgs<string>>();
        sut.AddHandler(
            LabelTimePicker.TextChangedEvent,
            new RoutedPropertyChangedEventHandler<string>((_, e) => raised.Add(e)));

        sut.Text = "09:30";

        // The picker re-formats the parsed time in the UI culture afterwards, which is a further (legitimate) text change.
        Assert.NotEmpty(raised);
        Assert.Equal(textBefore, raised[0].OldValue);
        Assert.Equal("09:30", raised[0].NewValue);
    }

    [StaFact]
    public void LabelDatePicker_IsValid_MandatoryAndEmpty_ReportsFieldIsRequired()
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        var sut = new LabelDatePicker { IsMandatory = true };

        Assert.False(sut.IsValid());
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void LabelDatePicker_IsValid_WithASelectedDate_IsValid()
    {
        var sut = new LabelDatePicker { IsMandatory = true, SelectedDate = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Unspecified) };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void LabelTimePicker_IsValid_MandatoryAndEmpty_ReportsFieldIsRequired()
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        var sut = new LabelTimePicker { IsMandatory = true };

        Assert.False(sut.IsValid());
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void LabelTimePicker_IsValid_WithASelectedTime_IsValid()
    {
        var sut = new LabelTimePicker { IsMandatory = true, SelectedTime = new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Unspecified) };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void LabelDateTimePicker_IsValid_MandatoryAndEmpty_ReportsFieldIsRequired()
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        var sut = new LabelDateTimePicker { IsMandatory = true };

        Assert.False(sut.IsValid());
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void LabelDateTimePicker_IsValid_WithASelectedDate_IsValid()
    {
        var sut = new LabelDateTimePicker { IsMandatory = true, SelectedDate = new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Unspecified) };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void LabelDateTimePicker_TimeTextCommitted_LostFocusValidCarriesTheDateAndTheTime()
    {
        UseEnglishUi();
        var sut = new LabelDateTimePicker
        {
            CustomCulture = CultureInfo.GetCultureInfo("en-US"),
            SelectedDate = new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Unspecified),
        };
        ValueChangedEventArgs<DateTime?>? raised = null;
        sut.LostFocusValid += (_, e) => raised = e;

        sut.TextTime = "2:45 PM";

        Assert.NotNull(raised);
        Assert.Equal(new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Unspecified), raised.OldValue);
        Assert.Equal(new DateTime(2026, 1, 2, 14, 45, 0, DateTimeKind.Unspecified), raised.NewValue);
        Assert.Equal(new DateTime(2026, 1, 2, 14, 45, 0, DateTimeKind.Unspecified), sut.SelectedDate);
    }

    [StaFact]
    public void LabelDateTimePicker_DateTextCommitted_LostFocusValidKeepsTheTime()
    {
        UseEnglishUi();
        var sut = new LabelDateTimePicker
        {
            CustomCulture = CultureInfo.GetCultureInfo("en-US"),
            SelectedDate = new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Unspecified),
        };
        ValueChangedEventArgs<DateTime?>? raised = null;
        sut.LostFocusValid += (_, e) => raised = e;

        sut.TextDate = "3/4/2026";

        Assert.NotNull(raised);
        Assert.Equal(new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Unspecified), raised.OldValue);
        Assert.Equal(new DateTime(2026, 3, 4, 9, 30, 0, DateTimeKind.Unspecified), raised.NewValue);
    }

    [StaFact]
    public void LabelDatePicker_InvalidTextCommitted_KeepsTheTextAndShowsTheError()
    {
        UseEnglishUi();
        var sut = new LabelDatePicker
        {
            CustomCulture = CultureInfo.GetCultureInfo("en-US"),
            SelectedDate = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Unspecified),
        };

        sut.Text = "not a date";

        Assert.Equal("not a date", sut.Text);
        Assert.Null(sut.SelectedDate);
        Assert.False(string.IsNullOrEmpty(sut.ValidationText));
    }

    [StaFact]
    public void LabelTimePicker_InvalidTextCommitted_KeepsTheTextAndShowsTheError()
    {
        UseEnglishUi();
        var sut = new LabelTimePicker
        {
            CustomCulture = CultureInfo.GetCultureInfo("en-US"),
            SelectedTime = new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Unspecified),
        };

        sut.Text = "not a time";

        Assert.Equal("not a time", sut.Text);
        Assert.Null(sut.SelectedTime);
        Assert.False(string.IsNullOrEmpty(sut.ValidationText));
    }

    [StaFact]
    public void LabelDateTimePicker_InvalidTimeTextCommitted_KeepsBothTextsAndShowsTheError()
    {
        UseEnglishUi();
        var sut = new LabelDateTimePicker
        {
            CustomCulture = CultureInfo.GetCultureInfo("en-US"),
            SelectedDate = new DateTime(2026, 1, 2, 9, 30, 0, DateTimeKind.Unspecified),
        };
        var dateTextBefore = sut.TextDate;

        sut.TextTime = "not a time";

        Assert.Equal(dateTextBefore, sut.TextDate);
        Assert.Equal("not a time", sut.TextTime);
        Assert.Null(sut.SelectedDate);
        Assert.False(string.IsNullOrEmpty(sut.ValidationText));
    }

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
}