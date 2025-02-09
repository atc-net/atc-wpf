namespace Atc.Wpf.SourceGenerators.Tests.Generators;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public class ViewModelGeneratorTests : GeneratorTestBase
{
    [Fact]
    public void GeneratorViewModel_ObservableProperty_Name()
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
    public void GeneratorViewModel_ObservableProperty_CustomName()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [ObservableProperty("MyName)]
                private string name;
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public string MyName
                {
                    get => name;
                    set
                    {
                        if (name == value)
                        {
                            return;
                        }
            
                        name = value;
                        RaisePropertyChanged(nameof(MyName));
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
    public void GeneratorViewModel_ObservableProperties_With_NotifyPropertiesChangedFor()
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
                        RaisePropertyChanged(nameof(FullName));
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
                        RaisePropertyChanged(nameof(FullName));
                        RaisePropertyChanged(nameof(Age));
                        RaisePropertyChanged(nameof(Email));
                        RaisePropertyChanged(nameof(TheProperty));
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

                public string? TheProperty
                {
                    get => myTestProperty;
                    set
                    {
                        if (myTestProperty == value)
                        {
                            return;
                        }

                        myTestProperty = value;
                        RaisePropertyChanged(nameof(TheProperty));
                        RaisePropertyChanged(nameof(FullName));
                        RaisePropertyChanged(nameof(Age));
                    }
                }
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_RelayCommand_NoParameter()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand]
                public void Save()
                {
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommand SaveCommand => new RelayCommand(Save);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_RelayCommand_NoParameter_CustomName()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand("MySave")]
                public void Save()
                {
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommand MySaveCommand => new RelayCommand(Save);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_RelayCommand_ParameterString()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand]
                public void Save(string val)
                {
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommand<string> SaveCommand => new RelayCommand<string>(Save);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_RelayCommand_ParameterInt()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand]
                public void Save(int val)
                {
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommand<int> SaveCommand => new RelayCommand<int>(Save);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_RelayCommand_NoParameter_CanExecute()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand(CanExecute = nameof(CanSave))]
                public void Save()
                {
                }
                
                public bool CanSave()
                {
                    return true;
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommand SaveCommand => new RelayCommand(Save, CanSave);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_RelayCommand_ParameterString_CanExecute()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand(CanExecute = nameof(CanSave))]
                public void Save(string val)
                {
                }
                
                public bool CanSave()
                {
                    return true;
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommand<string> SaveCommand => new RelayCommand<string>(Save, CanSave);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_RelayCommand_ParameterInt_CanExecute()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand(CanExecute = nameof(CanSave))]
                public void Save(int val)
                {
                }
                
                public bool CanSave()
                {
                    return true;
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommand<int> SaveCommand => new RelayCommand<int>(Save, CanSave);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_AsyncRelayCommand_NoParameter()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand]
                public Task Save()
                {
                    return Task.CompletedTask;
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommandAsync SaveCommand => new RelayCommandAsync(Save);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_AsyncRelayCommand_NoParameter_CustomName()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand("MySave")]
                public Task Save()
                {
                    return Task.CompletedTask;
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommandAsync MySaveCommand => new RelayCommandAsync(Save);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_AsyncRelayCommand_ParameterString()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand]
                public Task Save(string val)
                {
                    return Task.CompletedTask;
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommandAsync<string> SaveCommand => new RelayCommandAsync<string>(Save);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_AsyncRelayCommand_ParameterInt()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand]
                public Task Save(int val)
                {
                    return Task.CompletedTask;
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommandAsync<int> SaveCommand => new RelayCommandAsync<int>(Save);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_AsyncRelayCommand_NoParameter_CanExecute()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand(CanExecute = nameof(CanSave))]
                public Task Save()
                {
                    return Task.CompletedTask;
                }
                
                public bool CanSave()
                {
                    return true;
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommandAsync SaveCommand => new RelayCommandAsync(Save, CanSave);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_AsyncRelayCommand_ParameterString_CanExecute()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand(CanExecute = nameof(CanSave))]
                public Task Save(string val)
                {
                    return Task.CompletedTask;
                }
                
                public bool CanSave(string val)
                {
                    return true;
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommandAsync<string> SaveCommand => new RelayCommandAsync<string>(Save, CanSave);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }

    [Fact]
    public void GeneratorViewModel_AsyncRelayCommand_ParameterInt_CanExecute()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestViewModel : ViewModelBase
            {
                [RelayCommand(CanExecute = nameof(CanSave))]
                public Task Save(int val)
                {
                    return Task.CompletedTask;
                }

                public bool CanSave(int val)
                {
                    return true;
                }
            }
            """;

        const string expectedCode =
            """
            #nullable enable

            namespace TestNamespace;

            public partial class TestViewModel
            {
                public IRelayCommandAsync<int> SaveCommand => new RelayCommandAsync<int>(Save, CanSave);
            }
            """;

        var generatorResult = GeneratorViewModel(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }
}