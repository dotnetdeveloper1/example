using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Repositories;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Services;
using PWP.InvoiceCapture.InvoiceManagement.Business.Extensions;
using PWP.InvoiceCapture.InvoiceManagement.Business.Validation.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Services
{
    internal class FieldService : IFieldService
    {
        public FieldService(IFieldRepository fieldRepository,
            IFieldGroupService fieldGroupService,
            IFormulaValidator formulaValidator,
            IFormulaFieldRepository formulaFieldRepository,
            IFormulaExtractionService formulaExtractionService)
        {
            Guard.IsNotNull(fieldRepository, nameof(fieldRepository));
            Guard.IsNotNull(fieldGroupService, nameof(fieldGroupService));
            Guard.IsNotNull(formulaValidator, nameof(formulaValidator));
            Guard.IsNotNull(formulaFieldRepository, nameof(formulaFieldRepository));
            Guard.IsNotNull(formulaExtractionService, nameof(formulaExtractionService));

            this.fieldRepository = fieldRepository;
            this.fieldGroupService = fieldGroupService;
            this.formulaValidator = formulaValidator;
            this.formulaFieldRepository = formulaFieldRepository;
            this.formulaExtractionService = formulaExtractionService;
        }

        public Dictionary<string, int> GetFieldTypes()
        {
            return EnumExtensions.GetKeyValues<FieldType, int>();
        }

        public Dictionary<string, int> GetTargetFieldTypes()
        {
            return EnumExtensions.GetKeyValues<TargetFieldType, int>();
        }

        public async Task<Field> GetAsync(int fieldId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));

            return await fieldRepository.GetAsync(fieldId, cancellationToken);
        }

        public async Task<List<Field>> GetListAsync(CancellationToken cancellationToken)
        {
            return await fieldRepository.GetListAsync(cancellationToken);
        }

        public async Task<OperationResult> CreateAsync(Field field, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(field, nameof(field));

            var targetFieldResult = await FieldIsValidAsync(field, cancellationToken);

            if (!targetFieldResult.IsSuccessful)
            {
                return targetFieldResult;
            }

            if (!string.IsNullOrEmpty(field.Formula))
            {
                var formulaFieldsIds = formulaExtractionService.GetFieldIds(field.Formula);
                var fields = await fieldRepository.GetListAsync(cancellationToken);
                var validationResult = formulaValidator.Validate(fields, field, false);

                if (!validationResult.IsValid)
                {
                    return new OperationResult
                    {
                        Status = OperationResultStatus.Failed,
                        Message = validationResult.Message
                    };
                }

                await formulaFieldRepository.CreateAsync(field.Id, formulaFieldsIds.ToList(), cancellationToken);
            }

            await fieldRepository.CreateAsync(field, cancellationToken);


            return new OperationResult
            {
                Status = OperationResultStatus.Success,
                Message = $"Field with name = {field.DisplayName} was created."
            };
        }

        public async Task<OperationResult> DeleteAsync(int fieldId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));

            var field = await GetAsync(fieldId, cancellationToken);
            if (field == null)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Success,
                    Message = $"Field with id = {fieldId} already removed."
                };
            }
            else if (field.IsProtected)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Field with id = {fieldId} is protected and cannot be removed."
                };
            }

            if (await formulaFieldRepository.UsedAsOperandInFormulaAsync(fieldId, cancellationToken))
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Field with id = {fieldId} is used in custom validation rule and cannot be removed."
                };
            }

            await formulaFieldRepository.DeleteAllByResultFieldIdAsync(fieldId, cancellationToken);
            await fieldRepository.DeleteAsync(fieldId, cancellationToken);

            return new OperationResult
            {
                Status = OperationResultStatus.Success,
                Message = $"Field with id = {fieldId} was removed."
            };
        }

        public async Task<OperationResult> UpdateAsync(int fieldId, Field field, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(fieldId, nameof(fieldId));
            Guard.IsNotNull(field, nameof(field));

            var targetFieldResult = await FieldIsValidAsync(field, cancellationToken);

            if (!targetFieldResult.IsSuccessful)
            {
                return targetFieldResult;
            }

            var existingField = await GetAsync(fieldId, cancellationToken);
            if (existingField == null)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.NotFound,
                    Message = $"Field with id = {fieldId} not found."
                };
            }

            var isFieldUsedAsOperand = (await formulaFieldRepository.GetByOperandFieldIdAsync(fieldId, cancellationToken)).Count > 0;
            var fields = await fieldRepository.GetListAsync(cancellationToken);
            var validationResult = formulaValidator.Validate(fields, field, isFieldUsedAsOperand);

            if (!validationResult.IsValid)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = validationResult.Message
                };
            }

            await fieldRepository.UpdateAsync(fieldId, field, cancellationToken);

            return new OperationResult
            {
                Status = OperationResultStatus.Success,
                Message = $"Field with id = {fieldId} was updated."
            };
        }

        private async Task<OperationResult> FieldIsValidAsync(Field field, CancellationToken cancellationToken)
        {
            if (!TargetFieldTypeIsDefined(field.TargetFieldType))
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = "Field contains unknown target field type."
                };
            }

            var groupExistsTask = GroupExistsAsync(field, cancellationToken);
            var targetFieldAlreadyAssignedTask = TargetFieldTypeAlreadyAssignedAsync(field, cancellationToken);

            if (!await groupExistsTask)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Group with id = {field.GroupId} not exists."
                };
            }

            if (await targetFieldAlreadyAssignedTask)
            {
                return new OperationResult
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"Target Field with value = {(int)field.TargetFieldType} already assigned to another field."
                };
            }

            return new OperationResult { Status = OperationResultStatus.Success };
        }

        private async Task<bool> GroupExistsAsync(Field field, CancellationToken cancellationToken)
        {
            var fieldGroup = await fieldGroupService.GetAsync(field.GroupId, cancellationToken);

            return fieldGroup != null;
        }

        private async Task<bool> TargetFieldTypeAlreadyAssignedAsync(Field field, CancellationToken cancellationToken)
        {
            if (field.TargetFieldType.HasValue)
            {
                var fields = await GetListAsync(cancellationToken);

                return fields.Any(existingField => existingField.Id != field.Id && existingField.TargetFieldType == field.TargetFieldType.Value);
            }

            return false;
        }

        private bool TargetFieldTypeIsDefined(TargetFieldType? targetFieldType)
        {
            if (targetFieldType.HasValue)
            {
                return Enum.IsDefined(typeof(TargetFieldType), targetFieldType.Value);
            }

            return true;
        }



        private readonly IFieldRepository fieldRepository;
        private readonly IFieldGroupService fieldGroupService;
        private readonly IFormulaValidator formulaValidator;
        private readonly IFormulaFieldRepository formulaFieldRepository;
        private readonly IFormulaExtractionService formulaExtractionService;
    }
}
