using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PWP.InvoiceCapture.Core.Telemetry;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.UnitTests.Telemetry
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class OperationHolderAdapterTests
    {
        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
            operationHolderMock = mockRepository.Create<IOperationHolder<RequestTelemetry>>();

            target = new OperationHolderAdapter(operationHolderMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Instance_WhenOperationHolderIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new OperationHolderAdapter(null));
        }

        [TestMethod]
        public void Instance_ShouldImplementIDisposableInterface()
        {
            Assert.IsInstanceOfType(target, typeof(IDisposable));
        }

        [TestMethod]
        public void Dispose_ShouldCallOperationHolderDisposeMethod()
        {
            operationHolderMock
                .Setup(operationHolder => operationHolder.Dispose())
                .Verifiable();

            target.Dispose();
        }

        private Mock<IOperationHolder<RequestTelemetry>> operationHolderMock;
        private MockRepository mockRepository;
        private OperationHolderAdapter target;
    }
}
