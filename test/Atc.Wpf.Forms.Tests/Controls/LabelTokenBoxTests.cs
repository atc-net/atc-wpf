namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelTokenBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void IsValid_MandatoryWithNoTokens_ReportsFieldIsRequired()
    {
        UseEnglishUi();
        var sut = Create();
        sut.IsMandatory = true;

        Assert.False(sut.IsValid());
        Assert.Equal("Field is required", sut.ValidationText);

        sut.SelectedKeys.Add("b");

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void IsValid_NotMandatory_IsValidWithNoTokens()
    {
        var sut = Create();

        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void IsValid_TokensBeyondTheLimit_AreKeptButInvalid()
    {
        UseEnglishUi();
        var sut = Create();
        sut.MaxSelectionCount = 1;

        sut.SelectedKeys = ["a", "b"];

        Assert.False(sut.IsValid());
        Assert.Equal(["a", "b"], sut.SelectedKeys);
        Assert.Contains("1", sut.ValidationText, StringComparison.Ordinal);
    }

    [StaFact]
    public void Items_SelectedKeys_and_options_reach_the_inner_token_box()
    {
        var sut = Create();
        Func<string, string?> validator = _ => null;
        var template = new DataTemplate();

        sut.AllowCreate = true;
        sut.CreateValidator = validator;
        sut.Separators = "|";
        sut.FilterMode = TokenFilterMode.StartsWith;
        sut.MinimumPrefixLength = 0;
        sut.MaxSuggestions = 7;
        sut.TokensWrap = false;
        sut.MaxVisibleTokens = 3;
        sut.MaxSelectionCount = 5;
        sut.FixedKeys.Add("a");
        sut.AllowEditTokens = true;
        sut.ShowClearButton = true;
        sut.TokenTemplate = template;
        sut.SuggestionDelay = TimeSpan.FromSeconds(1);
        sut.WatermarkText = "Tags";
        sut.SelectedKeys.Add("c");

        var inner = sut.TokenBox;
        Assert.Same(sut.Items, inner.Items);
        Assert.Same(sut.SelectedKeys, inner.SelectedKeys);
        Assert.Same(sut.FixedKeys, inner.FixedKeys);
        Assert.True(inner.AllowCreate);
        Assert.Same(validator, inner.CreateValidator);
        Assert.Equal("|", inner.Separators);
        Assert.Equal(TokenFilterMode.StartsWith, inner.FilterMode);
        Assert.Equal(0, inner.MinimumPrefixLength);
        Assert.Equal(7, inner.MaxSuggestions);
        Assert.False(inner.TokensWrap);
        Assert.Equal(3, inner.MaxVisibleTokens);
        Assert.Equal(5, inner.MaxSelectionCount);
        Assert.True(inner.AllowEditTokens);
        Assert.True(inner.ShowClearButton);
        Assert.Same(template, inner.TokenTemplate);
        Assert.Equal(TimeSpan.FromSeconds(1), inner.SuggestionDelay);
        Assert.Equal("Tags", Atc.Wpf.Helpers.TextBoxHelper.GetWatermark(inner));
    }

    [StaFact]
    public void Picking_raises_SelectedKeysChanged_with_the_identifier()
    {
        var sut = Create();
        sut.LabelText = "Systems";
        var raised = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => raised.Add(e);

        Type(sut, "gamma;");

        Assert.Equal(["c"], sut.SelectedKeys);
        var change = Assert.Single(raised);
        Assert.Equal(sut.Identifier, change.Identifier);
        Assert.Equal(["c"], change.NewValue);
    }

    [StaFact]
    public void Removing_the_last_token_while_mandatory_raises_the_invalid_event()
    {
        var sut = Create();
        sut.IsMandatory = true;
        sut.SelectedKeys.Add("a");
        var valid = 0;
        var invalid = 0;
        sut.SelectedKeysChanged += (_, _) => valid++;
        sut.SelectedKeysChangedInvalid += (_, _) => invalid++;

        sut.TokenBox.ClearTokens();

        Assert.Equal(0, valid);
        Assert.Equal(1, invalid);
        Assert.False(string.IsNullOrEmpty(sut.ValidationText));
    }

    [StaFact]
    public void TokenCreating_is_passed_on_from_the_inner_token_box()
    {
        var sut = Create();
        sut.AllowCreate = true;
        sut.TokenCreating += (_, e) => e.Key = "new:" + e.Text;

        Type(sut, "delta,");

        Assert.Equal(["new:delta"], sut.SelectedKeys);
        Assert.Equal("delta", sut.Items["new:delta"]);
    }

    [StaFact]
    public void Implements_ILabelTokenBox()
    {
        ILabelTokenBox sut = Create();
        var raised = 0;
        sut.SelectedKeysChanged += (_, _) => raised++;

        Type((LabelTokenBox)sut, "alpha; beta");

        Assert.Equal(["a", "b"], sut.SelectedKeys);
        Assert.Equal(1, raised);
    }

    [StaFact]
    public void Factory_creates_a_labeled_token_box_with_items_and_tokens()
    {
        var sut = LabelControlFactory.CreateLabelTokenBox(
            groupIdentifier: "Group",
            labelText: "AssignedUsers",
            isReadOnly: false,
            isMandatory: true,
            items: Items(),
            selectedKeys: ["b"],
            allowCreate: true);

        Assert.Equal("Group", sut.GroupIdentifier);
        Assert.Equal("Assigned users", sut.LabelText);
        Assert.True(sut.IsMandatory);
        Assert.True(sut.AllowCreate);
        Assert.Equal(3, sut.Items.Count);
        Assert.Equal(["b"], sut.SelectedKeys);
        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void Form_GetKeyValues_returns_the_token_keys()
    {
        var sut = LabelControlFactory.CreateLabelTokenBox(
            groupIdentifier: "Export",
            labelText: "Columns",
            isReadOnly: false,
            isMandatory: false,
            items: Items(),
            selectedKeys: ["c", "a"]);
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { sut });

        var keyValues = form.GetKeyValues();

        var value = Assert.Single(keyValues).Value;
        Assert.Equal(["c", "a"], Assert.IsAssignableFrom<IEnumerable<string>>(value), StringComparer.Ordinal);
    }

    [StaFact]
    public void Form_values_write_the_token_keys_back_to_a_model()
    {
        var sut = LabelControlFactory.CreateLabelTokenBox(
            groupIdentifier: "ExportSettings",
            labelText: "Columns",
            isReadOnly: false,
            isMandatory: false,
            items: Items(),
            selectedKeys: ["a", "c"]);
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { sut });

        var actual = LabelControlsFormToModelWriter.Update(new ExportSettings(), form);

        Assert.Equal(["a", "c"], actual.Columns, StringComparer.Ordinal);
    }

    private static LabelTokenBox Create()
        => new()
        {
            LabelText = "Systems",
            Items = Items(),
        };

    private static Dictionary<string, string> Items()
        => new(StringComparer.Ordinal)
        {
            ["a"] = "Alpha",
            ["b"] = "Beta",
            ["c"] = "Gamma",
        };

    /// <summary>
    /// Sets the typed text through UI Automation: text with a separator is handled as a paste.
    /// </summary>
    private static void Type(
        LabelTokenBox sut,
        string text)
        => new TokenBoxAutomationPeer(sut.TokenBox).SetValue(text);

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
}