# RestoreWindowPlace


Save and restore the place of WPF windows  
  
  
## Example
  
in App.xaml.cs
```cs
using RestoreWindowPlace;

public partial class App : Application
{
    public WindowPlace WindowPlace { get; }

    public App()
    {
        // Set a name of config file
        this.WindowPlace = new WindowPlace("placement.config");
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

        // Using the name of type as key
        ((App)Application.Current).WindowPlace.Register(this);
    }
}
```

If you want to use a key other than type name, or want to save individual settings for multiple instances, set string as key.

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