namespace Atc.Wpf.Forms.Tests.FontEditing;

public sealed class AdvancedFontPickerTests
{
    [StaFact]
    public void CanBeCreatedWithoutApplicationResources()
    {
        var exception = Record.Exception(() => new AdvancedFontPicker());

        Assert.Null(exception);
    }

    [StaFact]
    public void TheDefaultFamily_HasItsWeightsStylesAndStretchesListed()
    {
        var sut = new AdvancedFontPicker();

        Assert.Equal("Segoe UI", sut.SelectedFontFamily?.Source);
        Assert.Contains(FontWeights.Normal, sut.AvailableFontWeights);
        Assert.Contains(FontWeights.Bold, sut.AvailableFontWeights);
        Assert.Contains(FontStyles.Normal, sut.AvailableFontStyles);
        Assert.Contains(FontStretches.Normal, sut.AvailableFontStretches);
    }

    [StaFact]
    public void ChoosingAFamily_ListsItsTypefaces()
    {
        var sut = new AdvancedFontPicker();

        sut.SelectedFontFamily = new FontFamily("Courier New");

        Assert.Contains(FontWeights.Bold, sut.AvailableFontWeights);
        Assert.Contains(FontStyles.Italic, sut.AvailableFontStyles);
        Assert.Equal(
            sut.AvailableFontWeights.OrderBy(w => w.ToOpenTypeWeight()),
            sut.AvailableFontWeights);
    }

    [StaFact]
    public void ChoosingAFamilyWithoutTheSelectedWeight_SnapsToTheClosestWeight()
    {
        var sut = new AdvancedFontPicker
        {
            SelectedFontWeight = FontWeights.Black,
        };

        sut.SelectedFontFamily = new FontFamily("Courier New");

        Assert.Contains(sut.SelectedFontWeight, sut.AvailableFontWeights);
        Assert.Equal(FontWeights.Bold, sut.SelectedFontWeight);
    }

    [StaTheory]
    [InlineData(13.0, 5)]
    [InlineData(100.0, 16)]
    [InlineData(7.0, 0)]
    public void ASizeNotInTheList_IsInsertedInOrder(
        double size,
        int expectedIndex)
    {
        var sut = new AdvancedFontPicker();

        sut.SelectedFontSize = size;

        Assert.Equal(expectedIndex, sut.AvailableFontSizes.IndexOf(size));
        Assert.Equal(sut.AvailableFontSizes.Order(), sut.AvailableFontSizes);
    }

    [StaFact]
    public void ASizeAlreadyInTheList_IsNotAddedTwice()
    {
        var sut = new AdvancedFontPicker();
        var count = sut.AvailableFontSizes.Count;

        sut.SelectedFontSize = 12;

        Assert.Equal(count, sut.AvailableFontSizes.Count);
    }

    [StaFact]
    public void FamilyFilter_ShowsOnlyMatchingFamilies_AndClearingItShowsAll()
    {
        var sut = new AdvancedFontPicker();
        var view = CollectionViewSource.GetDefaultView(sut.AvailableFontFamilies);

        sut.FamilyFilter = "courier";
        var filtered = view.Cast<FontFamily>().ToList();

        Assert.NotEmpty(filtered);
        Assert.All(filtered, f => Assert.Contains("courier", f.Source, StringComparison.OrdinalIgnoreCase));

        sut.FamilyFilter = string.Empty;

        Assert.Equal(sut.AvailableFontFamilies.Count, view.Cast<FontFamily>().Count());
    }

    [StaFact]
    public void QuickToggles_FollowTheSelectedWeightStyleAndDecorations()
    {
        var sut = new AdvancedFontPicker();

        sut.SelectedFontWeight = FontWeights.Bold;
        sut.SelectedFontStyle = FontStyles.Italic;
        sut.SelectedTextDecorations = new TextDecorationCollection(TextDecorations.Underline);

        Assert.True(sut.BoldToggle.IsChecked);
        Assert.True(sut.ItalicToggle.IsChecked);
        Assert.True(sut.UnderlineToggle.IsChecked);
        Assert.False(sut.StrikethroughToggle.IsChecked);
    }

