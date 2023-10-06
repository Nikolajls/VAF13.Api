namespace VAF13.Klubadmin.Domain.Services.Skywin;

public interface IWindowsApiService
{
    nint FindWindow(string? lpClassName, string? lpWindowName);
    List<IntPtr> GetAllChildHandles(nint windowHandle);
    void SendMessage_SetText(nint controlPtr, string text);
    void SendMessage_ComboSelect(nint controlPtr, string text);
}