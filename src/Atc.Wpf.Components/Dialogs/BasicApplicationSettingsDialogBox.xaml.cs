namespace Atc.Wpf.Components.Dialogs;

public partial class BasicApplicationSettingsDialogBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BasicApplicationSettingsDialogBox"/> class.
    /// </summary>
    public BasicApplicationSettingsDialogBox()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BasicApplicationSettingsDialogBox"/> class
    /// using the given view model as data context.
    /// </summary>
    public BasicApplicationSettingsDialogBox(
        IBasicApplicationSettingsDialogBoxViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    /// <summary>
    /// Gets the edited application settings serialized as JSON, or <c>"{}"</c> when the data context
    /// is not a <see cref="BasicApplicationSettingsDialogBoxViewModel"/>.
    /// </summary>
    public string GetDataAsJson()
        => DataContext is BasicApplicationSettingsDialogBoxViewModel vm
            ? vm.ToJson()
            : "{}";
}