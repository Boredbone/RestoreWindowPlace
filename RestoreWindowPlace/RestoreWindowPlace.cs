using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;

namespace RestoreWindowPlace
{
    /// <summary>
    /// Windowの位置とサイズをファイルに保存
    /// </summary>
    public class RestoreWindowPlace
    {
        //private string configFileName;
        private XmlSettingManager<Dictionary<string, Rectangle>> ConfigXml { get; }
        private Dictionary<string, Rectangle> WindowPlaces { get; set; }


        /// <summary>
        /// Windowの位置とサイズをファイルに保存
        /// </summary>
        /// <param name="filePath">保存するxmlファイルの名前またはパス</param>
        public RestoreWindowPlace(string filePath)
        {
            //this.configFileName = filePath;

            var directory = System.IO.Path.GetDirectoryName(filePath);
            var fileName = System.IO.Path.GetFileName(filePath);

            this.WindowPlaces = new Dictionary<string, Rectangle>();

            this.ConfigXml = new XmlSettingManager<Dictionary<string, Rectangle>>
                (fileName) { Directory = directory };

            this.Load();
        }

        /// <summary>
        /// 設定ファイルを保存
        /// </summary>
        public void Save()
        {
            this.ConfigXml.SaveXml(this.WindowPlaces);
        }

        /// <summary>
        /// 設定ファイルを読み込み
        /// </summary>
        public void Load()
        {
            this.WindowPlaces = this.ConfigXml.LoadXml(XmlLoadingOptions.IgnoreAllException).Value;
        }

        /// <summary>
        /// 位置とサイズを復元
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
        /// サイズを変更せず、位置のみを復元
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
        /// 位置とサイズを保存
        /// </summary>
        /// <param name="window"></param>
        /// <param name="key"></param>
        public void Store(Window window, string key)
        {
            var place = new WindowInformation(new WindowInteropHelper(window).Handle).GetPlace();
            this.WindowPlaces[key] = place;
        }


        /// <summary>
        /// Windowの位置とサイズを自動で保存・復元するイベントを登録
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
