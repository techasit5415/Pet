using System.Runtime.InteropServices;
namespace DesktopPet;
internal static class Native {
    [StructLayout(LayoutKind.Sequential)] internal struct Point { public int X,Y; }
    [StructLayout(LayoutKind.Sequential)] internal struct Rect { public int Left,Top,Right,Bottom; }
    [StructLayout(LayoutKind.Sequential)] struct Info { public int Size; public Rect Monitor,Work; public uint Flags; }
    delegate bool MonitorProc(IntPtr monitor, IntPtr dc, ref Rect bounds, IntPtr data);
    [DllImport("user32.dll")] static extern bool EnumDisplayMonitors(IntPtr dc, IntPtr clip, MonitorProc callback, IntPtr data);
    [DllImport("user32.dll", EntryPoint="GetMonitorInfoW")] static extern bool GetMonitorInfo(IntPtr monitor, ref Info info);
    [DllImport("user32.dll")] internal static extern bool GetCursorPos(out Point point);
    [DllImport("user32.dll")] internal static extern bool GetWindowRect(IntPtr window, out Rect rect);
    [DllImport("user32.dll")] internal static extern bool SetWindowPos(IntPtr window, IntPtr after, int x,int y,int w,int h,uint flags);
    internal static List<Rect> WorkAreas() {
        var list = new List<Rect>();
        EnumDisplayMonitors(IntPtr.Zero,IntPtr.Zero,(IntPtr m,IntPtr d,ref Rect r,IntPtr p)=> {
            var info = new Info { Size = Marshal.SizeOf<Info>() };
            if (GetMonitorInfo(m,ref info)) list.Add(info.Work);
            return true;
        },IntPtr.Zero);
        return list;
    }
}
