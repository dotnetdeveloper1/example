using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Options;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ImageServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            calculationServiceMock = mockRepository.Create<ICalculationService>();
            target = new ImageService(GetValidOptions(), calculationServiceMock.Object);
        }

        [TestMethod]
        public void Instance_WhenCalculationServiceIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ImageService(GetValidOptions(), null));
        }

        [TestMethod]
        public void Instance_WhenConfigurationIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ImageService(null, calculationServiceMock.Object));
        }

        [TestMethod]
        [DataRow("txt")]
        public void Instance_WhenImageFormatIsNotSupportableIsSmall_ShouldThrowArgumentException(string fileExtension)
        {
            var options = GetValidOptions();
            options.Value.ImageFormat = fileExtension;

            Assert.ThrowsException<ArgumentException>(() => new ImageService(options, calculationServiceMock.Object));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("       ")]
        public void Instance_WhenImageFormatIsNullIsSmall_ShouldThrowArgumentException(string fileExtension)
        {
            var options = GetValidOptions();
            options.Value.ImageFormat = fileExtension;

            Assert.ThrowsException<ArgumentNullException>(() => new ImageService(options, calculationServiceMock.Object));
        }

        [TestMethod]
        public void Instance_WhenWidthIsSmall_ShouldThrowArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => new ImageService(GetInvalidWidthInMemoryOptions(), calculationServiceMock.Object));
        }

        [TestMethod]
        public void ConvertToDefaultFormatImages_WhenStreamIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => target.ConvertToDefaultFormatImages(null).FirstOrDefault());
        }

        [TestMethod]
        [DataRow(1000)]
        [DataRow(400)]
        public void ConvertToDefaultFormatImages_WhenStreamContainsImage_ShouldReturnOneImage(int calculatedHeight)
        {
            var validOptions = GetValidOptions();

            var bytes = File.ReadAllBytes($"Files/{imageFileName}.png");

            var pageSizes = new List<SizeF>();

            calculationServiceMock
                .Setup(calculationService => calculationService.CalculateHeight(validOptions.Value.Width, Capture.In(pageSizes)))
                .Returns(calculatedHeight);

            var images = target.ConvertToDefaultFormatImages(bytes);

            Assert.IsTrue(images.Count == 1);
            Assert.IsTrue(pageSizes.Count == 1);
            Assert.IsTrue(images.All(p => p.ImageData != null));
        }

        private IOptions<ImageConversionOptions> GetInvalidWidthInMemoryOptions()
        {
            return Options.Create<ImageConversionOptions>(new ImageConversionOptions()
            {
                Dpi = 300,
                Width = 0,
                ImageFormat = "png"
            });
        }

        private IOptions<ImageConversionOptions> GetInvalidDpiInMemoryOptions()
        {
            return Options.Create<ImageConversionOptions>(new ImageConversionOptions()
            {
                Dpi = 0,
                Width = 1280,
                ImageFormat = "png"
            });
        }

        private IOptions<ImageConversionOptions> GetValidOptions()
        {
            return Options.Create<ImageConversionOptions>(new ImageConversionOptions()
            {
                Dpi = 300,
                Width = 1280,
                ImageFormat = "png"
            });
        }

        private Mock<ICalculationService> calculationServiceMock;
        private MockRepository mockRepository;
        private ImageService target;

        private const string imageFileName = "0";
    }
}
