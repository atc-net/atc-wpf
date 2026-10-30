namespace Atc.Wpf.Forms.Tests.Extractors;

[Collection("TestCollection")]
[Trait(Traits.Category, Traits.Categories.Integration)]
[Trait(Traits.Category, Traits.Categories.SkipWhenLiveUnitTesting)]
public sealed class ModelToLabelControlExtractorMultiSelectTests
{
    [StaFact]
    public void Extract_MultiSelectModel_CreatesAControlForEveryMultiSelectProperty()
    {
        // Act
        var actual = ModelToLabelControlExtractor.Extract(new MultiSelectModel());

        // Assert
        Assert.Equal(9, actual.Count);
    }

    [StaFact]
    public void Extract_FlagsEnum_CreatesMultiSelectComboBoxWithSingleFlagsAndSetFlagsSelected()
    {
        // Arrange
        var model = new MultiSelectModel { Rights = AccessRights.ReadWrite };

        // Act
        var actual = ModelToLabelControlExtractor.Extract(model);

        // Assert
        var control = Assert.IsType<LabelMultiSelectComboBox>(actual.Single(x => x.Identifier == "Rights"));
        Assert.Equal(["Read", "Write", "Delete"], control.Items.Keys, StringComparer.Ordinal);
        Assert.Equal(["Read", "Write"], control.SelectedKeys!, StringComparer.Ordinal);
        Assert.True(control.IsMandatory);
        Assert.True(control.IsEnabled);
        Assert.Equal(typeof(AccessRights), control.InputDataType);
    }

    [StaFact]
    public void Extract_NullableFlagsEnumWithoutValue_SelectsNothing()
    {
        // Act
        var actual = ModelToLabelControlExtractor.Extract(new MultiSelectModel());

        // Assert
        var control = Assert.IsType<LabelMultiSelectComboBox>(actual.Single(x => x.Identifier == "OptionalRights"));
        Assert.Empty(control.SelectedKeys!);
        Assert.False(control.IsMandatory);
    }

    [StaFact]
    public void Extract_EnumList_CreatesMultiSelectComboBoxWithEnumItems()
    {
        // Arrange
        var model = new MultiSelectModel { WorkDays = [DayOfWeek.Monday, DayOfWeek.Friday] };

        // Act
        var actual = ModelToLabelControlExtractor.Extract(model);

        // Assert
        var control = Assert.IsType<LabelMultiSelectComboBox>(actual.Single(x => x.Identifier == "WorkDays"));
        Assert.Equal(7, control.Items.Count);
        Assert.Equal("Monday", control.Items["Monday"]);
        Assert.Equal(["Monday", "Friday"], control.SelectedKeys!, StringComparer.Ordinal);
        Assert.Equal(3, control.MaxSelectionCount);
        Assert.False(control.IsMandatory);
    }

    [StaTheory]
    [InlineData("DaysOff")]
    [InlineData("MeetingDays")]
    public void Extract_NullEnumCollection_CreatesMultiSelectComboBoxWithoutSelection(
        string identifier)
    {
        // Act
        var actual = ModelToLabelControlExtractor.Extract(new MultiSelectModel());

        // Assert
        var control = Assert.IsType<LabelMultiSelectComboBox>(actual.Single(x => x.Identifier == identifier));
        Assert.Equal(7, control.Items.Count);
        Assert.Empty(control.SelectedKeys!);
        Assert.Null(control.MaxSelectionCount);
    }

    [StaFact]
    public void Extract_StringList_CreatesTokenBoxThatAcceptsFreeValues()
    {
        // Arrange
        var model = new MultiSelectModel { Tags = ["red", "green"] };

        // Act
        var actual = ModelToLabelControlExtractor.Extract(model);

        // Assert
        var control = Assert.IsType<LabelTokenBox>(actual.Single(x => x.Identifier == "Tags"));
        Assert.True(control.AllowCreate);
        Assert.True(control.IsMandatory);
        Assert.Equal(["red", "green"], control.SelectedKeys!, StringComparer.Ordinal);
        Assert.True(control.IsValid());
    }

