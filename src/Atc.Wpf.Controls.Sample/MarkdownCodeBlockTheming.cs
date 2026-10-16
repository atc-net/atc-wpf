namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// Keeps the AvalonEdit code blocks of a markdown document readable on a dark background. AvalonEdit's built-in
/// syntax colours are made for a light page; when the editor's background is dark, every foreground that is too
/// dark is lifted to a readable lightness after the highlighting colorizer has run. The decision follows the
/// editor's actual background brush, which the style binds to a theme resource, so a theme switch is picked up
/// through that brush without listening to the theme manager.
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

    /// <summary>Mirrors the editor's Background through a one-way self binding, so changes to it can be followed without pinning the editor.</summary>
    private static readonly DependencyProperty BackgroundProxyProperty = DependencyProperty.RegisterAttached(
        "BackgroundProxy",
        typeof(Brush),
        typeof(MarkdownCodeBlockTheming),
        new PropertyMetadata(
            defaultValue: null,
            OnBackgroundProxyChanged));

    /// <summary>Gets whether the editor's syntax colours are adjusted to its background.</summary>
    /// <param name="obj">The text editor.</param>
    /// <returns><see langword="true"/> when enabled.</returns>
    public static bool GetIsEnabled(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (bool)obj.GetValue(IsEnabledProperty);
    }

    /// <summary>Sets whether the editor's syntax colours are adjusted to its background.</summary>
    /// <param name="obj">The text editor.</param>
    /// <param name="value"><see langword="true"/> to enable.</param>
    public static void SetIsEnabled(
        DependencyObject obj,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(IsEnabledProperty, value);
    }

    /// <summary>
    /// Returns whether <paramref name="brush"/> is a dark solid colour, the case in which light-page syntax colours
    /// need lifting. Anything that is not a solid colour counts as light, so the editor is left alone.
    /// </summary>
    internal static bool IsDarkBackground(Brush? brush)
        => brush is SolidColorBrush { Color: var color } &&
           color.A > 0 &&
           ((0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B)) / 255d < 0.5;

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

    private static void OnBackgroundProxyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d.GetValue(AttachmentProperty) is Attachment attachment)
        {
            attachment.Apply();
        }
    }

    /// <summary>
    /// Lifts dark syntax foregrounds on a dark background. Runs after AvalonEdit's own highlighting colorizer, which
    /// the editor keeps at the front of the transformer list.
    /// </summary>
    private sealed class DarkBackgroundSyntaxTransformer : IVisualLineTransformer
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
        private readonly DarkBackgroundSyntaxTransformer transformer = new();

        public Attachment(TextEditor editor)
        {
            this.editor = editor;
            editor.TextArea.TextView.LineTransformers.Add(transformer);
            BindingOperations.SetBinding(
                editor,
                BackgroundProxyProperty,
                new Binding(nameof(Control.Background))
                {
                    RelativeSource = RelativeSource.Self,
                    Mode = BindingMode.OneWay,
                });
            Apply();
        }

        public void Detach()
        {
            BindingOperations.ClearBinding(editor, BackgroundProxyProperty);
            editor.TextArea.TextView.LineTransformers.Remove(transformer);
        }

        public void Apply()
        {
            var isDark = IsDarkBackground(editor.Background);
            if (transformer.IsActive == isDark)
            {
                return;
            }

            transformer.IsActive = isDark;
            editor.TextArea.TextView.Redraw();
        }
    }
}