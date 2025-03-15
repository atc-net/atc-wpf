// ReSharper disable RedundantAttributeUsageProperty
// ReSharper disable CheckNamespace
namespace Atc.Wpf.Mvvm;

/// <summary>
/// Specifies a property in the ViewModel that should be generated for a field.
/// The class need to inherits from <see cref="IViewModelBase"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class ObservablePropertyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservablePropertyAttribute"/> class.
    /// </summary>
    public ObservablePropertyAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservablePropertyAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property to generate</param>
    public ObservablePropertyAttribute(
        string propertyName)
    {
        PropertyName = propertyName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservablePropertyAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property to generate</param>
    /// <param name="dependentProperties">The name of the dependent properties to generate</param>
    public ObservablePropertyAttribute(
        string propertyName,
        params string[] dependentProperties)
    {
        PropertyName = propertyName;
        DependentProperties = dependentProperties;
    }

    /// <summary>
    /// Gets the name of property to generate.
    /// </summary>
    public string? PropertyName { get; }

    /// <summary>
    /// Gets or sets the dependent property names.
    /// </summary>
    public string[]? DependentProperties { get; set; }

    /// <summary>
    /// Gets or sets the method(s) or expressions to execute before property changes.
    /// Example A: 'DoStuffA();'.
    /// Example B: nameof(DoStuffA).
    /// </summary>
    public string? BeforeChangedCallback { get; set; }

    /// <summary>
    /// Gets or sets the method(s) or expressions to execute after property changes.
    /// Example A: 'EntrySelected?.Invoke(this, selectedEntry); DoStuffB();'.
    /// Example b: nameof(DoStuffB).
    /// </summary>
    public string? AfterChangedCallback { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the generated property should broadcast a property change message
    /// via the Messenger instance when the property changes.
    /// <para>
    /// Example usage in a ViewModel:
    /// <code language="csharp">
    /// public partial class PersonViewModel : ViewModelBase
    /// {
    ///     // When the 'FirstName' property changes, it will automatically broadcast a message.
    ///     [ObservableProperty(BroadcastOnChange = true)]
    ///     private string firstName = string.Empty;
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Example consumer registration to handle the broadcasted message:
    /// <code language="csharp">
    /// public partial class MyControl : UserControl
    /// {
    ///     public MyControl()
    ///     {
    ///         InitializeComponent();
    ///
    ///         // Register to receive notifications for changes to properties of type string.
    ///         Messenger.Default.Register&lt;PropertyChangedMessage&lt;string&gt;&gt;(this, OnPropertyChangedMessage);
    ///     }
    ///
    ///     private void OnPropertyChangedMessage(PropertyChangedMessage&lt;string&gt; message)
    ///     {
    ///         // Check if the message is for the "FirstName" property from the type PersonViewModel.
    ///         if (message.Sender?.GetType() == typeof(PersonViewModel) &amp;&amp;
    ///             message.PropertyName == nameof(PersonViewModel.FirstName))
    ///         {
    ///             var oldValue = message.OldValue;
    ///             var newValue = message.NewValue;
    ///             Debug.WriteLine($"PersonViewModel.FirstName: {oldValue} -> {newValue}");
    ///         }
    ///     }
    /// }
    /// </code>
    /// </para>
    /// </summary>
    public bool BroadcastOnChange { get; set; }
}