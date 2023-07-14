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
    internal class GroupService : IGroupService
    {
        public GroupService(
            IGroupRepository groupRepository,
            IUserService userService)
        {
            Guard.IsNotNull(groupRepository, nameof(groupRepository));
            Guard.IsNotNull(userService, nameof(userService));

            this.groupRepository = groupRepository;
            this.userService = userService;
        }

        public async Task<OperationResult<GroupCreationResponse>> CreateAsync(GroupCreationParameters groupCreationParameters, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(groupCreationParameters, nameof(groupCreationParameters));

            if (string.IsNullOrWhiteSpace(groupCreationParameters.Name))
            {
                return new OperationResult<GroupCreationResponse>
                {
                    Status = OperationResultStatus.Failed,
                    Message = "Name required."
                };
            }

            if (groupCreationParameters.ParentGroupId.HasValue)
            {
                var parentGroupExists = await groupRepository.ExistsAsync(groupCreationParameters.ParentGroupId.Value, cancellationToken);
                
                if (!parentGroupExists)
                {
                    return new OperationResult<GroupCreationResponse>
                    {
                        Status = OperationResultStatus.Failed,
                        Message = "Parent group not found."
                    };
                }
            }

            var group = new Group()
            {
                Name = groupCreationParameters.Name,
                ParentGroupId = groupCreationParameters.ParentGroupId,
            };

            int groupId = 0;
            OperationResult<UserCreationResponse> userCreationResult;

            using (var transaction = TransactionManager.Create())
            {
                groupId = await groupRepository.CreateAsync(group, cancellationToken);

                userCreationResult = await userService.CreateAsync(groupId, cancellationToken);
              
                if (!userCreationResult.IsSuccessful)
                {
                    return new OperationResult<GroupCreationResponse>
                    {
                        Data = new GroupCreationResponse() { Id = groupId },
                        Status = OperationResultStatus.Failed,
                        Message = "Can't create default user for group."
                    };
                }

                transaction.Complete();
            }

            return new OperationResult<GroupCreationResponse>
            {
                Data = new GroupCreationResponse() { 
                    Id = groupId,
                    Name = groupCreationParameters.Name,
                    ParentGroupId = groupCreationParameters.ParentGroupId,
                    DefaultUser = userCreationResult.Data
                },
                Status = OperationResultStatus.Success,
                Message = $"Group with name '{groupCreationParameters.Name}' created."
            };
        }

        public async Task<List<Group>> GetListAsync(CancellationToken cancellationToken)
        {
            return await groupRepository.GetListAsync(cancellationToken);
        }

        private readonly IGroupRepository groupRepository;
        private readonly IUserService userService;
    }
}
