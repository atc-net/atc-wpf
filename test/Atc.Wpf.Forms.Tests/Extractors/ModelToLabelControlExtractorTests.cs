namespace Atc.Wpf.Forms.Tests.Extractors;

[Collection("TestCollection")]
[Trait(Traits.Category, Traits.Categories.Integration)]
[Trait(Traits.Category, Traits.Categories.SkipWhenLiveUnitTesting)]
public sealed class ModelToLabelControlExtractorTests
{
    [StaFact]
    public void GetLabelControls_PrimitiveTypesModel()
    {
        // Arrange
        var model = new PrimitiveTypesModel();

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(40, actual.Count);
    }

    [StaFact]
    public void Extract_NonFiniteAndUnboundedValues_DoesNotThrow()
    {
        // Arrange
        var model = new NonFiniteModel();

        // Act
        var actual = ModelToLabelControlExtractor.Extract(model);

        // Assert
        Assert.Equal(11, actual.Count);
        Assert.Equal(0m, Control<LabelDecimalBox>(actual, nameof(NonFiniteModel.NotNumber)).Value);
        Assert.Equal(decimal.MaxValue, Control<LabelDecimalBox>(actual, nameof(NonFiniteModel.Infinity)).Value);
        Assert.Equal(decimal.MaxValue, Control<LabelDecimalBox>(actual, nameof(NonFiniteModel.Huge)).Value);
        Assert.Equal(decimal.MinValue, Control<LabelDecimalBox>(actual, nameof(NonFiniteModel.HugeFloat)).Value);

        var unboundedDouble = Control<LabelDecimalBox>(actual, nameof(NonFiniteModel.UnboundedDouble));
        Assert.Equal(0m, unboundedDouble.Minimum);
        Assert.Equal(decimal.MaxValue, unboundedDouble.Maximum);
        Assert.Equal(3.5m, unboundedDouble.Value);

        var unboundedInt = Control<LabelIntegerBox>(actual, nameof(NonFiniteModel.UnboundedInt));
        Assert.Equal(0, unboundedInt.Minimum);
        Assert.Equal(int.MaxValue, unboundedInt.Maximum);

        var unboundedLong = Control<LabelDecimalBox>(actual, nameof(NonFiniteModel.UnboundedLong));
        Assert.Equal(0m, unboundedLong.Minimum);
        Assert.Equal(long.MaxValue, unboundedLong.Maximum);

        var unboundedSize = Control<LabelPixelSizeBox>(actual, nameof(NonFiniteModel.UnboundedSize));
        Assert.Equal(0, unboundedSize.Minimum);
        Assert.Equal(int.MaxValue, unboundedSize.Maximum);

        var unboundedPoint = Control<LabelDecimalXyBox>(actual, nameof(NonFiniteModel.UnboundedPoint));
        Assert.Equal(0m, unboundedPoint.Minimum);
        Assert.Equal(decimal.MaxValue, unboundedPoint.Maximum);
    }

    [StaFact]
    public void GetLabelControls_Address()
    {
        // Arrange
        var model = new Address();

        // Act
        var labelControls = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(labelControls);
        Assert.Equal(4, labelControls.Count);
    }

    [StaTheory]
    [InlineData(true, "", "My city name")]
    [InlineData(false, "Field is required", "")]
    [InlineData(false, "Min. length: 2", "M")]
    [InlineData(false, "Max. length: 16", "My city name - too long")]
    public void GetLabelControls_Address_Validate_CityName(
        bool expectedIsValid,
        string expectedValidationText,
        string? cityName)
    {
        // Arrange
        var model = new Address
        {
            CityName = cityName,
        };

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        var controlPostalCode = actual.Single(x => x.Identifier == "CityName");
        Assert.Equal(expectedIsValid, controlPostalCode.IsValid());
        Assert.Equal(expectedValidationText, ((ILabelControl)controlPostalCode).ValidationText);
    }

    [StaTheory]
    [InlineData(true, "", "1234")]
    [InlineData(false, "Field is required", "")]
    [InlineData(false, "Regular expression don't match", "12345")]
    public void GetLabelControls_Address_Validate_PostalCode(
        bool expectedIsValid,
        string expectedValidationText,
        string? postalCode)
    {
        // Arrange
        var model = new Address
        {
            PostalCode = postalCode,
        };

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        var controlPostalCode = actual.Single(x => x.Identifier == "PostalCode");
        Assert.Equal(expectedIsValid, controlPostalCode.IsValid());
        Assert.Equal(expectedValidationText, ((ILabelControl)controlPostalCode).ValidationText);
    }

    [StaFact]
    public void GetLabelControls_Person()
    {
        // Arrange
        var model = new Person();

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(7, actual.Count);
    }

    [StaFact]
    public void GetLabelControls_Account()
    {
        // Arrange
        var model = new Account();

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(11, actual.Count);
    }

    [StaFact]
    public void GetLabelControls_DriveItem()
    {
        // Arrange
        var model = new DriveItem();

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(3, actual.Count);
    }

    [StaFact]
    public void GetLabelControls_CollectionsModel_SkipsUnsupportedCollectionProperties()
    {
        // Arrange
        var model = new CollectionsModel
        {
            Name = "Name",
            Age = 42,
            Tags = { "a", "b" },
            Counts = { ["a"] = 1 },
        };

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(3, actual.Count);
        Assert.Contains(actual, x => x.Identifier == "Name");
        Assert.Contains(actual, x => x.Identifier == "Age");
        var tags = Assert.IsType<LabelTokenBox>(actual.Single(x => x.Identifier == "Tags"));
        Assert.False(tags.IsEnabled);
        Assert.DoesNotContain(actual, x => x.Identifier == "Counts");
    }

    [StaFact]
    public void GetLabelControls_SelfReferencingType_StopsAtDepthLimit()
    {
        // Arrange
        var model = new Node { Name = "Root" };

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(ModelToLabelControlExtractor.MaxNestingDepth + 1, actual.Count);
        Assert.All(actual, x => Assert.Equal("Name", x.Identifier));
    }

    [StaFact]
    public void GetLabelControls_SelfReferencingChainLongerThanLimit_StopsAtDepthLimit()
    {
        // Arrange
        var model = new Node { Name = "0" };
        var current = model;
        for (var i = 1; i < 30; i++)
        {
            current.Next = new Node { Name = i.ToString(GlobalizationConstants.EnglishCultureInfo) };
            current = current.Next;
        }

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(ModelToLabelControlExtractor.MaxNestingDepth + 1, actual.Count);
    }

    private static TControl Control<TControl>(
        IEnumerable<ILabelControlBase> labelControls,
        string identifier)
        where TControl : ILabelControlBase
        => labelControls
            .OfType<TControl>()
            .Single(x => x.Identifier == identifier);
}