    [StaFact]
    public void QuickToggles_SetTheSelectedWeightAndStyle()
    {
        var sut = new AdvancedFontPicker();

        Toggle(sut.BoldToggle, isChecked: true);
        Toggle(sut.ItalicToggle, isChecked: true);

        Assert.Equal(FontWeights.Bold, sut.SelectedFontWeight);
        Assert.Equal(FontStyles.Italic, sut.SelectedFontStyle);

        Toggle(sut.BoldToggle, isChecked: false);

        Assert.Equal(FontWeights.Normal, sut.SelectedFontWeight);
    }

    [StaFact]
    public void DecorationToggles_AddAndRemoveOnlyTheirOwnDecoration()
    {
        var sut = new AdvancedFontPicker();

        Toggle(sut.UnderlineToggle, isChecked: true);
        Toggle(sut.StrikethroughToggle, isChecked: true);
        Toggle(sut.UnderlineToggle, isChecked: false);

        var locations = sut.SelectedTextDecorations!.Select(d => d.Location).ToList();
        Assert.Equal([TextDecorationLocation.Strikethrough], locations);
    }

    [StaFact]
    public void FontDescription_RoundTrips()
    {
        var sut = new AdvancedFontPicker();
        var foreground = new SolidColorBrush(Colors.DarkRed);
        var background = new SolidColorBrush(Colors.LightYellow);
        var description = new FontDescription(
            new FontFamily("Courier New"),
            18,
            FontWeights.Bold,
            FontStyles.Italic,
            FontStretches.Normal,
            foreground,
            background,
            new TextDecorationCollection(TextDecorations.Underline));

        sut.SetFontDescription(description);
        var result = sut.GetFontDescription();

        Assert.Equal("Courier New", result.Family.Source);
        Assert.Equal(18, result.Size);
        Assert.Equal(FontWeights.Bold, result.Weight);
        Assert.Equal(FontStyles.Italic, result.Style);
        Assert.Same(foreground, result.Foreground);
        Assert.Same(background, result.Background);
        Assert.Equal([TextDecorationLocation.Underline], result.TextDecorations!.Select(d => d.Location));
    }

    [StaFact]
    public void SetFontDescription_WithoutBrushes_KeepsTheCurrentBrushes()
    {
        var sut = new AdvancedFontPicker();
        var foreground = new SolidColorBrush(Colors.DarkRed);
        sut.SelectedForegroundBrush = foreground;

        sut.SetFontDescription(new FontDescription(new FontFamily("Arial"), 12, FontWeights.Normal, FontStyles.Normal, FontStretches.Normal, foreground: null, background: null, textDecorations: null));

        Assert.Same(foreground, sut.SelectedForegroundBrush);
    }

    [StaFact]
    public void RecordCurrentFamilyAsRecent_StoresItAndListsItFirst()
    {
        var original = FontPickerStorage.Current;
        FontPickerStorage.Current = new InMemoryFontPickerStorage();
        try
        {
            var sut = new AdvancedFontPicker { SelectedFontFamily = new FontFamily("Courier New") };

            sut.RecordCurrentFamilyAsRecent();

            Assert.Equal("Courier New", Assert.Single(sut.RecentFontFamilies).Source);
            Assert.Contains("Courier New", FontPickerStorage.Current.GetRecentFontFamilies(), StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            FontPickerStorage.Current = original;
        }
    }

    [StaFact]
    public void RecentFamiliesThatAreNotInstalled_AreLeftOut()
    {
        var original = FontPickerStorage.Current;
        var storage = new InMemoryFontPickerStorage();
        storage.RecordRecentFontFamily("No Such Font Family 1234");
        storage.RecordRecentFontFamily("Arial");
        FontPickerStorage.Current = storage;
        try
        {
            var sut = new AdvancedFontPicker { SelectedFontFamily = new FontFamily("Courier New") };

            sut.RecordCurrentFamilyAsRecent();

            Assert.Equal(
                ["Courier New", "Arial"],
                sut.RecentFontFamilies.Select(f => f.Source),
                StringComparer.Ordinal);
        }
        finally
        {
            FontPickerStorage.Current = original;
        }
    }

    private static void Toggle(
        ToggleButton toggle,
        bool isChecked)
    {
        toggle.IsChecked = isChecked;
        toggle.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    }
}