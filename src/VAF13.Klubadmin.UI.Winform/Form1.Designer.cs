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
            txt_name = new TextBox();
            dataGridView1 = new DataGridView();
            progressBar1 = new ProgressBar();
            btn_search = new Button();
            btn_Clear = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // txt_name
            // 
            txt_name.Location = new Point(12, 12);
            txt_name.Name = "txt_name";
            txt_name.Size = new Size(643, 23);
            txt_name.TabIndex = 0;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = true;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(16, 63);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(1020, 366);
            dataGridView1.TabIndex = 2;
            dataGridView1.CellClick += dataGridViewSoftware_CellClick;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(666, 12);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(122, 23);
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.TabIndex = 3;
            progressBar1.Value = 10;
            progressBar1.Visible = false;
            // 
            // btn_search
            // 
            btn_search.Location = new Point(666, 12);
            btn_search.Name = "btn_search";
            btn_search.Size = new Size(122, 23);
            btn_search.TabIndex = 1;
            btn_search.Text = "Search";
            btn_search.UseVisualStyleBackColor = true;
            btn_search.Click += search_Click;
            // 
            // btn_Clear
            // 
            btn_Clear.Location = new Point(794, 12);
            btn_Clear.Name = "btn_Clear";
            btn_Clear.Size = new Size(122, 23);
            btn_Clear.TabIndex = 4;
            btn_Clear.Text = "Clear";
            btn_Clear.UseVisualStyleBackColor = true;
            btn_Clear.Visible = false;
            btn_Clear.Click += btn_Clear_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1048, 450);
            Controls.Add(btn_Clear);
            Controls.Add(progressBar1);
            Controls.Add(dataGridView1);
            Controls.Add(btn_search);
            Controls.Add(txt_name);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txt_name;
        private DataGridView dataGridView1;
        private ProgressBar progressBar1;
        private Button btn_search;
        private Button btn_Clear;
    }
}