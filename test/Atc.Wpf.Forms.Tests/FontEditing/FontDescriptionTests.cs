namespace Atc.Wpf.Forms.Tests.FontEditing;

public sealed class FontDescriptionTests
{
    [StaFact]
    public void Default_Constructor_Creates_Sensible_Defaults()
    {
        var sut = new FontDescription();

        Assert.Equal("Segoe UI", sut.Family.Source);
        Assert.Equal(FontDescription.DefaultSize, sut.Size);
        Assert.Equal(FontWeights.Normal, sut.Weight);
        Assert.Equal(FontStyles.Normal, sut.Style);
        Assert.Equal(FontStretches.Normal, sut.Stretch);
        Assert.Null(sut.Foreground);
        Assert.Null(sut.Background);
    }

    [StaFact]
    public void Equals_Returns_True_For_Same_Values()
    {
        var a = new FontDescription(new FontFamily("Arial"), 14, FontWeights.Bold, FontStyles.Italic, FontStretches.Condensed);
        var b = new FontDescription(new FontFamily("Arial"), 14, FontWeights.Bold, FontStyles.Italic, FontStretches.Condensed);

        Assert.True(a.Equals(b));
        Assert.True(((object)a).Equals(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [StaTheory]
    [InlineData("Arial", "Verdana")]
    public void Equals_Returns_False_For_Different_Family(
        string familyA,
        string familyB)
    {
        var a = new FontDescription(new FontFamily(familyA), 12, FontWeights.Normal, FontStyles.Normal, FontStretches.Normal);
        var b = new FontDescription(new FontFamily(familyB), 12, FontWeights.Normal, FontStyles.Normal, FontStretches.Normal);

        Assert.False(a.Equals(b));
    }

    [StaFact]
    public void Equals_Returns_False_For_Different_Size()
    {
        var a = new FontDescription(new FontFamily("Arial"), 12, FontWeights.Normal, FontStyles.Normal, FontStretches.Normal);
        var b = new FontDescription(new FontFamily("Arial"), 14, FontWeights.Normal, FontStyles.Normal, FontStretches.Normal);

        Assert.False(a.Equals(b));
    }

    [StaFact]
    public void Equals_Returns_False_For_Different_Weight()
    {
        var a = new FontDescription(new FontFamily("Arial"), 12, FontWeights.Normal, FontStyles.Normal, FontStretches.Normal);
        var b = new FontDescription(new FontFamily("Arial"), 12, FontWeights.Bold, FontStyles.Normal, FontStretches.Normal);

        Assert.False(a.Equals(b));
    }

    [StaFact]
    public void Clone_Creates_Equal_But_Distinct_Instance()
    {
        var original = new FontDescription(
            new FontFamily("Arial"),
            14,
            FontWeights.Bold,
            FontStyles.Italic,
            FontStretches.Condensed,
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Yellow));

        var clone = original.Clone();

        Assert.NotSame(original, clone);
        Assert.True(clone.Equals(original));
    }

    [StaFact]
    public void Equals_Returns_True_When_Brush_Colors_Match_Different_Instances()
    {
        var a = new FontDescription(
            new FontFamily("Arial"),
            12,
            FontWeights.Normal,
            FontStyles.Normal,
            FontStretches.Normal,
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Yellow));
        var b = new FontDescription(
            new FontFamily("Arial"),
            12,
            FontWeights.Normal,
            FontStyles.Normal,
            FontStretches.Normal,
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Yellow));

        Assert.True(a.Equals(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [StaFact]
    public void Equals_Returns_False_When_Foreground_Differs()
    {
        var a = new FontDescription(
            new FontFamily("Arial"),
            12,
            FontWeights.Normal,
            FontStyles.Normal,
            FontStretches.Normal,
            new SolidColorBrush(Colors.Red));
        var b = new FontDescription(
            new FontFamily("Arial"),
            12,
            FontWeights.Normal,
            FontStyles.Normal,
            FontStretches.Normal,
            new SolidColorBrush(Colors.Blue));

        Assert.False(a.Equals(b));
    }

    [StaFact]
    public void Equals_Returns_False_When_One_Foreground_Is_Null()
    {
        var a = new FontDescription(
            new FontFamily("Arial"),
            12,
            FontWeights.Normal,
            FontStyles.Normal,
            FontStretches.Normal,
            new SolidColorBrush(Colors.Red));
        var b = new FontDescription(
            new FontFamily("Arial"),
            12,
            FontWeights.Normal,
            FontStyles.Normal,
            FontStretches.Normal);

        Assert.False(a.Equals(b));
    }

    [StaFact]
    public void ApplyTo_TextBlock_Sets_All_Font_Properties()
    {
        var description = new FontDescription(new FontFamily("Verdana"), 18, FontWeights.Bold, FontStyles.Italic, FontStretches.Expanded);
        var textBlock = new TextBlock();

        description.ApplyTo(textBlock);

        Assert.Equal("Verdana", textBlock.FontFamily.Source);
        Assert.Equal(18, textBlock.FontSize);
        Assert.Equal(FontWeights.Bold, textBlock.FontWeight);
        Assert.Equal(FontStyles.Italic, textBlock.FontStyle);
        Assert.Equal(FontStretches.Expanded, textBlock.FontStretch);
    }

    [StaFact]
    public void ApplyTo_TextBlock_Sets_Brushes_When_Provided()
    {
        var description = new FontDescription(
            new FontFamily("Verdana"),
            18,
            FontWeights.Bold,
            FontStyles.Italic,
            FontStretches.Expanded,
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Yellow));
        var textBlock = new TextBlock();

        description.ApplyTo(textBlock);

        var fg = Assert.IsType<SolidColorBrush>(textBlock.Foreground);
        var bg = Assert.IsType<SolidColorBrush>(textBlock.Background);
        Assert.Equal(Colors.Red, fg.Color);
        Assert.Equal(Colors.Yellow, bg.Color);
    }

    [StaFact]
    public void ApplyTo_TextBlock_Preserves_Existing_Brushes_When_Description_Brushes_Are_Null()
    {
        var description = new FontDescription(new FontFamily("Verdana"), 18, FontWeights.Bold, FontStyles.Italic, FontStretches.Expanded);
        var existingForeground = new SolidColorBrush(Colors.Magenta);
        var existingBackground = new SolidColorBrush(Colors.Cyan);
        var textBlock = new TextBlock
        {
            Foreground = existingForeground,
            Background = existingBackground,
        };

        description.ApplyTo(textBlock);

        Assert.Same(existingForeground, textBlock.Foreground);
        Assert.Same(existingBackground, textBlock.Background);
    }

    [StaFact]
    public void FromTextBlock_Reads_All_Font_Properties()
    {
        var textBlock = new TextBlock
        {
            FontFamily = new FontFamily("Verdana"),
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            FontStyle = FontStyles.Italic,
            FontStretch = FontStretches.Expanded,
            Foreground = new SolidColorBrush(Colors.Red),
            Background = new SolidColorBrush(Colors.Yellow),
        };

        var description = FontDescription.FromTextBlock(textBlock);

        Assert.Equal("Verdana", description.Family.Source);
        Assert.Equal(18, description.Size);
        Assert.Equal(FontWeights.Bold, description.Weight);
        Assert.Equal(FontStyles.Italic, description.Style);
        Assert.Equal(FontStretches.Expanded, description.Stretch);
        Assert.NotNull(description.Foreground);
        Assert.Equal(Colors.Red, description.Foreground.Color);
        Assert.NotNull(description.Background);
        Assert.Equal(Colors.Yellow, description.Background.Color);
    }

    [StaFact]
    public void Constructor_Throws_For_Null_Family()
        => Assert.Throws<ArgumentNullException>(
            () => new FontDescription(family: null!, 12, FontWeights.Normal, FontStyles.Normal, FontStretches.Normal));

    [StaFact]
    public void ApplyTo_Throws_For_Null_TextBlock()
    {
        var sut = new FontDescription();

        Assert.Throws<ArgumentNullException>(() => sut.ApplyTo(textBlock: (TextBlock)null!));
    }

    [StaFact]
    public void ToString_Includes_Family_And_Size()
    {
        var sut = new FontDescription(new FontFamily("Arial"), 14, FontWeights.Bold, FontStyles.Italic, FontStretches.Normal);

        var result = sut.ToString();

        Assert.Contains("Arial", result, StringComparison.Ordinal);
        Assert.Contains("14", result, StringComparison.Ordinal);
    }

    [StaFact]
    public void FontPicker_SelectedFontFamily_DP_DefaultValue_IsSegoeUiFontFamily()
    {
        var defaultValue = BaseControls.FontPicker.SelectedFontFamilyProperty.DefaultMetadata.DefaultValue;

        var fontFamily = Assert.IsType<FontFamily>(defaultValue);
        Assert.Equal("Segoe UI", fontFamily.Source);
    }
}