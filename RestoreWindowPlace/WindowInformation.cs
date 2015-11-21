using System;

namespace RestoreWindowPlace
{
    /// <summary>
    /// ウインドウの情報
    /// </summary>
    internal class WindowInformation
    {
        public string Title { get; set; }
        public IntPtr Handle { get; set; }
        public int Id { get; set; }

        public WindowInformation()
        {

        }

        public WindowInformation(IntPtr handle)
        {
            this.Handle = handle;
        }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