    [StaFact]
    public void Extract_StringCollectionWithLength_SetsMaxSelectionCount()
    {
        // Arrange
        var model = new MultiSelectModel { Labels = ["x"] };

        // Act
        var actual = ModelToLabelControlExtractor.Extract(model);

        // Assert
        var control = Assert.IsType<LabelTokenBox>(actual.Single(x => x.Identifier == "Labels"));
        Assert.Equal(2, control.MaxSelectionCount);
        Assert.Equal(["x"], control.SelectedKeys!, StringComparer.Ordinal);
    }

    [StaFact]
    public void Extract_NullStringArray_CreatesTokenBoxWithoutTokens()
    {
        // Act
        var actual = ModelToLabelControlExtractor.Extract(new MultiSelectModel());

        // Assert
        var control = Assert.IsType<LabelTokenBox>(actual.Single(x => x.Identifier == "Aliases"));
        Assert.Empty(control.SelectedKeys!);
    }

    [StaFact]
    public void Extract_GetOnlyCollection_IsReadOnlyOrSkipped()
    {
        // Arrange
        var model = new MultiSelectModel();
        model.Notes.Add("first");

        // Act
        var withReadOnly = ModelToLabelControlExtractor.Extract(model, includeReadOnly: true);
        var withoutReadOnly = ModelToLabelControlExtractor.Extract(model, includeReadOnly: false);

        // Assert
        var control = Assert.IsType<LabelTokenBox>(withReadOnly.Single(x => x.Identifier == "Notes"));
        Assert.False(control.IsEnabled);
        Assert.Equal(["first"], control.SelectedKeys!, StringComparer.Ordinal);
        Assert.DoesNotContain(withoutReadOnly, x => x.Identifier == "Notes");
    }

    [StaFact]
    public void RoundTrip_EditedMultiSelectControls_AreWrittenBackToTheModel()
    {
        // Arrange
        var model = new MultiSelectModel
        {
            Rights = AccessRights.Read,
            WorkDays = [DayOfWeek.Monday],
            Tags = ["old"],
        };

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(ModelToLabelControlExtractor.Extract(model));

        var controls = labelControlsForm.Rows!.SelectMany(r => r.Columns!).SelectMany(c => c.LabelControls).ToList();
        SetKeys(controls, "Rights", "Write", "Delete");
        SetKeys(controls, "OptionalRights", "Read");
        SetKeys(controls, "WorkDays", "Tuesday", "Wednesday");
        SetKeys(controls, "DaysOff", "Sunday");
        SetKeys(controls, "MeetingDays", "Thursday");
        SetKeys(controls, "Tags", "new", "fresh");
        SetKeys(controls, "Aliases", "alias");
        SetKeys(controls, "Labels", "a", "b");

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, labelControlsForm);

