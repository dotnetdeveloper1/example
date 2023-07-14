using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.DataAccess.Repositories
{
    internal class UserRepository : IUserRepository
    {
        public UserRepository(ITenantsDatabaseContextFactory contextFactory)
        {
            Guard.IsNotNull(contextFactory, nameof(contextFactory));

            this.contextFactory = contextFactory;
        }

        public async Task<int> CreateAsync(User user, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(user, nameof(user));

            var currentDate = DateTime.UtcNow;

            user.CreatedDate = currentDate;
            user.ModifiedDate = currentDate;

            using (var context = contextFactory.Create())
            {
                context.Entry(user).State = EntityState.Added;

                await context.SaveChangesAsync(cancellationToken);
            }

            return user.Id;
        }

        public async Task<bool> IsUsernameExistsAsync(string userName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(userName, nameof(userName));

            using (var context = contextFactory.Create())
            {
                return await context.Users.AnyAsync(user => user.Username == userName, cancellationToken);
            }
        }

        public async Task CreateAsync(List<User> users, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrEmpty(users, nameof(users));

            var currentDate = DateTime.UtcNow;

            users.ForEach(user =>
            {
                user.CreatedDate = currentDate;
                user.ModifiedDate = currentDate;
            });

            using (var context = contextFactory.Create())
            {
                context.Users.AddRange(users);
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<User>> GetListAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            using (var context = contextFactory.Create())
            {
                return await context.Users.Where(user => user.GroupId == groupId).ToListAsync(cancellationToken);
            }
        }

        public async Task<User> GetAsync(int userId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(userId, nameof(userId));

            using (var context = contextFactory.Create())
            {
                return await context.Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
            }
        }

        public async Task<User> GetAsync(string userName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(userName, nameof(userName));

            using (var context = contextFactory.Create())
            {
                return await context.Users.FirstOrDefaultAsync(user => user.Username == userName, cancellationToken);
            }
        }

        private readonly ITenantsDatabaseContextFactory contextFactory;
    }
}
