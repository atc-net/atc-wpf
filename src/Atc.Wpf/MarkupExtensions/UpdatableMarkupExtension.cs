// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
namespace Atc.Wpf.MarkupExtensions;

/// <summary>
/// Base class for markup extensions that can update the provided value on the target after it was first provided.
/// </summary>
public abstract class UpdatableMarkupExtension : MarkupExtension
{
    /// <summary>
    /// Gets the object the markup extension is applied to.
    /// </summary>
    protected object? TargetObject { get; private set; }

    /// <summary>
    /// Gets the property the markup extension is applied to.
    /// </summary>
    protected object? TargetProperty { get; private set; }

    /// <inheritdoc />
    public sealed override object ProvideValue(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target)
        {
            TargetObject = target.TargetObject;
            TargetProperty = target.TargetProperty;
        }

        return ProvideValueInternal(serviceProvider);
    }

    /// <summary>
    /// Sets the specified value on the target property of the target object.
    /// </summary>
    /// <param name="value">The new value.</param>
    protected void UpdateValue(object value)
    {
        if (TargetObject is null)
        {
            return;
        }

        if (TargetProperty is DependencyProperty dp)
        {
            HandleDependencyProperty(
                value,
                dp);
        }
        else
        {
            if (TargetObject is Binding targetObject)
            {
                var newBinding = CloneDataBinding(targetObject);

                TargetObject = newBinding;

                var prop = TargetProperty as PropertyInfo;
                prop?.SetValue(
                    TargetObject,
                    value,
                    index: null);
            }
            else
            {
                var prop = TargetProperty as PropertyInfo;
                prop?.SetValue(
                    TargetObject,
                    value,
                    index: null);
            }
        }
    }

    /// <summary>
    /// When implemented in a derived class, returns the value to set on the target property.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>The value to set on the target property.</returns>
    protected abstract object ProvideValueInternal(
        IServiceProvider serviceProvider);

    private void HandleDependencyProperty(
        object value,
        DependencyProperty dp)
    {
        if (TargetObject is not DependencyObject d)
        {
            return;
        }

        void UpdateAction()
            => d.SetValue(
                dp,
                value);
        if (d.CheckAccess())
        {
            UpdateAction();
        }
        else
        {
            d.Dispatcher.Invoke(UpdateAction);
        }
    }

    private static Binding CloneDataBinding(Binding orgBinding)
    {
        var newBinding = new Binding
        {
            AsyncState = orgBinding.AsyncState,
            BindingGroupName = orgBinding.BindingGroupName,
            BindsDirectlyToSource = orgBinding.BindsDirectlyToSource,
            Converter = orgBinding.Converter,
            ConverterCulture = orgBinding.ConverterCulture,
            ConverterParameter = orgBinding.ConverterParameter,
            FallbackValue = orgBinding.FallbackValue,
            IsAsync = orgBinding.IsAsync,
            Mode = orgBinding.Mode,
            NotifyOnSourceUpdated = orgBinding.NotifyOnSourceUpdated,
            NotifyOnTargetUpdated = orgBinding.NotifyOnTargetUpdated,
            NotifyOnValidationError = orgBinding.NotifyOnValidationError,
            Path = orgBinding.Path,
            StringFormat = orgBinding.StringFormat,
            TargetNullValue = orgBinding.TargetNullValue,
            UpdateSourceExceptionFilter = orgBinding.UpdateSourceExceptionFilter,
            UpdateSourceTrigger = orgBinding.UpdateSourceTrigger,
            ValidatesOnDataErrors = orgBinding.ValidatesOnDataErrors,
            ValidatesOnExceptions = orgBinding.ValidatesOnExceptions,
            XPath = orgBinding.XPath,
        };

        if (orgBinding.ElementName is not null)
        {
            newBinding.ElementName = orgBinding.ElementName;
        }

        if (orgBinding.RelativeSource is not null)
        {
            newBinding.RelativeSource = orgBinding.RelativeSource;
        }

        if (orgBinding.Source is not null)
        {
            newBinding.Source = orgBinding.Source;
        }

        return newBinding;
    }
}