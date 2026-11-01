namespace Atc.Wpf.Controls.DataGrids.Internal;

/// <summary>
/// The content of the column filter pop-up: a check list of the column's distinct values (with filter box and
/// Select all), a text filter, a range filter for number and date columns, and Clear filter / OK / Cancel.
/// Enter is OK and Esc is Cancel.
/// </summary>
internal sealed partial class DataGridFilterPopupContent
{
    private const string KeyPrefix = "#";

    private readonly List<string> valueKeys = [];
    private IReadOnlyList<string> hiddenIncludedValues = [];
    private bool hasHiddenExcludedValues;
    private string columnKey = string.Empty;
    private DataGridFilterValueKind valueKind;
    private CultureInfo culture = CultureInfo.CurrentCulture;

    public DataGridFilterPopupContent()
    {
        InitializeComponent();
        PreviewKeyDown += OnPreviewKeyDown;

        PartValuesMode.Content = Atc.Wpf.Resources.Miscellaneous.Values;
        PartTextMode.Content = Atc.Wpf.Resources.Miscellaneous.TextFilter;
        PartRangeMode.Content = Atc.Wpf.Resources.Miscellaneous.Range;
        PartClearButton.Content = Atc.Wpf.Resources.Miscellaneous.ClearFilter;
        PartOkButton.Content = Word.Ok;
        PartCancelButton.Content = Word.Cancel;
        PartFromLabel.Text = Atc.Wpf.Resources.Miscellaneous.From;
        PartToLabel.Text = Atc.Wpf.Resources.Miscellaneous.To;
        AutomationProperties.SetName(PartFrom, Atc.Wpf.Resources.Miscellaneous.From);
        AutomationProperties.SetName(PartTo, Atc.Wpf.Resources.Miscellaneous.To);
        AutomationProperties.SetName(PartText, Atc.Wpf.Resources.Miscellaneous.TextFilter);

        PartTextOperator.ItemsSource = new[]
        {
            new KeyValuePair<DataGridTextFilterOperator, string>(DataGridTextFilterOperator.Contains, Atc.Wpf.Resources.Miscellaneous.Contains),
            new KeyValuePair<DataGridTextFilterOperator, string>(DataGridTextFilterOperator.IsEqualTo, Atc.Wpf.Resources.Miscellaneous.IsEqualTo),
            new KeyValuePair<DataGridTextFilterOperator, string>(DataGridTextFilterOperator.StartsWith, Atc.Wpf.Resources.Miscellaneous.StartsWith),
        };
        PartTextOperator.DisplayMemberPath = "Value";
        PartTextOperator.SelectedValuePath = "Key";
        PartTextOperator.SelectedIndex = 0;
    }

    /// <summary>
    /// Occurs when OK (or Enter) is chosen.
    /// </summary>
    public event EventHandler? OkRequested;

    /// <summary>
    /// Occurs when Cancel (or Esc) is chosen.
    /// </summary>
    public event EventHandler? CancelRequested;

    /// <summary>
    /// Occurs when Clear filter is chosen.
    /// </summary>
    public event EventHandler? ClearRequested;

    /// <summary>
    /// Gets the check list of distinct values.
    /// </summary>
    internal CheckListBox ValueList => PartValues;

    /// <summary>
    /// Gets the text filter box.
    /// </summary>
    internal TextBox TextFilterBox => PartText;

    /// <summary>
    /// Gets the lower range box.
    /// </summary>
    internal TextBox FromBox => PartFrom;

    /// <summary>
    /// Gets the upper range box.
    /// </summary>
    internal TextBox ToBox => PartTo;

    /// <summary>
    /// Gets the buttons and lists that UI Automation exposes under the filter button while the pop-up is open.
    /// </summary>
    internal Button OkButton => PartOkButton;

    /// <summary>
    /// Gets the Cancel button.
    /// </summary>
    internal Button CancelButton => PartCancelButton;

    /// <summary>
    /// Gets the Clear filter button.
    /// </summary>
    internal Button ClearButton => PartClearButton;

