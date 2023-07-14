using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.UnitTests.Fakes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace PWP.InvoiceCapture.Core.UnitTests.Services
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SerializationServiceTests
    {
        [TestInitialize]
        public void Initialize()
        {
            target = new Core.Services.SerializationService();
        }

        [TestMethod]
        public void Serialize_WhenObjectToSerializeIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => target.Serialize<FakeTestObject>(null));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        public void Deserialize_WhenJsonIsNullOrWhiteSpace_ShouldReturnArgumentNullException(string json)
        {
            Assert.ThrowsException<ArgumentNullException>(() => target.Deserialize<FakeTestObject>(json));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        public void Deserialize_WhenJsonIsNullOrWhiteSpaceAndTypeExists_ShouldReturnArgumentNullException(string jsonText)
        {
            Assert.ThrowsException<ArgumentNullException>(() => target.Deserialize(jsonText, typeof(FakeTestObject)));
        }

        [TestMethod]
        public void Deserialize_WhenTypeIsNull_ShouldReturnArgumentNullException()
        {
            var json = File.ReadAllText($"Files/{fileName}.json");
            Assert.ThrowsException<ArgumentNullException>(() => target.Deserialize(json, null));
        }

        [TestMethod]
        public void Deserialize_WhenStringContainsJsonAndTypeIsCorrect_ShouldReturnTestObject()
        {
            var json = File.ReadAllText($"Files/{fileName}.json");

            var deserializedObject = target.Deserialize(json, typeof(FakeTestObject)) as FakeTestObject;

            Assert.IsNotNull(deserializedObject);
            Assert.IsNotNull(deserializedObject.InnerObject);
            Assert.AreEqual(deserializedObject.Id, testObjectId);
            Assert.AreEqual(deserializedObject.Text, testObjectText);
            Assert.AreEqual(deserializedObject.InnerObject.X, testObjectX);
            Assert.AreEqual(deserializedObject.InnerObject.Y, testObjectY);
        }

        [TestMethod]
        public void Deserialize_WhenJsonContainsTestObjectJson_ShouldReturnTestObject()
        {
            var json = File.ReadAllText($"Files/{fileName}.json");

            var deserializedObject = target.Deserialize<FakeTestObject>(json);

            Assert.IsNotNull(deserializedObject);
            Assert.IsNotNull(deserializedObject.InnerObject);
            Assert.AreEqual(deserializedObject.Id, testObjectId);
            Assert.AreEqual(deserializedObject.Text, testObjectText);
            Assert.AreEqual(deserializedObject.InnerObject.X, testObjectX);
            Assert.AreEqual(deserializedObject.InnerObject.Y, testObjectY);
        }

        [TestMethod]
        public void Serialize_WhenTestObjectIsNotNull_ShouldReturnTestObjectJson()
        {
            var testObject = CreateTestObject();

            var json = target.Serialize(testObject);
                
            Assert.IsFalse(string.IsNullOrWhiteSpace(json));

            var deserializedObject = target.Deserialize<FakeTestObject>(json);

            Assert.AreEqual(deserializedObject.Id, testObject.Id);
            Assert.AreEqual(deserializedObject.Text, testObject.Text);
            Assert.IsNotNull(testObject.InnerObject);
            Assert.AreEqual(deserializedObject.InnerObject.X, testObject.InnerObject.X);
            Assert.AreEqual(deserializedObject.InnerObject.Y, testObject.InnerObject.Y);
        }

        private FakeTestObject CreateTestObject()
        {
            return new FakeTestObject
            {
                Id = testObjectId,
                Text = testObjectText,
                InnerObject = new FakeInnerTestObject
                {
                    X = testObjectX,
                    Y = testObjectY
                }
            };
        }

        private Core.Services.SerializationService target;
        
        private const string fileName = "test";
        private const string testObjectText = "Serialization unit test";
        private const int testObjectId = 1;
        private const float testObjectX = 555;
        private const float testObjectY = 99;
    }
}
