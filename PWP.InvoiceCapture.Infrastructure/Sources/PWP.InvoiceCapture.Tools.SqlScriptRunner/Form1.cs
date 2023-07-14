using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.Tools.SqlScriptRunner.Contract;
using PWP.InvoiceCapture.Tools.SqlScriptRunner.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PWP.InvoiceCapture.Tools.SqlScriptRunner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            radioButton_scriptFromFolder.Checked = true;
            var environment = comboBox_environment.Text;
            InitializeScriptRunningService(environment);
        }

        private void InitializeScriptRunningService(string environment)
        {
            var host = CreateHost(environment);
            var service = host.Services.GetRequiredService<IScriptRunningService>();
            this.scriptRunningService = service;
            UpdateTenantsDbList();
        }

        private void UpdateTenantsDbList()
        {
            try
            {
                tenantDabases = scriptRunningService.GetTenantsDatabasesList(textBox_getTenants.Text);
                UpdateTenantsDbCheckList(tenantDabases);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void UpdateOtherDbList()
        {
            try
            {
                otherDatabases = scriptRunningService.GetOtherDatabasesList(textBox_scriptOtherDb.Text);
                UpdateOtherDbCheckList(otherDatabases);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void UpdateTenantsDbCheckList(List<string> tenantDabases)
        {
            var index = 0;
            checkedListBox_tenants.Items.Clear();
            foreach (var item in tenantDabases)
            {
                checkedListBox_tenants.Items.Insert(index, item);
                checkedListBox_tenants.SetItemChecked(index, true);
                index++;
            }
        }

        private void UpdateOtherDbCheckList(List<string> otherDabases)
        {
            var index = 0;
            checkedListBox_otherDb.Items.Clear();
            foreach (var item in otherDabases)
            {
                checkedListBox_otherDb.Items.Insert(index, item);
                checkedListBox_otherDb.SetItemChecked(index, true);
                index++;
            }
        }

        private void button_run_Click(object sender, EventArgs e)
        {
            var environment = comboBox_environment.Text;
            var checkedDatabases = new List<string>();
           
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                { 
                    checkedDatabases = checkedListBox_tenants.CheckedItems.Cast<string>().ToList();
                    break;
                }
                
                case 1:
                { 
                    checkedDatabases = checkedListBox_otherDb.CheckedItems.Cast<string>().ToList();
                    break;
                }
            }

            if (radioButton_scriptFromFolder.Checked)
            {
                RunScriptFromFolder(environment, checkedDatabases);
            }
            else
            {
                RunEnteredScript(environment, checkedDatabases);
            }
        }

        private void RunEnteredScript(string environment, List<string> checkedDatabases)
        {
            var script = richTextBox_script.Text;
            var result = MessageBox.Show($"Are sure you want run script: {script}  for {environment} for {checkedDatabases.Count()} database(s) : {string.Join(", ", checkedDatabases)}",
                "RUN DATABASE UPDATE", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {

                var executionResult = scriptRunningService.RunScript(script, checkedDatabases.ToList());
                if (executionResult.Succeed)
                {
                    MessageBox.Show("Script was executed");
                }
                else
                {
                    MessageBox.Show(executionResult.Exception, "Script execution failed");
                }
            }
        }

        private void RunScriptFromFolder(string environment, List<string> checkedDatabases)
        {
            var scriptsToRun = scriptRunningService.GetScriptsToRunPerDatabase(checkedDatabases, label_scriptFolder.Text);
            var dryRunResult = scriptRunningService.GetDryRunResult(scriptsToRun);
            var result = MessageBox.Show($"Dry run result: {dryRunResult}Are sure you want run this for {environment}?", "RUN DATABASE UPDATE", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                var executionResult = scriptRunningService.RunScript(scriptsToRun);
                if (executionResult.Succeed)
                {
                    MessageBox.Show("Script was executed");
                }
                else
                {
                    MessageBox.Show(executionResult.Exception, "Script execution failed");
                }
            }
        }

        private IHost CreateHost(string environment)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) => ConfigureAppConfiguration(builder, environment))
                .ConfigureServices((context, builder) => ConfigureServices(context, builder, environment))
                .Build();
        }

        private void ConfigureAppConfiguration(IConfigurationBuilder builder, string environment)
        {
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environment}.json", optional: false)
                .AddJsonFile($"appsettings.json", optional: false);
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services, string environment)
        {
            ConfigureOptions(context, services, environment);

            services.AddTransient<IScriptRunningService, ScriptRunningService>();
        }

        private void ConfigureOptions(HostBuilderContext context, IServiceCollection services, string environment)
        {
            services.Configure<DatabaseOptions>(options => context.Configuration.GetSection("InvoicesDatabaseOptions").Bind(options));

            var settingsOptions = Microsoft.Extensions.Options.Options.Create(environment);
            services.AddTransient((serviceProvider) => settingsOptions);
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            UpdateTenantsDbList();
        }

        private void comboBox_environment_SelectedIndexChanged(object sender, EventArgs e)
        {
            var environment = comboBox_environment.Text;
            InitializeScriptRunningService(environment);
        }

        private void radioButton_scriptFromFolder_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox_script.Enabled = false;
            button_openFolder.Enabled = true;
            button_run.Enabled = false;
        }

        private void radioButton_enteredScript_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox_script.Enabled = true;
            button_openFolder.Enabled = false;
            label_scriptFolder.Text = string.Empty;
            button_run.Enabled = true;
        }

        private void button_openFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                label_scriptFolder.Text = folderBrowserDialog1.SelectedPath;
                button_run.Enabled = true;
            }
        }

        private void button_updateOtherDbList_Click(object sender, EventArgs e)
        {
            UpdateOtherDbList();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateOtherDbList();
        }

        private IScriptRunningService scriptRunningService;
        private List<string> tenantDabases;
        private List<string> otherDatabases;
    }
}
