using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class UserService : IUserService
    {
        public UserService(
            IUserRepository userRepository, 
            IGroupRepository groupRepository, 
            IPasswordHashService passwordHashService, 
            IPasswordGenerator passwordGenerator, 
            INameGenerator nameGenerator)
        {
            Guard.IsNotNull(userRepository, nameof(userRepository));
            Guard.IsNotNull(groupRepository, nameof(groupRepository));
            Guard.IsNotNull(passwordHashService, nameof(passwordHashService));
            Guard.IsNotNull(passwordGenerator, nameof(passwordGenerator));
            Guard.IsNotNull(nameGenerator, nameof(nameGenerator));

            this.userRepository = userRepository;
            this.groupRepository = groupRepository;
            this.passwordHashService = passwordHashService;
            this.passwordGenerator = passwordGenerator;
            this.nameGenerator = nameGenerator;
        }

        public async Task<List<User>> GetListAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            return await userRepository.GetListAsync(groupId, cancellationToken);
        }

        public async Task<User> GetAsync(int userId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(userId, nameof(userId));

            return await userRepository.GetAsync(userId, cancellationToken);
        }

        public async Task<User> GetAsync(string userName, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(userName, nameof(userName));

            return await userRepository.GetAsync(userName, cancellationToken);
        }

        public async Task<OperationResult<UserCreationResponse>> CreateAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));
           
            var groupExists = await groupRepository.ExistsAsync(groupId, cancellationToken);

            if (!groupExists)
            {
                return new OperationResult<UserCreationResponse>
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Group with id {groupId} doesn't exist."
                };
            }

            var username = nameGenerator.GenerateName();

            if (string.IsNullOrWhiteSpace(username))
            {
                return new OperationResult<UserCreationResponse>
                {
                    Status = OperationResultStatus.Failed,
                    Message = "Can't generate user."
                };
            }

            if (await userRepository.IsUsernameExistsAsync(username, cancellationToken))
            {
                return new OperationResult<UserCreationResponse>
                { 
                    Status = OperationResultStatus.Failed,
                    Message = $"User with name '{username}' already exists."
                };
            }            

            var password = passwordGenerator.GeneratePassword();

            if (string.IsNullOrWhiteSpace(password))
            {
                return new OperationResult<UserCreationResponse>
                {
                    Status = OperationResultStatus.Failed,
                    Message = "Can't generate user."
                };
            }

            var passwordHash = passwordHashService.GetHash(password);

            var user = new User()
            {
                IsActive = true,
                GroupId = groupId,
                Username = username,
                PasswordHash = passwordHash
            };

            var userId = await userRepository.CreateAsync(user, cancellationToken);

            return new OperationResult<UserCreationResponse>
            { 
                Data = new UserCreationResponse () { Id = userId, Username = username, Password = password },
                Status = OperationResultStatus.Success,
                Message = $"User with name '{user.Username}' created."
            };
        }

        private readonly IUserRepository userRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IPasswordHashService passwordHashService;
        private readonly IPasswordGenerator passwordGenerator;
        private readonly INameGenerator nameGenerator;
    }
}
