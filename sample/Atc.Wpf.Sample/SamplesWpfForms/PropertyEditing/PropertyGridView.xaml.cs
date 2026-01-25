namespace Atc.Wpf.Sample.SamplesWpfForms.PropertyEditing;

public partial class PropertyGridView
{
    public PropertyGridView()
    {
        InitializeComponent();

        var sampleModel = new SamplePropertyGridModel
        {
            Name = "John Doe",
            Age = 30,
            Email = "john.doe@example.com",
            IsActive = true,
            Salary = 75000.50m,
            HireDate = DateTime.Now.AddYears(-2),
            WorkHours = new TimeOnly(9, 0),
            FavoriteColor = Colors.DodgerBlue,
            Status = EmployeeStatus.Active,
            Alignment = HorizontalAlignment.Center,
            Margin = new Thickness(5, 10, 5, 10),
            HomeAddress = new SampleAddress
            {
                Street = "123 Main St",
                City = "New York",
                ZipCode = "10001",
            },
        };

        SamplePropertyGrid.SelectedObject = sampleModel;
    }
}