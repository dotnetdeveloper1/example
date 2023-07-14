using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Document.Business.Contract.Models;
using PWP.InvoiceCapture.Document.DataAccess.Repositories;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Document.DataAccess.IntegrationTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class AzureBlobRepositoryTests
    {
        public AzureBlobRepositoryTests()
        {
            var documentStorageOptions = new DocumentStorageOptions()
            {
                BlobConnectionString = blobEmulatorConnectionString,
                BlobContainer = blobEmulatorContainer,
                LinkTimeToLiveInSeconds = 10,
                BlobRetryAttempts = 3,
                BlobRetryIntervalInSeconds = 2
            };
            var options = Options.Create(documentStorageOptions);
            repository = new AzureBlobRepository(options);
        }

        [TestMethod]
        [DataRow("testFile.txt")]
        public async Task SaveAsync_WhenFileSaved_ShouldReturnCorrectFileId(string expectedFileId)
        {
            var expectedContent = new MemoryStream(Encoding.UTF8.GetBytes(firstTestFileText));
            var actualId = await repository.SaveAsync(new CreateDocumentArgs() { FileContent = expectedContent, FileId = expectedFileId }, CancellationToken.None);

            Assert.IsNotNull(actualId);
            Assert.AreEqual(expectedFileId, actualId);
        }

        [TestMethod]
        [DataRow("testFile.txt")]
        public async Task SaveAsync_GetStreamAsync_ShouldReturnTheSameFile(string expectedFileId)
        {
            var actualContent = string.Empty;
            var expectedContent = new MemoryStream(Encoding.UTF8.GetBytes(firstTestFileText));
            var savedId = await repository.SaveAsync(new CreateDocumentArgs() { FileContent = expectedContent, FileId = expectedFileId }, CancellationToken.None);
            var documentStreamResult = await repository.GetStreamAsync(savedId, CancellationToken.None);
            
            using (var reader = new StreamReader(documentStreamResult.FileStream))
            {
                actualContent = reader.ReadToEnd();
            }

            Assert.AreEqual(firstTestFileText, actualContent);
        }

        [TestMethod]
        [DataRow("testFile.txt")]
        public async Task GetStreamAsync_WhenFileDoesNotExist_ShouldReturnNull(string expectedFileId)
        {
            var fileContent = new MemoryStream(Encoding.UTF8.GetBytes(firstTestFileText));
            var fileId = await repository.SaveAsync(new CreateDocumentArgs() { FileContent = fileContent, FileId = expectedFileId }, CancellationToken.None);
            var documentStreamResult = await repository.GetStreamAsync("somefake.txt", CancellationToken.None);
            
            Assert.IsNotNull(fileId);
            Assert.IsNull(documentStreamResult);
        }

        [TestMethod]
        [DataRow("testFile.txt")]
        public async Task SaveAsync_WhenFileAlreadyExists_ShouldOverwite(string expectedFileId)
        {
            var firstFileContent = new MemoryStream(Encoding.UTF8.GetBytes(firstTestFileText));
            var expectedContent = new MemoryStream(Encoding.UTF8.GetBytes(secondTestFileText));
            var actualContent = string.Empty;
            var firstSavedFileId = await repository.SaveAsync(new CreateDocumentArgs() { FileContent = firstFileContent, FileId = expectedFileId }, CancellationToken.None);
            var secondSavedFileId = await repository.SaveAsync(new CreateDocumentArgs() { FileContent = expectedContent, FileId = expectedFileId }, CancellationToken.None);
            var documentStreamResult = await repository.GetStreamAsync(expectedFileId, CancellationToken.None);
            
            using (var reader = new StreamReader(documentStreamResult.FileStream))
            {
                actualContent = reader.ReadToEnd();
            }

            Assert.IsNotNull(firstSavedFileId);
            Assert.IsNotNull(secondSavedFileId);
            Assert.AreEqual(secondTestFileText, actualContent);
        }

        [TestMethod]
        [DataRow("testFile.txt")]
        public async Task GetTemporaryLinkAsync_WhenFileDoesNotExist_ShouldReturnNull(string expectedFileId)
        {
            var link = string.Empty;
            using (var fileContent = new MemoryStream(Encoding.UTF8.GetBytes(firstTestFileText)))
            {
                var fileId = await repository.SaveAsync(new CreateDocumentArgs() { FileContent = fileContent, FileId = expectedFileId }, CancellationToken.None);
                link = await repository.GetTemporaryLinkAsync("somefake.txt", cancellationToken);
            }

            Assert.IsNull(link);
        }

        [TestMethod]
        [DataRow("testFile.txt")]
        public async Task GetTemporaryLinkAsync_ShouldReturnLinkAsync(string expectedFileId)
        {
            await GenerateLinkAndCheck(expectedFileId);
        }

        [TestMethod]
        [DataRow("testFile.txt")]
        public async Task GetTemporaryLinkAsync_ShouldExpireAfterTTL(string expectedFileId)
        {
            var link = await GenerateLinkAndCheck(expectedFileId);

            await Task.Delay(10001);
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => CheckLinkContent(link));
        }

        private async Task<string> GenerateLinkAndCheck(string expectedFileId)
        {
            var expectedContent = new MemoryStream(Encoding.UTF8.GetBytes(firstTestFileText));
            await repository.SaveAsync(new CreateDocumentArgs() { FileContent = expectedContent, FileId = expectedFileId }, CancellationToken.None);
            var link = await repository.GetTemporaryLinkAsync(expectedFileId, cancellationToken);

            Assert.IsNotNull(link);
            Assert.IsFalse(string.IsNullOrWhiteSpace(link));
            Assert.IsTrue(Uri.IsWellFormedUriString(link, UriKind.Absolute));

            await CheckLinkContent(link);

            return link;
        }

        private async Task CheckLinkContent(string link)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var downloadedText = string.Empty;
            
            using (var stream = await response.Content.ReadAsStreamAsync())           
            using (var reader = new StreamReader(stream))
            {
                downloadedText = reader.ReadToEnd();
            }

            Assert.AreEqual(firstTestFileText, downloadedText);
        }

        private readonly AzureBlobRepository repository;
        private const string blobEmulatorConnectionString = "UseDevelopmentStorage=true";
        private const string blobEmulatorContainer = "document-files";
        private const string firstTestFileText = "firstTestFileText";
        private const string secondTestFileText = "secondTestFileText";
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
