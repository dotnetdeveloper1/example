using PWP.InvoiceCapture.Core.Enumerations;

namespace PWP.InvoiceCapture.Core.Models
{
    public class OperationResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public OperationResultStatus Status { get; set; }

        public bool IsSuccessful => Status == OperationResultStatus.Success;

        public static OperationResult Success => new OperationResult { Status = OperationResultStatus.Success };
        public static OperationResult Failed => new OperationResult { Status = OperationResultStatus.Failed };
        public static OperationResult Forbidden => new OperationResult { Status = OperationResultStatus.Forbidden };
        public static OperationResult NotFound => new OperationResult { Status = OperationResultStatus.NotFound };
    }

    public class OperationResult<TResult> : OperationResult 
    {
        public TResult Data { get; set; }

        public new static OperationResult<TResult> Success(TResult data) => new OperationResult<TResult> { Status = OperationResultStatus.Success, Data = data };
        public new static OperationResult<TResult> Failed => new OperationResult<TResult> { Status = OperationResultStatus.Failed };
        public new static OperationResult<TResult> Forbidden => new OperationResult<TResult> { Status = OperationResultStatus.Forbidden };
        public new static OperationResult<TResult> NotFound => new OperationResult<TResult> { Status = OperationResultStatus.NotFound };
        
    }
}
