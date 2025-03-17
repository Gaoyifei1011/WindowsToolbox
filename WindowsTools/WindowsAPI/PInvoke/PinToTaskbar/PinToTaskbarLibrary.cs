using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace WindowsTools.WindowsAPI.PInvoke.PinToTaskbar
{
    public static class PinToTaskbarLibrary
    {
        private const string PinToTaskbar = "pinToTaskbar.dll";

        [DllImport(PinToTaskbar, CharSet = CharSet.Unicode, EntryPoint = "StartHook", PreserveSig = true, SetLastError = false)]
        public static extern void StartHook(uint threadId);

        [DllImport(PinToTaskbar, CharSet = CharSet.Unicode, EntryPoint = "StopHook", PreserveSig = true, SetLastError = false)]
        public static extern void StopHook();
    }
}
