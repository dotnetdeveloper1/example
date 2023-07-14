using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Attributes;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models
{
    //TODO: Move to Core package
    public class PaginatedRequest
    {
        [MinValue(1)]
        public int PageNumber { get; set; } = 1;

        [MinValue(1)]
        public int ItemsPerPage { get; set; } = 10;

        [EnumType]
        public SortType SortType { get; set; }
    }

    public class PaginatedRequest<TSortField> : PaginatedRequest where TSortField : struct
    {
        [EnumType]
        public TSortField SortBy { get; set; }
    }
}
