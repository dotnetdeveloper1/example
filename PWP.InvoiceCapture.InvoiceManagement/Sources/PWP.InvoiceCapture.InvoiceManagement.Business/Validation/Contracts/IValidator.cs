namespace PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts
{
    public interface IValidator<TEntity>
    {
        ValidationResult Validate(TEntity entity);
    }

    public interface IValidator<TEntity1, TEntity2>
    {
        ValidationResult Validate(TEntity1 entity1, TEntity2 entity2);
    }

    public interface IValidator<TEntity1, TEntity2, TEntity3>
    {
        ValidationResult Validate(TEntity1 entity1, TEntity2 entity2, TEntity3 entity3);
    }
    public interface IValidator<TEntity1, TEntity2, TEntity3, TEntity4>
    {
        ValidationResult Validate(TEntity1 entity1, TEntity2 entity2, TEntity3 entity3, TEntity4 entity4);
    }
}
