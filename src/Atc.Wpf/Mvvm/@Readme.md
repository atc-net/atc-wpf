# MVVM

The Windows Presentation Framework (WPF) takes full advantage of the Model-View-ViewModel (MVVM) pattern.

Therefore `Atc.Wpf` provide a good starting point for using MVVM.

| Tools set in the package | Description                                                                      |
|--------------------------| ---------------------------------------------------------------------------------|
| ViewModelBase            | A base class for a the ViewModels                                                |
| MainWindowViewModelBase  | A base class for a the MainWindow-ViewModel                                      |
| ViewModelDialogBase      | A base class for a the Dialog-ViewModel                                          |
| ObservableObject         | A base class for a observable class that implement a PropertyChangedEventHandler |
| RelayCommand             | Command with `CanExecute`                                                        |
| RelayCommand{T}          | Command with `CanExecute`                                                        |
| RelayCommandAsync        | Command with `CanExecute` as async                                               |
| RelayCommandAsync{T}     | Command with `CanExecute` as async                                               |

See more about [RelayCommand's](src/Atc.Wpf/Command/@Readme.md) and how to use them.
