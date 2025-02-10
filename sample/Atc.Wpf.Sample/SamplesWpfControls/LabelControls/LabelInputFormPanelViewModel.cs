namespace Atc.Wpf.Sample.SamplesWpfControls.LabelControls;

public sealed class LabelInputFormPanelViewModel : ViewModelBase
{
    private readonly LabelInputFormPanelSettings formSettingsForModel = new()
    {
        UseGroupBox = true,
    };

    private LabelInputFormPanel formPanel = new();

    public IRelayCommand ShowInputForm1ColumnCommand => new RelayCommand(ShowInputForm1ColumnCommandHandler);

    public IRelayCommand ShowInputForm2ColumnsCommand => new RelayCommand(ShowInputForm2ColumnsCommandHandler);

    public IRelayCommand ShowInputForm3ColumnsCommand => new RelayCommand(ShowInputForm3ColumnsCommandHandler);

    public IRelayCommand ShowInputForm4ColumnsCommand => new RelayCommand(ShowInputForm4ColumnsCommandHandler);

    public IRelayCommand ShowInputFormAddressWithDataCommands => new RelayCommand(ShowInputFormAddressWithDataCommandHandler);

    public IRelayCommand ShowInputFormAddressWithoutDataCommands => new RelayCommand(ShowInputFormAddressWithoutDataCommandHandler);

    public IRelayCommand ShowInputFormPersonWithDataCommands => new RelayCommand(ShowInputFormPersonWithDataCommandHandler);

    public IRelayCommand ShowInputFormPersonWithoutDataCommands => new RelayCommand(ShowInputFormPersonWithoutDataCommandHandler);

    public IRelayCommand ShowInputFormAccountWithDataCommands => new RelayCommand(ShowInputFormAccountWithDataCommandHandler);

    public IRelayCommand ShowInputFormAccountWithoutDataCommands => new RelayCommand(ShowInputFormAccountWithoutDataCommandHandler);

    public LabelInputFormPanel FormPanel
    {
        get => formPanel;
        set
        {
            formPanel = value;
            RaisePropertyChanged();
        }
    }

    private void ShowInputForm1ColumnCommandHandler()
    {
        var labelControls = new List<ILabelControlBase>
        {
            new LabelTextBox
            {
                LabelText = "FirstName",
                IsMandatory = true,
                MinLength = 2,
            },
            new LabelTextBox
            {
                LabelText = "LastName",
                IsMandatory = true,
                MinLength = 2,
            },
            new LabelIntegerBox
            {
                LabelText = "Age",
                Minimum = 0,
            },
            new LabelTextBox
            {
                LabelText = "Note",
            },
        };

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var labelInputFormPanel = new LabelInputFormPanel(
            new LabelInputFormPanelSettings(),
            labelControlsForm);

        FormPanel = labelInputFormPanel;
    }

    private void ShowInputForm2ColumnsCommandHandler()
    {
        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(CreateLabelControlsColumn1());
        labelControlsForm.AddColumn(CreateLabelControlsColumn2());

        var labelInputFormPanel = new LabelInputFormPanel(
            new LabelInputFormPanelSettings(),
            labelControlsForm);

        FormPanel = labelInputFormPanel;
    }

    private void ShowInputForm3ColumnsCommandHandler()
    {
        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(CreateLabelControlsColumn1());
        labelControlsForm.AddColumn(CreateLabelControlsColumn2());
        labelControlsForm.AddColumn(CreateLabelControlsColumn3());

        var labelInputFormPanel = new LabelInputFormPanel(
            new LabelInputFormPanelSettings(),
            labelControlsForm);

        FormPanel = labelInputFormPanel;
    }

    private void ShowInputForm4ColumnsCommandHandler()
    {
        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(CreateLabelControlsColumn1());
        labelControlsForm.AddColumn(CreateLabelControlsColumn2());
        labelControlsForm.AddColumn(CreateLabelControlsColumn3());
        labelControlsForm.AddColumn(CreateLabelControlsColumn4());

        var labelInputFormPanel = new LabelInputFormPanel(
            new LabelInputFormPanelSettings(),
            labelControlsForm);

        FormPanel = labelInputFormPanel;
    }

    private void ShowInputFormAddressWithDataCommandHandler()
    {
        var address = new Address(
            streetName: "My street",
            cityName: "My city",
            postalCode: "1234",
            country: new CultureInfo("da-DK"));

        var labelControls = ModelToLabelControlExtractor.Extract(address);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var labelInputFormPanel = new LabelInputFormPanel(
            formSettingsForModel,
            labelControlsForm);

        FormPanel = labelInputFormPanel;
    }

