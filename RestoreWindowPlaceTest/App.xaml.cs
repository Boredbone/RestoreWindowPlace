using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RestoreWindowPlace;

namespace RestoreWindowPlaceTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public WindowPlace WindowPlace { get; }

        public App()
        {
            // Set the name or path of the config file
            this.WindowPlace = new WindowPlace(@"placement.config");

            // Select the behavior when window is snapped
            //  (true: Save snapped position, false(default): Save original position before snapped)
            this.WindowPlace.IsSavingSnappedPositionEnabled = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.WindowPlace.Save();
        }
    }
}
