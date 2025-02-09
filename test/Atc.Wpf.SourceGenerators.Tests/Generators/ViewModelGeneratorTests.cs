namespace Atc.Wpf.SourceGenerators.Tests.Generators;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public class ViewModelGeneratorTests : GeneratorTestBase
{
    [Fact]
    public void GeneratorViewModel_ObservableProperty_Name_IsWellDefined()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [ObservableProperty]
                private string name;
            }
            """;

        const string expectedCode =
            """
            #nullable enable
            
            namespace TestNamespace;
            
            public partial class TestViewModel
            {
                public string Name
                {
                    get => name;
                    set
                    {
                        if (name == value)
                        {
                            return;
                        }
            
                        name = value;
                        RaisePropertyChanged(nameof(Name));
                    }
                }
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_ClassName_Invalid()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class Test : ViewModelBase
            {
                [ObservableProperty]
                private string name;
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultIsEmpty(generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_ClassAccessor_Invalid()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public class TestViewModel : ViewModelBase
            {
                [ObservableProperty]
                private string name;
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultIsEmpty(generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_ObservableProperty_Name_Invalid_AccessorPublic()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [ObservableProperty]
                public string name;
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultIsEmpty(generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_ObservableProperty_Name_Invalid_AccessorProtected()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [ObservableProperty]
                protected string name;
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultIsEmpty(generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_ObservableProperty_Name_Invalid_AccessorInternal()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [ObservableProperty]
                internal string name;
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultIsEmpty(generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_ObservableProperty_Name_Invalid_Casing()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [ObservableProperty]
                private string Name;
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultIsEmpty(generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_ObservableProperties_With_NotifyPropertiesChangedFor_IsWellDefined()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class PersonViewModel : ViewModelBase
            {
                [ObservableProperty]
                [NotifyPropertyChangedFor(nameof(FullName))]
                private string firstName = string.Empty;
            
                [ObservableProperty]
                [NotifyPropertyChangedFor(nameof(FullName), nameof(Age))]
                [NotifyPropertyChangedFor(nameof(Email))]
                [NotifyPropertyChangedFor(nameof(TheProperty))]
                private string? lastName;
            
                [ObservableProperty]
                private int? age;
            
                [ObservableProperty]
                private string? email;
            
                [ObservableProperty("TheProperty", nameof(FullName), nameof(Age))]
                private string? myTestProperty;
            
                public string FullName => $"{FirstName} {LastName}";
            }
            """;

        const string expectedCode =
            """
            #nullable enable
            
            namespace TestNamespace;
            
            public partial class PersonViewModel
            {
                public string FirstName
                {
                    get => firstName;
                    set
                    {
                        if (firstName == value)
                        {
                            return;
                        }

                        firstName = value;
                        RaisePropertyChanged(nameof(FirstName));
                    }
                }

                public string? LastName
                {
                    get => lastName;
                    set
                    {
                        if (lastName == value)
                        {
                            return;
                        }

                        lastName = value;
                        RaisePropertyChanged(nameof(LastName));
                    }
                }

                public int? Age
                {
                    get => age;
                    set
                    {
                        if (age == value)
                        {
                            return;
                        }

                        age = value;
                        RaisePropertyChanged(nameof(Age));
                    }
                }

                public string? Email
                {
                    get => email;
                    set
                    {
                        if (email == value)
                        {
                            return;
                        }

                        email = value;
                        RaisePropertyChanged(nameof(Email));
                    }
                }

                public string? MyTestProperty
                {
                    get => myTestProperty;
                    set
                    {
                        if (myTestProperty == value)
                        {
                            return;
                        }

                        myTestProperty = value;
                        RaisePropertyChanged(nameof(MyTestProperty));
                    }
                }
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }
}