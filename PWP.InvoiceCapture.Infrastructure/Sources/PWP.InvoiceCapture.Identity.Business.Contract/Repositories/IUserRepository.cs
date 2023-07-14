using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Contract.Repositories
{
    public interface IUserRepository
    {
        Task<int> CreateAsync(User user, CancellationToken cancellationToken);
        Task CreateAsync(List<User> user, CancellationToken cancellationToken);
        Task<List<User>> GetListAsync(int groupId, CancellationToken cancellationToken);
        Task<User> GetAsync(int userId, CancellationToken cancellationToken);
        Task<User> GetAsync(string user, CancellationToken cancellationToken);
        Task<bool> IsUsernameExistsAsync(string userName, CancellationToken cancellationToken);
    }
}
