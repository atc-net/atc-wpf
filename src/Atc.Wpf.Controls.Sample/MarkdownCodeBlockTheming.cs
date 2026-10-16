namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// Keeps the AvalonEdit code blocks of a markdown document readable in the dark theme. AvalonEdit's built-in
/// syntax colours are made for a light page; when the application theme is dark, every foreground that is too dark
/// is lifted to a readable lightness after the highlighting colorizer has run, and the editor is redrawn when the
/// theme changes.
/// </summary>
public static class MarkdownCodeBlockTheming
{
    /// <summary>Identifies the IsEnabled attached property.</summary>
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled",
        typeof(bool),
        typeof(MarkdownCodeBlockTheming),
        new PropertyMetadata(
            defaultValue: false,
            OnIsEnabledChanged));

    private static readonly DependencyProperty AttachmentProperty = DependencyProperty.RegisterAttached(
        "Attachment",
        typeof(Attachment),
        typeof(MarkdownCodeBlockTheming),
        new PropertyMetadata(defaultValue: null));

    /// <summary>Gets whether the editor's syntax colours follow the dark theme.</summary>
    /// <param name="obj">The text editor.</param>
    /// <returns><see langword="true"/> when enabled.</returns>
    public static bool GetIsEnabled(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (bool)obj.GetValue(IsEnabledProperty);
    }

    /// <summary>Sets whether the editor's syntax colours follow the dark theme.</summary>
    /// <param name="obj">The text editor.</param>
    /// <param name="value"><see langword="true"/> to enable.</param>
    public static void SetIsEnabled(
        DependencyObject obj,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(IsEnabledProperty, value);
    }

    private static void OnIsEnabledChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextEditor editor)
        {
            return;
        }

        if (d.GetValue(AttachmentProperty) is Attachment existing)
        {
            existing.Detach();
            d.ClearValue(AttachmentProperty);
        }

        if ((bool)e.NewValue)
        {
            d.SetValue(AttachmentProperty, new Attachment(editor));
        }
    }

    /// <summary>
    /// Lifts dark syntax foregrounds on a dark background. Runs after AvalonEdit's own highlighting colorizer, which
    /// the editor keeps at the front of the transformer list.
    /// </summary>
    private sealed class DarkThemeSyntaxTransformer : IVisualLineTransformer
    {
        public bool IsActive { get; set; }

        public void Transform(
            ITextRunConstructionContext context,
            IList<VisualLineElement> elements)
        {
            if (!IsActive)
            {
                return;
            }

            foreach (var element in elements)
            {
                if (element.TextRunProperties.ForegroundBrush is not SolidColorBrush { Color: var color } brush)
                {
                    continue;
                }

                var readable = SyntaxColorAdjuster.EnsureReadableOnDark(color);
                if (readable == color)
                {
                    continue;
                }

                var adjusted = new SolidColorBrush(readable) { Opacity = brush.Opacity };
                adjusted.Freeze();
                element.TextRunProperties.SetForegroundBrush(adjusted);
            }
        }
    }

    private sealed class Attachment
    {
        private readonly TextEditor editor;
        private readonly DarkThemeSyntaxTransformer transformer = new();

        public Attachment(TextEditor editor)
        {
            this.editor = editor;
            transformer.IsActive = IsDarkTheme();
            editor.TextArea.TextView.LineTransformers.Add(transformer);
            editor.Loaded += OnLoaded;
            editor.Unloaded += OnUnloaded;

            if (editor.IsLoaded)
            {
                ThemeManager.Current.ThemeChanged += OnThemeChanged;
            }
        }

        public void Detach()
        {
            editor.Loaded -= OnLoaded;
            editor.Unloaded -= OnUnloaded;
            ThemeManager.Current.ThemeChanged -= OnThemeChanged;
            editor.TextArea.TextView.LineTransformers.Remove(transformer);
        }

        private void OnLoaded(
            object sender,
            RoutedEventArgs e)
        {
            ThemeManager.Current.ThemeChanged -= OnThemeChanged;
            ThemeManager.Current.ThemeChanged += OnThemeChanged;
            Apply(IsDarkTheme());
        }

        private void OnUnloaded(
            object sender,
            RoutedEventArgs e)
            => ThemeManager.Current.ThemeChanged -= OnThemeChanged;

        private void OnThemeChanged(
            object? sender,
            ThemeChangedEventArgs e)
            => Apply(string.Equals(e.NewTheme.BaseColorScheme, ThemeManager.BaseColorDark, StringComparison.Ordinal));

        private static bool IsDarkTheme()
            => string.Equals(
                ThemeManager.Current.DetectTheme()?.BaseColorScheme,
                ThemeManager.BaseColorDark,
                StringComparison.Ordinal);

        private void Apply(bool isDark)
        {
            if (transformer.IsActive == isDark)
            {
                return;
            }

            transformer.IsActive = isDark;
            editor.TextArea.TextView.Redraw();
        }
    }
}