namespace Atc.Wpf.Controls.Tests.Flyouts;

public sealed class FlyoutFormResultTests
{
    private sealed class TestModel
    {
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        // Arrange
        var model = new TestModel { Name = "John", Age = 30 };
        var errors = new List<string> { "Error 1", "Error 2" };

        // Act
        var result = new FlyoutFormResult<TestModel>(
            model,
            isSubmitted: true,
            isValid: false,
            errors);

        // Assert
        Assert.Same(model, result.Model);
        Assert.True(result.IsSubmitted);
        Assert.False(result.IsValid);
        Assert.Equal(2, result.ValidationErrors.Count);
        Assert.Equal("Error 1", result.ValidationErrors[0]);
        Assert.Equal("Error 2", result.ValidationErrors[1]);
    }

    [Fact]
    public void Constructor_WithNullValidationErrors_SetsEmptyList()
    {
        // Arrange
        var model = new TestModel();

        // Act
        var result = new FlyoutFormResult<TestModel>(
            model,
            isSubmitted: true,
            isValid: true,
            validationErrors: null);

        // Assert
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public void IsCancelled_ReturnsTrueWhenNotSubmitted()
    {
        // Arrange
        var model = new TestModel();

        // Act
        var result = new FlyoutFormResult<TestModel>(
            model,
            isSubmitted: false,
            isValid: false);

        // Assert
        Assert.True(result.IsCancelled);
    }

    [Fact]
    public void IsCancelled_ReturnsFalseWhenSubmitted()
    {
        // Arrange
        var model = new TestModel();

        // Act
        var result = new FlyoutFormResult<TestModel>(
            model,
            isSubmitted: true,
            isValid: true);

        // Assert
        Assert.False(result.IsCancelled);
    }

    [Fact]
    public void IsSuccess_ReturnsTrueWhenSubmittedAndValid()
    {
        // Arrange
        var model = new TestModel();

        // Act
        var result = new FlyoutFormResult<TestModel>(
            model,
            isSubmitted: true,
            isValid: true);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void IsSuccess_ReturnsFalseWhenNotSubmitted()
    {
        // Arrange
        var model = new TestModel();

        // Act
        var result = new FlyoutFormResult<TestModel>(
            model,
            isSubmitted: false,
            isValid: true);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void IsSuccess_ReturnsFalseWhenNotValid()
    {
        // Arrange
        var model = new TestModel();

        // Act
        var result = new FlyoutFormResult<TestModel>(
            model,
            isSubmitted: true,
            isValid: false);

        // Assert
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Success_CreatesSuccessfulResult()
    {
        // Arrange
        var model = new TestModel { Name = "Jane", Age = 25 };

        // Act
        var result = FlyoutFormResult<TestModel>.Success(model);

        // Assert
        Assert.Same(model, result.Model);
        Assert.True(result.IsSubmitted);
        Assert.True(result.IsValid);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsCancelled);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public void Cancelled_CreatesCancelledResult()
    {
        // Arrange
        var model = new TestModel { Name = "Bob", Age = 35 };

        // Act
        var result = FlyoutFormResult<TestModel>.Cancelled(model);

        // Assert
        Assert.Same(model, result.Model);
        Assert.False(result.IsSubmitted);
        Assert.False(result.IsValid);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsCancelled);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public void ValidationFailed_CreatesValidationFailureResult()
    {
        // Arrange
        var model = new TestModel { Name = "", Age = -1 };
        var errors = new List<string> { "Name is required", "Age must be positive" };

        // Act
        var result = FlyoutFormResult<TestModel>.ValidationFailed(model, errors);

        // Assert
        Assert.Same(model, result.Model);
        Assert.True(result.IsSubmitted);
        Assert.False(result.IsValid);
        Assert.False(result.IsSuccess);
        Assert.False(result.IsCancelled);
        Assert.Equal(2, result.ValidationErrors.Count);
        Assert.Contains("Name is required", result.ValidationErrors);
        Assert.Contains("Age must be positive", result.ValidationErrors);
    }
}