using System.Windows.Forms;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App
{
    partial class Form1
    {
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.textBox_tenantId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.er = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_userName = new System.Windows.Forms.TextBox();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_environment = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_FolderPath = new System.Windows.Forms.TextBox();
            this.button_start = new System.Windows.Forms.Button();
            this.button_open = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_collectInvoices = new System.Windows.Forms.Button();
            this.textBox_ThreadsCount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_InvoiceUploadInterval = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_InvoicePollingInterval = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_tenantId
            // 
            this.textBox_tenantId.Location = new System.Drawing.Point(9, 32);
            this.textBox_tenantId.Name = "textBox_tenantId";
            this.textBox_tenantId.Size = new System.Drawing.Size(681, 20);
            this.textBox_tenantId.TabIndex = 2;
            this.textBox_tenantId.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "TenantId";
            // 
            // er
            // 
            this.er.AutoSize = true;
            this.er.Location = new System.Drawing.Point(6, 59);
            this.er.Name = "er";
            this.er.Size = new System.Drawing.Size(57, 13);
            this.er.TabIndex = 0;
            this.er.Text = "UserName";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password";
            // 
            // textBox_userName
            // 
            this.textBox_userName.Location = new System.Drawing.Point(9, 75);
            this.textBox_userName.Name = "textBox_userName";
            this.textBox_userName.Size = new System.Drawing.Size(681, 20);
            this.textBox_userName.TabIndex = 2;
            this.textBox_userName.Text = "tenantKey";
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(9, 118);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.Size = new System.Drawing.Size(681, 20);
            this.textBox_password.TabIndex = 3;
            this.textBox_password.Text = "password";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_password);
            this.groupBox2.Controls.Add(this.textBox_userName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.er);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBox_tenantId);
            this.groupBox2.Location = new System.Drawing.Point(22, 280);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(701, 154);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Environment";
            // 
            // comboBox_environment
            // 
            this.comboBox_environment.DisplayMember = "1";
            this.comboBox_environment.FormattingEnabled = true;
            this.comboBox_environment.Items.AddRange(new object[] {
            "Development",
            "Staging"});
            this.comboBox_environment.Location = new System.Drawing.Point(9, 32);
            this.comboBox_environment.Name = "comboBox_environment";
            this.comboBox_environment.Size = new System.Drawing.Size(681, 21);
            this.comboBox_environment.TabIndex = 4;
            this.comboBox_environment.Text = "Development";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Folder";
            // 
            // textBox_FolderPath
            // 
            this.textBox_FolderPath.Location = new System.Drawing.Point(9, 75);
            this.textBox_FolderPath.Name = "textBox_FolderPath";
            this.textBox_FolderPath.Size = new System.Drawing.Size(588, 20);
            this.textBox_FolderPath.TabIndex = 6;
            this.textBox_FolderPath.Text = "C:\\Temp\\Invoices";
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(625, 450);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(87, 23);
            this.button_start.TabIndex = 7;
            this.button_start.Text = "Start";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.Button_Start_ClickAsync);
            // 
            // button_open
            // 
            this.button_open.Location = new System.Drawing.Point(603, 75);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(87, 23);
            this.button_open.TabIndex = 8;
            this.button_open.Text = "Browse";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.button_open_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_InvoicePollingInterval);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBox_InvoiceUploadInterval);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBox_ThreadsCount);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.button_open);
            this.groupBox1.Controls.Add(this.textBox_FolderPath);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBox_environment);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(22, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(701, 242);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // button_collectInvoices
            // 
            this.button_collectInvoices.Location = new System.Drawing.Point(532, 450);
            this.button_collectInvoices.Name = "button_collectInvoices";
            this.button_collectInvoices.Size = new System.Drawing.Size(87, 23);
            this.button_collectInvoices.TabIndex = 8;
            this.button_collectInvoices.Text = "Collect";
            this.button_collectInvoices.UseVisualStyleBackColor = true;
            this.button_collectInvoices.Click += new System.EventHandler(this.Button_CollectInvoices_ClickAsync);
            // 
            // textBox_ThreadsCount
            // 
            this.textBox_ThreadsCount.Location = new System.Drawing.Point(9, 119);
            this.textBox_ThreadsCount.Name = "textBox_ThreadsCount";
            this.textBox_ThreadsCount.Size = new System.Drawing.Size(681, 20);
            this.textBox_ThreadsCount.TabIndex = 10;
            this.textBox_ThreadsCount.Text = "10";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Threads";
            // 
            // textBox_InvoiceUploadInterval
            // 
            this.textBox_InvoiceUploadInterval.Location = new System.Drawing.Point(9, 164);
            this.textBox_InvoiceUploadInterval.Name = "textBox_InvoiceUploadInterval";
            this.textBox_InvoiceUploadInterval.Size = new System.Drawing.Size(681, 20);
            this.textBox_InvoiceUploadInterval.TabIndex = 12;
            this.textBox_InvoiceUploadInterval.Text = "20000";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(199, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Invoice Uploading Inteval in Milliseconds";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 191);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(185, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Invoice Polling Interval in Milliseconds";
            // 
            // textBox_InvoicePollingInterval
            // 
            this.textBox_InvoicePollingInterval.Location = new System.Drawing.Point(9, 207);
            this.textBox_InvoicePollingInterval.Name = "textBox_InvoicePollingInterval";
            this.textBox_InvoicePollingInterval.Size = new System.Drawing.Size(681, 20);
            this.textBox_InvoicePollingInterval.TabIndex = 14;
            this.textBox_InvoicePollingInterval.Text = "500";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(742, 496);
            this.Controls.Add(this.button_collectInvoices);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.groupBox2);
            this.Name = "Form1";
            this.Text = "OCR Performance Testing App";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.IContainer components = null;
        private FolderBrowserDialog folderBrowserDialog1;
        private TextBox textBox_tenantId;
        private Label label1;
        private Label er;
        private Label label2;
        private TextBox textBox_userName;
        private TextBox textBox_password;
        private GroupBox groupBox2;
        private Label label3;
        private ComboBox comboBox_environment;
        private Label label4;
        private TextBox textBox_FolderPath;
        private Button button_start;
        private Button button_open;
        private GroupBox groupBox1;
        private Button button_collectInvoices;
        private TextBox textBox_ThreadsCount;
        private Label label5;
        private TextBox textBox_InvoicePollingInterval;
        private Label label7;
        private TextBox textBox_InvoiceUploadInterval;
        private Label label6;
    }
}
