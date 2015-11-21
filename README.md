# RestoreWindowPlace
======================

Save and restore the place of the WPF window  
  
  
## Example
  
in App.xaml.cs
```cs
public partial class App : Application
{
    public RestoreWindowPlace.RestoreWindowPlace WindowPlace { get; }

    public App()
    {
        // Set a name of config file
        this.WindowPlace = new RestoreWindowPlace.RestoreWindowPlace("placement.config");
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        this.WindowPlace.Save();
    }
}
```
  
  
in MainWindow.xaml.cs
```cs
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Set a unique window key
        ((App)Application.Current).WindowPlace.Register(this, "MainWindow");
    }
}
```