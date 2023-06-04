using System;

namespace FileRenamer.WindowsAPI.PInvoke.DwmApi
{
    [Flags]
    public enum DwmWindowAttribute : uint
    {
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
        DWMWA_MICA_EFFECT = 1029,
        DWMWA_SYSTEMBACKDROP_TYPE = 38
    }
}
