namespace Atc.Wpf.Forms.Tests.Writers;

[Collection("TestCollection")]
[Trait(Traits.Category, Traits.Categories.Integration)]
[Trait(Traits.Category, Traits.Categories.SkipWhenLiveUnitTesting)]
public sealed class LabelControlsFormToModelWriterTests
{
    [StaFact]
    public void Update_Account_From_KeyValues()
    {
        // Arrange
        var account = new Account();

        var labelControls = ModelToLabelControlExtractor.Extract(account);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var keyValues = labelControlsForm.GetKeyValues();
        keyValues["Account.AccountNumber"] = "1234";
        keyValues["Account.FirstDayOfWeek"] = DayOfWeek.Friday;
        keyValues["Account.PrimaryContactPerson.FirstName"] = "John";
        keyValues["Account.PrimaryContactPerson.LastName"] = "Doe";
        keyValues["Account.PrimaryContactPerson.Age"] = 42;
        keyValues["Account.PrimaryContactPerson.Address.Country"] = "da-DK";

        // Act
        var actual = LabelControlsFormToModelWriter.Update(account, keyValues);

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
    public void Update_Account()
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
        var actual = LabelControlsFormToModelWriter.Update(account, labelControlsForm);

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
    public void Update_DriveItem_From_KeyValues()
    {
        // Arrange
        var driveItem = new DriveItem();

        var labelControls = ModelToLabelControlExtractor.Extract(driveItem);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var keyValues = labelControlsForm.GetKeyValues();
        keyValues["DriveItem.Name"] = "Hello";
        keyValues["DriveItem.Directory"] = new DirectoryInfo(@"C:\Temp");
        keyValues["DriveItem.File"] = new FileInfo(@"C:\Temp\test.txt");

        // Act
        var actual = LabelControlsFormToModelWriter.Update(driveItem, keyValues);

        // Arrange
        Assert.NotNull(actual);
        Assert.Equal(@"Hello", actual.Name);
        Assert.Equal(@"C:\Temp", actual.Directory?.FullName);
        Assert.Equal(@"C:\Temp\test.txt", actual.File?.FullName);
    }

    [StaFact]
    public void Update_DriveItem()
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
        var actual = LabelControlsFormToModelWriter.Update(driveItem, labelControlsForm);

        // Arrange
        Assert.NotNull(actual);
        Assert.Equal(@"Hello", actual.Name);
        Assert.Equal(@"C:\Temp", actual.Directory?.FullName);
        Assert.Equal(@"C:\Temp\test.txt", actual.File?.FullName);
    }

    [StaFact]
    public void Update_DatesModel_UntouchedValues_RoundTrip()
    {
        // Arrange
        var model = new DatesModel
        {
            DateTimeValue = new DateTime(2024, 5, 17, 13, 45, 30, DateTimeKind.Unspecified),
            DateTimeNullable = new DateTime(2023, 1, 2, 3, 4, 5, DateTimeKind.Unspecified),
            DateOnlyValue = new DateOnly(2024, 5, 17),
            DateOnlyNullable = new DateOnly(2023, 1, 2),
            TimeOnlyValue = new TimeOnly(13, 45, 30),
            TimeOnlyNullable = new TimeOnly(3, 4, 5),
            DateTimeOffsetValue = new DateTimeOffset(2024, 5, 17, 13, 45, 30, TimeSpan.FromHours(5)),
            DateTimeOffsetNullable = new DateTimeOffset(2023, 1, 2, 3, 4, 5, TimeSpan.Zero),
        };

        var labelControlsForm = CreateForm(model);

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, labelControlsForm);

