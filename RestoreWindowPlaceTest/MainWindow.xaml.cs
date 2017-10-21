using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RestoreWindowPlaceTest
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Save setting when this window close
            this.Closed += (o, e) => ((App)Application.Current).WindowPlace.Save();

            // Resister the window using type name as key
            ((App)Application.Current).WindowPlace.Register(this);

            // Or you can register the window with any unique key
            //((App)Application.Current).WindowPlace.Register(this, "MainWindow");
        }
    }
}
