// ReSharper disable IdentifierTypo
// ReSharper disable InvertIf
namespace Atc.Wpf.MarkupExtensions;

public abstract class UpdatableMarkupExtension : MarkupExtension
{
    protected object? TargetObject { get; private set; }

    protected object? TargetProperty { get; private set; }

    public sealed override object ProvideValue(
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target)
        {
            TargetObject = target.TargetObject;
            TargetProperty = target.TargetProperty;
        }

        return ProvideValueInternal(serviceProvider);
    }

    protected void UpdateValue(object value)
    {
        if (TargetObject is null)
        {
            return;
        }

        if (TargetProperty is DependencyProperty dp)
        {
            HandleDependencyProperty(value, dp);
        }
        else
        {
            if (TargetObject is Binding targetObject)
            {
                var newBinding = CloneDataBinding(targetObject);

                TargetObject = newBinding;

                var prop = TargetProperty as PropertyInfo;
                prop?.SetValue(TargetObject, value, index: null);
            }
            else
            {
                var prop = TargetProperty as PropertyInfo;
                prop?.SetValue(TargetObject, value, index: null);
            }
        }
    }

    protected abstract object ProvideValueInternal(
        IServiceProvider serviceProvider);

    private void HandleDependencyProperty(
        object value,
        DependencyProperty dp)
    {
        if (TargetObject is not DependencyObject obj)
        {
            return;
        }

        void UpdateAction() => obj.SetValue(dp, value);
        if (obj.CheckAccess())
        {
            UpdateAction();
        }
        else
        {
            obj.Dispatcher.Invoke(UpdateAction);
        }
    }

    private static Binding CloneDataBinding(
        Binding orgBinding)
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