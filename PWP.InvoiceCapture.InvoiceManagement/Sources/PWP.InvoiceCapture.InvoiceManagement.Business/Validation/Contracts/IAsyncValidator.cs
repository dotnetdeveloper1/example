using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts
{
    public interface IAsyncValidator<TEntity>
    {
        Task<ValidationResult> ValidateAsync(TEntity entity, CancellationToken cancellationToken);
    }

    public interface IAsyncValidator<TEntity1, TEntity2>
    {
        Task<ValidationResult> ValidateAsync(TEntity1 entity1, TEntity2 entity2, CancellationToken cancellationToken);
    }
}
