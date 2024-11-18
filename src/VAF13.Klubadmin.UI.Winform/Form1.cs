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

        private List<PersonDetails> _lastSearched = Array.Empty<PersonDetails>().ToList();

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
            dataGridView1.ColumnCount = 9;
            dataGridView1.Name = "Members view";
            dataGridView1.Columns[0].Name = "First Name";
            dataGridView1.Columns[1].Name = "Last Name";
            dataGridView1.Columns[2].Name = "Club";
            dataGridView1.Columns[3].Name = "C";
            dataGridView1.Columns[4].Name = "DFU No";
            dataGridView1.Columns[4].Name = "Contact Relation";
            dataGridView1.Columns[5].Name = "Contact Name";
            dataGridView1.Columns[6].Name = "Contact Phone";
            dataGridView1.Columns[7].Name = "Gender";

            DataGridViewButtonColumn button = new DataGridViewButtonColumn();
            button.Name = "Skywin";
            button.HeaderText = "Skywin";
            button.Text = "-> Skywin";
            button.UseColumnTextForButtonValue = true; //dont forget this line
            dataGridView1.Columns.Add(button);

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            dataGridView1.Rows.Clear();
        }

        private void dataGridViewSoftware_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            _logger.LogInformation("Cell clicked! col:{ColumnIndex} row:{RowIndex}", e.ColumnIndex, e.RowIndex);
            if (e.ColumnIndex != 9)
                return;

            var dataToInsert = _lastSearched.ElementAtOrDefault(e.RowIndex);
            if (dataToInsert is null)
            {
                _logger.LogError("Unable to find element at");
                return;
            }

            if (MessageBox.Show($@"Insert into empty members dialog {dataToInsert.FirstName} {dataToInsert.LastName}",
                    "", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                _logger.LogInformation("Aborting inserting into members!");
                return;
            }

            _skywinMembersDialogService.InsertData(dataToInsert);
        }

        private async void search_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Search clicked!");
            var searchName = txt_name.Text;
            if (string.IsNullOrEmpty(searchName))
            {
                return;
            }

            progressBar1.Visible = true;
            btn_search.Visible = false;
            btn_Clear.Visible = false;
            var searchResults = await _test
                .SearchAll(searchName)
                .ConfigureAwait(true);

            _lastSearched = searchResults
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ThenBy(c => c.Club)
                .ToList();

            _logger.LogInformation("Result from search resulted in #{Count}", _lastSearched.Count);
            dataGridView1.Rows.Clear();

            foreach (var person in _lastSearched)
            {
                object[] row = [person.FirstName, person.LastName, person.Club, person.Certificate, person.Id, person.ContactRelation, person.ContactName, person.Phone, person.Gender];
                dataGridView1.Rows.Add(row);
            }

            if (_lastSearched.Count > 0)
            {
                btn_Clear.Visible = true;
            }
            progressBar1.Visible = false;
            btn_search.Visible = true;
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            _logger.LogInformation("Clear clicked!");
            txt_name.Text = string.Empty;
            _lastSearched = Array.Empty<PersonDetails>().ToList();
            dataGridView1.Rows.Clear();
            btn_Clear.Visible = false;
        }
    }
}