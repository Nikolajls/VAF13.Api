using System.Runtime.InteropServices;
using VAF13.Klubadmin.Domain.Helpers;

namespace VAF13.Klubadmin.Domain.Services.Skywin;

public class WindowsApiService : IWindowsApiService
{
    private const int WM_SETTEXT = 0x000C;
    private const int CB_SELECTSTRING = 0x014d;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern nint SendMessage(nint hWnd, uint Msg, nint wParam, string lParam);

    public IntPtr FindWindow(string? lpClassName, string? lpWindowName)
    {
        nint membersHandle = NativeMethods.FindWindow(null, "Members");
        return membersHandle;
    }

    public List<IntPtr> GetAllChildHandles(IntPtr windowHandle)
    {
        return new WindowHandleInfo(windowHandle).GetAllChildHandles();
    }

    public void SendMessage_SetText(IntPtr controlPtr, string text)
    {
        SendMessage(controlPtr, (uint)WindowsMessages.SETTEXT, nint.Zero, text.Trim());
    }

    public void SendMessage_ComboSelect(IntPtr controlPtr, string text)
    {
        SendMessage(controlPtr, CB_SELECTSTRING, nint.Zero, text.Trim());
    }
}