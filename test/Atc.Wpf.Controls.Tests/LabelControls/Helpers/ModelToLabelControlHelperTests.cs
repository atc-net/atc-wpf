namespace Atc.Wpf.Controls.Tests.LabelControls.Helpers;

public class ModelToLabelControlHelperTests
{
    [StaFact]
    public void GetLabelControls_PrimitiveTypesModel()
    {
        // Arrange
        var model = new PrimitiveTypesModel();

        // Act
        var labelControls = ModelToLabelControlHelper.GetLabelControls(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(labelControls);
        Assert.Equal(13, labelControls.Count);
    }

    [StaFact]
    public void GetLabelControls_Address()
    {
        // Arrange
        var model = new Address();

        // Act
        var labelControls = ModelToLabelControlHelper.GetLabelControls(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(labelControls);
        Assert.Equal(4, labelControls.Count);
    }

    [StaFact]
    public void GetLabelControls_Person()
    {
        // Arrange
        var model = new Person();

        // Act
        var labelControls = ModelToLabelControlHelper.GetLabelControls(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(labelControls);
        Assert.Equal(3, labelControls.Count);
    }
}