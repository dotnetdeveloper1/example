using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PWP.InvoiceCapture.Core.API.Mappers;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PWP.InvoiceCapture.Core.API.UnitTests.Mappers
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class OperationResultMapperTests
    {
        [TestMethod]
        public void ToActionResult_WhenOperationResultIsNull_ShouldThrowArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => OperationResultMapper.ToActionResult(null));
            Assert.ThrowsException<ArgumentNullException>(() => OperationResultMapper.ToActionResult<string>(null));
            Assert.ThrowsException<ArgumentNullException>(() => OperationResultMapper.ToActionResult<string, string>(null, MapStringData));
        }

        [TestMethod]
        public void ToActionResult_WhenUnknownOperationResultStatus_ShouldThrowArgumentException()
        {
            var invalidStatus = (OperationResultStatus)123456;

            Assert.ThrowsException<ArgumentException>(() => OperationResultMapper.ToActionResult(new OperationResult { Status = invalidStatus }));
            Assert.ThrowsException<ArgumentException>(() => OperationResultMapper.ToActionResult(new OperationResult<string> { Status = invalidStatus }));
            Assert.ThrowsException<ArgumentException>(() => OperationResultMapper.ToActionResult(new OperationResult<string> { Status = invalidStatus, Data = string.Empty }, MapStringData));
        }

        [TestMethod]
        [DataRow(0, "", OperationResultStatus.Success, typeof(ObjectResult))]
        [DataRow(10, "message", OperationResultStatus.Success, typeof(ObjectResult))]
        [DataRow(0, null, OperationResultStatus.Success, typeof(ObjectResult))]
        [DataRow(0, "", OperationResultStatus.Failed, typeof(BadRequestObjectResult))]
        [DataRow(10, "message", OperationResultStatus.Failed, typeof(BadRequestObjectResult))]
        [DataRow(0, null, OperationResultStatus.Failed, typeof(BadRequestObjectResult))]
        [DataRow(0, "", OperationResultStatus.NotFound, typeof(NotFoundObjectResult))]
        [DataRow(10, "message", OperationResultStatus.NotFound, typeof(NotFoundObjectResult))]
        [DataRow(0, null, OperationResultStatus.NotFound, typeof(NotFoundObjectResult))]
        public void ToActionResult_WhenResultStatusIsSuccessOrFailedOrNotFound_ShouldReturnObjectResult(int code, string message, OperationResultStatus status, Type expectedType)
        {
            var operationResult = new OperationResult 
            {
                Status = status,
                Code = code,
                Message = message
            };

            var actionResult = OperationResultMapper.ToActionResult(operationResult);

            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, expectedType);

            var result = (ObjectResult)actionResult;
            Assert.IsInstanceOfType(result.Value, typeof(ApiResponse));

            var content = (ApiResponse)result.Value;
            Assert.AreEqual(operationResult.Code, content.Code);
            Assert.AreEqual(operationResult.Message, content.Message);
        }

        [TestMethod]
        [DataRow(0, "")]
        [DataRow(10, "message")]
        [DataRow(0, null)]
        public void ToActionResult_WhenResultStatusIsForbidden_ShouldReturnForbidResult(int code, string message) 
        {
            var operationResult = new OperationResult
            {
                Status = OperationResultStatus.Forbidden,
                Code = code,
                Message = message
            };

            var actionResult = OperationResultMapper.ToActionResult(operationResult);

            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
        }

        [TestMethod]
        [DataRow(0, "", "", OperationResultStatus.Success, typeof(ObjectResult))]
        [DataRow(10, "message", "data", OperationResultStatus.Success, typeof(ObjectResult))]
        [DataRow(0, null, null, OperationResultStatus.Success, typeof(ObjectResult))]
        [DataRow(0, "", "", OperationResultStatus.Failed, typeof(BadRequestObjectResult))]
        [DataRow(10, "message", "data", OperationResultStatus.Failed, typeof(BadRequestObjectResult))]
        [DataRow(0, null, null, OperationResultStatus.Failed, typeof(BadRequestObjectResult))]
        [DataRow(0, "", "", OperationResultStatus.NotFound, typeof(NotFoundObjectResult))]
        [DataRow(10, "message", "data", OperationResultStatus.NotFound, typeof(NotFoundObjectResult))]
        [DataRow(0, null, null, OperationResultStatus.NotFound, typeof(NotFoundObjectResult))]
        public void ToActionResult_WhenGenericAndResultStatusIsSuccessOrFailedOrNotFound_ShouldReturnObjectResult(int code, string message, string data, OperationResultStatus status, Type expectedType)
        {
            var operationResult = new OperationResult<string>
            {
                Status = status,
                Code = code,
                Message = message,
                Data = data
            };

            var actionResult = OperationResultMapper.ToActionResult(operationResult);

            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, expectedType);

            var result = (ObjectResult)actionResult;
            Assert.IsInstanceOfType(result.Value, typeof(ApiResponse<string>));

            var content = (ApiResponse<string>)result.Value;
            Assert.AreEqual(operationResult.Code, content.Code);
            Assert.AreEqual(operationResult.Message, content.Message);
            Assert.AreEqual(operationResult.Data, content.Data);
        }

        [TestMethod]
        [DataRow(0, "", "")]
        [DataRow(10, "message", "data")]
        [DataRow(0, null, null)]
        public void ToActionResult_WhenGenericAndResultStatusIsForbidden_ShouldReturnForbidResult(int code, string message, string data)
        {
            var operationResult = new OperationResult<string>
            {
                Status = OperationResultStatus.Forbidden,
                Code = code,
                Message = message,
                Data = data
            };

            var actionResult = OperationResultMapper.ToActionResult(operationResult);

            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
        }

        [TestMethod]
        public void ToActionResult_WhenMapperIsNull_ShouldThrowArgumentNullException() 
        {
            var operationResult = new OperationResult<string>();

            Assert.ThrowsException<ArgumentNullException>(() => OperationResultMapper.ToActionResult<string, string>(operationResult, null));
        }

        [TestMethod]
        [DataRow(0, "", "", OperationResultStatus.Success, typeof(ObjectResult))]
        [DataRow(10, "message", "data", OperationResultStatus.Success, typeof(ObjectResult))]
        [DataRow(0, "", "", OperationResultStatus.Failed, typeof(BadRequestObjectResult))]
        [DataRow(10, "message", "data", OperationResultStatus.Failed, typeof(BadRequestObjectResult))]
        [DataRow(0, "", "", OperationResultStatus.NotFound, typeof(NotFoundObjectResult))]
        [DataRow(10, "message", "data", OperationResultStatus.NotFound, typeof(NotFoundObjectResult))]
        public void ToActionResult_WhenMapperIsProvidedAndResultStatusIsSuccessOrFailedOrNotFound_ShouldReturnObjectResult(int code, string message, string data, OperationResultStatus status, Type expectedType)
        {
            var operationResult = new OperationResult<string>
            {
                Status = status,
                Code = code,
                Message = message,
                Data = data
            };

            var actionResult = OperationResultMapper.ToActionResult(operationResult, MapStringData);

            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, expectedType);

            var result = (ObjectResult)actionResult;
            Assert.IsInstanceOfType(result.Value, typeof(ApiResponse<string>));

            var content = (ApiResponse<string>)result.Value;
            Assert.AreEqual(operationResult.Code, content.Code);
            Assert.AreEqual(operationResult.Message, content.Message);
            Assert.AreEqual(MapStringData(operationResult.Data), content.Data);
        }

        [TestMethod]
        [DataRow(0, "", "")]
        [DataRow(10, "message", "data")]
        public void ToActionResult_WhenMapperProvidedAndResultStatusIsForbidden_ShouldReturnForbidResult(int code, string message, string data)
        {
            var operationResult = new OperationResult<string>
            {
                Status = OperationResultStatus.Forbidden,
                Code = code,
                Message = message,
                Data = data
            };

            var actionResult = OperationResultMapper.ToActionResult(operationResult, MapStringData);

            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(ForbidResult));
        }

        [TestMethod]
        public void ToApiResponse_WhenOperationResultIsNull_ShouldReturnNull()
        {
            Assert.IsNull(OperationResultMapper.ToApiResponse(null));
            Assert.IsNull(OperationResultMapper.ToApiResponse<string>(null));
            Assert.IsNull(OperationResultMapper.ToApiResponse<string, string>(null, MapStringData));
        }

        [TestMethod]
        [DataRow(0, "")]
        [DataRow(10, "message")]
        [DataRow(0, null)]
        public void ToApiResponse_WhenOperationResultIsNotNull_ShouldMapToApiResponse(int code, string message)
        {
            var operationResult = new OperationResult
            {
                Code = code,
                Message = message
            };

            var apiResponse = OperationResultMapper.ToApiResponse(operationResult);

            Assert.IsNotNull(apiResponse);
            Assert.IsInstanceOfType(apiResponse, typeof(ApiResponse));
            Assert.AreEqual(operationResult.Code, apiResponse.Code);
            Assert.AreEqual(operationResult.Message, apiResponse.Message);
        }

        [TestMethod]
        [DataRow(0, "", "")]
        [DataRow(10, "message", "data")]
        [DataRow(0, null, null)]
        public void ToApiResponse_WhenGenericOperationResultIsNotNull_ShouldMapToApiResponse(int code, string message, string data)
        {
            var operationResult = new OperationResult<string>
            {
                Code = code,
                Message = message,
                Data = data
            };

            var apiResponse = OperationResultMapper.ToApiResponse(operationResult);

            Assert.IsNotNull(apiResponse);
            Assert.IsInstanceOfType(apiResponse, typeof(ApiResponse<string>));
            Assert.AreEqual(operationResult.Code, apiResponse.Code);
            Assert.AreEqual(operationResult.Message, apiResponse.Message);
            Assert.AreEqual(operationResult.Data, apiResponse.Data);
        }

        [TestMethod]
        public void ToApiResponse_WhenMapperIsNull_ShouldThrowArgumentNullException()
        {
            var operationResult = new OperationResult<string>();

            Assert.ThrowsException<ArgumentNullException>(() => OperationResultMapper.ToApiResponse<string, string>(operationResult, null));
        }

        [TestMethod]
        [DataRow(0, "", "")]
        [DataRow(10, "message", "data")]
        public void ToApiResponse_WhenMapperProvided_ShouldMapToApiResponse(int code, string message, string data)
        {
            var operationResult = new OperationResult<string>
            {
                Code = code,
                Message = message,
                Data = data
            };

            var apiResponse = OperationResultMapper.ToApiResponse(operationResult, MapStringData);

            Assert.IsNotNull(apiResponse);
            Assert.IsInstanceOfType(apiResponse, typeof(ApiResponse<string>));
            Assert.AreEqual(operationResult.Code, apiResponse.Code);
            Assert.AreEqual(operationResult.Message, apiResponse.Message);
            Assert.AreEqual(MapStringData(operationResult.Data), apiResponse.Data);
        }

        private string MapStringData(string data) => data.ToUpper();
    }
}
