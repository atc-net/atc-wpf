// ReSharper disable InvertIf
namespace Atc.Wpf.Sample.SamplesWpfSourceGenerators;

public partial class ViewModelView
{
    public ViewModelView()
    {
        InitializeComponent();

        Messenger.Default.Register<PropertyChangedMessage<string>>(this, Handle);
    }

    private static void Handle(PropertyChangedMessage<string> message)
    {
        if (message.Sender?.GetType() == typeof(PersonViewModel) &&
            message.PropertyName == nameof(PersonViewModel.FirstName))
        {
            var oldValue = message.OldValue;
            var newValue = message.NewValue;

            Debug.WriteLine($"PersonViewModel.FirstName: {oldValue} -> {newValue}");
        }
    }
}