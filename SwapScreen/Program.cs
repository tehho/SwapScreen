using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Net.Mime;

namespace SwapScreen
{
    class Program
    {
        static void Main(string[] args)
        {
            
            RotateWindows();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            Display.ResetAllRotations();
        }

        


        private static void RotateWindows()
        {
            try
            {
                int i = 0;
                while (++i < 64)
                {
                    var rest = Display.Rotate((uint)i, Display.Orientations.DEGREES_CW_180);
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}