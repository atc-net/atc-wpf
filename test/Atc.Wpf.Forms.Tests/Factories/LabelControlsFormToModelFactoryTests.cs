namespace Atc.Wpf.Forms.Tests.Factories;

[Collection("TestCollection")]
[Trait(Traits.Category, Traits.Categories.Integration)]
[Trait(Traits.Category, Traits.Categories.SkipWhenLiveUnitTesting)]
public sealed class LabelControlsFormToModelFactoryTests
{
    [StaFact]
    public void Create_Account_From_KeyValues()
    {
        // Arrange
        var keyValues = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["Account.AccountNumber"] = "1234",
            ["Account.FirstDayOfWeek"] = DayOfWeek.Friday,
            ["Account.PrimaryContactPerson.FirstName"] = "John",
            ["Account.PrimaryContactPerson.LastName"] = "Doe",
            ["Account.PrimaryContactPerson.Age"] = 42,
            ["Account.PrimaryContactPerson.Address.Country"] = "da-DK",
        };

        // Act
        var actual = LabelControlsFormToModelFactory.Create<Account>(keyValues);

        // Arrange
        Assert.NotNull(actual);
        Assert.Equal("1234", actual.AccountNumber);
        Assert.Equal(DayOfWeek.Friday, actual.FirstDayOfWeek);
        Assert.NotNull(actual.PrimaryContactPerson);
        Assert.Equal("John", actual.PrimaryContactPerson.FirstName);
        Assert.Equal("Doe", actual.PrimaryContactPerson.LastName);
        Assert.Equal(42, actual.PrimaryContactPerson.Age);
        Assert.NotNull(actual.PrimaryContactPerson.Address);
        Assert.NotNull(actual.PrimaryContactPerson.Address.Country);
        Assert.Equal("da-DK", actual.PrimaryContactPerson.Address.Country.Name);
    }

    [StaFact]
    public void Create_Account()
    {
        // Arrange
        var account = new Account();

        var labelControls = ModelToLabelControlExtractor.Extract(account);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        foreach (var row in labelControlsForm.Rows!)
        {
            foreach (var column in row.Columns!)
            {
                foreach (var labelControl in column.LabelControls)
                {
                    switch (labelControl)
                    {
                        case LabelTextBox { Identifier: "AccountNumber" } labelTextBox:
                            labelTextBox.Text = "1234";
                            break;
                        case LabelTextBox { Identifier: "FirstName" } labelTextBox:
                            labelTextBox.Text = "John";
                            break;
                        case LabelTextBox { Identifier: "LastName" } labelTextBox:
                            labelTextBox.Text = "Doe";
                            break;
                        case LabelIntegerBox { Identifier: "Age" } labelIntegerBox:
                            labelIntegerBox.Value = 42;
                            break;
                        case LabelComboBox { Identifier: "FirstDayOfWeek" } labelComboBox:
                            labelComboBox.SelectedKey = "Friday";
                            break;
                        case LabelCountrySelector { Identifier: "Country" } labelCountrySelector:
                            labelCountrySelector.SelectedKey = "1030";
                            break;
                    }
                }
            }
        }

        // Act
        var actual = LabelControlsFormToModelFactory.Create<Account>(labelControlsForm);

        // Arrange
        Assert.NotNull(actual);
        Assert.Equal("1234", actual.AccountNumber);
        Assert.Equal(DayOfWeek.Friday, actual.FirstDayOfWeek);
        Assert.NotNull(actual.PrimaryContactPerson);
        Assert.Equal("John", actual.PrimaryContactPerson.FirstName);
        Assert.Equal("Doe", actual.PrimaryContactPerson.LastName);
        Assert.Equal(42, actual.PrimaryContactPerson.Age);
        Assert.NotNull(actual.PrimaryContactPerson.Address);
        Assert.NotNull(actual.PrimaryContactPerson.Address.Country);
        Assert.Equal("da-DK", actual.PrimaryContactPerson.Address.Country.Name);
    }

    [StaFact]
    public void Create_DriveItem_From_KeyValues()
    {
        // Arrange
        var keyValues = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["DriveItem.Name"] = "Hello",
            ["DriveItem.Directory"] = new DirectoryInfo(@"C:\Temp"),
            ["DriveItem.File"] = new FileInfo(@"C:\Temp\test.txt"),
        };

        // Act
        var actual = LabelControlsFormToModelFactory.Create<DriveItem>(keyValues);

        // Arrange
        Assert.NotNull(actual);
        Assert.Equal(@"Hello", actual.Name);
        Assert.Equal(@"C:\Temp", actual.Directory?.FullName);
        Assert.Equal(@"C:\Temp\test.txt", actual.File?.FullName);
    }

    [StaFact]
    public void Create_DriveItem()
    {
        // Arrange
        var driveItem = new DriveItem();

        var labelControls = ModelToLabelControlExtractor.Extract(driveItem);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        foreach (var row in labelControlsForm.Rows!)
        {
            foreach (var column in row.Columns!)
            {
                foreach (var labelControl in column.LabelControls)
                {
                    switch (labelControl)
                    {
                        case LabelTextBox { Identifier: "Name" } labelTextBox:
                            labelTextBox.Text = "Hello";
                            break;
                        case LabelDirectoryPicker { Identifier: "Directory" } labelDirectoryPicker:
                            labelDirectoryPicker.Value = new DirectoryInfo(@"C:\Temp");
                            break;
                        case LabelFilePicker { Identifier: "File" } labelFilePicker:
                            labelFilePicker.Value = new FileInfo(@"C:\Temp\test.txt");
                            break;
                    }
                }
            }
        }

        // Act
        var actual = LabelControlsFormToModelFactory.Create<DriveItem>(labelControlsForm);

        // Arrange
        Assert.NotNull(actual);
        Assert.Equal(@"Hello", actual.Name);
        Assert.Equal(@"C:\Temp", actual.Directory?.FullName);
        Assert.Equal(@"C:\Temp\test.txt", actual.File?.FullName);
    }
}