namespace Atc.Wpf.Components.Tests.Flyouts;

public sealed class FlyoutFormResultFactoryTests
{
    private sealed class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void Success_returns_submitted_valid_result_with_no_errors()
    {
        var model = new TestModel { Name = "Alice" };

        var result = FlyoutFormResultFactory.Success(model);

        Assert.Same(model, result.Model);
        Assert.True(result.IsSubmitted);
        Assert.True(result.IsValid);
        Assert.True(result.IsSuccess);
        Assert.False(result.IsCancelled);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public void Cancelled_returns_unsubmitted_invalid_result_with_no_errors()
    {
        var model = new TestModel { Name = "Bob" };

        var result = FlyoutFormResultFactory.Cancelled(model);

        Assert.Same(model, result.Model);
        Assert.False(result.IsSubmitted);
        Assert.False(result.IsValid);
        Assert.False(result.IsSuccess);
        Assert.True(result.IsCancelled);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact]
    public void ValidationFailed_marks_result_as_submitted_but_invalid_with_errors()
    {
        var model = new TestModel { Name = string.Empty };
        var errors = new[] { "Name is required" };

        var result = FlyoutFormResultFactory.ValidationFailed(model, errors);

        Assert.Same(model, result.Model);
        Assert.True(result.IsSubmitted);
        Assert.False(result.IsValid);
        Assert.False(result.IsSuccess);
        Assert.False(result.IsCancelled);
        Assert.Single(result.ValidationErrors);
        Assert.Equal("Name is required", result.ValidationErrors[0]);
    }
}