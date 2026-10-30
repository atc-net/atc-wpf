namespace Atc.Wpf.Forms;

/// <summary>
/// A labeled token field (<see cref="TokenBox"/>) with a mandatory marker and validation text: the user types to pick
/// several values, each shown as a removable chip. When <c>IsMandatory</c> is set, at least one token is required;
/// with <c>MaxSelectionCount</c>, no more than that many.
/// </summary>
public partial class LabelTokenBox : ILabelTokenBox
{
    /// <summary>
    /// Identifies the <see cref="MaxDropDownHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxDropDownHeightProperty = TokenBox.MaxDropDownHeightProperty.AddOwner(
        typeof(LabelTokenBox));

    /// <summary>
    /// Identifies the <see cref="CreateValidator"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CreateValidatorProperty = TokenBox.CreateValidatorProperty.AddOwner(
        typeof(LabelTokenBox));

    /// <summary>
    /// Identifies the <see cref="CustomFilter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CustomFilterProperty = TokenBox.CustomFilterProperty.AddOwner(
        typeof(LabelTokenBox));

    /// <summary>
    /// Identifies the <see cref="SuggestionsProvider"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SuggestionsProviderProperty = TokenBox.SuggestionsProviderProperty.AddOwner(
        typeof(LabelTokenBox));

    /// <summary>
    /// The known values, keyed by their selection key, with their display value.
    /// </summary>
    [DependencyProperty]
    private Dictionary<string, string> items;

    /// <summary>
    /// The keys of the tokens, in token order. Two-way by default; updated in place.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// Whether text that matches no item becomes a new token.
    /// </summary>
    [DependencyProperty]
    private bool allowCreate;

    /// <summary>
    /// The characters that commit the typed text; pasted text is split on them and on new lines.
    /// </summary>
    [DependencyProperty(DefaultValue = ",;")]
    private string separators;

    /// <summary>
    /// How the typed text is matched against the display values of <see cref="Items"/>.
    /// </summary>
    [DependencyProperty(DefaultValue = TokenFilterMode.Contains)]
    private TokenFilterMode filterMode;

    /// <summary>
    /// How many characters must be typed before suggestions show; 0 shows them on focus.
    /// </summary>
    [DependencyProperty(DefaultValue = 1)]
    private int minimumPrefixLength;

    /// <summary>
    /// The most suggestions the pop-up shows.
    /// </summary>
    [DependencyProperty(DefaultValue = 50)]
    private int maxSuggestions;

    /// <summary>
    /// Whether the tokens wrap over several lines (<see langword="false"/>: one line that scrolls horizontally).
    /// </summary>
    [DependencyProperty(DefaultValue = true)]
    private bool tokensWrap;

    /// <summary>
    /// When the field has no focus, the most tokens shown before a "+N" chip; <see langword="null"/> shows them all.
    /// </summary>
    [DependencyProperty]
    private int? maxVisibleTokens;

    /// <summary>
    /// The maximum number of tokens; <see langword="null"/> for no limit. Keys set from code beyond the limit are kept,
    /// but make <see cref="IsValid"/> return <see langword="false"/>.
    /// </summary>
    [DependencyProperty]
    private int? maxSelectionCount;

    /// <summary>
    /// The keys of tokens that cannot be removed.
    /// </summary>
    [DependencyProperty]
    private ObservableCollection<string> fixedKeys;

    /// <summary>
    /// Whether a double-click on a created token edits its text.
    /// </summary>
    [DependencyProperty]
    private bool allowEditTokens;

    /// <summary>
    /// Whether an × at the end of the field removes every token that is not fixed.
    /// </summary>
    [DependencyProperty]
    private bool showClearButton;

    /// <summary>
    /// The template of a suggestion row (data context: <see cref="TokenSuggestion"/>).
    /// </summary>
    [DependencyProperty]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// The template of a token (data context: <see cref="MultiSelectChipItem"/>); <see langword="null"/> for the
    /// default small input chip.
    /// </summary>
    [DependencyProperty]
    private DataTemplate? tokenTemplate;

    /// <summary>
    /// The pause in typing before <see cref="SuggestionsProvider"/> runs.
    /// </summary>
    [DependencyProperty(DefaultValue = "TimeSpan.FromMilliseconds(250)")]
    private TimeSpan suggestionDelay;

    /// <summary>
    /// The watermark shown in the empty field.
    /// </summary>
    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    /// <summary>
    /// Occurs when the tokens change through the field and pass validation.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Occurs when the tokens change through the field and fail validation (a mandatory field with no tokens, or more
    /// tokens than <see cref="MaxSelectionCount"/>).
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChangedInvalid;

    /// <summary>
    /// Occurs before a value that matches no item becomes a token; lets the app assign the key or refuse the value.
    /// </summary>
    public event EventHandler<TokenCreatingEventArgs>? TokenCreating;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelTokenBox"/> class.
    /// </summary>
    public LabelTokenBox()
    {
        InitializeComponent();

        PartTokenBox = new TokenBox();
        ((LabelContent)Content).Content = PartTokenBox;

        SetCurrentValue(SelectedKeysProperty, new Atc.Wpf.Collections.ObservableCollectionEx<string>());
        SetCurrentValue(FixedKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(ItemsProperty, new Dictionary<string, string>(StringComparer.Ordinal));

        // Options first, then SelectedKeys and Items last.
        Forward(MinHeightProperty, ContentMinHeightProperty);
        Forward(TextBoxHelper.WatermarkProperty, WatermarkTextProperty);
        Forward(TokenBox.MaxDropDownHeightProperty, MaxDropDownHeightProperty);
        Forward(TokenBox.AllowCreateProperty, AllowCreateProperty);
        Forward(TokenBox.CreateValidatorProperty, CreateValidatorProperty);
        Forward(TokenBox.SeparatorsProperty, SeparatorsProperty);
        Forward(TokenBox.FilterModeProperty, FilterModeProperty);
        Forward(TokenBox.CustomFilterProperty, CustomFilterProperty);
        Forward(TokenBox.MinimumPrefixLengthProperty, MinimumPrefixLengthProperty);
        Forward(TokenBox.MaxSuggestionsProperty, MaxSuggestionsProperty);
        Forward(TokenBox.TokensWrapProperty, TokensWrapProperty);
        Forward(TokenBox.MaxVisibleTokensProperty, MaxVisibleTokensProperty);
        Forward(TokenBox.MaxSelectionCountProperty, MaxSelectionCountProperty);
        Forward(TokenBox.FixedKeysProperty, FixedKeysProperty);
        Forward(TokenBox.AllowEditTokensProperty, AllowEditTokensProperty);
        Forward(TokenBox.ShowClearButtonProperty, ShowClearButtonProperty);
        Forward(TokenBox.ItemTemplateProperty, ItemTemplateProperty);
        Forward(TokenBox.TokenTemplateProperty, TokenTemplateProperty);
        Forward(TokenBox.SuggestionsProviderProperty, SuggestionsProviderProperty);
        Forward(TokenBox.SuggestionDelayProperty, SuggestionDelayProperty);
        Forward(TokenBox.SelectedKeysProperty, SelectedKeysProperty, BindingMode.TwoWay);
        Forward(TokenBox.ItemsProperty, ItemsProperty);

        PartTokenBox.SelectedKeysChanged += OnTokenBoxSelectedKeysChanged;
        PartTokenBox.TokenCreating += OnTokenBoxTokenCreating;
    }

    /// <summary>
    /// Gets or sets the maximum height of the suggestion pop-up.
    /// </summary>
    public double MaxDropDownHeight
    {
        get => (double)GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the check of a new value: it returns an error text to reject the value, or <see langword="null"/>
    /// to accept it.
    /// </summary>
    public Func<string, string?>? CreateValidator
    {
        get => (Func<string, string?>?)GetValue(CreateValidatorProperty);
        set => SetValue(CreateValidatorProperty, value);
    }

    /// <summary>
    /// Gets or sets the filter used when <see cref="FilterMode"/> is <see cref="TokenFilterMode.Custom"/>.
    /// </summary>
    public Func<string, KeyValuePair<string, string>, bool>? CustomFilter
    {
        get => (Func<string, KeyValuePair<string, string>, bool>?)GetValue(CustomFilterProperty);
        set => SetValue(CustomFilterProperty, value);
    }

    /// <summary>
    /// Gets or sets an asynchronous lookup that replaces the filtering of <see cref="Items"/> (debounced by
    /// <see cref="SuggestionDelay"/>; a new keystroke cancels the previous lookup).
    /// </summary>
    public Func<string, CancellationToken, Task<IReadOnlyList<KeyValuePair<string, string>>>>? SuggestionsProvider
    {
        get => (Func<string, CancellationToken, Task<IReadOnlyList<KeyValuePair<string, string>>>>?)GetValue(SuggestionsProviderProperty);
        set => SetValue(SuggestionsProviderProperty, value);
    }

    /// <summary>
    /// Gets the inner token field.
    /// </summary>
    internal TokenBox TokenBox => PartTokenBox;

    private TokenBox PartTokenBox { get; }

    /// <inheritdoc />
    public override bool IsValid()
    {
        Validate();
        return string.IsNullOrEmpty(ValidationText);
    }

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelTokenBox)d;
        if (!string.IsNullOrEmpty(control.ValidationText))
        {
            control.Validate();
        }
    }

    private void Forward(
        DependencyProperty innerProperty,
        DependencyProperty outerProperty,
        BindingMode mode = BindingMode.OneWay)
        => PartTokenBox.SetBinding(
            innerProperty,
            new Binding
            {
                Path = new PropertyPath(outerProperty),
                Source = this,
                Mode = mode,
            });

    private bool Validate()
    {
        var count = SelectedKeys is null
            ? 0
            : SelectedKeys.Distinct(StringComparer.Ordinal).Count();

        if (IsMandatory && count == 0)
        {
            ValidationText = Validations.FieldIsRequired;
            return false;
        }

        if (MaxSelectionCount is { } max && count > max)
        {
            ValidationText = string.Format(CultureInfo.CurrentCulture, Validations.SelectAtMost1, max);
            return false;
        }

        ValidationText = string.Empty;
        return true;
    }

    private void OnTokenBoxSelectedKeysChanged(
        object? sender,
        ValueChangedEventArgs<IReadOnlyList<string>> e)
    {
        var args = new ValueChangedEventArgs<IReadOnlyList<string>>(
            Identifier,
            e.OldValue,
            e.NewValue);

        if (Validate())
        {
            SelectedKeysChanged?.Invoke(this, args);
        }
        else
        {
            SelectedKeysChangedInvalid?.Invoke(this, args);
        }
    }

    private void OnTokenBoxTokenCreating(
        object? sender,
        TokenCreatingEventArgs e)
        => TokenCreating?.Invoke(this, e);
}