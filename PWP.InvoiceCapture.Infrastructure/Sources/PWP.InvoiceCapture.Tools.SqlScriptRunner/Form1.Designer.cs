
namespace PWP.InvoiceCapture.Tools.SqlScriptRunner
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_environment = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox_script = new System.Windows.Forms.RichTextBox();
            this.button_run = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.textBox_getTenants = new System.Windows.Forms.TextBox();
            this.checkedListBox_tenants = new System.Windows.Forms.CheckedListBox();
            this.button_update = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.checkedListBox_otherDb = new System.Windows.Forms.CheckedListBox();
            this.textBox_scriptOtherDb = new System.Windows.Forms.TextBox();
            this.button_updateOtherDbList = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.radioButton_scriptFromFolder = new System.Windows.Forms.RadioButton();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button_openFolder = new System.Windows.Forms.Button();
            this.label_scriptFolder = new System.Windows.Forms.Label();
            this.radioButton_enteredScript = new System.Windows.Forms.RadioButton();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Environment";
            // 
            // comboBox_environment
            // 
            this.comboBox_environment.FormattingEnabled = true;
            this.comboBox_environment.Items.AddRange(new object[] {
            "Development",
            "Staging",
            "Production"});
            this.comboBox_environment.Location = new System.Drawing.Point(13, 39);
            this.comboBox_environment.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.comboBox_environment.Name = "comboBox_environment";
            this.comboBox_environment.Size = new System.Drawing.Size(140, 23);
            this.comboBox_environment.TabIndex = 1;
            this.comboBox_environment.Text = "Development";
            this.comboBox_environment.SelectedIndexChanged += new System.EventHandler(this.comboBox_environment_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 19);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Script To Get Invoices Database List";
            // 
            // richTextBox_script
            // 
            this.richTextBox_script.Enabled = false;
            this.richTextBox_script.Location = new System.Drawing.Point(12, 579);
            this.richTextBox_script.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.richTextBox_script.Name = "richTextBox_script";
            this.richTextBox_script.Size = new System.Drawing.Size(558, 105);
            this.richTextBox_script.TabIndex = 8;
            this.richTextBox_script.Text = "";
            // 
            // button_run
            // 
            this.button_run.Enabled = false;
            this.button_run.Location = new System.Drawing.Point(426, 709);
            this.button_run.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_run.Name = "button_run";
            this.button_run.Size = new System.Drawing.Size(148, 29);
            this.button_run.TabIndex = 9;
            this.button_run.Text = "Run Script";
            this.button_run.UseVisualStyleBackColor = true;
            this.button_run.Click += new System.EventHandler(this.button_run_Click);
            // 
            // textBox_getTenants
            // 
            this.textBox_getTenants.Location = new System.Drawing.Point(18, 38);
            this.textBox_getTenants.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox_getTenants.Name = "textBox_getTenants";
            this.textBox_getTenants.Size = new System.Drawing.Size(427, 23);
            this.textBox_getTenants.TabIndex = 10;
            this.textBox_getTenants.Text = "SELECT Id, DatabaseName FROM dbo.Tenant WHERE IsActive = 1";
            // 
            // checkedListBox_tenants
            // 
            this.checkedListBox_tenants.FormattingEnabled = true;
            this.checkedListBox_tenants.Location = new System.Drawing.Point(18, 94);
            this.checkedListBox_tenants.Name = "checkedListBox_tenants";
            this.checkedListBox_tenants.Size = new System.Drawing.Size(181, 238);
            this.checkedListBox_tenants.TabIndex = 12;
            // 
            // button_update
            // 
            this.button_update.Location = new System.Drawing.Point(453, 37);
            this.button_update.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_update.Name = "button_update";
            this.button_update.Size = new System.Drawing.Size(68, 23);
            this.button_update.TabIndex = 14;
            this.button_update.Text = "Update";
            this.button_update.UseVisualStyleBackColor = true;
            this.button_update.Click += new System.EventHandler(this.button_update_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(13, 78);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(561, 395);
            this.tabControl1.TabIndex = 16;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.checkedListBox_tenants);
            this.tabPage1.Controls.Add(this.textBox_getTenants);
            this.tabPage1.Controls.Add(this.button_update);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(553, 367);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Invoices Databases";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 76);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 15);
            this.label4.TabIndex = 15;
            this.label4.Text = "Dabases To Apply Script:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.checkedListBox_otherDb);
            this.tabPage2.Controls.Add(this.textBox_scriptOtherDb);
            this.tabPage2.Controls.Add(this.button_updateOtherDbList);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(553, 367);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Other Databases";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 94);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 15);
            this.label3.TabIndex = 20;
            this.label3.Text = "Dabases To Apply Script:";
            // 
            // checkedListBox_otherDb
            // 
            this.checkedListBox_otherDb.FormattingEnabled = true;
            this.checkedListBox_otherDb.Location = new System.Drawing.Point(18, 112);
            this.checkedListBox_otherDb.Name = "checkedListBox_otherDb";
            this.checkedListBox_otherDb.Size = new System.Drawing.Size(181, 220);
            this.checkedListBox_otherDb.TabIndex = 18;
            // 
            // textBox_scriptOtherDb
            // 
            this.textBox_scriptOtherDb.Location = new System.Drawing.Point(18, 38);
            this.textBox_scriptOtherDb.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.textBox_scriptOtherDb.Name = "textBox_scriptOtherDb";
            this.textBox_scriptOtherDb.Size = new System.Drawing.Size(522, 23);
            this.textBox_scriptOtherDb.TabIndex = 17;
            this.textBox_scriptOtherDb.Text = "select [name] from sys.databases where [name] not like \'Invoices_%\' and [name] <>" +
    " \'master\'";
            // 
            // button_updateOtherDbList
            // 
            this.button_updateOtherDbList.Location = new System.Drawing.Point(472, 68);
            this.button_updateOtherDbList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_updateOtherDbList.Name = "button_updateOtherDbList";
            this.button_updateOtherDbList.Size = new System.Drawing.Size(68, 23);
            this.button_updateOtherDbList.TabIndex = 19;
            this.button_updateOtherDbList.Text = "Update";
            this.button_updateOtherDbList.UseVisualStyleBackColor = true;
            this.button_updateOtherDbList.Click += new System.EventHandler(this.button_updateOtherDbList_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 19);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(179, 15);
            this.label5.TabIndex = 16;
            this.label5.Text = "Script To Get Other Database List";
            // 
            // radioButton_scriptFromFolder
            // 
            this.radioButton_scriptFromFolder.AutoSize = true;
            this.radioButton_scriptFromFolder.Location = new System.Drawing.Point(17, 529);
            this.radioButton_scriptFromFolder.Name = "radioButton_scriptFromFolder";
            this.radioButton_scriptFromFolder.Size = new System.Drawing.Size(146, 19);
            this.radioButton_scriptFromFolder.TabIndex = 17;
            this.radioButton_scriptFromFolder.Text = "Run scripts from folder";
            this.radioButton_scriptFromFolder.UseVisualStyleBackColor = true;
            this.radioButton_scriptFromFolder.CheckedChanged += new System.EventHandler(this.radioButton_scriptFromFolder_CheckedChanged);
            // 
            // button_openFolder
            // 
            this.button_openFolder.Location = new System.Drawing.Point(178, 527);
            this.button_openFolder.Name = "button_openFolder";
            this.button_openFolder.Size = new System.Drawing.Size(75, 23);
            this.button_openFolder.TabIndex = 18;
            this.button_openFolder.Text = "Open";
            this.button_openFolder.UseVisualStyleBackColor = true;
            this.button_openFolder.Click += new System.EventHandler(this.button_openFolder_Click);
            // 
            // label_scriptFolder
            // 
            this.label_scriptFolder.AutoSize = true;
            this.label_scriptFolder.Location = new System.Drawing.Point(282, 531);
            this.label_scriptFolder.Name = "label_scriptFolder";
            this.label_scriptFolder.Size = new System.Drawing.Size(0, 15);
            this.label_scriptFolder.TabIndex = 19;
            // 
            // radioButton_enteredScript
            // 
            this.radioButton_enteredScript.AutoSize = true;
            this.radioButton_enteredScript.Location = new System.Drawing.Point(17, 554);
            this.radioButton_enteredScript.Name = "radioButton_enteredScript";
            this.radioButton_enteredScript.Size = new System.Drawing.Size(122, 19);
            this.radioButton_enteredScript.TabIndex = 20;
            this.radioButton_enteredScript.Text = "Run Entered Script";
            this.radioButton_enteredScript.UseVisualStyleBackColor = true;
            this.radioButton_enteredScript.CheckedChanged += new System.EventHandler(this.radioButton_enteredScript_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 810);
            this.Controls.Add(this.radioButton_enteredScript);
            this.Controls.Add(this.label_scriptFolder);
            this.Controls.Add(this.button_openFolder);
            this.Controls.Add(this.radioButton_scriptFromFolder);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button_run);
            this.Controls.Add(this.richTextBox_script);
            this.Controls.Add(this.comboBox_environment);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Form1";
            this.Text = "SQL Script Runner";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_environment;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBox_script;
        private System.Windows.Forms.Button button_run;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.TextBox textBox_getTenants;
        private System.Windows.Forms.CheckedListBox checkedListBox_tenants;
        private System.Windows.Forms.Button button_update;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RadioButton radioButton_scriptFromFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_openFolder;
        private System.Windows.Forms.Label label_scriptFolder;
        private System.Windows.Forms.RadioButton radioButton_enteredScript;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox checkedListBox_otherDb;
        private System.Windows.Forms.TextBox textBox_scriptOtherDb;
        private System.Windows.Forms.Button button_updateOtherDbList;
        private System.Windows.Forms.Label label5;
    }
}

