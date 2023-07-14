namespace PWP.InvoiceCapture.Core.Models
{
    public class ApiResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class ApiResponse<TResult> : ApiResponse
    {
        public TResult Data { get; set; }
    }
}
