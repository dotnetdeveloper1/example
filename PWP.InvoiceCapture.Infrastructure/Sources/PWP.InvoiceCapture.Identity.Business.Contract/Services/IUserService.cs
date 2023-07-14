using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Services
{
    public interface IUserService
    {
        Task<List<User>> GetListAsync(int groupId, CancellationToken cancellationToken);
        Task<User> GetAsync(int userId, CancellationToken cancellationToken);
        Task<User> GetAsync(string userName, CancellationToken cancellationToken);
        Task<OperationResult<UserCreationResponse >> CreateAsync(int groupId, CancellationToken cancellationToken);
    }
}
