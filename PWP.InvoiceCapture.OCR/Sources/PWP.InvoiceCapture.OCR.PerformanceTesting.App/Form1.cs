using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PWP.InvoiceCapture.Core.DataAccess.Extensions;
using PWP.InvoiceCapture.Core.Extensions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Contracts;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Database;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Models;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Options;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Repositories;
using PWP.InvoiceCapture.OCR.PerformanceTesting.App.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Button_Start_ClickAsync(object sender, EventArgs e)
        {
            var confirmationResult = MessageBox.Show("Run OCR Performance testing?", "OCR Performance Testing Confirmation", MessageBoxButtons.OKCancel);

            if (confirmationResult == DialogResult.Cancel)
            {
                return;
            }

            SwitchButtons(false);

            var stopwatch = Stopwatch.StartNew();
            var settings = GetSettings();
            await ProcessAsync(settings);

            SwitchButtons(true);

            MessageBox.Show($"Task finished in {stopwatch.Elapsed}", "Task finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void Button_CollectInvoices_ClickAsync(object sender, EventArgs e)
        {
            var confirmationResult = MessageBox.Show("Run Invoice Collector?", "Collector Confirmation", MessageBoxButtons.OKCancel);

            if (confirmationResult == DialogResult.Cancel)
            {
                return;
            }

            SwitchButtons(false);

            var stopwatch = Stopwatch.StartNew();
            var settings = GetSettings();

            await CollectAsync(settings);

            SwitchButtons(true);

            MessageBox.Show($"Task finished  in {stopwatch.Elapsed}", "Task finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox_FolderPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private async Task ProcessAsync(Settings settings)
        {
            var host = CreateHost(settings);
            var service = host.Services.GetRequiredService<IProcessingService>();

            await service.ProcessAsync(cancellationToken);
        }

        private async Task CollectAsync(Settings settings)
        {
            var host = CreateHost(settings);
            var service = host.Services.GetRequiredService<ICollectorService>();

            await service.CollectAsync(cancellationToken);
        }

        private IHost CreateHost(Settings settings) 
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) => ConfigureAppConfiguration(builder, settings))
                .ConfigureServices((context, builder) => ConfigureServices(context, builder, settings))
                .Build();
        }

        private void ConfigureAppConfiguration(IConfigurationBuilder builder, Settings settings)
        {
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{settings.Environment}.json", optional: false)
                .AddJsonFile($"appsettings.json", optional: false);
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services, Settings settings)
        {
            ConfigureOptions(context, services, settings);

            services.AddInvoicesDatabaseNameProvider();
            services.AddTransient<IDocumentAggregationClient, DocumentAggregationClient>();
            services.AddTransient<IInvoiceManagementClient, InvoiceManagementClient>();
            services.AddTransient<IProcessingService, ProcessingService>();
            services.AddTransient<ICollectorService, CollectorService>();
            services.AddTransient<IDataAnnotationComparisonService, DataAnotationComparisonService>();
            services.AddTransient<IReportingService, ExcelReportingService>();
            services.AddTransient<IInvoicesDatabaseConnectionStringProvider, InvoicesDatabaseConnectionStringProvider>();
            services.AddTransient<IInvoiceProcessingResultRepository, InvoiceProcessingResultRepository>();
            services.AddTransient<IInvoicesDatabaseContextFactory, InvoicesDatabaseContextFactory>();
        }

        private void ConfigureOptions(HostBuilderContext context, IServiceCollection services, Settings settings) 
        {
            services.Configure<AuthenticationOptions>(options => SetupAuthenticationOptions(context, options, settings));
            services.Configure<DatabaseOptions>(options => context.Configuration.GetSection("InvoicesDatabaseOptions").Bind(options));

            var settingsOptions = Microsoft.Extensions.Options.Options.Create(settings);
            services.AddTransient((serviceProvider) => settingsOptions);
        }

        private void SetupAuthenticationOptions(HostBuilderContext context, AuthenticationOptions options, Settings settings) 
        {
            context.Configuration.GetSection("AuthenticationOptions").Bind(options);

            options.Username = settings.UserName;
            options.Password = settings.Password;
            options.TenantId = settings.TenantId;
        }

        private Settings GetSettings()
        {
            return new Settings()
            {
                Environment = comboBox_environment.Text,
                FolderPath = textBox_FolderPath.Text,
                TenantId = Convert.ToInt32(textBox_tenantId.Text.Trim()),
                Password = textBox_password.Text,
                UserName = textBox_userName.Text,
                ThreadsCount = Convert.ToInt32(textBox_ThreadsCount.Text.Trim()),
                InvoicePollingIntervalMilliseconds = Convert.ToInt32(textBox_InvoicePollingInterval.Text.Trim()),
                InvoiceUploadingIntervalMilliseconds = Convert.ToInt32(textBox_InvoiceUploadInterval.Text.Trim())
            };
        }

        private void SwitchButtons(bool isEnabled) 
        {
            button_collectInvoices.Enabled = isEnabled;
            button_start.Enabled = isEnabled;
        }

        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
