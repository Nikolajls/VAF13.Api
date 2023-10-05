using System.Runtime.InteropServices;

namespace VAF13.Klubadmin.Domain.Helpers;

public class WindowHandleInfo
{
    private delegate bool EnumWindowProc(nint hwnd, nint lParam);

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumChildWindows(nint window, EnumWindowProc callback, nint lParam);

    private nint _MainHandle;

    public WindowHandleInfo(nint handle)
    {
        _MainHandle = handle;
    }

    public List<nint> GetAllChildHandles()
    {
        List<nint> childHandles = new List<nint>();

        GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
        nint pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

        try
        {
            EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
            EnumChildWindows(_MainHandle, childProc, pointerChildHandlesList);
        }
        finally
        {
            gcChildhandlesList.Free();
        }

        return childHandles;
    }

    private bool EnumWindow(nint hWnd, nint lParam)
    {
        GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

        if (gcChildhandlesList == null || gcChildhandlesList.Target == null)
        {
            return false;
        }

        List<nint> childHandles = gcChildhandlesList.Target as List<nint>;
        childHandles.Add(hWnd);

        return true;
    }
}