        // Assert
        Assert.Equal(AccessRights.Write | AccessRights.Delete, actual.Rights);
        Assert.Equal(AccessRights.Read, actual.OptionalRights);
        Assert.Equal([DayOfWeek.Tuesday, DayOfWeek.Wednesday], actual.WorkDays);
        Assert.Equal([DayOfWeek.Sunday], actual.DaysOff!);
        Assert.Equal([DayOfWeek.Thursday], actual.MeetingDays!);
        Assert.Equal(["new", "fresh"], actual.Tags, StringComparer.Ordinal);
        Assert.Equal(["alias"], actual.Aliases!, StringComparer.Ordinal);
        Assert.Equal(["a", "b"], actual.Labels!, StringComparer.Ordinal);
    }

    [StaFact]
    public void RoundTrip_EmptySelection_WritesNoFlagsAndEmptyCollections()
    {
        // Arrange
        var model = new MultiSelectModel
        {
            Rights = AccessRights.Read,
            OptionalRights = AccessRights.Write,
            WorkDays = [DayOfWeek.Monday],
            DaysOff = [DayOfWeek.Sunday],
            Tags = ["old"],
            Aliases = ["alias"],
        };

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(ModelToLabelControlExtractor.Extract(model));

        var controls = labelControlsForm.Rows!.SelectMany(r => r.Columns!).SelectMany(c => c.LabelControls).ToList();
        SetKeys(controls, "OptionalRights");
        SetKeys(controls, "WorkDays");
        SetKeys(controls, "DaysOff");
        SetKeys(controls, "Aliases");

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, labelControlsForm);

        // Assert
        Assert.Equal(AccessRights.Read, actual.Rights);
        Assert.Equal(AccessRights.None, actual.OptionalRights);
        Assert.Empty(actual.WorkDays);
        Assert.Empty(actual.DaysOff!);
        Assert.Equal(["old"], actual.Tags, StringComparer.Ordinal);
        Assert.Empty(actual.Aliases!);
    }

    [StaFact]
    public void RoundTrip_NullCollectionsLeftUntouched_BecomeEmptyCollections()
    {
        // Arrange
        var model = new MultiSelectModel
        {
            Rights = AccessRights.Read,
            Tags = ["tag"],
        };

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(ModelToLabelControlExtractor.Extract(model));

        // Act
        var actual = LabelControlsFormToModelWriter.Update(model, labelControlsForm);

        // Assert
        Assert.Equal(AccessRights.Read, actual.Rights);
        Assert.Equal(AccessRights.None, actual.OptionalRights);
        Assert.Empty(actual.DaysOff!);
        Assert.Empty(actual.MeetingDays!);
        Assert.Empty(actual.Aliases!);
        Assert.Empty(actual.Labels!);
        Assert.Equal(["tag"], actual.Tags, StringComparer.Ordinal);
    }

    [StaFact]
    public void Update_KeyValuesWithStringLists_WritesCollections()
    {
        // Arrange
        var keyValues = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["MultiSelectModel.Tags"] = new List<string> { "a", "b" },
            ["MultiSelectModel.Aliases"] = new List<string> { "c" },
            ["MultiSelectModel.MeetingDays"] = new List<string> { "Friday" },
            ["MultiSelectModel.WorkDays"] = new List<string> { "Monday", "Tuesday" },
        };

        // Act
        var actual = LabelControlsFormToModelWriter.Update(new MultiSelectModel(), keyValues);

        // Assert
        Assert.Equal(["a", "b"], actual.Tags, StringComparer.Ordinal);
        Assert.Equal(["c"], actual.Aliases!, StringComparer.Ordinal);
        Assert.Equal([DayOfWeek.Friday], actual.MeetingDays!);
        Assert.Equal([DayOfWeek.Monday, DayOfWeek.Tuesday], actual.WorkDays);
    }

    [StaFact]
    public void GetKeyValues_CheckListBoxForAFlagsEnum_ReturnsTheCombinedFlags()
    {
        // Arrange
        var control = LabelControlFactory.CreateLabelCheckListBox(
            groupIdentifier: "MultiSelectModel",
            labelText: "Rights",
            isReadOnly: false,
            isMandatory: false,
            new Dictionary<string, string>(StringComparer.Ordinal) { ["Read"] = "Read", ["Delete"] = "Delete" },
            ["Read", "Delete"]);
        control.InputDataType = typeof(AccessRights);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(new List<ILabelControlBase> { control });

        // Act
        var keyValues = labelControlsForm.GetKeyValues();
        var actual = LabelControlsFormToModelWriter.Update(new MultiSelectModel(), keyValues);

        // Assert
        Assert.Equal(AccessRights.Read | AccessRights.Delete, keyValues["MultiSelectModel.Rights"]);
        Assert.Equal(AccessRights.Read | AccessRights.Delete, actual.Rights);
    }

    private static void SetKeys(
        IEnumerable<ILabelControlBase> controls,
        string identifier,
        params string[] keys)
    {
        var control = controls.Single(x => x.Identifier == identifier);
        var selectedKeys = new Atc.Wpf.Collections.ObservableCollectionEx<string>();
        selectedKeys.AddRange(keys);
        switch (control)
        {
            case LabelMultiSelectComboBox multiSelectComboBox:
                multiSelectComboBox.SelectedKeys = selectedKeys;
                break;
            case LabelTokenBox tokenBox:
                tokenBox.SelectedKeys = selectedKeys;
                break;
            default:
                throw new InvalidOperationException($"Unexpected control for {identifier}: {control.GetType().Name}");
        }
    }
}