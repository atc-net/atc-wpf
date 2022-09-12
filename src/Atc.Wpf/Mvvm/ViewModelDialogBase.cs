namespace Atc.Wpf.Mvvm;

public class ViewModelDialogBase : ViewModelBase
{
    private bool dialogResult;

    /// <summary>
    /// Gets or sets a value indicating whether dialog result.
    /// </summary>
    public bool DialogResult
    {
        get => dialogResult;
        set
        {
            dialogResult = value;
            RaisePropertyChanged();
        }
    }
}