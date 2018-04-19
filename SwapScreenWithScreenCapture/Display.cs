using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SwapScreen
{




    public class Display
    {

        public enum WindowsTaskbarSetting
        {
            SW_HIDE = 0,
            SW_SHOW = 1
        }


        public enum WallpaperSettings
        {
            SPI_SETDESKWALLPAPER = 20,
            SPIF_UPDATEINIFILE = 0x01,
            SPIF_SENDWININICHANGE = 0x02
        }

        public enum Minimize
        {
            WM_COMMAND = 0x111,
            MIN_ALL = 419,
            MIN_ALL_UNDO = 416
        }

        public enum Orientations
        {
            DEGREES_CW_0 = 0,
            DEGREES_CW_90 = 3,
            DEGREES_CW_180 = 2,
            DEGREES_CW_270 = 1
        }

        public static void SetWindowsTaskbar(WindowsTaskbarSetting setting)
        {
            var handle = User32.FindWindow("Shell_TrayWnd", "");
            User32.ShowWindow((int)handle, (int)setting);
        }

        public static void SetImageAsBackground(Image img)
        {
            img.Save("C:/TMP/test.png");

            User32.SystemParametersInfo((int)WallpaperSettings.SPI_SETDESKWALLPAPER,
                0,
                "C:/TMP/test.png",
                (int)WallpaperSettings.SPIF_UPDATEINIFILE | (int)WallpaperSettings.SPIF_SENDWININICHANGE);

        }

        public static bool MinimizeAll()
        {
            IntPtr hwnd = User32.FindWindow("Shell_TrayWnd", null);
            User32.SendMessage(hwnd, (int)Minimize.WM_COMMAND, (IntPtr)Minimize.MIN_ALL, IntPtr.Zero);
            return true;
        }

        public static bool MinimizeWindow(string name)
        {

            return true;
        }

        public static bool Rotate(uint DisplayNumber, Orientations Orientation)
        {
            if (DisplayNumber == 0)
                throw new ArgumentOutOfRangeException("DisplayNumber", DisplayNumber, "First display is 1.");

            bool result = false;
            DISPLAY_DEVICE d = new DISPLAY_DEVICE();
            DEVMODE dm = new DEVMODE();
            d.cb = Marshal.SizeOf(d);

            if (!NativeMethods.EnumDisplayDevices(null, DisplayNumber - 1, ref d, 0))
                throw new ArgumentOutOfRangeException("DisplayNumber", DisplayNumber, "Number is greater than connected displays.");

            var hresult = NativeMethods.EnumDisplaySettings(
                d.DeviceName, NativeMethods.ENUM_CURRENT_SETTINGS, ref dm);

            if (0 != hresult)
            {
                if ((dm.dmDisplayOrientation + (int)Orientation) % 2 == 1) // Need to swap height and width?
                {
                    int temp = dm.dmPelsHeight;
                    dm.dmPelsHeight = dm.dmPelsWidth;
                    dm.dmPelsWidth = temp;
                }

                switch (Orientation)
                {
                    case Orientations.DEGREES_CW_90:
                        dm.dmDisplayOrientation = NativeMethods.DMDO_270;
                        break;
                    case Orientations.DEGREES_CW_180:
                        dm.dmDisplayOrientation = NativeMethods.DMDO_180;
                        break;
                    case Orientations.DEGREES_CW_270:
                        dm.dmDisplayOrientation = NativeMethods.DMDO_90;
                        break;
                    case Orientations.DEGREES_CW_0:
                        dm.dmDisplayOrientation = NativeMethods.DMDO_DEFAULT;
                        break;
                    default:
                        break;
                }

                DISP_CHANGE ret = NativeMethods.ChangeDisplaySettingsEx(
                    d.DeviceName, ref dm, IntPtr.Zero,
                    DisplaySettingsFlags.CDS_UPDATEREGISTRY, IntPtr.Zero);

                result = ret == 0;
            }

            return result;
        }

        public static void ResetAllRotations()
        {
            try
            {
                uint i = 0;
                while (++i <= 64)
                {
                    Rotate(i, Orientations.DEGREES_CW_0);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // Everything is fine, just reached the last display
            }
        }
    }
}