using Newtonsoft.Json;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Document.API.Client.Contracts;
using PWP.InvoiceCapture.Document.API.Client.Models;
using PWP.InvoiceCapture.OCR.Core.Contract.Services;
using PWP.InvoiceCapture.OCR.Core.Contracts;
using PWP.InvoiceCapture.OCR.Core.DataAccess.Repositories;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Mappers;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Repositories;
using PWP.InvoiceCapture.OCR.Recognition.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TroubleShootingApp.Contracts;
using TroubleShootingApp.Models;
using TroubleShootingApp.Services;

namespace TroubleShootingApp
{
    public partial class InvoiceName : Form
    {
        public InvoiceName(IApplicationRunner appRunner)
        {
            InitializeComponent();
            SetDefaultValues();

            this.frClient = appRunner.GetFrClient();
            this.invoiceTemplateRepository = appRunner.GetInvoiceTemplateRepository();
            this.labelRepo = appRunner.GetLabelRepository();
            this.documentApiClient = appRunner.GetDocumentApiClient();
            this.recognitionEngine = appRunner.GetRecognitionEngine();
            this.trainingBlobRepository = appRunner.GetTrainingBlobRepository();
            this.tenantRepo = appRunner.GetTenantsRepository();
            this.invoiceRepo = appRunner.GetInvoicesRepository();
            this.fieldRepo = appRunner.GetFieldsRepository();
            this.fieldMapper = appRunner.GetFieldMapper();
        }

        private void SettingsTab_Click(object sender, EventArgs e)
        {

        }

        private void SetDefaultValues()
        {
            ApiAddress.Text = "https://pwp-invoice-capture-form-recognizer-dev.cognitiveservices.azure.com";
            ApiKey.Text = "fedad746a2724ccfa5fae8c43a99192c";
            ModelId.Text = "943c772e-22ac-4fc9-9382-ea6380c8f6ef";
            TenantId.Text = "Default";
        }

        private async void Button2_ClickAsync(object sender, EventArgs e)
        {
            string content;

            try
            {
                var modelListResponse = await frClient.GetListModelResponseAsync(defaultFormRecognizerId, CancellationToken.None);
                content = JsonConvert.SerializeObject(modelListResponse, Formatting.Indented);
            }
            catch (Exception ex)
            {
                content = "Error during API call: " + Environment.NewLine + ex.Message;
            }

            Output.Text = content;
        }

        private async void Button1_ClickAsync(object sender, EventArgs e)
        {
            var modelId = ModelId.Text;
            string content;

            try
            {   
                var modelResponse = await frClient.GetModelDetailsAsync(modelId, defaultFormRecognizerId, cancellationToken);
                content = JsonConvert.SerializeObject(modelResponse, Formatting.Indented);
            }
            catch (Exception ex)
            {
                content = "Error during API call: " + Environment.NewLine + ex.Message;
            }

            Output.Text = content;
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://fott.azurewebsites.net/") { UseShellExecute = true });
        }

        private async void Button3_ClickAsync(object sender, EventArgs e)
        {
            var templates = await invoiceTemplateRepository.GetAllAsync(cancellationToken);
            BindTemplates(templates);

        }

        private void BindTemplates(List<InvoiceTemplate> templates)
        {
            TemplatesList.DataSource = templates;
            TemplatesList.ValueMember = "Id";
            TemplatesList.DisplayMember = "Id";

        }

        private async void TemplatesList_SelectedValueChangedAsync(object sender, EventArgs e)
        {
            var id = ((InvoiceTemplate)((ListBox)sender).SelectedItem).Id;
            var template = await invoiceTemplateRepository.GetByIdAsync(id, cancellationToken);
            Output.Text = GetTemplateDescription(template);
        }

        private string GetTemplateDescription(InvoiceTemplate template)
        {
            var builder = new StringBuilder();
            builder.Append("Id:");
            builder.Append(Environment.NewLine);
            builder.Append(template.Id);
            builder.Append(Environment.NewLine);
            builder.Append("Tenant:");
            builder.Append(Environment.NewLine);
            builder.Append(template.TenantId);
            builder.Append(Environment.NewLine);
            builder.Append("Training File Count / Blob Container:");
            builder.Append(Environment.NewLine);
            builder.Append($"{template.TrainingFileCount} / {template.TrainingBlobUri}");
            builder.Append(Environment.NewLine);
            builder.Append("FR Model Id:");
            builder.Append(Environment.NewLine);
            builder.Append(template.FormRecognizerModelId);
            builder.Append(Environment.NewLine);

            return builder.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {   
        }

        private void SelectInvoice_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                SelectedInvoice.Text = file;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                SelectedPage.Text = file;
            }
        }

