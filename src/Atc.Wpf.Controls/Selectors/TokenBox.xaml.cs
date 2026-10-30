// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A text field where the user types to pick several values; each pick becomes a removable chip ("token") in the
/// field. Typing opens a suggestion pop-up with the matching items (the match in bold, picked items left out);
/// <c>Enter</c>, <c>Tab</c>, a separator or a click picks the highlighted suggestion. With <see cref="AllowCreate"/>,
/// text that matches no item becomes a new token; pasted text is split on the separators and on new lines.
/// </summary>
/// <remarks>
/// Picks, removals, a paste and Clear update <see cref="SelectedKeys"/> in place and raise
/// <see cref="SelectedKeysChanged"/> once per action. Changes made to <see cref="SelectedKeys"/> from outside (or a new
/// instance) only update the tokens. Tokens follow the order of <see cref="SelectedKeys"/>.
/// </remarks>
public partial class TokenBox
{
    /// <summary>
    /// Identifies the <see cref="MaxDropDownHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxDropDownHeightProperty = ComboBox.MaxDropDownHeightProperty.AddOwner(
        typeof(TokenBox));

    /// <summary>
    /// Identifies the <see cref="CreateValidator"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CreateValidatorProperty = DependencyProperty.Register(
        nameof(CreateValidator),
        typeof(Func<string, string?>),
        typeof(TokenBox),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="CustomFilter"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CustomFilterProperty = DependencyProperty.Register(
        nameof(CustomFilter),
        typeof(Func<string, KeyValuePair<string, string>, bool>),
        typeof(TokenBox),
        new PropertyMetadata(defaultValue: null));

    /// <summary>
    /// Identifies the <see cref="SuggestionsProvider"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SuggestionsProviderProperty = DependencyProperty.Register(
        nameof(SuggestionsProvider),
        typeof(Func<string, CancellationToken, Task<IReadOnlyList<KeyValuePair<string, string>>>>),
        typeof(TokenBox),
        new PropertyMetadata(defaultValue: null));

    private static readonly DependencyPropertyKey ErrorTextPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(ErrorText),
        typeof(string),
        typeof(TokenBox),
        new PropertyMetadata(string.Empty, OnErrorTextChanged));

    /// <summary>
    /// Identifies the <see cref="ErrorText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ErrorTextProperty = ErrorTextPropertyKey.DependencyProperty;

    private static readonly DependencyPropertyKey HasErrorPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(HasError),
        typeof(bool),
        typeof(TokenBox),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="HasError"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HasErrorProperty = HasErrorPropertyKey.DependencyProperty;

    /// <summary>
    /// The known values, keyed by their selection key, with their display value. Changing it drops the tokens whose
    /// keys are gone, except created values while <see cref="AllowCreate"/> is set.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemsChanged))]
    private Dictionary<string, string> items;

    /// <summary>
    /// The keys of the tokens, in token order. Two-way by default; updated in place.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnSelectedKeysChanged))]
    private ObservableCollection<string> selectedKeys;

    /// <summary>
    /// Whether text that matches no item becomes a new token on <c>Enter</c>, <c>Tab</c> or a separator. The key of a
    /// new token is its text unless <see cref="TokenCreating"/> sets another; the pair is added to <see cref="Items"/>.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnTokensInputChanged))]
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
    /// How many characters must be typed before suggestions show; 0 shows suggestions as soon as the field gets focus.
    /// </summary>
    [DependencyProperty(DefaultValue = 1)]
    private int minimumPrefixLength;

    /// <summary>
    /// The most suggestions the pop-up shows; the rest needs more typing.
    /// </summary>
    [DependencyProperty(DefaultValue = 50)]
    private int maxSuggestions;

    /// <summary>
    /// Whether the tokens wrap over several lines (the field grows in height); <see langword="false"/> keeps one line
    /// that scrolls horizontally while the field has focus and ends in a "+N" chip when it has not.
    /// </summary>
    [DependencyProperty(DefaultValue = true, PropertyChangedCallback = nameof(OnTokensInputChanged))]
    private bool tokensWrap;

    /// <summary>
    /// When the field has no focus, the most tokens shown before a "+N" chip; <see langword="null"/> (the default)
    /// shows them all (or, on one line, as many as fit). All tokens show again on focus.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnTokensInputChanged))]
    private int? maxVisibleTokens;

    /// <summary>
    /// The maximum number of tokens; <see langword="null"/> for no limit. At the limit typing is blocked and the field
    /// shows "{0} of max {1}".
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnTokensInputChanged))]
    private int? maxSelectionCount;

    /// <summary>
    /// The keys of tokens that cannot be removed (no remove button; <c>Backspace</c> and Clear skip them).
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnFixedKeysChanged))]
    private ObservableCollection<string> fixedKeys;

    /// <summary>
    /// Whether a double-click on a created token (a value that was not in <see cref="Items"/>) edits its text:
    /// <c>Enter</c> or a click elsewhere commits, <c>Esc</c> cancels.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnTokensInputChanged))]
    private bool allowEditTokens;

    /// <summary>
    /// Whether an × at the end of the field removes every token that is not fixed (one change).
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnTokensInputChanged))]
    private bool showClearButton;

    /// <summary>
    /// The template of a suggestion row; its data context is a <see cref="TokenSuggestion"/>. <see langword="null"/>
    /// (the default) shows the display value with the typed text in bold.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnItemTemplateChanged))]
    private DataTemplate? itemTemplate;

    /// <summary>
    /// The template of a token; its data context is a <see cref="MultiSelectChipItem"/>. <see langword="null"/>
    /// (the default) uses a small input <c>Chip</c>.
    /// </summary>
    [DependencyProperty(PropertyChangedCallback = nameof(OnTokensInputChanged))]
    private DataTemplate? tokenTemplate;

    /// <summary>
    /// The pause in typing before <see cref="SuggestionsProvider"/> runs.
    /// </summary>
    [DependencyProperty(DefaultValue = "TimeSpan.FromMilliseconds(250)")]
    private TimeSpan suggestionDelay;

    /// <summary>
    /// The typed text (not yet a token). Two-way by default.
    /// </summary>
    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnTextChanged))]
    private string text;

    /// <summary>
    /// Whether the suggestion pop-up is open. Two-way by default.
    /// </summary>
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        PropertyChangedCallback = nameof(OnIsDropDownOpenChanged))]
    private bool isDropDownOpen;

    private ObservableCollection<string>? subscribedSelectedKeys;
    private ObservableCollection<string>? subscribedFixedKeys;
    private Window? subscribedWindow;
    private bool isUpdatingText;

    /// <summary>
    /// Occurs when the tokens change through the field: once per pick, removal, paste, edit, Clear, or
    /// <see cref="Items"/> change that drops keys.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;

    /// <summary>
    /// Occurs before a value that matches no item becomes a token (only with <see cref="AllowCreate"/>); lets the app
    /// assign the key or refuse the value.
    /// </summary>
    public event EventHandler<TokenCreatingEventArgs>? TokenCreating;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenBox"/> class.
    /// </summary>
    public TokenBox()
    {
        selectionCleanup = new SelectedKeysCleanup(this, SelectedKeysProperty, CleanUpSelection);
        InitializeComponent();

        PartTextBox = new TextBox
        {
            BorderThickness = new Thickness(0),
            Background = Brushes.Transparent,
            MinWidth = 0,
            MinHeight = 0,
            Padding = new Thickness(2, 0, 2, 0),
            Margin = new Thickness(0, 1, 0, 1),
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            AcceptsReturn = false,
        };
        AutomationProperties.SetAutomationId(PartTextBox, "PART_TextBox");
        BindingOperations.SetBinding(
            PartTextBox,
            TextBox.TextProperty,
            new Binding
            {
                Path = new PropertyPath(TextProperty),
                Source = this,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        ChipPanel = CreateChipPanel();
        ChipPanel.TrailingElement = PartTextBox;
        PartScrollViewer.Content = ChipPanel;

        Bind(PartPopupBorder, MinWidthProperty, ActualWidthProperty);
        Bind(PartPopupBorder, MaxHeightProperty, MaxDropDownHeightProperty);
        Bind(PartPopupBorder, FlowDirectionProperty, FlowDirectionProperty);
        PartPopup.PlacementTarget = this;
        PartSuggestionList.ItemTemplate = (DataTemplate)Resources["DefaultSuggestionTemplate"];

        PartTextBox.PreviewTextInput += OnTextBoxPreviewTextInput;
        DataObject.AddPastingHandler(PartTextBox, OnTextBoxPasting);
        PartSuggestionList.PreviewMouseLeftButtonDown += OnSuggestionListPreviewMouseLeftButtonDown;
        PartSuggestionList.SelectionChanged += OnSuggestionListSelectionChanged;
        IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        CultureManager.UiCultureChanged += OnUiCultureChanged;

        SetCurrentValue(FixedKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(SelectedKeysProperty, new ObservableCollection<string>());
        SetCurrentValue(ItemsProperty, new Dictionary<string, string>(StringComparer.Ordinal));

        UpdateTokens();
    }

    /// <summary>
    /// Gets or sets the maximum height of the suggestion pop-up; the list scrolls (virtualized) beyond it.
    /// </summary>
    public double MaxDropDownHeight
    {
        get => (double)GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the check of a new value before it becomes a token: it returns an error text to reject the value
    /// (the text then stays in the field with the theme's error look, and <see cref="ErrorText"/> holds the error), or
    /// <see langword="null"/> to accept it.
    /// </summary>
    public Func<string, string?>? CreateValidator
    {
        get => (Func<string, string?>?)GetValue(CreateValidatorProperty);
        set => SetValue(CreateValidatorProperty, value);
    }

    /// <summary>
    /// Gets or sets the filter used when <see cref="FilterMode"/> is <see cref="TokenFilterMode.Custom"/>: it gets the
    /// typed text and an item, and returns whether the item is suggested.
    /// </summary>
    public Func<string, KeyValuePair<string, string>, bool>? CustomFilter
    {
        get => (Func<string, KeyValuePair<string, string>, bool>?)GetValue(CustomFilterProperty);
        set => SetValue(CustomFilterProperty, value);
    }

    /// <summary>
    /// Gets or sets an asynchronous lookup for huge or remote lists: it gets the typed text and a cancellation token
    /// and returns the suggestions (key, display value). When set, it replaces the filtering of <see cref="Items"/>;
    /// it runs after a pause in typing of <see cref="SuggestionDelay"/>, and a new keystroke cancels the previous
    /// lookup. A picked suggestion is added to <see cref="Items"/>.
    /// </summary>
    public Func<string, CancellationToken, Task<IReadOnlyList<KeyValuePair<string, string>>>>? SuggestionsProvider
    {
        get => (Func<string, CancellationToken, Task<IReadOnlyList<KeyValuePair<string, string>>>>?)GetValue(SuggestionsProviderProperty);
        set => SetValue(SuggestionsProviderProperty, value);
    }

    /// <summary>
    /// Gets the error from <see cref="CreateValidator"/> for the typed text; empty when there is none.
    /// </summary>
    public string ErrorText
    {
        get => (string)GetValue(ErrorTextProperty);
        private set => SetValue(ErrorTextPropertyKey, value);
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="ErrorText"/> is not empty (the field shows the error look).
    /// </summary>
    public bool HasError
    {
        get => (bool)GetValue(HasErrorProperty);
        private set => SetValue(HasErrorPropertyKey, BooleanBoxes.Box(value));
    }

    /// <summary>
    /// Gets the text box the user types in.
    /// </summary>
    internal TextBox PartTextBox { get; }

    /// <summary>
    /// Removes every token that is not fixed and clears the typed text, raising <see cref="SelectedKeysChanged"/> once.
    /// </summary>
    public void ClearTokens()
    {
        SetText(string.Empty);
        ErrorText = string.Empty;
        RemoveKeys(SelectedKeys?.ToList() ?? []);
    }

    /// <summary>
    /// Handles a key press in the field.
    /// </summary>
    /// <returns><see langword="true"/> when the key was used.</returns>
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "One switch over the keys.")]
    internal bool HandleKey(
        Key key,
        ModifierKeys modifiers)
    {
        if (EditingKey is not null)
        {
            switch (key)
            {
                case Key.Enter:
                    CommitEditToken();
                    return true;
                case Key.Escape:
                    CancelEditToken();
                    return true;
                default:
                    return false;
            }
        }

        var atTextStart = PartTextBox.CaretIndex == 0 && PartTextBox.SelectionLength == 0;
        switch (key)
        {
            case Key.Down when modifiers == ModifierKeys.None:
            case Key.Up when modifiers == ModifierKeys.None:
                return MoveHighlight(key == Key.Down ? 1 : -1);

            case Key.Enter when modifiers == ModifierKeys.None:
                if (IsDropDownOpen && HighlightedSuggestion is not null)
                {
                    return CommitCurrent();
                }

                if (string.IsNullOrWhiteSpace(Text))
                {
                    return false;
                }

                CommitCurrent();
                return true;

            case Key.Tab when modifiers == ModifierKeys.None:
                if (IsDropDownOpen && HighlightedSuggestion is not null)
                {
                    return CommitCurrent();
                }

                return AllowCreate && !string.IsNullOrWhiteSpace(Text) && CommitCurrent();

            case Key.Back when modifiers == ModifierKeys.None && atTextStart:
                if (SelectedTokenIndex >= 0)
                {
                    return RemoveSelectedToken();
                }

                if (Text.Length == 0)
                {
                    return SelectLastRemovableToken();
                }

                return false;

            case Key.Delete when modifiers == ModifierKeys.None && SelectedTokenIndex >= 0:
                return RemoveSelectedToken();

            case Key.Left when modifiers == ModifierKeys.None && atTextStart:
                return MoveTokenSelection(-1);

            case Key.Right when modifiers == ModifierKeys.None && SelectedTokenIndex >= 0:
                return MoveTokenSelection(1);

            case Key.Escape:
                if (IsDropDownOpen)
                {
                    SetCurrentValue(IsDropDownOpenProperty, false);
                    return true;
                }

                if (SelectedTokenIndex >= 0)
                {
                    SelectedTokenIndex = -1;
                    return true;
                }

                if (Text.Length > 0)
                {
                    SetText(string.Empty);
                    ErrorText = string.Empty;
                    return true;
                }

                return false;

            default:
                return false;
        }
    }

    /// <summary>
    /// Handles typed characters before they reach the text box: at the limit they are blocked; a separator commits the
    /// typed text (or picks the highlighted suggestion) instead of being typed.
    /// </summary>
    /// <returns><see langword="true"/> when the input was used (it does not reach the text box).</returns>
    internal bool HandleTextInput(string input)
    {
        if (EditingKey is not null || string.IsNullOrEmpty(input))
        {
            return false;
        }

        if (IsAtLimit)
        {
            return true;
        }

        SelectedTokenIndex = -1;
        if (input.Length == 1 && (Separators ?? string.Empty).Contains(input[0], StringComparison.Ordinal))
        {
            CommitCurrent();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sets the typed text from UI Automation: text with a separator or a new line is handled as a paste (every part
    /// is committed), other text is just typed.
    /// </summary>
    internal void SetTextFromAutomation(string value)
    {
        value ??= string.Empty;
        if (ContainsSeparator(value))
        {
            SetText(string.Empty);
            Paste(value);
            return;
        }

        SetText(value);
        PartTextBox.CaretIndex = value.Length;
        UpdateSuggestions(force: false);
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer()
        => new TokenBoxAutomationPeer(this);

    /// <inheritdoc />
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewKeyDown(e);
        if (!e.Handled && HandleKey(e.Key == Key.System ? e.SystemKey : e.Key, Keyboard.Modifiers))
        {
            e.Handled = true;
        }
    }

    /// <inheritdoc />
    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnGotKeyboardFocus(e);
        if (ReferenceEquals(e.NewFocus, this))
        {
            // The field itself is focusable (for UI Automation and Focus()); the caret belongs in the text box.
            PartTextBox.Focus();
        }
    }

    /// <inheritdoc />
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewMouseLeftButtonDown(e);
        if (EditingKey is null && !PartTextBox.IsKeyboardFocusWithin && IsEnabled)
        {
            _ = Dispatcher.BeginInvoke(DispatcherPriority.Input, () => PartTextBox.Focus());
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == TextBoxHelper.WatermarkProperty)
        {
            UpdateFieldState();
        }
    }

    private static void OnErrorTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (TokenBox)d;
        var error = e.NewValue as string;
        control.HasError = !string.IsNullOrEmpty(error);
        control.PartTextBox.ToolTip = string.IsNullOrEmpty(error) ? null : error;
    }

    private static void OnItemTemplateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (TokenBox)d;
        control.PartSuggestionList.ItemTemplate = e.NewValue as DataTemplate
            ?? (DataTemplate)control.Resources["DefaultSuggestionTemplate"];
    }

    private static void OnTokensInputChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        => ((TokenBox)d).UpdateTokens();

    private static void OnSelectedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (TokenBox)d;
        if (control.subscribedSelectedKeys is not null)
        {
            control.subscribedSelectedKeys.CollectionChanged -= control.OnSelectedKeysCollectionChanged;
        }

        control.subscribedSelectedKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedSelectedKeys is not null)
        {
            control.subscribedSelectedKeys.CollectionChanged += control.OnSelectedKeysCollectionChanged;
        }

        control.UpdateTokens();
    }

    private static void OnFixedKeysChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (TokenBox)d;
        if (control.subscribedFixedKeys is not null)
        {
            control.subscribedFixedKeys.CollectionChanged -= control.OnFixedKeysCollectionChanged;
        }

        control.subscribedFixedKeys = e.NewValue as ObservableCollection<string>;
        if (control.subscribedFixedKeys is not null)
        {
            control.subscribedFixedKeys.CollectionChanged += control.OnFixedKeysCollectionChanged;
        }

        control.UpdateTokens();
    }

    private static void OnTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (TokenBox)d;
        if (control.isUpdatingText)
        {
            return;
        }

        var value = e.NewValue as string ?? string.Empty;
        if (control.HasError)
        {
            control.ErrorText = string.Empty;
        }

        if (value.Length > 0)
        {
            control.SelectedTokenIndex = -1;
        }

        if (control.ContainsSeparator(value))
        {
            // A separator arrived without a key press (IME, binding, UI Automation): commit the parts before it.
            var leftover = control.CommitText(value, commitLast: false);
            control.SetText(leftover);
            control.PartTextBox.CaretIndex = leftover.Length;
            return;
        }

        control.UpdateSuggestions(force: false);
        control.RaiseValueChanged((string)e.OldValue ?? string.Empty, value);
    }

    private static void OnIsDropDownOpenChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (TokenBox)d;
        var isOpen = (bool)e.NewValue;
        if (isOpen && control.Suggestions.Count == 0 && !control.IsBusy && !control.isFillingSuggestions)
        {
            // Opened from outside (binding, UI Automation): show the suggestions for the current text.
            control.UpdateSuggestions(force: true);
        }

        if (!isOpen)
        {
            control.CancelLookup();
        }

        control.PartPopup.IsOpen = isOpen && (control.Suggestions.Count > 0 || control.PartStatusText.Visibility == Visibility.Visible);

        if (UIElementAutomationPeer.FromElement(control) is TokenBoxAutomationPeer peer)
        {
            peer.RaiseExpandCollapseStateChanged((bool)e.OldValue, isOpen);
        }
    }

    private void Bind(
        DependencyObject target,
        DependencyProperty targetProperty,
        DependencyProperty sourceProperty)
        => BindingOperations.SetBinding(
            target,
            targetProperty,
            new Binding
            {
                Path = new PropertyPath(sourceProperty),
                Source = this,
                Mode = BindingMode.OneWay,
            });

    private void SetText(string value)
    {
        isUpdatingText = true;
        try
        {
            var old = Text;
            SetCurrentValue(TextProperty, value);
            PartTextBox.Text = value;
            RaiseValueChanged(old ?? string.Empty, value);
        }
        finally
        {
            isUpdatingText = false;
        }
    }

    private void RaiseValueChanged(
        string oldValue,
        string newValue)
    {
        if (!string.Equals(oldValue, newValue, StringComparison.Ordinal) &&
            UIElementAutomationPeer.FromElement(this) is TokenBoxAutomationPeer peer)
        {
            peer.RaiseValueChanged(oldValue, newValue);
        }
    }

    private void RaiseSelectedKeysChanged(
        IReadOnlyList<string> oldKeys,
        IReadOnlyList<string> newKeys)
        => SelectedKeysChanged?.Invoke(
            this,
            new ValueChangedEventArgs<IReadOnlyList<string>>(
                Name,
                oldKeys,
                newKeys));

    private void OnSelectedKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
        => UpdateTokens();

    private void OnFixedKeysCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
        => UpdateTokens();

    private void OnTextBoxPreviewTextInput(
        object sender,
        TextCompositionEventArgs e)
    {
        if (HandleTextInput(e.Text))
        {
            e.Handled = true;
        }
    }

    private void OnTextBoxPasting(
        object sender,
        DataObjectPastingEventArgs e)
    {
        if (!e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, autoConvert: true) ||
            e.SourceDataObject.GetData(DataFormats.UnicodeText, autoConvert: true) is not string pasted)
        {
            return;
        }

        if (!ContainsSeparator(pasted))
        {
            return;
        }

        e.CancelCommand();
        Paste(pasted);
    }

    private void OnIsKeyboardFocusWithinChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        if (IsKeyboardFocusWithin)
        {
            if (MinimumPrefixLength <= 0 && EditingKey is null)
            {
                UpdateSuggestions(force: false);
            }
        }
        else
        {
            SelectedTokenIndex = -1;
            SetCurrentValue(IsDropDownOpenProperty, false);
            if (EditingKey is not null)
            {
                CommitEditToken();
            }
        }

        UpdateTokens();
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (ReferenceEquals(window, subscribedWindow))
        {
            return;
        }

        UnsubscribeWindow();
        subscribedWindow = window;
        if (window is not null)
        {
            window.Deactivated += OnWindowDeactivatedOrMoved;
            window.LocationChanged += OnWindowDeactivatedOrMoved;
        }
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        CancelLookup();
        SetCurrentValue(IsDropDownOpenProperty, false);
        UnsubscribeWindow();
    }

    private void UnsubscribeWindow()
    {
        if (subscribedWindow is null)
        {
            return;
        }

        subscribedWindow.Deactivated -= OnWindowDeactivatedOrMoved;
        subscribedWindow.LocationChanged -= OnWindowDeactivatedOrMoved;
        subscribedWindow = null;
    }

    private void OnWindowDeactivatedOrMoved(
        object? sender,
        EventArgs e)
        => SetCurrentValue(IsDropDownOpenProperty, false);

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        tokensSignature = null;
        UpdateTokens();
        UpdateStatusText();
    }

    private void OnClearButtonClick(
        object sender,
        RoutedEventArgs e)
    {
        ClearTokens();
        PartTextBox.Focus();
        e.Handled = true;
    }
}