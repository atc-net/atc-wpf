// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Atc.Wpf.Mvvm;

/// <summary>
/// A base class for a the ViewModel class, to be used in the MVVM pattern design.
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
        this.MessengerInstance = messenger ?? Messenger.Default;
    }

    /// <inheritdoc />
    public bool IsEnable
    {
        get => this.isEnable;
        set
        {
            this.isEnable = value;
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public bool IsVisible
    {
        get => this.isVisible;
        set
        {
            if (this.isVisible == value)
            {
                return;
            }

            this.isVisible = value;
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public bool IsBusy
    {
        get => this.isBusy;
        set
        {
            this.isBusy = value;
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public bool IsDirty
    {
        get => this.isDirty;
        set
        {
            this.isDirty = value;
            this.RaisePropertyChanged();
        }
    }

    /// <inheritdoc />
    public bool IsSelected
    {
        get => this.isSelected;
        set
        {
            this.isSelected = value;
            this.RaisePropertyChanged();
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
    public virtual void Cleanup()
    {
        this.MessengerInstance.UnRegister(this);
    }

    /// <inheritdoc />
    public void Broadcast<T>(string propertyName, T oldValue, T newValue)
    {
        var message = new PropertyChangedMessage<T>(this, propertyName, oldValue, newValue);
        this.MessengerInstance.Send(message);
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

        this.RaisePropertyChanged(propertyName);
        if (broadcast)
        {
            this.Broadcast(propertyName, oldValue, newValue);
        }
    }
}