        private async void button5_Click_1(object sender, EventArgs e)
        {
            string fileId = string.Empty;
            string imageFileId = string.Empty;
            string tenantId = TenantId.Text;

            using(var fileStream = File.OpenRead(SelectedInvoice.Text))
            {
                ApiResponse<UploadDocumentResponse> apiResponse = await documentApiClient.UploadDocumentAsync(fileStream, "test.pdf", cancellationToken);
                fileId = apiResponse.Data.FileId;
            }

            using (var fileStream = File.OpenRead(SelectedPage.Text))
            {
                ApiResponse<UploadDocumentResponse> apiResponse = await documentApiClient.UploadDocumentAsync(fileStream, "test.pdf", cancellationToken);
                imageFileId = apiResponse.Data.FileId;
            }

            if(string.IsNullOrEmpty(fileId) || string.IsNullOrEmpty(imageFileId))
            {
                MessageBox.Show("Error uploading one of the documents to document API");
            }

            else
            {
                var notDeletedfields = await fieldRepo.GetNotDeletedAsync(tenantId);

                var fieldTargetFields = fieldMapper.ToFieldTargetFieldList(notDeletedfields);

                var response = await recognitionEngine.ProcessDocumentAsync(fieldTargetFields, invoiceId, fileId, imageFileId,tenantId,cancellationToken);
                var responseString = JsonConvert.SerializeObject(response, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                Output.Text = responseString;
            }

        }

        private async void button7_Click(object sender, EventArgs e)
        {
            var container = ContainerName.Text;
            var folder = TrainingDirectory.Text;
            Output.Text = "Downloading Invoice List... ";
            await trainingBlobRepository.DownloadBlobToFolderAsync(container, folder, new CancellationToken());
            Output.Text += "Done!";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var tenants = tenantRepo.GetAll();
            BindTenants(tenants); 
        }

        private void BindTenants(List<Tenant> tenants)
        {

            TenantsCombo.DataSource = tenants;
            TenantsCombo.ValueMember = "DatabaseName";
            TenantsCombo.DisplayMember = "Name";
        }

        private void DisplayInvoices(List<Invoice> invoices)
        {
            StringBuilder outputBuilder = new StringBuilder();
            foreach(var invoice in invoices)
            {
                outputBuilder.AppendLine("******");
                outputBuilder.AppendLine("Name: " + invoice.Name);
                outputBuilder.AppendLine("FileId: " + invoice.FileId);
                outputBuilder.AppendLine("File Name: " + invoice.FileName);
                outputBuilder.AppendLine("Status Id: " + invoice.StatusId);
                outputBuilder.AppendLine("Template Id: " + invoice.TemplateId);
                outputBuilder.AppendLine("******");

            }
            Output.Text = outputBuilder.ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Tenant tenantItem = ((Tenant)TenantsCombo.SelectedItem);
            var invoices = invoiceRepo.GetAll(tenantItem.Name);
            DisplayInvoices(invoices);
        }

        private readonly CancellationToken cancellationToken = CancellationToken.None;
        private readonly IFormRecognizerClient frClient;
        private readonly IInvoiceTemplateRepository invoiceTemplateRepository;
        private readonly ILabelOfInterestRepository labelRepo;
        private readonly IDocumentApiClient documentApiClient;
        private readonly IRecognitionEngine recognitionEngine;
        private readonly ITrainingBlobRepository trainingBlobRepository;
        private readonly Contracts.ITenantRepository tenantRepo;
        private readonly IInvoicesRepository invoiceRepo;
        private readonly IFieldsRepository fieldRepo;
        private readonly IFieldMapper fieldMapper;

        private const int invoiceId = -1;
        //TODO: Add dropdown list to the UI
        private const int defaultFormRecognizerId = 1;
    }
}
