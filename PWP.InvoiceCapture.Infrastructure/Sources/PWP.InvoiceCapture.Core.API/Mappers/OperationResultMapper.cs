using Microsoft.AspNetCore.Mvc;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using System;

namespace PWP.InvoiceCapture.Core.API.Mappers
{
    public static class OperationResultMapper
    {
        public static IActionResult ToActionResult(this OperationResult operationResult)
        {
            Guard.IsNotNull(operationResult, nameof(operationResult));

            return ToActionResult(operationResult, operationResult.ToApiResponse());
        }

        public static IActionResult ToActionResult<TData>(this OperationResult<TData> operationResult)
        {
            Guard.IsNotNull(operationResult, nameof(operationResult));

            return ToActionResult(operationResult, operationResult.ToApiResponse());
        }

        public static IActionResult ToActionResult<TBusinessData, TPresentationData>(this OperationResult<TBusinessData> operationResult, Func<TBusinessData, TPresentationData> mapper)
        {
            Guard.IsNotNull(operationResult, nameof(operationResult));
            Guard.IsNotNull(mapper, nameof(mapper));

            return ToActionResult(operationResult, operationResult.ToApiResponse(mapper));
        }

        public static ApiResponse ToApiResponse(this OperationResult operationResult) 
        {
            if (operationResult == null)
            {
                return null;
            }

            return new ApiResponse
            {
                Code = operationResult.Code,
                Message = operationResult.Message
            };
        }

        public static ApiResponse<TData> ToApiResponse<TData>(this OperationResult<TData> operationResult)
        {
            if (operationResult == null)
            {
                return null;
            }

            return new ApiResponse<TData>
            {
                Code = operationResult.Code,
                Message = operationResult.Message,
                Data = operationResult.Data
            };
        }

        public static ApiResponse<TPresentationData> ToApiResponse<TBusinessData, TPresentationData>(this OperationResult<TBusinessData> operationResult, Func<TBusinessData, TPresentationData> mapper)
        {
            Guard.IsNotNull(mapper, nameof(mapper));

            if (operationResult == null)
            {
                return null;
            }

            return new ApiResponse<TPresentationData>
            {
                Code = operationResult.Code,
                Message = operationResult.Message,
                Data = mapper(operationResult.Data)
            };
        }

        private static IActionResult ToActionResult<TContent>(OperationResult operationResult, TContent content)
        {
            switch (operationResult.Status)
            {
                case OperationResultStatus.Success:
                    return new ObjectResult(content);
                case OperationResultStatus.Failed:
                    return new BadRequestObjectResult(content);
                case OperationResultStatus.Forbidden:
                    return new ForbidResult();
                case OperationResultStatus.NotFound:
                    return new NotFoundObjectResult(content);
            }

            throw new ArgumentException();
        }
    }
}
