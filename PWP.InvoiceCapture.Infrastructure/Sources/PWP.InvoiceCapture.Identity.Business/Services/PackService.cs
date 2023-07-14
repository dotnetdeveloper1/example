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
    internal class PackService : IPackService
    {
        public PackService(IPackRepository packRepository, IGroupPackRepository groupPackRepository, IGroupRepository groupRepository, ICurrencyRepository currencyRepository)
        {
            Guard.IsNotNull(packRepository, nameof(packRepository));
            Guard.IsNotNull(groupPackRepository, nameof(groupPackRepository));
            Guard.IsNotNull(groupRepository, nameof(groupRepository));
            Guard.IsNotNull(currencyRepository, nameof(currencyRepository));
            
            this.packRepository = packRepository;
            this.groupRepository = groupRepository;
            this.groupPackRepository = groupPackRepository;
            this.currencyRepository = currencyRepository;
        }

        public async Task<List<GroupPack>> GetGroupPackListAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            return await groupPackRepository.GetListAsync(groupId, cancellationToken);
        }

        public async Task<OperationResult> CreateGroupPackAsync(int groupId, int packId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));
            Guard.IsNotZeroOrNegative(packId, nameof(packId));

            if (!await groupRepository.ExistsAsync(groupId, cancellationToken))
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Group with id={groupId} was not found"
                };
            }

            var pack = await packRepository.GetByIdAsync(packId, cancellationToken);
            if (pack == null)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Pack with id={packId} was not found"
                };
            }

            var groupPack = new GroupPack() 
            {
                PackId = packId,
                GroupId = groupId,
                UploadedDocumentsCount = 0
            };

            await groupPackRepository.CreateAsync(groupPack, cancellationToken);
            
            return new OperationResult { Status = OperationResultStatus.Success };
        }

        public Task<Pack> GetGroupPackByIdAsync(int packId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(packId, nameof(packId));

            return packRepository.GetByIdAsync(packId, cancellationToken);
        }

        public Task<List<GroupPack>> GetActiveGroupPackAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            return groupPackRepository.GetActiveAsync(groupId, cancellationToken);
        }

        public Task IncreaseCountOfUploadedInvoices(GroupPack groupPack, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(groupPack, nameof(groupPack));

            groupPack.UploadedDocumentsCount++;

            return groupPackRepository.UpdateAsync(groupPack, cancellationToken);
        }

        public async Task<List<Pack>> GetPackListAsync(CancellationToken cancellationToken)
        {
            return await packRepository.GetListAsync(cancellationToken);
        }

        public async Task<OperationResult<int>> CreatePackAsync(PackCreationParameters packCreationParameters, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(packCreationParameters, nameof(packCreationParameters));

            if (string.IsNullOrWhiteSpace(packCreationParameters.PackName))
            {
                return new OperationResult<int>
                {
                    Status = OperationResultStatus.Failed,
                    Message = "Name required."
                };
            }

            if (packCreationParameters.AllowedDocumentsCount < 0)
            {
                return new OperationResult<int>
                {
                    Status = OperationResultStatus.Failed,
                    Message = "AllowedDocumentsCount is less then zero."
                };
            }

            if (!await currencyRepository.ExistsAsync(packCreationParameters.CurrencyId, cancellationToken))
            {
                return new OperationResult<int>
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Currency with id {packCreationParameters.CurrencyId} is not exists."
                };
            }

            var pack = new Pack()
            {
                Name = packCreationParameters.PackName,
                CurrencyId = packCreationParameters.CurrencyId,
                AllowedDocumentsCount = packCreationParameters.AllowedDocumentsCount,
                Price = packCreationParameters.Price
            };

            var packId = await packRepository.CreateAsync(pack, cancellationToken);

            return new OperationResult<int>
            {
                Data = packId,
                Status = OperationResultStatus.Success,
                Message = $"Pack with id '{packId}' was created."
            };
        }

        public async Task LockGroupPacksAsync(int groupId, CancellationToken cancellationToken)
        {
            await groupPackRepository.LockByGroupIdAsync(groupId, cancellationToken);
        }

        public async Task<OperationResult> DeleteGroupPackByIdAsync(int groupPackId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupPackId, nameof(groupPackId));

            var groupPackToDelete = await groupPackRepository.GetByIdAsync(groupPackId, cancellationToken);
            if (groupPackToDelete == null)
            {
                return new OperationResult<int>
                {
                    Status = OperationResultStatus.NotFound,
                    Message = $"There is no groupPack with id = '{groupPackId}'."
                };
            }

            await groupPackRepository.DeleteAsync(groupPackToDelete.Id, cancellationToken);

            return OperationResult.Success;
        }

        private readonly IPackRepository packRepository;
        private readonly IGroupRepository groupRepository;
        private readonly IGroupPackRepository groupPackRepository;
        private readonly ICurrencyRepository currencyRepository;
    }
}
