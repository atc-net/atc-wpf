namespace Atc.Wpf.Controls.Monitoring;

public sealed class ApplicationFilterViewModel : ViewModelBase
{
    private bool severityInformation;
    private bool severityWarning;
    private bool severityError;
    private string matchOnTextInData = string.Empty;

    public ApplicationFilterViewModel()
    {
        SeverityInformation = true;
        SeverityWarning = true;
        SeverityError = true;
        MatchOnTextInData = string.Empty;
    }

    public bool SeverityInformation
    {
        get => severityInformation;
        set
        {
            if (value == severityInformation)
            {
                return;
            }

            severityInformation = value;
            RaisePropertyChanged();
        }
    }

    public bool SeverityWarning
    {
        get => severityWarning;
        set
        {
            if (value == severityWarning)
            {
                return;
            }

            severityWarning = value;
            RaisePropertyChanged();
        }
    }

    public bool SeverityError
    {
        get => severityError;
        set
        {
            if (value == severityError)
            {
                return;
            }

            severityError = value;
            RaisePropertyChanged();
        }
    }

    public string MatchOnTextInData
    {
        get => matchOnTextInData;
        set
        {
            if (value == matchOnTextInData)
            {
                return;
            }

            matchOnTextInData = value;
            RaisePropertyChanged();
        }
    }
}