using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.DocumentAggregation.Business.Services;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.DocumentAggregation.Business.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class EmailServiceTest
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new EmailService();
        }

        [TestMethod]
        [DataRow("wrongEmail.com")]
        [DataRow("wrongEmail@asddsf")]
        [DataRow("@sdfsdf.com")]
        public void FindEmail_WhenTextContainsIncorrectEmail_ShouldReturnEmptyString(string wrongEmail)
        {
            var textWithWrongEmail = $"Email was send to {wrongEmail}.";

            var email = target.FindEmail(textWithWrongEmail);

            Assert.IsTrue(string.IsNullOrWhiteSpace(email));
        }

        [TestMethod]
        [DataRow("testEmail@email.com")]
        [DataRow("blablabla@EmAILINVOICE.workplacecloud.com")]
        [DataRow("testEmail@emailinvoice.workplacecloud.com")]
        [DataRow("blablabla@emailinvoice.WorkplacECloud.Com")]
        public void FindEmail_WhenTextContainsCorrectEmail_ShouldReturnEmail(string correctEmail)
        {
            var textWithEmail = $"Email was send to {correctEmail}.";

            var email = target.FindEmail(textWithEmail);

            Assert.IsFalse(string.IsNullOrWhiteSpace(email));
            Assert.AreEqual(correctEmail, email);
        }

        private EmailService target;
    }
}
