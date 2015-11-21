using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace RestoreWindowPlace
{
    /// <summary>
    /// xmlファイルへのオブジェクトの保存と読み込みを行う
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class XmlSettingManager<T> where T : class, new()
    {
        static readonly string backUpNameHeader = "backup_";
        private string fileName;

        /// <summary>
        /// 保存するファイル名を指定してインスタンスを初期化
        /// </summary>
        /// <param name="fileName">xmlファイル名</param>
        public XmlSettingManager(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// オブジェクトをシリアライズしてxmlファイルに保存
        /// </summary>
        /// <param name="obj"></param>
        public void SaveXml(T obj)
        {

            if (obj == null)
            {
                throw new ArgumentException();
            }


            try
            {
                //ライターを生成
                using (var xw = XmlWriter.Create(this.fileName,
                    new XmlWriterSettings
                    {
                        Indent = true,
                        Encoding = new System.Text.UTF8Encoding(false)
                    }))
                {
                    //シリアライズして保存
                    var serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(xw, obj);
                    xw.Flush();
                }

            }
            catch
            {
                //例外はそのまま投げる
                throw;
            }
        }

        /// <summary>
        /// xmlファイルを読み込み
        /// ファイルが見つからなかった場合はオブジェクトをnewして返す
        /// 正常に読み込めたらそのファイルを自動でバックアップ
        /// </summary>
        /// <returns></returns>
        public LoadedObjectContainer<T> LoadXml()
        {
            return LoadXml(XmlLoadingOptions.UseBackup | XmlLoadingOptions.IgnoreNotFound);
        }

        /// <summary>
        /// xmlファイルを読み込み
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public LoadedObjectContainer<T> LoadXml(XmlLoadingOptions options)
        {


            Exception errorMessage = null;

            try
            {
                //ファイルから読み込み
                var loaded = this.LoadMain(this.fileName);

                //自動バックアップを使用する場合、正常に読み込めたファイルを別名でコピー
                if (options.HasFlag(XmlLoadingOptions.UseBackup) || options.HasFlag(XmlLoadingOptions.DoBackup))
                {
                    try
                    {
                        File.Copy(this.fileName, backUpNameHeader + this.fileName, true);
                    }
                    catch (Exception e)
                    {
                        return new LoadedObjectContainer<T>(loaded, e);
                    }
                }

                //コンテナに入れて返す
                return new LoadedObjectContainer<T>(loaded, null);
            }
            catch (FileNotFoundException)
            {
                //ファイルが存在しない場合

                if (options.HasFlag(XmlLoadingOptions.UseBackup))
                {
                    //バックアップを使用する設定の場合はスルー
                    //errorMessage = null;
                }
                else if (options.HasFlag(XmlLoadingOptions.IgnoreAllException)
                   || options.HasFlag(XmlLoadingOptions.IgnoreNotFound))
                {
                    //例外を無視する設定の場合はnewして返す
                    return new LoadedObjectContainer<T>(
                        (options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()),
                        null);
                }
                else
                {
                    //例外を投げる
                    throw;
                }
            }
            catch (Exception e)
            {
                //その他の例外

                if (options.HasFlag(XmlLoadingOptions.UseBackup))
                {
                    //例外を記憶
                    errorMessage = e;
                }
                else if (options.HasFlag(XmlLoadingOptions.IgnoreAllException))
                {
                    //例外を無視する設定の場合はnewして返す
                    return new LoadedObjectContainer<T>
                        ((options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()), e);
                }
                else
                {
                    //例外を投げる
                    throw;
                }
            }


            //バックアップを使用する設定の場合、バックアップファイルを読み込む
            return this.LoadBackupMain(options, errorMessage);
        }

        /// <summary>
        /// バックアップのxmlファイルを読み込む
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public LoadedObjectContainer<T> LoadBackupXml(XmlLoadingOptions options)
        {
            return this.LoadBackupMain(options, null);

        }

        private LoadedObjectContainer<T> LoadBackupMain(XmlLoadingOptions options, Exception errorMessage)
        {

            try
            {
                //バックアップファイルを読み込む
                var loaded = this.LoadMain(backUpNameHeader + fileName);

                return new LoadedObjectContainer<T>(loaded, errorMessage);
            }
            catch (FileNotFoundException)
            {
                //バックアップファイルも存在しなかった場合

                if (options.HasFlag(XmlLoadingOptions.IgnoreAllException)
                   || options.HasFlag(XmlLoadingOptions.IgnoreNotFound))
                {
                    //例外を無視する設定の場合はnewして返す
                    return new LoadedObjectContainer<T>
                        ((options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()), errorMessage);
                }
                else
                {
                    //例外を投げる
                    throw;
                }
            }
            catch (Exception e)
            {
                //その他の例外

                if (options.HasFlag(XmlLoadingOptions.IgnoreAllException))
                {
                    return new LoadedObjectContainer<T>
                        ((options.HasFlag(XmlLoadingOptions.ReturnNull) ? null : new T()), errorMessage ?? e);
                }
                else
                {
                    throw;
                }
            }
        }
        
        private T LoadMain(string name)
        {
            using (var xr = XmlReader.Create(name))
            {
                var serializer = new DataContractSerializer(typeof(T));
                var value = serializer.ReadObject(xr);

                return (T)value;
            }
        }


    }

    /// <summary>
    /// xmlファイルから読み込まれたオブジェクトと読み込み時に発生した例外の情報
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class LoadedObjectContainer<T>
    {
        public T Value { get; private set; }
        public Exception Message { get; private set; }

        public LoadedObjectContainer(T value, Exception message)
        {
            this.Value = value;
            this.Message = message;
        }
    }

    [Flags]
    internal enum XmlLoadingOptions
    {
        /// <summary>
        /// 発生した全ての例外を投げる
        /// </summary>
        ThrowAll = 0x00,

        /// <summary>
        /// バックアップファイルを使用する
        /// </summary>
        UseBackup = 0x01,

        /// <summary>
        /// FileNotFoundExceptionを内部で処理する
        /// </summary>
        IgnoreNotFound = 0x02,

        /// <summary>
        /// 全ての例外を内部で処理する
        /// </summary>
        IgnoreAllException = 0x04,

        /// <summary>
        /// ロードに失敗した場合はnullを返す
        /// </summary>
        ReturnNull = 0x08,

        /// <summary>
        /// バックアップを行うが読み込まない
        /// </summary>
        DoBackup = 0x10,
    }
}
