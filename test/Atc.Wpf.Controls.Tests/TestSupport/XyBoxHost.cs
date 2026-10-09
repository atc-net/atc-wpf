namespace Atc.Wpf.Controls.Tests.TestSupport;

/// <summary>
/// Hosts a <see cref="DecimalXyBox"/> the way the Label* XY wrappers do: its ValueX is bound two-way to the inner box
/// (which updates the binding on lost focus), and its own lost-focus event comes from a tracker on the host.
/// </summary>
public sealed class XyBoxHost : UserControl
{
    public static readonly DependencyProperty ValueXProperty = DependencyProperty.Register(
        nameof(ValueX),
        typeof(decimal),
        typeof(XyBoxHost),
        new FrameworkPropertyMetadata(0m, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public XyBoxHost()
    {
        Inner = new DecimalXyBox { DecimalPlaces = 2 };
        Inner.SetBinding(
            DecimalXyBox.ValueXProperty,
            new Binding(nameof(ValueX)) { Source = this, Mode = BindingMode.TwoWay });
        Content = Inner;

        _ = new LostFocusValueTracker<decimal>(
            this,
            () => ValueX,
            (oldValue, newValue) => ValueXLostFocus?.Invoke(
                this,
                new ValueChangedEventArgs<decimal?>(string.Empty, oldValue, newValue)));
    }

    public event EventHandler<ValueChangedEventArgs<decimal?>>? ValueXLostFocus;

    public DecimalXyBox Inner { get; }

    public decimal ValueX
    {
        get => (decimal)GetValue(ValueXProperty);
        set => SetValue(ValueXProperty, value);
    }
}