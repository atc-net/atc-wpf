namespace Atc.Wpf.Sample.SamplesWpfForms.PropertyEditing;

/// <summary>
/// Sample model demonstrating PropertyGrid capabilities.
/// </summary>
public class SamplePropertyGridModel : ViewModelBase
{
    private string name = string.Empty;
    private int age;
    private string email = string.Empty;
    private bool isActive;
    private decimal salary;
    private DateTime hireDate;
    private TimeOnly workHours;
    private Color favoriteColor;
    private EmployeeStatus status;
    private HorizontalAlignment alignment;
    private Thickness margin;
    private SampleAddress? homeAddress;
    private FileInfo? configFile;
    private DirectoryInfo? workDirectory;

    [Category("Personal")]
    [DisplayName("Full Name")]
    [Description("Enter the employee's full name")]
    [Required]
    public string Name
    {
        get => name;
        set
        {
            name = value;
            RaisePropertyChanged();
        }
    }

    [Category("Personal")]
    [Description("Employee age in years")]
    [Range(18, 100)]
    public int Age
    {
        get => age;
        set
        {
            age = value;
            RaisePropertyChanged();
        }
    }

    [Category("Personal")]
    [DisplayName("Email Address")]
    [Description("Work email address")]
    [MaxLength(100)]
    public string Email
    {
        get => email;
        set
        {
            email = value;
            RaisePropertyChanged();
        }
    }

    [Category("Employment")]
    [DisplayName("Is Active")]
    [Description("Whether the employee is currently active")]
    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
            RaisePropertyChanged();
        }
    }

    [Category("Employment")]
    [Description("Annual salary")]
    [Range(0, 1000000)]
    public decimal Salary
    {
        get => salary;
        set
        {
            salary = value;
            RaisePropertyChanged();
        }
    }

    [Category("Employment")]
    [DisplayName("Hire Date")]
    [Description("Date when the employee was hired")]
    public DateTime HireDate
    {
        get => hireDate;
        set
        {
            hireDate = value;
            RaisePropertyChanged();
        }
    }

    [Category("Employment")]
    [DisplayName("Work Hours Start")]
    [Description("Time when work hours begin")]
    public TimeOnly WorkHours
    {
        get => workHours;
        set
        {
            workHours = value;
            RaisePropertyChanged();
        }
    }

    [Category("Employment")]
    [Description("Current employment status")]
    public EmployeeStatus Status
    {
        get => status;
        set
        {
            status = value;
            RaisePropertyChanged();
        }
    }

    [Category("Appearance")]
    [DisplayName("Favorite Color")]
    [Description("Employee's favorite color")]
    public Color FavoriteColor
    {
        get => favoriteColor;
        set
        {
            favoriteColor = value;
            RaisePropertyChanged();
        }
    }

    [Category("Layout")]
    [Description("Horizontal alignment preference")]
    public HorizontalAlignment Alignment
    {
        get => alignment;
        set
        {
            alignment = value;
            RaisePropertyChanged();
        }
    }

    [Category("Layout")]
    [Description("Margin preferences (Left, Top, Right, Bottom)")]
    public Thickness Margin
    {
        get => margin;
        set
        {
            margin = value;
            RaisePropertyChanged();
        }
    }

    [Category("Address")]
    [DisplayName("Home Address")]
    [Description("Employee's home address details")]
    public SampleAddress? HomeAddress
    {
        get => homeAddress;
        set
        {
            homeAddress = value;
            RaisePropertyChanged();
        }
    }

    [Category("Files")]
    [DisplayName("Config File")]
    [Description("Configuration file path")]
    public FileInfo? ConfigFile
    {
        get => configFile;
        set
        {
            configFile = value;
            RaisePropertyChanged();
        }
    }

    [Category("Files")]
    [DisplayName("Work Directory")]
    [Description("Working directory path")]
    public DirectoryInfo? WorkDirectory
    {
        get => workDirectory;
        set
        {
            workDirectory = value;
            RaisePropertyChanged();
        }
    }

    [Browsable(false)]
    public string InternalId { get; set; } = Guid.NewGuid().ToString();
}