    /// <summary>
    /// Gets or sets the filter mode shown.
    /// </summary>
    internal DataGridColumnFilterKind Mode
    {
        get
        {
            if (PartTextMode.IsChecked == true)
            {
                return DataGridColumnFilterKind.Text;
            }

            if (PartRangeMode.IsChecked == true)
            {
                return valueKind == DataGridFilterValueKind.Date
                    ? DataGridColumnFilterKind.DateRange
                    : DataGridColumnFilterKind.NumberRange;
            }

            return DataGridColumnFilterKind.Values;
        }

        set
        {
            switch (value)
            {
                case DataGridColumnFilterKind.Text:
                    PartTextMode.IsChecked = true;
                    break;
                case DataGridColumnFilterKind.NumberRange or DataGridColumnFilterKind.DateRange when valueKind != DataGridFilterValueKind.Text:
                    PartRangeMode.IsChecked = true;
                    break;
                default:
                    PartValuesMode.IsChecked = true;
                    break;
            }

            UpdateModePanels();
        }
    }

    /// <summary>
    /// Fills the pop-up for a column.
    /// </summary>
    public void Load(
        string key,
        IReadOnlyList<DataGridFilterValue> values,
        DataGridFilterValueKind kind,
        DataGridColumnFilter? current,
        CultureInfo formatCulture)
    {
        ArgumentNullException.ThrowIfNull(values);

        columnKey = key;
        valueKind = kind;
        culture = formatCulture;
        PartRangeMode.Visibility = kind == DataGridFilterValueKind.Text
            ? Visibility.Collapsed
            : Visibility.Visible;

        var items = new Dictionary<string, string>(values.Count, StringComparer.Ordinal);
        valueKeys.Clear();
        foreach (var value in values)
        {
            var itemKey = KeyPrefix + value.Text;
            items[itemKey] = value.DisplayText;
            valueKeys.Add(itemKey);
        }

        PartValues.Items = items;
        var allowed = current?.Kind == DataGridColumnFilterKind.Values
            ? new HashSet<string>(current.Values, StringComparer.Ordinal)
            : null;
        PartValues.SelectedKeys = new ObservableCollection<string>(
            valueKeys.Where(x => allowed is null || allowed.Contains(x[KeyPrefix.Length..])));

        if (current?.Kind == DataGridColumnFilterKind.Text)
        {
            PartTextOperator.SelectedValue = current.TextOperator;
            PartText.Text = current.Text ?? string.Empty;
        }

        if (current?.Kind == DataGridColumnFilterKind.NumberRange)
        {
            PartFrom.Text = current.Minimum?.ToString(culture) ?? string.Empty;
            PartTo.Text = current.Maximum?.ToString(culture) ?? string.Empty;
        }
        else if (current?.Kind == DataGridColumnFilterKind.DateRange)
        {
            PartFrom.Text = current.From?.ToString("d", culture) ?? string.Empty;
            PartTo.Text = current.To?.ToString("d", culture) ?? string.Empty;
        }

        Mode = current?.Kind ?? DataGridColumnFilterKind.Values;
    }

    /// <summary>
    /// Sets the values the list does not show (their rows are hidden by other filters) that pass the column's filter,
    /// and whether some hidden values do not pass: a values filter built on OK keeps both as they are.
    /// </summary>
    public void SetHiddenValues(
        (IReadOnlyList<string> Included, bool HasExcluded) hiddenValues)
    {
        hiddenIncludedValues = hiddenValues.Included;
        hasHiddenExcludedValues = hiddenValues.HasExcluded;
    }

