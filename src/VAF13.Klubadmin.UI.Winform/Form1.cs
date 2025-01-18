using Microsoft.Extensions.Logging;
using VAF13.Klubadmin.Domain.DTOs;
using VAF13.Klubadmin.Domain.Services.Skywin;
using VAF13.Klubadmin.Domain.Services.VAFapi;

namespace VAF13.Klubadmin.UI.Winform
{
  public partial class Form1 : Form
  {
    private readonly IVafApiIntegration _test;
    private readonly ISkywinMembersDialogService _skywinMembersDialogService;
    private readonly ILogger<Form1> _logger;

    private PersonDetailsResponse[] _lastSearched = [];

    public Form1(IVafApiIntegration test, ISkywinMembersDialogService skywinMembersDialogService, ILogger<Form1> logger)
    {
      _test = test;
      _skywinMembersDialogService = skywinMembersDialogService;
      _logger = logger;
      InitializeComponent();
      Initialize_DataGridView();
    }

    private void Initialize_DataGridView()
    {
      members_Datagrid.ColumnCount = 9;
      members_Datagrid.Name = "Members view";
      members_Datagrid.Columns[0].Name = "First Name";
      members_Datagrid.Columns[1].Name = "Last Name";
      members_Datagrid.Columns[2].Name = "Club";
      members_Datagrid.Columns[3].Name = "C";
      members_Datagrid.Columns[4].Name = "DFU No";
      members_Datagrid.Columns[5].Name = "Contact Relation";
      members_Datagrid.Columns[6].Name = "Contact Name";
      members_Datagrid.Columns[7].Name = "Contact Phone";
      members_Datagrid.Columns[8].Name = "Gender";

      DataGridViewButtonColumn button = new DataGridViewButtonColumn();
      button.Name = "Skywin";
      button.HeaderText = "Skywin";
      button.Text = "-> Skywin";
      button.UseColumnTextForButtonValue = true; //dont forget this line
      members_Datagrid.Columns.Add(button);

      members_Datagrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      members_Datagrid.MultiSelect = false;

      members_Datagrid.Rows.Clear();
    }

    private void dataGridViewSoftware_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      _logger.LogInformation("Cell in dataGrid clicked at columIndex:{ColumnIndex} rowIndex:{RowIndex}", e.ColumnIndex, e.RowIndex);
      if (e.ColumnIndex != 9)
      {
        // If it's not the column with the button, return
        return;
      }

      var dataToInsert = _lastSearched.ElementAtOrDefault(e.RowIndex);
      if (dataToInsert is null)
      {
        _logger.LogError("Unable to find the selected element from the List by index: {Index}", e.RowIndex);
        return;
      }

      if (MessageBox.Show($@"Insert into empty members dialog {dataToInsert.FirstName} {dataToInsert.LastName}",
              "Insert data", MessageBoxButtons.YesNo) == DialogResult.No)
      {
        _logger.LogInformation("Aborting inserting data into Skywin members dialog!");
        return;
      }

      _skywinMembersDialogService.InsertData(dataToInsert);
    }

    private async void SearchButtonClick(object sender, EventArgs e)
    {
      var searchName = name_Text.Text;
      _logger.LogInformation("Search button clicked with searchName:{Name}", searchName);
      if (string.IsNullOrEmpty(searchName))
      {
        return;
      }

      search_progressBar.Visible = true;
      search_Button.Visible = false;
      searchClear_Button.Visible = false;

      List<PersonDetailsResponse> searchResults = await _test
          .SearchAll(searchName)
          .ConfigureAwait(true);

      SetSearchResultsAndShow(searchResults);
    }

    private void SetSearchResultsAndShow(List<PersonDetailsResponse> searchResults)
    {
      _lastSearched = searchResults
          .OrderBy(c => c.FirstName)
          .ThenBy(c => c.LastName)
          .ThenBy(c => c.Club)
          .ToArray();

      _logger.LogInformation("Result from search resulted in #{Count}", _lastSearched.Length);
      members_Datagrid.Rows.Clear();

      foreach (var person in _lastSearched)
      {
        object[] row = [
            person.FirstName, //0
                    person.LastName,//1
                    person.Club,//2
                    person.Certificate,//3
                    person.Id,//4
                    person.ContactRelation,//5
                    person.ContactName,//6
                    person.Phone,//7
                    person.Gender//8
        ];
        members_Datagrid.Rows.Add(row);
      }

      if (_lastSearched.Length > 0)
      {
        searchClear_Button.Visible = true;
      }
      search_progressBar.Visible = false;
      search_Button.Visible = true;
    }

    private void SearchClearButtonClick(object sender, EventArgs e)
    {
      _logger.LogInformation("Search button clicked!");
      name_Text.Text = string.Empty;
      _lastSearched = [];
      members_Datagrid.Rows.Clear();
      searchClear_Button.Visible = false;
    }
  }
}