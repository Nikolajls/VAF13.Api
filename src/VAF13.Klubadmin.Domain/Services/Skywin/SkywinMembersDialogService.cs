using System.Runtime.InteropServices;
using System.Text;
using VAF13.Klubadmin.Domain.DTOs;
using VAF13.Klubadmin.Domain.Helpers;

namespace VAF13.Klubadmin.Domain.Services.Skywin;

public class SkywinMembersDialogService : ISkywinMembersDialogService
{
    private const int WM_SETTEXT = 0x000C;
    private const int CB_SELECTSTRING = 0x014d;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern nint SendMessage(nint hWnd, uint Msg, nint wParam, string lParam);

    public void InsertData(PersonDetails person)
    {
        nint membersHandle = NativeMethods.FindWindow(null, "Members");
        if (membersHandle == nint.Zero)
        {
            Console.WriteLine("No members dialog found");
            return;
        }


        var dx = new WindowHandleInfo(membersHandle).GetAllChildHandles();
        var inputFields = new Dictionary<int, Win32CustomControl>();
        var comboBoxes = new Dictionary<int, Win32CustomControl>();
        int inputFieldsIdx = 1;
        int comboBoxesIdx = 1;
        foreach (var p in dx)
        {
            var sb = new StringBuilder();
            NativeMethods.GetClassName(p, sb, 100);
            var classNameOfCtrl = sb.ToString();
            if (classNameOfCtrl == "ThunderRT6TextBox")
            {
                inputFields[inputFieldsIdx] = new Win32CustomControl(classNameOfCtrl, p);
                inputFieldsIdx++;
            }
            else if (classNameOfCtrl == "ThunderRT6ComboBox")
            {
                comboBoxes[comboBoxesIdx] = new Win32CustomControl(classNameOfCtrl, p);
                comboBoxesIdx++;
            }
        }

        IfHasControlThenDo(inputFields, SkywinMembersConstants.NextOfKinPhone, person.ContactPhone, ControlSendMessage);
        IfHasControlThenDo(inputFields, SkywinMembersConstants.NextOfKin, person.ContactName, ControlSendMessage);
        IfHasControlThenDo(inputFields, SkywinMembersConstants.Email, person.Mail, ControlSendMessage);
        IfHasControlThenDo(inputFields, SkywinMembersConstants.City, person.City, ControlSendMessage);
        IfHasControlThenDo(inputFields, SkywinMembersConstants.Zip, person.Zip, ControlSendMessage);
        IfHasControlThenDo(inputFields, SkywinMembersConstants.Address, person.Address, ControlSendMessage);
        IfHasControlThenDo(inputFields, SkywinMembersConstants.LastName, person.LastName, ControlSendMessage);
        IfHasControlThenDo(inputFields, SkywinMembersConstants.DFUNo, person.Id, ControlSendMessage);
        IfHasControlThenDo(inputFields, SkywinMembersConstants.HomeDz, person.Club, ControlSendMessage);
        IfHasControlThenDo(comboBoxes, SkywinMembersConstants.Gender, person.Gender == "M" ? "Male" : "Female", ComboSelect);
        IfHasControlThenDo(inputFields, SkywinMembersConstants.Firstname, person.FirstName, ControlSendMessage);
        IfHasControlThenDo(inputFields, SkywinMembersConstants.LastName, person.LastName, ControlSendMessage);
    }


    private void IfHasControlThenDo(Dictionary<int, Win32CustomControl> dict, int controlIndex, string text, Action<nint, string> lambda)
    {
        if (!dict.TryGetValue(controlIndex, out var info))
        {
            return;
        }

        lambda(info.ptr, text);
    }

    private void ControlSendMessage(nint controlPtr, string text)
    {
        SendMessage(controlPtr, (uint)WindowsMessages.SETTEXT, nint.Zero, text.Trim());
    }

    private void ComboSelect(nint controlPtr, string text)
    {
        SendMessage(controlPtr, CB_SELECTSTRING, nint.Zero, text.Trim());
    }
}