    private void ShowInputFormAddressWithoutDataCommandHandler()
    {
        var address = new Address(
            streetName: string.Empty,
            cityName: string.Empty,
            postalCode: string.Empty,
            country: new CultureInfo("en-US"));

        var labelControls = ModelToLabelControlExtractor.Extract(address);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var labelInputFormPanel = new LabelInputFormPanel(
            formSettingsForModel,
            labelControlsForm);

        FormPanel = labelInputFormPanel;
    }

    private void ShowInputFormPersonWithDataCommandHandler()
    {
        var person = new Person(
            firstName: "John",
            lastName: "Doe",
            age: 33,
            favoriteColor: Colors.DarkCyan,
            myAddress: new Address(
                streetName: "My street",
                cityName: "My city",
                postalCode: "1234",
                country: new CultureInfo("da-DK")));

        var labelControls = ModelToLabelControlExtractor.Extract(
            person,
            includeReadOnly: false,
            groupIdentifier: "MyPerson");

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var labelInputFormPanel = new LabelInputFormPanel(
            formSettingsForModel,
            labelControlsForm);

        FormPanel = labelInputFormPanel;
    }

    private void ShowInputFormPersonWithoutDataCommandHandler()
    {
        var person = new Person(
            firstName: string.Empty,
            lastName: string.Empty,
            age: 0,
            favoriteColor: Colors.Aqua,
            myAddress: new Address(
                streetName: string.Empty,
                cityName: string.Empty,
                postalCode: string.Empty,
                country: new CultureInfo("en-US")));

        var labelControls = ModelToLabelControlExtractor.Extract(person);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var labelInputFormPanel = new LabelInputFormPanel(
            formSettingsForModel,
            labelControlsForm);

        FormPanel = labelInputFormPanel;
    }

    private void ShowInputFormAccountWithDataCommandHandler()
    {
        var account = new Account(
            accountNumber: "6545643218",
            createdDate: DateTime.Now.AddYears(-2).AddDays(3),
            primaryContactPerson: new Person(
                firstName: "John",
                lastName: "Doe",
                age: 33,
                favoriteColor: Colors.DarkCyan,
                myAddress: new Address(
                    streetName: "My street",
                    cityName: "My city",
                    postalCode: "1234",
                    country: new CultureInfo("da-DK"))));

        var labelControls = ModelToLabelControlExtractor.Extract(account);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var labelInputFormPanel = new LabelInputFormPanel(
            formSettingsForModel,
            labelControlsForm);

        FormPanel = labelInputFormPanel;
    }

    private void ShowInputFormAccountWithoutDataCommandHandler()
    {
        var account = new Account(
            accountNumber: string.Empty,
            createdDate: DateTime.Now.AddYears(-2).AddDays(3),
            primaryContactPerson: new Person(
                firstName: string.Empty,
                lastName: string.Empty,
                age: 0,
                favoriteColor: Colors.Aqua,
                myAddress: new Address(
                    streetName: string.Empty,
                    cityName: string.Empty,
                    postalCode: string.Empty,
                    country: new CultureInfo("en-US"))));

        var labelControls = ModelToLabelControlExtractor.Extract(account);

        var labelControlsForm = new LabelControlsForm();
        labelControlsForm.AddColumn(labelControls);

        var labelInputFormPanel = new LabelInputFormPanel(
            formSettingsForModel,
            labelControlsForm);

        FormPanel = labelInputFormPanel;
    }

    private static List<ILabelControlBase> CreateLabelControlsColumn1()
        => new()
        {
            new LabelTextBox
            {
                LabelText = "FirstName",
                IsMandatory = true,
                MinLength = 2,
            },
            new LabelTextBox
            {
                LabelText = "LastName",
                IsMandatory = true,
                MinLength = 2,
            },
        };

    private static List<ILabelControlBase> CreateLabelControlsColumn2()
        => new()
        {
            new LabelIntegerBox
            {
                LabelText = "Age",
                Minimum = 0,
            },
            new LabelTextBox
            {
                LabelText = "Note",
            },
        };

    private static List<ILabelControlBase> CreateLabelControlsColumn3()
        => new()
        {
            new LabelCheckBox
            {
                LabelText = "Use Foo",
            },
            new LabelComboBox
            {
                LabelText = "Items",
                Items = new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    { "Item1", "Item 1" },
                    { "Item2", "Item 2" },
                    { "Item3", "Item 3" },
                    { "Item4", "Item 4" },
                    { "Item5", "Item 5" },
                },
            },
        };

    private static List<ILabelControlBase> CreateLabelControlsColumn4()
        => new()
        {
            new LabelFilePicker
            {
                LabelText = "My file",
            },
            new LabelDirectoryPicker
            {
                LabelText = "My directory",
            },
        };
}