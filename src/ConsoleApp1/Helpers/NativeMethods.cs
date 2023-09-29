#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleApp1.Helpers
{
  #region Delegates

  public delegate bool EnumWindowsProc(nint hWnd, nint lParam);

  public delegate nint HookProc(int nCode, nint wParam, nint lParam);

  #endregion Delegates

  public static partial class NativeMethods
  {
    #region user32.dll

    [DllImport("user32.dll")]
    public static extern bool AnimateWindow(nint hwnd, int time, AnimateWindowFlags flags);

    [DllImport("user32.dll")]
    public static extern bool BringWindowToTop(nint hWnd);

    [DllImport("user32.dll")]
    public static extern nint CopyIcon(nint hIcon);

    [DllImport("user32.dll")]
    public static extern nint DefWindowProc(nint hWnd, uint uMsg, nuint wParam, nint lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyIcon(nint hIcon);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, nint lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumChildWindows(nint hwndParent, EnumWindowsProc lpEnumFunc, nint lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpfn, nint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetClassName(nint hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(nint hWnd, ref Point lpPoint);

    [DllImport("user32.dll")]
    public static extern bool DrawIcon(nint hDC, int X, int Y, nint hIcon);

    [DllImport("user32.dll")]
    public static extern bool DrawIconEx(nint hdc, int xLeft, int yTop, nint hIcon, int cxWidth, int cyHeight, int istepIfAniCur, nint hbrFlickerFreeDraw, int diFlags);

    [DllImport("user32.dll")]
    public static extern nint FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern nint FindWindowEx(nint hwndParent, nint hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll")]
    public static extern nint FindWindowEx(nint parentHwnd, nint childAfterHwnd, nint className, string windowText);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

    [DllImport("user32.dll")]
    public static extern nint GetActiveWindow();

    [DllImport("user32.dll")]
    public static extern uint GetClassLong(nint hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern nint GetClassLongPtr(nint hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern bool GetClientRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool GetCursorInfo(out CursorInfo pci);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    public static extern nint GetDC(nint hWnd);

    [DllImport("user32.dll")]
    public static extern nint GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern nint GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool GetIconInfo(nint hIcon, out IconInfo piconinfo);

    [DllImport("user32.dll")]
    public static extern nint CreateIconIndirect([In] ref IconInfo piconinfo);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    public static extern short GetKeyState(int keyCode);

    [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    public static extern nint GetParent(nint hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowPlacement(nint hWnd, ref WINDOWPLACEMENT lpwndpl);

    /// <summary>
    /// The GetNextWindow function retrieves a handle to the next or previous window in the Z-Order.
    /// The next window is below the specified window; the previous window is above.
    /// If the specified window is a topmost window, the function retrieves a handle to the next (or previous) topmost window.
    /// If the specified window is a top-level window, the function retrieves a handle to the next (or previous) top-level window.
    /// If the specified window is a child window, the function searches for a handle to the next (or previous) child window.
    /// </summary>
    [DllImport("user32.dll")]
    public static extern nint GetWindow(nint hWnd, GetWindowConstants wCmd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetScrollInfo(nint hwnd, int fnBar, ref SCROLLINFO lpsi);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int smIndex);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(SystemMetric smIndex);

    [DllImport("user32.dll")]
    public static extern nint GetWindowDC(nint hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowLong")]
    public static extern nint GetWindowLong32(nint hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowLongPtr")]
    public static extern nint GetWindowLongPtr64(nint hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
    public static extern nint SetWindowLongPtr32(nint hWnd, int nIndex, nint dwNewLong);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
    public static extern nint SetWindowLongPtr64(nint hWnd, int nIndex, nint dwNewLong);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern int GetWindowRgn(nint hWnd, nint hRgn);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowText(nint hWnd, [Out] StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowTextLength(nint hWnd);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

    /// <summary>Determines the visibility state of the specified window.</summary>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(nint hWnd);

    /// <summary>Determines whether the specified window is minimized (iconic).</summary>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsIconic(nint hWnd);

    /// <summary>Determines whether a window is maximized.</summary>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsZoomed(nint hWnd);

    [DllImport("user32.dll")]
    public static extern nint LoadCursor(nint hInstance, int iconId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PrintWindow(nint hwnd, nint hDC, uint nFlags);

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture(nint hwnd);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(nint hWnd, nint hDC);

    [DllImport("user32.dll")]
    public static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

    [DllImport("user32.dll")]
    public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern nint SendMessage(nint hWnd, uint Msg, nint wParam, nint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern nint SendMessage(nint hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern nint SendMessage(nint hWnd, int Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern nint SendMessageTimeout(nint hWnd, uint Msg, nint wParam, nint lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out nuint lpdwResult);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern nint SendMessageTimeout(nint hWnd, int Msg, int wParam, int lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out nint lpdwResult);

    [DllImport("user32.dll")]
    public static extern nint SetActiveWindow(nint hWnd);

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(nint hWnd);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPlacement(nint hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern bool UpdateLayeredWindow(nint hwnd, nint hdcDst, ref POINT pptDst, ref SIZE psize, nint hdcSrc, ref POINT pptSrc, uint crKey, [In] ref BLENDFUNCTION pblend, uint dwFlags);

    /// <summary> The RegisterHotKey function defines a system-wide hot key </summary>
    /// <param name="hWnd">Handle to the window that will receive WM_HOTKEY messages generated by the hot key.</param>
    /// <param name="id">Specifies the identifier of the hot key.</param>
    /// <param name="fsModifiers">Specifies keys that must be pressed in combination with the key
    /// specified by the 'vk' parameter in order to generate the WM_HOTKEY message.</param>
    /// <param name="vk">Specifies the virtual-key code of the hot key</param>
    /// <returns><c>true</c> if the function succeeds, otherwise <c>false</c></returns>
    /// <seealso cref="http://msdn.microsoft.com/en-us/library/ms646309(VS.85).aspx"/>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterHotKey(nint hWnd, int id);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowInfo(nint hwnd, ref WINDOWINFO pwi);

    [DllImport("user32.dll")]
    public static extern nint SetWindowsHookEx(int idHook, HookProc lpfn, nint hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll")]
    public static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    #endregion user32.dll

    #region kernel32.dll

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes,
        bool bInheritHandles, uint dwCreationFlags, nint lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

    [DllImport("kernel32.dll")]
    public static extern int ResumeThread(nint hThread);

    [DllImport("kernel32.dll")]
    public static extern int SuspendThread(nint hThread);

    [DllImport("kernel32.dll")]
    public static extern nint OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern bool SetProcessWorkingSetSize(nint handle, nint min, nint max);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern ushort GlobalAddAtom(string lpString);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern ushort GlobalDeleteAtom(ushort nAtom);

    [DllImport("kernel32.dll")]
    public static extern nint GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWow64Process([In] nint hProcess, [Out] out bool lpSystemInfo);

    [DllImport("kernel32.dll", PreserveSig = false)]
    public static extern void RegisterApplicationRestart(string pwzCommandline, RegisterApplicationRestartFlags dwFlags);

    #endregion kernel32.dll

    #region gdi32.dll



    [DllImport("gdi32.dll")]
    public static extern nint CreateCompatibleBitmap(nint hdc, int nWidth, int nHeight);

    [DllImport("gdi32.dll")]
    public static extern nint CreateCompatibleDC(nint hdc);

    [DllImport("gdi32.dll")]
    public static extern nint CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, nint lpInitData);

    [DllImport("gdi32.dll")]
    public static extern nint CreateRectRgn(int nLeftRect, int nTopRect, int nReghtRect, int nBottomRect);

    [DllImport("gdi32.dll")]
    public static extern nint CreateRoundRectRgn(int nLeftRect, int nTopRect, int nReghtRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteDC(nint hDC);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(nint hObject);

    [DllImport("gdi32.dll")]
    public static extern nint SelectObject(nint hdc, nint hgdiobj);

    [DllImport("gdi32.dll")]
    public static extern nint CreateDIBSection(nint hdc, [In] ref BITMAPINFOHEADER pbmi, uint pila, out nint ppvBits, nint hSection, uint dwOffset);

    [DllImport("gdi32.dll")]
    public static extern int GetDeviceCaps(nint hdc, int nIndex);

    [DllImport("gdi32.dll")]
    public static extern uint GetPixel(nint hdc, int nXPos, int nYPos);

    #endregion gdi32.dll

    #region gdiplus.dll

    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int GdipGetImageType(HandleRef image, out int type);

    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int GdipImageForceValidation(HandleRef image);

    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int GdipLoadImageFromFile(string filename, out nint image);

    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern int GdipDisposeImage(HandleRef image);

    [DllImport("gdiplus.dll")]
    public static extern int GdipWindingModeOutline(HandleRef path, nint matrix, float flatness);

    #endregion gdiplus.dll

    #region shell32.dll

    [DllImport("shell32.dll")]
    public static extern nint SHAppBarMessage(uint dwMessage, [In] ref APPBARDATA pData);

    [DllImport("shell32.dll", EntryPoint = "#727")]
    public extern static int SHGetImageList(int iImageList, ref Guid riid, ref IImageList ppv);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern nint SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

    [DllImport("shell32.dll")]
    public static extern int SHOpenFolderAndSelectItems(nint pidlFolder, int cild, nint apidl, int dwFlags);

    [DllImport("shell32.dll")]
    public static extern void SHChangeNotify(HChangeNotifyEventID wEventId, HChangeNotifyFlags uFlags, nint dwItem1, nint dwItem2);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern nint ILCreateFromPathW(string pszPath);

    [DllImport("shell32.dll")]
    public static extern void ILFree(nint pidl);

    #endregion shell32.dll

    #region dwmapi.dll

    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(nint hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(nint hwnd, int dwAttribute, out bool pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(nint hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll")]
    public static extern void DwmEnableBlurBehindWindow(nint hwnd, ref DWM_BLURBEHIND blurBehind);

    [DllImport("dwmapi.dll", PreserveSig = false)]
    public static extern void DwmEnableComposition(CompositionAction uCompositionAction);

    [DllImport("dwmapi.dll")]
    public static extern int DwmExtendFrameIntoClientArea(nint hwnd, ref MARGINS margins);

    [DllImport("dwmapi.dll", PreserveSig = false)]
    public static extern bool DwmIsCompositionEnabled();

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(nint hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("dwmapi.dll")]
    public static extern int DwmQueryThumbnailSourceSize(nint thumb, out SIZE size);

    [DllImport("dwmapi.dll")]
    public static extern int DwmRegisterThumbnail(nint dest, nint src, out nint thumb);

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetDxFrameDuration(nint hwnd, uint cRefreshes);

    [DllImport("dwmapi.dll")]
    public static extern int DwmUnregisterThumbnail(nint thumb);

    [DllImport("dwmapi.dll")]
    public static extern int DwmUpdateThumbnailProperties(nint hThumb, ref DWM_THUMBNAIL_PROPERTIES props);

    #endregion dwmapi.dll

    #region winmm.dll

    [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
    public static extern uint TimeBeginPeriod(uint uMilliseconds);

    [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
    public static extern uint TimeEndPeriod(uint uMilliseconds);

    [DllImport("winmm.dll", EntryPoint = "timeGetDevCaps")]
    public static extern uint TimeGetDevCaps(ref TimeCaps timeCaps, uint sizeTimeCaps);

    #endregion

    #region Other dll

    [DllImport("msvcrt.dll")]
    public static extern int memcmp(nint b1, nint b2, long count);

    /// <summary>
    /// Copy a block of memory.
    /// </summary>
    ///
    /// <param name="dst">Destination pointer.</param>
    /// <param name="src">Source pointer.</param>
    /// <param name="count">Memory block's length to copy.</param>
    ///
    /// <returns>Return's the value of <b>dst</b> - pointer to destination.</returns>
    [DllImport("ntdll.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int memcpy(int dst, int src, int count);

    /// <summary>
    /// Initialize the AVIFile library.
    /// </summary>
    [DllImport("avifil32.dll")]
    public static extern void AVIFileInit();

    /// <summary>
    /// Exit the AVIFile library.
    /// </summary>
    [DllImport("avifil32.dll")]
    public static extern void AVIFileExit();

    /// <summary>
    /// Open an AVI file.
    /// </summary>
    ///
    /// <param name="aviHandler">Opened AVI file interface.</param>
    /// <param name="fileName">AVI file name.</param>
    /// <param name="mode">Opening mode (see <see cref="OpenFileMode"/>).</param>
    /// <param name="handler">Handler to use (<b>null</b> to use default).</param>
    ///
    /// <returns>Returns zero on success or error code otherwise.</returns>
    [DllImport("avifil32.dll", CharSet = CharSet.Unicode)]
    public static extern int AVIFileOpen(out nint aviHandler, string fileName, OpenFileMode mode, nint handler);

    /// <summary>
    /// Release an open AVI stream.
    /// </summary>
    ///
    /// <param name="aviHandler">Open AVI file interface.</param>
    ///
    /// <returns>Returns the reference count of the file.</returns>
    [DllImport("avifil32.dll")]
    public static extern int AVIFileRelease(nint aviHandler);

    /// <summary>
    /// Get stream interface that is associated with a specified AVI file
    /// </summary>
    ///
    /// <param name="aviHandler">Handler to an open AVI file.</param>
    /// <param name="streamHandler">Stream interface.</param>
    /// <param name="streamType">Stream type to open.</param>
    /// <param name="streamNumner">Count of the stream type. Identifies which occurrence of the specified stream type to access. </param>
    ///
    /// <returns></returns>
    [DllImport("avifil32.dll")]
    public static extern int AVIFileGetStream(nint aviHandler, out nint streamHandler, int streamType, int streamNumner);

    /// <summary>
    /// Create a new stream in an existing file and creates an interface to the new stream.
    /// </summary>
    ///
    /// <param name="aviHandler">Handler to an open AVI file.</param>
    /// <param name="streamHandler">Stream interface.</param>
    /// <param name="streamInfo">Pointer to a structure containing information about the new stream.</param>
    ///
    /// <returns>Returns zero if successful or an error otherwise.</returns>
    [DllImport("avifil32.dll")]
    public static extern int AVIFileCreateStream(nint aviHandler, out nint streamHandler, ref AVISTREAMINFO streamInfo);

    /// <summary>
    /// Release an open AVI stream.
    /// </summary>
    ///
    /// <param name="streamHandler">Handle to an open stream.</param>
    ///
    /// <returns>Returns the current reference count of the stream.</returns>
    [DllImport("avifil32.dll")]
    public static extern int AVIStreamRelease(nint streamHandler);

    /// <summary>
    /// Set the format of a stream at the specified position.
    /// </summary>
    ///
    /// <param name="streamHandler">Handle to an open stream.</param>
    /// <param name="position">Position in the stream to receive the format.</param>
    /// <param name="format">Pointer to a structure containing the new format.</param>
    /// <param name="formatSize">Size, in bytes, of the block of memory referenced by <b>format</b>.</param>
    ///
    /// <returns>Returns zero if successful or an error otherwise.</returns>
    [DllImport("avifil32.dll")]
    public static extern int AVIStreamSetFormat(nint streamHandler, int position, ref BITMAPINFOHEADER format, int formatSize);

    /// <summary>
    /// Get the starting sample number for the stream.
    /// </summary>
    ///
    /// <param name="streamHandler">Handle to an open stream.</param>
    ///
    /// <returns>Returns the number if successful or  1 otherwise.</returns>
    [DllImport("avifil32.dll")]
    public static extern int AVIStreamStart(nint streamHandler);

    /// <summary>
    /// Get the length of the stream.
    /// </summary>
    ///
    /// <param name="streamHandler">Handle to an open stream.</param>
    ///
    /// <returns>Returns the stream's length, in samples, if successful or -1 otherwise.</returns>
    [DllImport("avifil32.dll")]
    public static extern int AVIStreamLength(nint streamHandler);

    /// <summary>
    /// Obtain stream header information.
    /// </summary>
    ///
    /// <param name="streamHandler">Handle to an open stream.</param>
    /// <param name="streamInfo">Pointer to a structure to contain the stream information.</param>
    /// <param name="infoSize">Size, in bytes, of the structure used for <b>streamInfo</b>.</param>
    ///
    /// <returns>Returns zero if successful or an error otherwise.</returns>
    [DllImport("avifil32.dll", CharSet = CharSet.Unicode)]
    public static extern int AVIStreamInfo(nint streamHandler, ref AVISTREAMINFO streamInfo, int infoSize);

    /// <summary>
    /// Prepare to decompress video frames from the specified video stream
    /// </summary>
    ///
    /// <param name="streamHandler">Pointer to the video stream used as the video source.</param>
    /// <param name="wantedFormat">Pointer to a structure that defines the desired video format. Specify NULL to use a default format.</param>
    ///
    /// <returns>Returns an object that can be used with the <see cref="AVIStreamGetFrame"/> function.</returns>
    [DllImport("avifil32.dll")]
    public static extern nint AVIStreamGetFrameOpen(nint streamHandler, ref BITMAPINFOHEADER wantedFormat);

    /// <summary>
    /// Prepare to decompress video frames from the specified video stream.
    /// </summary>
    ///
    /// <param name="streamHandler">Pointer to the video stream used as the video source.</param>
    /// <param name="wantedFormat">Pointer to a structure that defines the desired video format. Specify NULL to use a default format.</param>
    ///
    /// <returns>Returns a <b>GetFrame</b> object that can be used with the <see cref="AVIStreamGetFrame"/> function.</returns>
    [DllImport("avifil32.dll")]
    public static extern nint AVIStreamGetFrameOpen(nint streamHandler, int wantedFormat);

    /// <summary>
    /// Releases resources used to decompress video frames.
    /// </summary>
    ///
    /// <param name="getFrameObject">Handle returned from the <see cref="AVIStreamGetFrameOpen(nint,int)"/> function.</param>
    ///
    /// <returns>Returns zero if successful or an error otherwise.</returns>
    [DllImport("avifil32.dll")]
    public static extern int AVIStreamGetFrameClose(nint getFrameObject);

    /// <summary>
    /// Return the address of a decompressed video frame.
    /// </summary>
    ///
    /// <param name="getFrameObject">Pointer to a GetFrame object.</param>
    /// <param name="position">Position, in samples, within the stream of the desired frame.</param>
    ///
    /// <returns>Returns a pointer to the frame data if successful or NULL otherwise.</returns>
    [DllImport("avifil32.dll")]
    public static extern nint AVIStreamGetFrame(nint getFrameObject, int position);

    /// <summary>
    /// Write data to a stream.
    /// </summary>
    ///
    /// <param name="streamHandler">Handle to an open stream.</param>
    /// <param name="start">First sample to write.</param>
    /// <param name="samples">Number of samples to write.</param>
    /// <param name="buffer">Pointer to a buffer containing the data to write. </param>
    /// <param name="bufferSize">Size of the buffer referenced by <b>buffer</b>.</param>
    /// <param name="flags">Flag associated with this data.</param>
    /// <param name="samplesWritten">Pointer to a buffer that receives the number of samples written. This can be set to NULL.</param>
    /// <param name="bytesWritten">Pointer to a buffer that receives the number of bytes written. This can be set to NULL.</param>
    ///
    /// <returns>Returns zero if successful or an error otherwise.</returns>
    [DllImport("avifil32.dll")]
    public static extern int AVIStreamWrite(nint streamHandler, int start, int samples, nint buffer, int bufferSize, int flags, nint samplesWritten,
        nint bytesWritten);

    /// <summary>
    /// Retrieve the save options for a file and returns them in a buffer.
    /// </summary>
    ///
    /// <param name="window">Handle to the parent window for the Compression Options dialog box.</param>
    /// <param name="flags">Flags for displaying the Compression Options dialog box.</param>
    /// <param name="streams">Number of streams that have their options set by the dialog box.</param>
    /// <param name="streamInterfaces">Pointer to an array of stream interface pointers.</param>
    /// <param name="options">Pointer to an array of pointers to AVICOMPRESSOPTIONS structures.</param>
    ///
    /// <returns>Returns TRUE if the user pressed OK, FALSE for CANCEL, or an error otherwise.</returns>
    [DllImport("avifil32.dll")]
    public static extern int AVISaveOptions(nint window, int flags, int streams, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] nint[] streamInterfaces,
        [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] nint[] options);

    /// <summary>
    /// Free the resources allocated by the AVISaveOptions function.
    /// </summary>
    ///
    /// <param name="streams">Count of the AVICOMPRESSOPTIONS structures referenced in <b>options</b>.</param>
    /// <param name="options">Pointer to an array of pointers to AVICOMPRESSOPTIONS structures.</param>
    ///
    /// <returns>Returns 0.</returns>
    [DllImport("avifil32.dll")]
    public static extern int AVISaveOptionsFree(int streams, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] nint[] options);

    /// <summary>
    /// Create a compressed stream from an uncompressed stream and a
    /// compression filter, and returns the address of a pointer to
    /// the compressed stream.
    /// </summary>
    ///
    /// <param name="compressedStream">Pointer to a buffer that receives the compressed stream pointer.</param>
    /// <param name="sourceStream">Pointer to the stream to be compressed.</param>
    /// <param name="options">Pointer to a structure that identifies the type of compression to use and the options to apply.</param>
    /// <param name="clsidHandler">Pointer to a class identifier used to create the stream.</param>
    ///
    /// <returns>Returns 0 if successful or an error otherwise.</returns>
    [DllImport("avifil32.dll")]
    public static extern int AVIMakeCompressedStream(out nint compressedStream, nint sourceStream, ref AVICOMPRESSOPTIONS options, nint clsidHandler);

    [DllImport("dnsapi.dll")]
    public static extern uint DnsFlushResolverCache();

    #endregion Other dll
  }
}