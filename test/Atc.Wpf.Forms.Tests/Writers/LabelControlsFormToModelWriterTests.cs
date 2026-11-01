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

    [StaFact]
    public void Update_IntegersModel_UntouchedLargeValues_RoundTrip()
    {
        // Arrange
        var model = new IntegersModel
        {
            LongValue = 5_000_000_000,
            LongNullable = -5_000_000_000,
            UnsignedLongValue = 10_000_000_000_000_000_000,
            UnsignedIntValue = 3_000_000_000,
            UnsignedShortValue = 60_000,
            ShortValue = -30_000,
            DayValue = DayOfWeek.Friday,
            DayNullable = DayOfWeek.Sunday,
        };

        var labelControlsForm = CreateForm(model);

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, labelControlsForm);

        // Assert
        Assert.Equal(model.LongValue, actual.LongValue);
        Assert.Equal(model.LongNullable, actual.LongNullable);
        Assert.Equal(model.UnsignedLongValue, actual.UnsignedLongValue);
        Assert.Equal(model.UnsignedIntValue, actual.UnsignedIntValue);
        Assert.Equal(model.UnsignedShortValue, actual.UnsignedShortValue);
        Assert.Equal(model.ShortValue, actual.ShortValue);
        Assert.Equal(model.DayValue, actual.DayValue);
        Assert.Equal(model.DayNullable, actual.DayNullable);
    }

    [StaFact]
    public void GetKeyValues_IntegerBox_KeepsAnIntValue()
    {
        // Arrange
        var model = new IntegersModel { ShortValue = 42 };

        var labelControlsForm = CreateForm(model);

        // Act
        var actual = labelControlsForm.GetKeyValues();

        // Assert
        Assert.IsType<int>(actual["IntegersModel.ShortValue"]);
        Assert.Equal(42, actual["IntegersModel.ShortValue"]);
        Assert.IsType<long>(actual["IntegersModel.LongValue"]);
    }

    [StaFact]
    public void Update_IntegersModel_EditedLargeValues_AreWritten()
    {
        // Arrange
        var model = new IntegersModel();

        var labelControlsForm = CreateForm(model);
        Control<LabelDecimalBox>(labelControlsForm, nameof(IntegersModel.LongValue)).Value = 7_000_000_000;
        Control<LabelDecimalBox>(labelControlsForm, nameof(IntegersModel.UnsignedIntValue)).Value = 4_000_000_000;
        Control<LabelDecimalBox>(labelControlsForm, nameof(IntegersModel.UnsignedLongValue)).Value = 12_000_000_000_000_000_000.00m;
        Control<LabelIntegerBox>(labelControlsForm, nameof(IntegersModel.UnsignedShortValue)).Value = 65_000;

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, labelControlsForm);

        // Assert
        Assert.Equal(7_000_000_000, actual.LongValue);
        Assert.Equal(4_000_000_000u, actual.UnsignedIntValue);
        Assert.Equal(12_000_000_000_000_000_000ul, actual.UnsignedLongValue);
        Assert.Equal((ushort)65_000, actual.UnsignedShortValue);
    }

    [StaFact]
    public void Update_IntegersModel_OutOfRangeValues_KeepTheModelValue()
    {
        // Arrange
        var model = new IntegersModel
        {
            UnsignedIntValue = 7,
            UnsignedShortValue = 8,
        };

        var labelControlsForm = CreateForm(model);
        var unsignedIntBox = Control<LabelDecimalBox>(labelControlsForm, nameof(IntegersModel.UnsignedIntValue));
        Assert.Equal(uint.MinValue, unsignedIntBox.Minimum);
        Assert.Equal(uint.MaxValue, unsignedIntBox.Maximum);
        Assert.Equal(0, unsignedIntBox.DecimalPlaces);
        unsignedIntBox.Value = -5;

        var unsignedShortBox = Control<LabelIntegerBox>(labelControlsForm, nameof(IntegersModel.UnsignedShortValue));
        Assert.Equal(ushort.MinValue, unsignedShortBox.Minimum);
        Assert.Equal(ushort.MaxValue, unsignedShortBox.Maximum);
        unsignedShortBox.Value = 70_000;

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, labelControlsForm);

        // Assert
        Assert.Equal(7u, actual.UnsignedIntValue);
        Assert.Equal((ushort)8, actual.UnsignedShortValue);
    }

    [StaFact]
    public void Update_IntegersModel_UnselectedEnums_WriteNullOrKeepTheModelValue()
    {
        // Arrange
        var model = new IntegersModel
        {
            DayValue = DayOfWeek.Friday,
            DayNullable = DayOfWeek.Sunday,
        };

        var labelControlsForm = CreateForm(model);
        Control<LabelComboBox>(labelControlsForm, nameof(IntegersModel.DayValue)).SelectedKey = string.Empty;
        Control<LabelComboBox>(labelControlsForm, nameof(IntegersModel.DayNullable)).SelectedKey = Atc.Helpers.DropDownFirstItemTypeHelper
            .GetEnumGuid(DropDownFirstItemType.PleaseSelect)
            .ToString();

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, labelControlsForm);

        // Assert
        Assert.Equal(DayOfWeek.Friday, actual.DayValue);
        Assert.Null(actual.DayNullable);
    }

    [StaFact]
    public void Update_PropertiesWithConsecutiveCapitals_AreWritten()
    {
        // Arrange
        var model = new AcronymModel
        {
            UIntValue = 1,
            IPAddressText = "10.0.0.1",
            HTTPPort = 80,
        };

        var labelControlsForm = CreateForm(model);
        var keyValues = labelControlsForm.GetKeyValues();
        var uintKey = Assert.Single(keyValues.Keys, k => k.EndsWith("UIntValue", StringComparison.OrdinalIgnoreCase));
        var ipKey = Assert.Single(keyValues.Keys, k => k.EndsWith("IPAddressText", StringComparison.OrdinalIgnoreCase));
        var portKey = Assert.Single(keyValues.Keys, k => k.EndsWith("HTTPPort", StringComparison.OrdinalIgnoreCase));
        keyValues[uintKey] = 4_000_000_000m;
        keyValues[ipKey] = "192.168.1.1";
        keyValues[portKey] = 8080;

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, keyValues);

        // Assert
        Assert.Equal(4_000_000_000u, actual.UIntValue);
        Assert.Equal("192.168.1.1", actual.IPAddressText);
        Assert.Equal(8080, actual.HTTPPort);
    }

    private static TControl Control<TControl>(
        LabelControlsForm labelControlsForm,
        string identifier)
        where TControl : ILabelControlBase
        => AllLabelControls(labelControlsForm)
            .OfType<TControl>()
            .Single(x => x.Identifier == identifier);

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