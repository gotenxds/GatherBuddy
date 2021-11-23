using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherBuddy.Utility
{
    public static class KeyboardUtil
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        private static Process[] processes = Process.GetProcessesByName("ffxiv_dx11");

        public static void Press(int key) {
            PostMessage(processes[0].MainWindowHandle, 0x0100, key, 0);
            PostMessage(processes[0].MainWindowHandle, 0x0101, key, 0);
        }
    }
}
