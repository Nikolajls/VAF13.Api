using System.Text;
using Microsoft.Extensions.Logging;
using VAF13.Klubadmin.Domain.DTOs;
using VAF13.Klubadmin.Domain.Helpers;

namespace VAF13.Klubadmin.Domain.Services.Skywin;

public class SkywinMembersDialogService : ISkywinMembersDialogService
{
  private readonly IWindowsApiService _windowsApiService;
  private readonly ILogger<SkywinMembersDialogService> _logger;

  public SkywinMembersDialogService(IWindowsApiService windowsApiService, ILogger<SkywinMembersDialogService> logger)
  {
    _logger = logger;
    _windowsApiService = windowsApiService;
  }

  public void InsertData(PersonDetailsResponse person)
  {
    _logger.LogInformation("Finding members window");

    var membersHandle = _windowsApiService.FindWindow(null, "Members");

    if (membersHandle == nint.Zero)
    {
      _logger.LogError("No members dialog found");
      return;
    }

    var childHandles = _windowsApiService.GetAllChildHandles(membersHandle);
    var inputFields = new Dictionary<int, Win32CustomControl>();
    var comboBoxes = new Dictionary<int, Win32CustomControl>();
    int inputFieldsIdx = 1;
    int comboBoxesIdx = 1;
    foreach (var p in childHandles)
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

    IfHasControlThenDo(inputFields, SkywinMembersConstants.NextOfKinPhone, person.ContactPhone, _windowsApiService.SendMessage_SetText);
    IfHasControlThenDo(inputFields, SkywinMembersConstants.NextOfKin, person.ContactName, _windowsApiService.SendMessage_SetText);
    IfHasControlThenDo(inputFields, SkywinMembersConstants.Email, person.Email.ToString(), _windowsApiService.SendMessage_SetText);
    IfHasControlThenDo(inputFields, SkywinMembersConstants.City, !string.IsNullOrWhiteSpace(person.City) ? person.City : "NOWHERE", _windowsApiService.SendMessage_SetText);
    IfHasControlThenDo(inputFields, SkywinMembersConstants.Zip, !string.IsNullOrWhiteSpace(person.Zip) ? person.Zip : "0000", _windowsApiService.SendMessage_SetText);
    IfHasControlThenDo(inputFields, SkywinMembersConstants.Address, person.Address, _windowsApiService.SendMessage_SetText);
    IfHasControlThenDo(inputFields, SkywinMembersConstants.LastName, person.LastName, _windowsApiService.SendMessage_SetText);
    IfHasControlThenDo(inputFields, SkywinMembersConstants.DFUNo, person.Id.ToString(), _windowsApiService.SendMessage_SetText);
    IfHasControlThenDo(inputFields, SkywinMembersConstants.HomeDz, person.Club, _windowsApiService.SendMessage_SetText);
    IfHasControlThenDo(comboBoxes, SkywinMembersConstants.Gender, person.Gender == "M" ? "Male" : "Female", _windowsApiService.SendMessage_ComboSelect);
    IfHasControlThenDo(inputFields, SkywinMembersConstants.Firstname, person.FirstName, _windowsApiService.SendMessage_SetText);
    IfHasControlThenDo(inputFields, SkywinMembersConstants.LastName, person.LastName, _windowsApiService.SendMessage_SetText);
    _logger.LogInformation("Inserted info into Skywin");
  }

  private void IfHasControlThenDo(Dictionary<int, Win32CustomControl> dict, int controlIndex, string text, Action<nint, string> DoAction)
  {
    if (!dict.TryGetValue(controlIndex, out var info))
    {
      return;
    }
    DoAction(info.ptr, text);
  }
}