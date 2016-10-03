using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;

namespace RestoreWindowPlace
{
    /// <summary>
    /// Save size and position of window to file
    /// </summary>
    public class WindowPlace
    {
        private XmlSettingManager<Dictionary<string, Rectangle>> ConfigXml { get; }
        private Dictionary<string, Rectangle> WindowPlaces { get; set; }


        /// <summary>
        /// Save size and position of window to file
        /// </summary>
        /// <param name="filePath">Name or path of XML file to save</param>
        public WindowPlace(string filePath)
        {
            this.WindowPlaces = new Dictionary<string, Rectangle>();
            this.ConfigXml = new XmlSettingManager<Dictionary<string, Rectangle>>(filePath);
            this.Load();
        }

        /// <summary>
        /// Save setting file
        /// </summary>
        public void Save() => this.ConfigXml.SaveXml(this.WindowPlaces);

        /// <summary>
        /// Load setting file
        /// </summary>
        public void Load() => this.WindowPlaces = this.ConfigXml.LoadXml();

        /// <summary>
        /// Restore size and position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void Restore(Window window, string key)
        {
            Rectangle placement;
            if (this.WindowPlaces.TryGetValue(key, out placement))
            {
                new WindowInformation(new WindowInteropHelper(window).Handle).Relocate(placement);
            }
        }

        /// <summary>
        /// Restore only position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void RestorePosition(Window window, string key)
        {
            Rectangle place;
            if (this.WindowPlaces.TryGetValue(key, out place))
            {
                new WindowInformation(new WindowInteropHelper(window).Handle).Relocate(place.Left, place.Top);
            }
        }

        /// <summary>
        /// Store size and position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void Store(Window window, string key)
        {
            this.WindowPlaces[key] = new WindowInformation
                (new WindowInteropHelper(window).Handle).GetPlace();
        }


        /// <summary>
        /// Register the event that store/restore size and position of Window automatically
        /// </summary>
        /// <param name="window"></param>
        /// <param name="windowId"></param>
        public void Register(Window window, string windowId)
        {
            window.SourceInitialized += (o, e) => this.Restore(window, windowId);
            window.Closing += (o, e) =>
            {
                if (!e.Cancel)
                {
                    this.Store(window, windowId);
                }
            };
        }
    }
}
