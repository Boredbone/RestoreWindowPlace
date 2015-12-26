using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestoreWindowPlaceTest
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public RestoreWindowPlace.RestoreWindowPlace WindowPlace { get; }

        public App()
        {
            // Set a name of config file
            this.WindowPlace = new RestoreWindowPlace.RestoreWindowPlace(@"placement.config");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.WindowPlace.Save();
        }
    }
}
