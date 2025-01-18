namespace VAF13.Klubadmin.UI.Winform
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            name_Text = new TextBox();
            members_Datagrid = new DataGridView();
            search_progressBar = new ProgressBar();
            search_Button = new Button();
            searchClear_Button = new Button();
            ((System.ComponentModel.ISupportInitialize)members_Datagrid).BeginInit();
            SuspendLayout();
            // 
            // name_Text
            // 
            name_Text.Location = new Point(12, 12);
            name_Text.Name = "name_Text";
            name_Text.Size = new Size(643, 23);
            name_Text.TabIndex = 0;
            // 
            // members_Datagrid
            // 
            members_Datagrid.AllowUserToAddRows = false;
            members_Datagrid.AllowUserToDeleteRows = false;
            members_Datagrid.AllowUserToOrderColumns = true;
            members_Datagrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            members_Datagrid.Location = new Point(16, 63);
            members_Datagrid.Name = "members_Datagrid";
            members_Datagrid.Size = new Size(1020, 366);
            members_Datagrid.TabIndex = 2;
            members_Datagrid.CellClick += dataGridViewSoftware_CellClick;
            // 
            // search_progressBar
            // 
            search_progressBar.Location = new Point(666, 12);
            search_progressBar.Name = "search_progressBar";
            search_progressBar.Size = new Size(122, 23);
            search_progressBar.Style = ProgressBarStyle.Marquee;
            search_progressBar.TabIndex = 3;
            search_progressBar.Value = 10;
            search_progressBar.Visible = false;
            // 
            // search_Button
            // 
            search_Button.Location = new Point(666, 12);
            search_Button.Name = "search_Button";
            search_Button.Size = new Size(122, 23);
            search_Button.TabIndex = 1;
            search_Button.Text = "Search";
            search_Button.UseVisualStyleBackColor = true;
            search_Button.Click += SearchButtonClick;
            // 
            // searchClear_Button
            // 
            searchClear_Button.Location = new Point(794, 12);
            searchClear_Button.Name = "searchClear_Button";
            searchClear_Button.Size = new Size(122, 23);
            searchClear_Button.TabIndex = 4;
            searchClear_Button.Text = "Clear";
            searchClear_Button.UseVisualStyleBackColor = true;
            searchClear_Button.Visible = false;
            searchClear_Button.Click += SearchClearButtonClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1048, 450);
            Controls.Add(searchClear_Button);
            Controls.Add(search_progressBar);
            Controls.Add(members_Datagrid);
            Controls.Add(search_Button);
            Controls.Add(name_Text);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)members_Datagrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox name_Text;
        private DataGridView members_Datagrid;
        private ProgressBar search_progressBar;
        private Button search_Button;
        private Button searchClear_Button;
    }
}