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
        /// <summary>
        /// true: If the window is snapped, save its position as its actual position.
        /// false: Save the original position whether the window is snapped.
        /// </summary>
        public bool IsSavingSnappedPositionEnabled { get; set; } = false;

        private readonly XmlSettingManager<Dictionary<string, Rectangle>> configXml;
        private Dictionary<string, Rectangle> windowPlaces;

        /// <summary>
        /// Save size and position of window to file
        /// </summary>
        /// <param name="filePath">Name or path of XML file to save</param>
        public WindowPlace(string filePath)
        {
            this.configXml = new XmlSettingManager<Dictionary<string, Rectangle>>(filePath);
            this.Load();
        }

        /// <summary>
        /// Save setting file
        /// </summary>
        public void Save()
        {
            if (this.windowPlaces != null)
            {
                this.configXml.SaveXml(this.windowPlaces);
            }
        }

        /// <summary>
        /// Load setting file
        /// </summary>
        public void Load()
        {
            this.windowPlaces = this.LoadInner();
        }
        private Dictionary<string, Rectangle> LoadInner()
        {
            try
            {
                return this.configXml.LoadXml();
            }
            catch
            {
            }
            return new Dictionary<string, Rectangle>();
        }

        /// <summary>
        /// Restore size and position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void Restore(Window window, string key)
        {
            if (this.windowPlaces.TryGetValue(key, out var place))
            {
                WindowRelocate.Relocate(new WindowInteropHelper(window).Handle, place);
            }
        }

        /// <summary>
        /// Restore only position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void RestorePosition(Window window, string key)
        {
            if (this.windowPlaces.TryGetValue(key, out var place))
            {
                WindowRelocate.Relocate(new WindowInteropHelper(window).Handle, place.Left, place.Top);
            }
        }

        /// <summary>
        /// Store size and position
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void Store(Window window, string key)
        {
            this.windowPlaces[key] = WindowRelocate.GetPlace(new WindowInteropHelper(window).Handle,
                this.IsSavingSnappedPositionEnabled);
        }


        /// <summary>
        /// Register the event that store/restore size and position of Window automatically
        /// using unique ID
        /// </summary>
        /// <param name="window">target window</param>
        /// <param name="windowId">Unique ID associated with the window</param>
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

        /// <summary>
        /// Register the event that store/restore size and position of Window automatically
        /// using the name of the type extends Window class
        /// </summary>
        /// <param name="window">target window</param>
        public void Register<T>(T window) where T : Window
        {
            Register(window, typeof(T).Name);
        }

        /// <summary>
        /// Register the event that store/restore position of Window automatically
        /// using unique ID
        /// </summary>
        /// <param name="window">target window</param>
        /// <param name="windowId">Unique ID associated with the window</param>
        /// <remarks>The size of the window is not restored.</remarks>
        public void RegisterPositionOnly(Window window, string windowId)
        {
            window.SourceInitialized += (o, e) => this.RestorePosition(window, windowId);
            window.Closing += (o, e) =>
            {
                if (!e.Cancel)
                {
                    this.Store(window, windowId);
                }
            };
        }

        /// <summary>
        /// Register the event that store/restore position of Window automatically
        /// using the name of the type extends Window class
        /// </summary>
        /// <param name="window">target window</param>
        /// <remarks>The size of the window is not restored.</remarks>
        public void RegisterPositionOnly<T>(T window) where T : Window
        {
            RegisterPositionOnly(window, typeof(T).Name);
        }
        /// <summary>
        /// Checks if key is registered.
        /// </summary>
        /// <param name="key"></param>
        public bool IsRegistered(string key)
        {
            return this.windowPlaces.ContainsKey(key);
        }

    }
}
