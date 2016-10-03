using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace RestoreWindowPlace
{
    /// <summary>
    /// Save and load object to/from XML file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class XmlSettingManager<T> where T : class, new()
    {
        private string filePath;

        /// <summary>
        /// Set file name
        /// </summary>
        /// <param name="filePath">name of XML file</param>
        public XmlSettingManager(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Serialize object and save to XML file
        /// </summary>
        /// <param name="obj"></param>
        public void SaveXml(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentException();
            }

            var setting = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new System.Text.UTF8Encoding(false)
            };

            //ライターを生成
            using (var xw = XmlWriter.Create(this.filePath, setting))
            {
                //シリアライズして保存
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(xw, obj);
                xw.Flush();
            }
        }
        
        /// <summary>
        /// Load XML file
        /// </summary>
        /// <returns></returns>
        public T LoadXml()
        {
            try
            {
                using (var xr = XmlReader.Create(this.filePath))
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    var value = serializer.ReadObject(xr);

                    return (T)value;
                }
            }
            catch
            {
                //newして返す
                return new T();
            }
        }
    }
}
