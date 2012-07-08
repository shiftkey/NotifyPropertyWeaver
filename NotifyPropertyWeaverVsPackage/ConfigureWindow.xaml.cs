using System.Windows;

public partial class ConfigureWindow
{

    public ConfigureWindow(ConfigureWindowModel model)
    {
        Model = model;
        InitializeComponent();

        DataContext = Model;
    }

    public ConfigureWindowModel Model { get; private set; }

    void Ok(object sender, RoutedEventArgs e)
    {
        var errors = Model.GetErrors();
        if (errors != null)
        {
            MessageBox.Show(errors, "Errors", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        DialogResult = true;
        Close();
    }


    void Cancel(object sender, RoutedEventArgs e)
    {
        Close();
    }

    void SetDefaultInvoker(object sender, RoutedEventArgs e)
    {
        Model.EventInvokerName = "OnPropertyChanged";
    }

    void SetSameAsToolsDirectory(object sender, RoutedEventArgs e)
    {
        Model.DependenciesDirectory = Model.ToolsDirectory;
    }

    void SetDefaultToolsDirectory(object sender, RoutedEventArgs e)
    {
        Model.ToolsDirectory = @"$(SolutionDir)Tools\";
    }

    void SetDefaultDependenciesDirectory(object sender, RoutedEventArgs e)
    {
        Model.DependenciesDirectory = @"$(SolutionDir)Lib\";
    }


}