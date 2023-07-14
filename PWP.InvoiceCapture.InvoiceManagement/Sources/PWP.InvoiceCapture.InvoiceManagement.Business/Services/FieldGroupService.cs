using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    internal class FieldGroupService : IFieldGroupService
    {
        public FieldGroupService(IFieldGroupRepository fieldGroupRepository, IFieldRepository fieldRepository) 
        {
            Guard.IsNotNull(fieldGroupRepository, nameof(fieldGroupRepository));
            Guard.IsNotNull(fieldRepository, nameof(fieldRepository));

            this.fieldGroupRepository = fieldGroupRepository;
            this.fieldRepository = fieldRepository;
        }

        public async Task<FieldGroup> GetAsync(int fieldGroupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldGroupId, nameof(fieldGroupId));

            return await fieldGroupRepository.GetAsync(fieldGroupId, cancellationToken);
        }

        public async Task<List<FieldGroup>> GetListAsync(CancellationToken cancellationToken)
        {
            return await fieldGroupRepository.GetListAsync(cancellationToken);
        }

        public async Task CreateAsync(FieldGroup fieldGroup, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(fieldGroup, nameof(fieldGroup));

            await fieldGroupRepository.CreateAsync(fieldGroup, cancellationToken);
        }

        public async Task<OperationResult> DeleteAsync(int fieldGroupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldGroupId, nameof(fieldGroupId));

            var fieldGroup = await GetAsync(fieldGroupId, cancellationToken);
            if (fieldGroup == null)
            {
                return new OperationResult 
                { 
                    Status = OperationResultStatus.Success, 
                    Message = $"FieldGroup with id = {fieldGroupId} already removed."
                };
            }
            else if (fieldGroup.IsProtected)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"FieldGroup with id = {fieldGroupId} is protected and cannot be removed."
                };
            }

            var deleteFieldsResult = await DeleteFieldsByGroupIdAsync(fieldGroupId, cancellationToken);
            if (!deleteFieldsResult.IsSuccessful)
            {
                return deleteFieldsResult;
            }

            await fieldGroupRepository.DeleteAsync(fieldGroupId, cancellationToken);

            return new OperationResult
            {
                Status = OperationResultStatus.Success,
                Message = $"FieldGroup with id = {fieldGroupId} was removed."
            };
        }

        public async Task<OperationResult> DeleteFieldsByGroupIdAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            var fields = await fieldRepository.GetListAsync(cancellationToken);
            var fieldsToDelete = fields.Where(field => field.GroupId == groupId).ToList();
            if (!fieldsToDelete.Any())
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Success,
                    Message = $"No fields to remove for group with id = {groupId}."
                };
            }

            var hasProtectedField = fieldsToDelete.Any(field => field.IsProtected);
            if (hasProtectedField)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Group with id = {groupId} contains at least one protected field."
                };
            }

            var fieldIds = fieldsToDelete.Select(field => field.Id).ToList();
            await fieldRepository.DeleteAsync(fieldIds, cancellationToken);

            return new OperationResult
            {
                Status = OperationResultStatus.Success,
                Message = $"Fields were removed for group with id = {groupId}."
            };
        }

        public async Task<OperationResult> UpdateAsync(int fieldGroupId, FieldGroup fieldGroup, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldGroupId, nameof(fieldGroupId));
            Guard.IsNotNull(fieldGroup, nameof(fieldGroup));

            var existingieldGroup = await GetAsync(fieldGroupId, cancellationToken);
            if (existingieldGroup == null)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.NotFound,
                    Message = $"FieldGroup with id = {fieldGroupId} not found."
                };
            }

            await fieldGroupRepository.UpdateAsync(fieldGroupId, fieldGroup, cancellationToken);

            return new OperationResult
            {
                Status = OperationResultStatus.Success,
                Message = $"FieldGroup with id = {fieldGroupId} was updated."
            };
        }

        private readonly IFieldGroupRepository fieldGroupRepository;
        private readonly IFieldRepository fieldRepository;
    }
}
