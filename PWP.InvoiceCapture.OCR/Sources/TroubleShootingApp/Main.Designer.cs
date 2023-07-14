namespace TroubleShootingApp
{
    partial class InvoiceName
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ModelsTab = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.ModelId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.TemplatesTab = new System.Windows.Forms.TabPage();
            this.TemplatesList = new System.Windows.Forms.ListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.InvoicesTab = new System.Windows.Forms.TabPage();
            this.button8 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.TenantsCombo = new System.Windows.Forms.ComboBox();
            this.SettingsTab = new System.Windows.Forms.TabPage();
            this.ApiKey = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ApiAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TestTab = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.TenantId = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.SelectedPage = new System.Windows.Forms.Label();
            this.SelectedInvoice = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.SelectInvoice = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.TrainingDirectory = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.ContainerName = new System.Windows.Forms.TextBox();
            this.Output = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.ModelsTab.SuspendLayout();
            this.TemplatesTab.SuspendLayout();
            this.InvoicesTab.SuspendLayout();
            this.SettingsTab.SuspendLayout();
            this.TestTab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.ModelsTab);
            this.tabControl1.Controls.Add(this.TemplatesTab);
            this.tabControl1.Controls.Add(this.InvoicesTab);
            this.tabControl1.Controls.Add(this.SettingsTab);
            this.tabControl1.Controls.Add(this.TestTab);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(3, 2);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 2;
            this.tabControl1.Size = new System.Drawing.Size(411, 437);
            this.tabControl1.TabIndex = 0;
            // 
            // ModelsTab
            // 
            this.ModelsTab.Controls.Add(this.button2);
            this.ModelsTab.Controls.Add(this.ModelId);
            this.ModelsTab.Controls.Add(this.label3);
            this.ModelsTab.Controls.Add(this.button1);
            this.ModelsTab.Location = new System.Drawing.Point(4, 24);
            this.ModelsTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ModelsTab.Name = "ModelsTab";
            this.ModelsTab.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ModelsTab.Size = new System.Drawing.Size(403, 409);
            this.ModelsTab.TabIndex = 0;
            this.ModelsTab.Text = "Models";
            this.ModelsTab.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(183, 89);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(129, 22);
            this.button2.TabIndex = 0;
            this.button2.Text = "ListAll";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_ClickAsync);
            // 
            // ModelId
            // 
            this.ModelId.Location = new System.Drawing.Point(130, 20);
            this.ModelId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ModelId.Name = "ModelId";
            this.ModelId.Size = new System.Drawing.Size(182, 23);
            this.ModelId.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Model Id";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(229, 52);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 22);
            this.button1.TabIndex = 0;
            this.button1.Text = "Go";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_ClickAsync);
            // 
            // TemplatesTab
            // 
            this.TemplatesTab.Controls.Add(this.TemplatesList);
            this.TemplatesTab.Controls.Add(this.button3);
            this.TemplatesTab.Location = new System.Drawing.Point(4, 24);
            this.TemplatesTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TemplatesTab.Name = "TemplatesTab";
            this.TemplatesTab.Size = new System.Drawing.Size(403, 409);
            this.TemplatesTab.TabIndex = 2;
            this.TemplatesTab.Text = "Templates";
            // 
            // TemplatesList
            // 
            this.TemplatesList.FormattingEnabled = true;
            this.TemplatesList.ItemHeight = 15;
            this.TemplatesList.Location = new System.Drawing.Point(17, 12);
            this.TemplatesList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TemplatesList.Name = "TemplatesList";
            this.TemplatesList.Size = new System.Drawing.Size(367, 349);
            this.TemplatesList.TabIndex = 1;
            this.TemplatesList.SelectedValueChanged += new System.EventHandler(this.TemplatesList_SelectedValueChangedAsync);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(301, 380);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(82, 22);
            this.button3.TabIndex = 0;
            this.button3.Text = "Load";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3_ClickAsync);
            // 
            // InvoicesTab
            // 
            this.InvoicesTab.Controls.Add(this.button8);
            this.InvoicesTab.Controls.Add(this.button6);
            this.InvoicesTab.Controls.Add(this.TenantsCombo);
            this.InvoicesTab.Location = new System.Drawing.Point(4, 24);
            this.InvoicesTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.InvoicesTab.Name = "InvoicesTab";
            this.InvoicesTab.Size = new System.Drawing.Size(403, 409);
            this.InvoicesTab.TabIndex = 3;
            this.InvoicesTab.Text = "Invoices";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(234, 10);
            this.button8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(61, 22);
            this.button8.TabIndex = 1;
            this.button8.Text = "Select";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(325, 10);
            this.button6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(61, 22);
            this.button6.TabIndex = 1;
            this.button6.Text = "Load";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // TenantsCombo
            // 
            this.TenantsCombo.FormattingEnabled = true;
            this.TenantsCombo.Location = new System.Drawing.Point(10, 10);
            this.TenantsCombo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TenantsCombo.Name = "TenantsCombo";
            this.TenantsCombo.Size = new System.Drawing.Size(218, 23);
            this.TenantsCombo.TabIndex = 0;
            // 
            // SettingsTab
            // 
            this.SettingsTab.Controls.Add(this.ApiKey);
            this.SettingsTab.Controls.Add(this.label2);
            this.SettingsTab.Controls.Add(this.ApiAddress);
            this.SettingsTab.Controls.Add(this.label1);
            this.SettingsTab.Location = new System.Drawing.Point(4, 24);
            this.SettingsTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SettingsTab.Name = "SettingsTab";
            this.SettingsTab.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SettingsTab.Size = new System.Drawing.Size(403, 409);
            this.SettingsTab.TabIndex = 1;
            this.SettingsTab.Text = "Settings";
            this.SettingsTab.UseVisualStyleBackColor = true;
            this.SettingsTab.Click += new System.EventHandler(this.SettingsTab_Click);
            // 
            // ApiKey
            // 
            this.ApiKey.Location = new System.Drawing.Point(18, 105);
            this.ApiKey.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ApiKey.Name = "ApiKey";
            this.ApiKey.Size = new System.Drawing.Size(344, 23);
            this.ApiKey.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "API Key";
            // 
            // ApiAddress
            // 
            this.ApiAddress.Location = new System.Drawing.Point(18, 45);
            this.ApiAddress.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ApiAddress.Name = "ApiAddress";
            this.ApiAddress.Size = new System.Drawing.Size(344, 23);
            this.ApiAddress.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "API Address";
            // 
            // TestTab
            // 
            this.TestTab.Controls.Add(this.label5);
            this.TestTab.Controls.Add(this.TenantId);
            this.TestTab.Controls.Add(this.button5);
            this.TestTab.Controls.Add(this.SelectedPage);
            this.TestTab.Controls.Add(this.SelectedInvoice);
            this.TestTab.Controls.Add(this.button4);
            this.TestTab.Controls.Add(this.SelectInvoice);
            this.TestTab.Location = new System.Drawing.Point(4, 24);
            this.TestTab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TestTab.Name = "TestTab";
            this.TestTab.Size = new System.Drawing.Size(403, 409);
            this.TestTab.TabIndex = 4;
            this.TestTab.Text = "Test Invoice";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(144, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "Tenant Id";
            // 
            // TenantId
            // 
            this.TenantId.Location = new System.Drawing.Point(93, 25);
            this.TenantId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TenantId.Name = "TenantId";
            this.TenantId.Size = new System.Drawing.Size(176, 23);
            this.TenantId.TabIndex = 6;
            this.TenantId.Text = "Default";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(131, 274);
            this.button5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(82, 22);
            this.button5.TabIndex = 5;
            this.button5.Text = "Process";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click_1);
            // 
            // SelectedPage
            // 
            this.SelectedPage.AutoSize = true;
            this.SelectedPage.Location = new System.Drawing.Point(93, 170);
            this.SelectedPage.Name = "SelectedPage";
            this.SelectedPage.Size = new System.Drawing.Size(0, 15);
            this.SelectedPage.TabIndex = 4;
            // 
            // SelectedInvoice
            // 
            this.SelectedInvoice.AutoSize = true;
            this.SelectedInvoice.Location = new System.Drawing.Point(93, 59);
            this.SelectedInvoice.Name = "SelectedInvoice";
            this.SelectedInvoice.Size = new System.Drawing.Size(0, 15);
            this.SelectedInvoice.TabIndex = 3;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(89, 196);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(175, 22);
            this.button4.TabIndex = 1;
            this.button4.Text = "Select First Page";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // SelectInvoice
            // 
            this.SelectInvoice.Location = new System.Drawing.Point(93, 86);
            this.SelectInvoice.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SelectInvoice.Name = "SelectInvoice";
            this.SelectInvoice.Size = new System.Drawing.Size(175, 22);
            this.SelectInvoice.TabIndex = 1;
            this.SelectInvoice.Text = "Select Invoice";
            this.SelectInvoice.UseVisualStyleBackColor = true;
            this.SelectInvoice.Click += new System.EventHandler(this.SelectInvoice_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.TrainingDirectory);
            this.tabPage1.Controls.Add(this.button7);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.ContainerName);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(403, 409);
            this.tabPage1.TabIndex = 5;
            this.tabPage1.Text = "Training";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(276, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 15);
            this.label7.TabIndex = 2;
            this.label7.Text = "Local Folder";
            // 
            // TrainingDirectory
            // 
            this.TrainingDirectory.Location = new System.Drawing.Point(25, 89);
            this.TrainingDirectory.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TrainingDirectory.Name = "TrainingDirectory";
            this.TrainingDirectory.Size = new System.Drawing.Size(339, 23);
            this.TrainingDirectory.TabIndex = 4;
            this.TrainingDirectory.Text = "D:\\Invoices\\test";
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(282, 127);
            this.button7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(82, 22);
            this.button7.TabIndex = 3;
            this.button7.Text = "Download All";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(263, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "Blob Container";
            // 
            // ContainerName
            // 
            this.ContainerName.Location = new System.Drawing.Point(25, 28);
            this.ContainerName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ContainerName.Name = "ContainerName";
            this.ContainerName.Size = new System.Drawing.Size(335, 23);
            this.ContainerName.TabIndex = 0;
            this.ContainerName.Text = "fr-training-blob-114";
            // 
            // Output
            // 
            this.Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Output.Location = new System.Drawing.Point(419, 82);
            this.Output.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Output.Multiline = true;
            this.Output.Name = "Output";
            this.Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Output.Size = new System.Drawing.Size(789, 355);
            this.Output.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(1118, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Output";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(761, 23);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(129, 15);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Form OCR Testing Tool";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // InvoiceName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1218, 441);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Output);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InvoiceName";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Main_Load);
            this.tabControl1.ResumeLayout(false);
            this.ModelsTab.ResumeLayout(false);
            this.ModelsTab.PerformLayout();
            this.TemplatesTab.ResumeLayout(false);
            this.InvoicesTab.ResumeLayout(false);
            this.SettingsTab.ResumeLayout(false);
            this.SettingsTab.PerformLayout();
            this.TestTab.ResumeLayout(false);
            this.TestTab.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ModelsTab;
        private System.Windows.Forms.TabPage SettingsTab;
        private System.Windows.Forms.TextBox ApiAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ApiKey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox ModelId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox Output;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TabPage TemplatesTab;
        private System.Windows.Forms.ListBox TemplatesList;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TabPage InvoicesTab;
        private System.Windows.Forms.TabPage TestTab;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label SelectedInvoice;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button SelectInvoice;
        private System.Windows.Forms.Label SelectedPage;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox TenantId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox TrainingDirectory;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox ContainerName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ComboBox TenantsCombo;
        private System.Windows.Forms.Button button8;
    }
}

