using VAF13.Klubadmin.Domain.DTOs;
using VAF13.Klubadmin.Domain.Services.Skywin;
using VAF13.Klubadmin.Domain.Services.VAFapi;

namespace VAF13.Klubadmin.UI.Winform
{
    public partial class Form1 : Form
    {
        private readonly IVAFApiIntegration _test;
        private readonly ISkywinMembersDialogService _skywinMembersDialogService;
        private List<PersonDetails> _lastSearched;

        public Form1(IVAFApiIntegration test, ISkywinMembersDialogService skywinMembersDialogService)
        {
            _test = test;
            _skywinMembersDialogService = skywinMembersDialogService;
            InitializeComponent();

            dataGridView1.ColumnCount = 8;
            dataGridView1.Name = "Members view";
            dataGridView1.Columns[0].Name = "First Name";
            dataGridView1.Columns[1].Name = "Last Name";
            dataGridView1.Columns[2].Name = "Club";
            dataGridView1.Columns[3].Name = "DFU No";
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
            _lastSearched = Array.Empty<PersonDetails>().ToList();
        }

        private void dataGridViewSoftware_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 8)
                return;

            var dataToInsert = _lastSearched.ElementAtOrDefault(e.RowIndex);
            if (dataToInsert is null)
            {
                return;
            }

            if (MessageBox.Show($@"Insert into empty members dialog {dataToInsert.FirstName} {dataToInsert.LastName}",
                    "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _skywinMembersDialogService.InsertData(dataToInsert);
            }
        }

        private async void search_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            btn_search.Visible = false;
            var searchResults = await _test
                .SearchAll(txt_name.Text)
                .ConfigureAwait(true);

            _lastSearched = searchResults;
            dataGridView1.Rows.Clear();

            foreach (var person in _lastSearched)
            {
                object[] row = { person.FirstName, person.LastName, person.Club, person.Id, person.ContactRelation, person.ContactName, person.Phone, person.Gender };
                dataGridView1.Rows.Add(row);
            }

            progressBar1.Visible = false;
            btn_search.Visible = true;
        }
    }
}