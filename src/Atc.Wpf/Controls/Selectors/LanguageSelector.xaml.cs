// ReSharper disable IdentifierTypo
namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// Interaction logic for LanguageSelector.xaml
/// </summary>
public partial class LanguageSelector
{
    private string selectedKey;

    public static readonly DependencyProperty ShowLabelProperty = DependencyProperty.Register(
        nameof(ShowLabel),
        typeof(bool),
        typeof(LanguageSelector),
        new PropertyMetadata(defaultValue: true));

    public bool ShowLabel
    {
        get => (bool)GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(LanguageSelector),
        new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty RenderFlagIndicatorTypeTypeProperty = DependencyProperty.Register(
        nameof(RenderFlagIndicatorType),
        typeof(RenderFlagIndicatorType),
        typeof(LanguageSelector),
        new PropertyMetadata(RenderFlagIndicatorType.Shiny16));

    public RenderFlagIndicatorType RenderFlagIndicatorType
    {
        get => (RenderFlagIndicatorType)GetValue(RenderFlagIndicatorTypeTypeProperty);
        set => SetValue(RenderFlagIndicatorTypeTypeProperty, value);
    }

    public LanguageSelector()
    {
        InitializeComponent();

        DataContext = this;

        var cultures = GetSupportedCultures();
        var flags = GetFlags();

        Items = new List<LanguageItem>();
        foreach (var culture in cultures)
        {
            var (_, bitmapImage) = flags.First(x => x.Key.Contains(culture!.CountryCodeA2 + ".png", StringComparison.Ordinal));
            Items.Add(new LanguageItem(culture!, bitmapImage));
        }

        var defaultCulture = Items.FirstOrDefault(x => x.Culture.Lcid == Thread.CurrentThread.CurrentUICulture.LCID);
        selectedKey = defaultCulture?.Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo) ??
                      Items.First().Culture.Lcid.ToString(GlobalizationConstants.EnglishCultureInfo);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IList<LanguageItem> Items { get; }

    public string SelectedKey
    {
        get => selectedKey;
        set
        {
            selectedKey = value;
            OnPropertyChanged();
            CultureManager.UiCulture = new CultureInfo(int.Parse(selectedKey, NumberStyles.Any, GlobalizationConstants.EnglishCultureInfo));
        }
    }

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private static List<Culture?> GetSupportedCultures()
    {
        var lcids = new List<int>
        {
            GlobalizationLcidConstants.Denmark,
            GlobalizationLcidConstants.Germany,
            GlobalizationLcidConstants.UnitedStates,
            GlobalizationLcidConstants.GreatBritain,
        };

        var cultures = lcids
            .Select(lcid => CultureHelper.GetCultureByLcid(lcid))
            .Where(culture => culture is not null)
            .ToList();

        return cultures;
    }

    private Dictionary<string, BitmapImage> GetFlags()
    {
        var filterStyle = string.Empty;
        var filterSize = string.Empty;

        switch (RenderFlagIndicatorType)
        {
            case RenderFlagIndicatorType.None:
                break;
            case RenderFlagIndicatorType.Flat16:
                filterStyle = "Flat";
                filterSize = "16";
                break;
            case RenderFlagIndicatorType.Shiny16:
                filterStyle = "Shiny";
                filterSize = "16";
                break;
            default:
                throw new SwitchCaseDefaultException(RenderFlagIndicatorType);
        }

        var assembly = Assembly.GetAssembly(typeof(LanguageSelector))!;

        var flagLocations = assembly
            .GetManifestResourceNames()
            .Where(x => x.StartsWith("Atc.Wpf.Resource.Flags", StringComparison.Ordinal) &&
                        x.Contains(filterStyle, StringComparison.Ordinal) &&
                        x.Contains(filterSize, StringComparison.Ordinal))
            .ToList();

        var flagBitmaps = new Dictionary<string, BitmapImage>(StringComparer.Ordinal);
        foreach (var flagLocation in flagLocations)
        {
            var bitmap = new BitmapImage();
            using var stream = assembly.GetManifestResourceStream(flagLocation);
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            flagBitmaps.Add(flagLocation, bitmap);
        }

        return flagBitmaps;
    }
}