    /// <summary>
    /// Builds the filter the pop-up describes, or <see langword="null"/> when it restricts nothing
    /// (every value ticked with no hidden value excluded, an empty text, an open range).
    /// </summary>
    public DataGridColumnFilter? BuildFilter()
    {
        switch (Mode)
        {
            case DataGridColumnFilterKind.Text:
                return string.IsNullOrEmpty(PartText.Text)
                    ? null
                    : DataGridColumnFilter.ForText(
                        columnKey,
                        PartTextOperator.SelectedValue is DataGridTextFilterOperator op ? op : DataGridTextFilterOperator.Contains,
                        PartText.Text);

            case DataGridColumnFilterKind.NumberRange:
            {
                var minimum = ParseNumber(PartFrom.Text);
                var maximum = ParseNumber(PartTo.Text);
                return minimum is null && maximum is null
                    ? null
                    : DataGridColumnFilter.ForNumberRange(columnKey, minimum, maximum);
            }

            case DataGridColumnFilterKind.DateRange:
            {
                var from = ParseDate(PartFrom.Text);
                var to = ParseDate(PartTo.Text);
                return from is null && to is null
                    ? null
                    : DataGridColumnFilter.ForDateRange(columnKey, from, to);
            }

            default:
            {
                var selected = new HashSet<string>(PartValues.SelectedKeys, StringComparer.Ordinal);
                return valueKeys.TrueForAll(selected.Contains) && !hasHiddenExcludedValues
                    ? null
                    : DataGridColumnFilter.ForValues(
                        columnKey,
                        valueKeys
                            .Where(selected.Contains)
                            .Select(x => x[KeyPrefix.Length..])
                            .Concat(hiddenIncludedValues));
            }
        }
    }

    /// <summary>
    /// Moves the keyboard focus into the pop-up (the filter box of the value list, or the first box of the mode).
    /// </summary>
    internal void FocusFirst()
    {
        UIElement target = Mode switch
        {
            DataGridColumnFilterKind.Text => PartText,
            DataGridColumnFilterKind.NumberRange or DataGridColumnFilterKind.DateRange => PartFrom,
            _ => PartValues,
        };

        if (!target.Focus() || target is CheckListBox)
        {
            // The check list itself is not a tab stop: move into its filter box.
            target.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }
    }

    private double? ParseNumber(string text)
        => double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, culture, out var number)
            ? number
            : null;

    private DateTime? ParseDate(string text)
        => DateTime.TryParse(text, culture, DateTimeStyles.None, out var date)
            ? date
            : null;

    private void OnModeChecked(
        object sender,
        RoutedEventArgs e)
        => UpdateModePanels();

    private void UpdateModePanels()
    {
        if (PartValues is null)
        {
            // The first radio button is checked while the XAML loads.
            return;
        }

        var mode = Mode;
        PartValues.Visibility = mode == DataGridColumnFilterKind.Values ? Visibility.Visible : Visibility.Collapsed;
        PartTextPanel.Visibility = mode == DataGridColumnFilterKind.Text ? Visibility.Visible : Visibility.Collapsed;
        PartRangePanel.Visibility = mode is DataGridColumnFilterKind.NumberRange or DataGridColumnFilterKind.DateRange
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <summary>
    /// Handles Esc (Cancel) and Enter (OK, unless an open combo box or a button has the key).
    /// Returns whether the key was handled.
    /// </summary>
    internal bool HandleKey(
        Key key,
        object? originalSource)
    {
        switch (key)
        {
            case Key.Escape:
                CancelRequested?.Invoke(this, EventArgs.Empty);
                return true;
            case Key.Enter when originalSource is not ComboBox { IsDropDownOpen: true } and not Button:
                OkRequested?.Invoke(this, EventArgs.Empty);
                return true;
            default:
                return false;
        }
    }

    private void OnPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (HandleKey(e.Key, e.OriginalSource))
        {
            e.Handled = true;
        }
    }

    private void OnOkClick(
        object sender,
        RoutedEventArgs e)
        => OkRequested?.Invoke(this, EventArgs.Empty);

    private void OnCancelClick(
        object sender,
        RoutedEventArgs e)
        => CancelRequested?.Invoke(this, EventArgs.Empty);

    private void OnClearClick(
        object sender,
        RoutedEventArgs e)
        => ClearRequested?.Invoke(this, EventArgs.Empty);
}