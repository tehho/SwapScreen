using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using SwapScreen;

namespace SwapScreenWithScreenCapture
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //FuckWithPeople();

            Reset();

        }


        public static void FuckWithPeople()
        {
            Display.MinimizeAll();
            Thread.Sleep(2000);

            var image = CaptureScreen();

            image.RotateFlip(RotateFlipType.Rotate180FlipNone);

            Display.SetImageAsBackground(image);


            MoveAllFromDesktop();


            Display.Rotate(1, Display.Orientations.DEGREES_CW_180);
            Display.SetWindowsTaskbar(Display.WindowsTaskbarSetting.SW_HIDE);

        }

        public static void Reset()
        {
            Display.Rotate(1, Display.Orientations.DEGREES_CW_0);

            Display.SetWindowsTaskbar(Display.WindowsTaskbarSetting.SW_SHOW);
        }


        public static void MoveAllFromDesktop()
        {
            string src = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string target = "C:/TMP/ICON/";

            Directory.CreateDirectory(target);

            Directory.GetFileSystemEntries(src).ToList().ForEach(file =>
            {
                var srcfile = file;
                var targetfile = target + Path.GetFileName(file);
                File.Move(srcfile, targetfile);
            });
        }


        public static Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        private static Image CaptureWindow(IntPtr handler)
        {
            IntPtr hdcScr = User32.GetWindowDC(handler);
            var windowRect = new User32.RECT();

            User32.GetWindowRect(handler, ref windowRect);

            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;

            var hdcDesc = Gdi32.CreateCompatibleDC(hdcScr);
            var hBitmap = Gdi32.CreateCompatibleBitmap(hdcScr, width, height);

            var hOld = Gdi32.SelectObject(hdcDesc, hBitmap);

            Gdi32.BitBlt(hdcDesc, 0, 0, width, height, hdcScr, 0, 0, Gdi32.SRCCOPY);

            Gdi32.SelectObject(hdcDesc, hOld);
            Gdi32.DeleteDC(hdcDesc);
            User32.ReleaseDC(handler, hdcScr);

            var img = Image.FromHbitmap(hBitmap);
            Gdi32.DeleteObject(hBitmap);
            return img;
        }

        private static void CaptureWindowToFile(IntPtr handle, string fileName, ImageFormat format)
        {
            var img = CaptureWindow(handle);
            img.Save(fileName, format);
        }

        private static void CaptureScreenToFile(string fileName, ImageFormat format)
        {
            var img = CaptureScreen();
            img.Save(fileName, format);
        }
    }
}
