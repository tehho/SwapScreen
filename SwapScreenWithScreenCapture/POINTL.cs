using System.Runtime.InteropServices;

namespace SwapScreen
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINTL
    {
        long x;
        long y;
    }
}