namespace Atc.Wpf.Controls.Tests.Selectors;

// The country and language selectors list localized names, so these tests keep the UI culture still.
[Collection(nameof(UiCultureTestGroup))]
public sealed class SelectorDataContextTests : IDisposable
{
    private readonly CultureInfo originalUiCulture = Atc.Wpf.Translation.CultureManager.UiCulture;

    public void Dispose()
    {
        Atc.Wpf.Translation.CultureManager.UiCulture = originalUiCulture;
        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void CountrySelector_SelectedKeyBinding_FlowsBothWays()
        => AssertSelectedKeyFlowsBothWays(new CountrySelector(), CountrySelector.SelectedKeyProperty, "1031");

    [StaFact]
    public void LanguageSelector_SelectedKeyBinding_FlowsBothWays()
        => AssertSelectedKeyFlowsBothWays(new LanguageSelector { UpdateUiCultureOnChangeEvent = false }, LanguageSelector.SelectedKeyProperty, "1031");

    [StaFact]
    public void FontFamilySelector_SelectedKeyBinding_FlowsBothWays()
        => AssertSelectedKeyFlowsBothWays(new FontFamilySelector(), FontFamilySelector.SelectedKeyProperty, "Arial");

    [StaFact]
    public void WellKnownColorPicker_Bindings_UseTheInheritedDataContext()
    {
        var viewModel = new SelectorPageViewModel { ShowOnlyBasicColors = true };
        var sut = new Atc.Wpf.Controls.ColorEditing.WellKnownColorPicker();
        sut.SetBinding(
            Atc.Wpf.Controls.ColorEditing.WellKnownColorPicker.ShowOnlyBasicColorsProperty,
            new Binding(nameof(SelectorPageViewModel.ShowOnlyBasicColors)));
        sut.SetBinding(
            Atc.Wpf.Controls.ColorEditing.WellKnownColorPicker.ColorBrushProperty,
            new Binding(nameof(SelectorPageViewModel.ColorBrush)) { Mode = BindingMode.TwoWay });

        RunInWindow(
            new ContentControl { DataContext = viewModel, Content = sut },
            () =>
            {
                Assert.True(sut.ShowOnlyBasicColors);

                var palette = (ItemsControl)sut.Content;
                Assert.Same(sut.Palette, palette.ItemsSource);
                Assert.NotEmpty(palette.Items);

                sut.SetCurrentValue(
                    Atc.Wpf.Controls.ColorEditing.WellKnownColorPicker.ColorBrushProperty,
                    Brushes.Red);

                Assert.Same(Brushes.Red, viewModel.ColorBrush);
            });
    }

    private static void AssertSelectedKeyFlowsBothWays(
        UserControl sut,
        DependencyProperty selectedKeyProperty,
        string initialKey)
    {
        var viewModel = new SelectorPageViewModel { SelectedKey = initialKey };
        sut.SetBinding(
            selectedKeyProperty,
            new Binding(nameof(SelectorPageViewModel.SelectedKey)) { Mode = BindingMode.TwoWay });

        RunInWindow(
            new ContentControl { DataContext = viewModel, Content = sut },
            () =>
            {
                // View model to control.
                Assert.Equal(initialKey, (string)sut.GetValue(selectedKeyProperty));

                var comboBox = (ComboBox)sut.Content;
                Assert.Equal(initialKey, Convert.ToString(comboBox.SelectedValue, CultureInfo.InvariantCulture));

                // Control to view model: the user picks another item.
                comboBox.SelectedIndex = comboBox.SelectedIndex == 0 ? 1 : 0;
                var pickedKey = Convert.ToString(comboBox.SelectedValue, CultureInfo.InvariantCulture);

                Assert.NotEqual(initialKey, pickedKey, StringComparer.Ordinal);
                Assert.Equal(pickedKey, (string)sut.GetValue(selectedKeyProperty));
                Assert.Equal(pickedKey, viewModel.SelectedKey);
            });
    }

    private static void RunInWindow(
        FrameworkElement content,
        Action action)
    {
        var window = new Window
        {
            Content = content,
            Width = 300,
            Height = 400,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };
        window.Show();
        try
        {
            window.UpdateLayout();
            action();
        }
        finally
        {
            window.Close();
        }
    }
}