        // Assert
        Assert.Equal(model.DateTimeValue, actual.DateTimeValue);
        Assert.Equal(model.DateTimeNullable, actual.DateTimeNullable);
        Assert.Equal(model.DateOnlyValue, actual.DateOnlyValue);
        Assert.Equal(model.DateOnlyNullable, actual.DateOnlyNullable);
        Assert.Equal(model.TimeOnlyValue, actual.TimeOnlyValue);
        Assert.Equal(model.TimeOnlyNullable, actual.TimeOnlyNullable);
        Assert.Equal(model.DateTimeOffsetValue.UtcDateTime, actual.DateTimeOffsetValue.UtcDateTime);
        Assert.Equal(model.DateTimeOffsetNullable!.Value.UtcDateTime, actual.DateTimeOffsetNullable!.Value.UtcDateTime);
    }

    [StaFact]
    public void Update_DatesModel_DefaultValues_RoundTrip()
    {
        // Arrange
        var model = new DatesModel();

        var labelControlsForm = CreateForm(model);

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, labelControlsForm);

        // Assert
        Assert.Equal(default, actual.DateTimeValue);
        Assert.Null(actual.DateTimeNullable);
        Assert.Equal(default, actual.DateOnlyValue);
        Assert.Null(actual.DateOnlyNullable);
        Assert.Equal(default, actual.TimeOnlyValue);
        Assert.Null(actual.TimeOnlyNullable);
        Assert.Equal(default(DateTimeOffset).UtcDateTime, actual.DateTimeOffsetValue.UtcDateTime);
        Assert.Null(actual.DateTimeOffsetNullable);
    }

    [StaFact]
    public void Update_DatesModel_ClearedNullableDates_WriteNull()
    {
        // Arrange
        var model = new DatesModel
        {
            DateTimeNullable = new DateTime(2023, 1, 2, 3, 4, 5, DateTimeKind.Unspecified),
            DateOnlyNullable = new DateOnly(2023, 1, 2),
            TimeOnlyNullable = new TimeOnly(3, 4, 5),
            DateTimeOffsetNullable = new DateTimeOffset(2023, 1, 2, 3, 4, 5, TimeSpan.Zero),
        };

        var labelControlsForm = CreateForm(model);
        foreach (var labelControl in AllLabelControls(labelControlsForm))
        {
            switch (labelControl)
            {
                case LabelDateTimePicker labelDateTimePicker:
                    labelDateTimePicker.SelectedDate = null;
                    break;
                case LabelDatePicker labelDatePicker:
                    labelDatePicker.SelectedDate = null;
                    break;
                case LabelTimePicker labelTimePicker:
                    labelTimePicker.SelectedTime = null;
                    break;
            }
        }

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, labelControlsForm);

        // Assert
        Assert.Null(actual.DateTimeNullable);
        Assert.Null(actual.DateOnlyNullable);
        Assert.Null(actual.TimeOnlyNullable);
        Assert.Null(actual.DateTimeOffsetNullable);
    }

    [StaFact]
    public void Update_DatesModel_EditedDates_AreWrittenAsThePropertyType()
    {
        // Arrange
        var model = new DatesModel();

        var labelControlsForm = CreateForm(model);
        foreach (var labelControl in AllLabelControls(labelControlsForm))
        {
            switch (labelControl)
            {
                case LabelDateTimePicker { Identifier: nameof(DatesModel.DateTimeOffsetNullable) } labelDateTimePicker:
                    labelDateTimePicker.SelectedDate = new DateTime(2025, 6, 7, 8, 9, 10, DateTimeKind.Unspecified);
                    break;
                case LabelDatePicker { Identifier: nameof(DatesModel.DateOnlyNullable) } labelDatePicker:
                    labelDatePicker.SelectedDate = new DateTime(2025, 6, 7, 0, 0, 0, DateTimeKind.Unspecified);
                    break;
                case LabelTimePicker { Identifier: nameof(DatesModel.TimeOnlyNullable) } labelTimePicker:
                    labelTimePicker.SelectedTime = new DateTime(1, 1, 1, 8, 9, 10, DateTimeKind.Unspecified);
                    break;
            }
        }

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, labelControlsForm);

        // Assert
        Assert.Equal(new DateOnly(2025, 6, 7), actual.DateOnlyNullable);
        Assert.Equal(new TimeOnly(8, 9, 10), actual.TimeOnlyNullable);
        Assert.NotNull(actual.DateTimeOffsetNullable);
        Assert.Equal(new DateTime(2025, 6, 7, 8, 9, 10, DateTimeKind.Unspecified), actual.DateTimeOffsetNullable.Value.LocalDateTime);
    }

    private static LabelControlsForm CreateForm<T>(T model)
    {
        var labelControls = ModelToLabelControlExtractor.Extract(model);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);
        return labelControlsForm;
    }

    private static IEnumerable<ILabelControlBase> AllLabelControls(
        LabelControlsForm labelControlsForm)
        => labelControlsForm.Rows!
            .SelectMany(row => row.Columns!)
            .SelectMany(column => column.LabelControls);
}