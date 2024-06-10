// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Atc.Wpf.Mvvm;

/// <summary>
/// A base class for the ViewModel class, to be used in the MVVM pattern design.
/// </summary>
public abstract class ViewModelBase : ObservableObject, IViewModelBase
{
    private bool isEnable;
    private bool isVisible;
    private bool isBusy;
    private bool isDirty;
    private bool isSelected;

    public static Guid ViewModelId => Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
    /// </summary>
    protected ViewModelBase()
        : this(messenger: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
    /// </summary>
    /// <param name="messenger">The messenger.</param>
    protected ViewModelBase(IMessenger? messenger)
    {
        MessengerInstance = messenger ?? Messenger.Default;
    }

    /// <inheritdoc />
    public bool IsEnable
    {
        get => isEnable;
        set
        {
            if (isEnable == value)
            {
                return;
            }

            isEnable = value;
            RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public bool IsVisible
    {
        get => isVisible;
        set
        {
            if (isVisible == value)
            {
                return;
            }

            isVisible = value;
            RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            if (isBusy == value)
            {
                return;
            }

            isBusy = value;
            RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public bool IsDirty
    {
        get => isDirty;
        set
        {
            if (isDirty == value)
            {
                return;
            }

            isDirty = value;
            RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (isSelected == value)
            {
                return;
            }

            isSelected = value;
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is in design mode.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is in design mode; otherwise, <c>false</c>.
    /// </value>
    public static bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

    /// <summary>
    /// Gets the messenger instance.
    /// </summary>
    /// <value>
    /// The messenger instance.
    /// </value>
    protected IMessenger MessengerInstance { get; init; }

    /// <inheritdoc />
    public Task SetIsBusy(bool value, ushort delayInMs = 1)
    {
        IsBusy = value;

        // Give the UI a moment to refresh - show BusyIndicator.
        return Task.Delay(delayInMs, CancellationToken.None);
    }

    /// <inheritdoc />
    public virtual void Cleanup()
    {
        MessengerInstance.UnRegister(this);
    }

    /// <inheritdoc />
    public void Broadcast<T>(string propertyName, T oldValue, T newValue)
    {
        var message = new PropertyChangedMessage<T>(this, propertyName, oldValue, newValue);
        MessengerInstance.Send(message);
    }

    /// <inheritdoc />
    public void RaisePropertyChanged<T>(
        string propertyName,
        T? oldValue = default,
        T? newValue = default,
        bool broadcast = false)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            throw new ArgumentException("This method cannot be called with an empty string", propertyName);
        }

        RaisePropertyChanged(propertyName);
        if (broadcast)
        {
            Broadcast(propertyName, oldValue, newValue);
        }
    }
}