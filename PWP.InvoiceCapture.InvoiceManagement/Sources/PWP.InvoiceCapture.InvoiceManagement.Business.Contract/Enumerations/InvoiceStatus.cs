namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations
{
    public enum InvoiceStatus
    {
        NotStarted = 0,
        Queued = 1,
        InProgress = 2,
        PendingReview = 3,
        Completed = 4,
        Error = 5,
        LimitExceeded = 6
    }
}
