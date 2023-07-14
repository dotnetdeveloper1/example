using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Contracts;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Database;
using PWP.InvoiceCapture.InvoiceManagement.DataAccess.Repositories;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.UnitTests.Repositories
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class InvoiceTemplateCultureSettingRepositoryTests
    {
        [TestInitialize]
        public void Initialize()
        {
            contextFactory = new InMemoryDatabaseContextFactory();
            context = (DatabaseContext)contextFactory.Create();
            target = new InvoiceTemplateCultureSettingRepository(contextFactory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [TestMethod]
        public void Instance_WhenDbContextFactoryIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InvoiceTemplateCultureSettingRepository(null));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceTemplateCultureSettingIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.CreateAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task CreateAsync_WhenInvoiceTemplateCultureSettingIsNotNull_ShouldSave()
        {
            var expectedInvoiceTemplateCultureSetting = CreateInvoiceTemplateCultureSetting(1);

            await target.CreateAsync(expectedInvoiceTemplateCultureSetting, cancellationToken);

            var actualInvoiceTemplateCultureSetting = context.InvoiceTemplateCultureSettings.FirstOrDefault();

            Assert.IsNotNull(actualInvoiceTemplateCultureSetting);
            Assert.AreEqual(1, context.InvoiceTemplateCultureSettings.Count());

            AssertInvoiceTemplateCultureSettingsAreEqual(expectedInvoiceTemplateCultureSetting, actualInvoiceTemplateCultureSetting);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenInvoiceTemplateCultureSettingIsNull_ShouldThrowArgumentNullException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                target.UpdateAsync(null, cancellationToken));
        }

        [TestMethod]
        public async Task UpdateAsync_WhenInvoiceTemplateCultureSettingIsNotNull_ShouldUpdate()
        {
            var firstExpected = CreateInvoiceTemplateCultureSetting(1);
            var secondExpected = CreateInvoiceTemplateCultureSetting(2);
            context.InvoiceTemplateCultureSettings.Add(firstExpected);
            context.InvoiceTemplateCultureSettings.Add(secondExpected);
            context.SaveChanges();
            
            var newCultureName = "someNew";
            secondExpected.CultureName = newCultureName;
            
            await target.UpdateAsync(secondExpected, cancellationToken);

            var actualFirstInvoiceTemplateCultureSetting = context.InvoiceTemplateCultureSettings.ToArray()[0];
            var actualSecondInvoiceTemplateCultureSetting = context.InvoiceTemplateCultureSettings.ToArray()[1];


            Assert.IsNotNull(actualSecondInvoiceTemplateCultureSetting);
            Assert.AreEqual(newCultureName, actualSecondInvoiceTemplateCultureSetting.CultureName);
            Assert.AreEqual("TemplateId2", actualSecondInvoiceTemplateCultureSetting.TemplateId);
            Assert.AreNotEqual(firstExpected.ModifiedDate, actualSecondInvoiceTemplateCultureSetting.ModifiedDate);

            AssertInvoiceTemplateCultureSettingsAreEqual(firstExpected, actualFirstInvoiceTemplateCultureSetting);
        }

        [DataRow("")]
        [DataRow(null)]
        [DataRow(" ")]
        [TestMethod]
        public async Task GetByTemplateIdAsync_WhenTemplateIdIsNullOrWhitespace_ShouldThrowArgumentNullException(string templateId)
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
               target.GetByTemplateIdAsync(templateId, cancellationToken));
        }

        [TestMethod]
        public async Task GetByTemplateIdAsync_ShouldReturnInvoiceTemplateCultureSetting()
        {
            var firstExpected = CreateInvoiceTemplateCultureSetting(1);
            var secondExpected = CreateInvoiceTemplateCultureSetting(2);
            context.InvoiceTemplateCultureSettings.Add(firstExpected);
            context.InvoiceTemplateCultureSettings.Add(secondExpected);
            context.SaveChanges();

            var actual = await target.GetByTemplateIdAsync("TemplateId2", cancellationToken);

            Assert.IsNotNull(actual);
            AssertInvoiceTemplateCultureSettingsAreEqual(secondExpected, actual);
        }

        private InvoiceTemplateCultureSetting CreateInvoiceTemplateCultureSetting(int seed)
        {
            return new InvoiceTemplateCultureSetting
            {
                Id = seed,
                TemplateId = $"TemplateId{seed}",
                CultureName = $"CultureName{seed}"
            };
        }

        private void AssertInvoiceTemplateCultureSettingsAreEqual(InvoiceTemplateCultureSetting expected, InvoiceTemplateCultureSetting actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.TemplateId, actual.TemplateId);
            Assert.AreEqual(expected.CultureName, actual.CultureName);
            Assert.AreEqual(expected.CreatedDate, actual.CreatedDate);
            Assert.AreEqual(expected.ModifiedDate, actual.ModifiedDate);
          
            // Ensure all properties are tested
            Assert.AreEqual(5, actual.GetType().GetProperties().Length);
        }

        private DatabaseContext context;
        private IDatabaseContextFactory contextFactory;
        private IInvoiceTemplateCultureSettingRepository target;
        private readonly CancellationToken cancellationToken = CancellationToken.None;
    }
}
