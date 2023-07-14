using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.OCR.Core.Models;
using PWP.InvoiceCapture.OCR.DataAnalysis.API.Controllers;
using PWP.InvoiceCapture.OCR.DataAnalysis.Business.Contract.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.DataAnalysis.API.UnitTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InvoiceTemplatesControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            templateManagementServiceMock = mockRepository.Create<ITemplateManagementService>();

            target = new InvoiceTemplatesController(templateManagementServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public async Task GetTemplateTrainingsCountAsync_WhenValueIsLessOrEqualsZero_ShouldThrowArgumentExceptionAsync(int templateId)
        {
            var trainingsCountActionResult = await target.GetTrainingsCountAsync(templateId, cancellationToken);

            AssertActionResultIsValid(trainingsCountActionResult, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        [DataRow(10)]
        [DataRow(123)]
        public async Task GetTrainingsCountAsync_WhenCannotFindTemplate_ShouldReturnNotFoundAsync(int templateId)
        {
            templateManagementServiceMock
                .Setup(templateManagementService => templateManagementService.GetTemplateAsync(templateId, cancellationToken))
                .ReturnsAsync((InvoiceTemplate)null);

            var trainingsCountActionResult = await target.GetTrainingsCountAsync(templateId, cancellationToken);

            AssertActionResultIsValid(trainingsCountActionResult, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        [DataRow(10, 12)]
        [DataRow(123, 3333)]
        public async Task GetTrainingsCountAsync_WhenValueIsMoreThenZero_ShouldReturnCountAsync(int templateId, int count)
        {
            templateManagementServiceMock
                .Setup(templateManagementService => templateManagementService.GetTemplateAsync(templateId, cancellationToken))
                .ReturnsAsync(new InvoiceTemplate() { Id = templateId });

            templateManagementServiceMock
                .Setup(templateManagementService => templateManagementService.GetTemplateTrainingsCountAsync(templateId, cancellationToken))
                .ReturnsAsync(count);

            var trainingsCountActionResult = await target.GetTrainingsCountAsync(templateId, cancellationToken) as OkObjectResult;

            AssertActionResultIsValid(trainingsCountActionResult, typeof(OkObjectResult));

            var apiResponce = trainingsCountActionResult.Value as ApiResponse<int>;

            Assert.IsNotNull(apiResponce);
            Assert.AreEqual(count, apiResponce.Data);
        }

        private void AssertActionResultIsValid(IActionResult result, Type expectedType)
        {
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, expectedType);

            var objectResult = (ObjectResult)result;
            Assert.IsNotNull(objectResult.Value);
        }

        private Mock<ITemplateManagementService> templateManagementServiceMock;

        private InvoiceTemplatesController target;
        private MockRepository mockRepository;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
