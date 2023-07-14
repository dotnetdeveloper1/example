using PWP.InvoiceCapture.Core.Contracts;
using PWP.InvoiceCapture.Core.DataAccess.Contracts;
using PWP.InvoiceCapture.Core.Enumerations;
using PWP.InvoiceCapture.Core.Models;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.Identity.Business.Contract.Enumerations;
using PWP.InvoiceCapture.Identity.Business.Contract.Models;
using PWP.InvoiceCapture.Identity.Business.Contract.Repositories;
using PWP.InvoiceCapture.Identity.Business.Contract.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.Identity.Business.Services
{
    internal class TenantService : ITenantService
    {
        public TenantService(
            ITenantRepository tenantRepository,
            ITenantSettingRepository tenantSettingRepository,
            ISqlManagementService sqlManagementService,
            IInvoicesDatabaseNameProvider invoicesDatabaseNameProvider,
            IEmailProvider emailProvider)
        {
            Guard.IsNotNull(tenantRepository, nameof(tenantRepository));
            Guard.IsNotNull(tenantSettingRepository, nameof(tenantSettingRepository));
            Guard.IsNotNull(sqlManagementService, nameof(sqlManagementService));
            Guard.IsNotNull(invoicesDatabaseNameProvider, nameof(invoicesDatabaseNameProvider));
            Guard.IsNotNull(emailProvider, nameof(emailProvider));

            this.tenantRepository = tenantRepository;
            this.tenantSettingRepository = tenantSettingRepository;
            this.sqlManagementService = sqlManagementService;
            this.invoicesDatabaseNameProvider = invoicesDatabaseNameProvider;
            this.emailProvider = emailProvider;
        }

        public async Task<List<Tenant>> GetListAsync(CancellationToken cancellationToken)
        {
            return await tenantRepository.GetListAsync(cancellationToken);
        }

        public async Task<List<Tenant>> GetListByGroupIdAsync(int groupId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(groupId, nameof(groupId));

            return await tenantRepository.GetListByGroupIdAsync(groupId, cancellationToken);
        }

        public async Task<List<Tenant>> GetListExceptStatusAsync(TenantDatabaseStatus status, CancellationToken cancellationToken)
        {
            Guard.IsEnumDefined(status, nameof(status));

            return await tenantRepository.GetListExceptStatusAsync(status, cancellationToken);
        }

        public async Task<Tenant> GetAsync(int tenantId, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(tenantId, nameof(tenantId));

            return await tenantRepository.GetAsync(tenantId, cancellationToken);
        }

        public async Task<OperationResult<Tenant>> CreateAsync(TenantCreationParameters tenantCreationParameters, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(tenantCreationParameters, nameof(tenantCreationParameters));

            if (string.IsNullOrWhiteSpace(tenantCreationParameters.TenantName))
            {
                return new OperationResult<Tenant> { Status = OperationResultStatus.Failed, Message = "Tenant name required." };
            }

            if (tenantCreationParameters.GroupId < 1)
            {
                return new OperationResult<Tenant> { Status = OperationResultStatus.Failed, Message = "GroupId must be greater than zero." };
            }

            var tenant = new Tenant()
            {
                DatabaseName = "NoDatabase",
                IsActive = false,
                Name = tenantCreationParameters.TenantName,
                Status = TenantDatabaseStatus.NotCopied,
                GroupId = tenantCreationParameters.GroupId,
                DocumentUploadEmail = emailProvider.Generate()
            };

            using (var transaction = TransactionManager.Create())
            {
                // Aquire exclusive table lock on Tenants to be sure other transactions cannot modify or add new items till this transaction ends.
                await tenantRepository.LockAsync(cancellationToken);

                if (await tenantRepository.TenantNameExistsInGroupAsync(tenantCreationParameters.TenantName, tenantCreationParameters.GroupId, cancellationToken))
                {
                    return new OperationResult<Tenant> { Status = OperationResultStatus.Failed, Message = $"Tenant with name '{tenantCreationParameters.TenantName}' already exists." };
                }

                tenant.Id = await tenantRepository.CreateAsync(tenant, cancellationToken);
                transaction.Complete();
            }
                
            var databaseName = invoicesDatabaseNameProvider.Get(tenant.Id.ToString());
            tenant.DatabaseName = databaseName;
            tenant.Status = sqlManagementService.CreateSqlDatabase(databaseName, cancellationToken);

            await tenantRepository.UpdateAsync(tenant, cancellationToken);

            return new OperationResult<Tenant>
            {
                Data = tenant,
                Status = OperationResultStatus.Success,
                Message = $"Tenant with name '{tenantCreationParameters.TenantName}' is creating."
            };
        }

        public async Task CheckTenantsDatabasesStateAsync(CancellationToken cancellationToken)
        {
            var tenants = await GetListAsync(cancellationToken);
            var sqlDatabases = await sqlManagementService.GetSqlDatabasesAsync(cancellationToken);
            var updateTenantsList = new List<Tenant>();
           
            foreach (var tenant in tenants)
            {
                if (tenant.IsActive) 
                {
                    continue; 
                }
                var sqlDatabase = sqlDatabases.FirstOrDefault(sqlDatabase => sqlDatabase.Name == tenant.DatabaseName);
                
                if (IsNotCopiedDatabase(sqlDatabase, tenant))
                {
                    tenant.SetNotCopiedState();
                    updateTenantsList.Add(tenant);
                    continue;
                }

                if (IsOnlineDatabase(sqlDatabase, tenant))
                {
                    tenant.SetCopiedState();
                    updateTenantsList.Add(tenant);

                    await sqlManagementService.SetupBackupLongTermPolicesAsync(sqlDatabase.Name, cancellationToken);

                    continue;
                }

                if (IsFailedDatabase(sqlDatabase, tenant))
                {
                    tenant.SetFailedState();
                    updateTenantsList.Add(tenant);
                }
            }

            if (updateTenantsList.Count > 0)
            {
                await UpdateAsync(updateTenantsList, cancellationToken);
            }
        }

        public async Task<OperationResult> UpdateTenantNameAsync(int tenantId, string tenantName, CancellationToken cancellationToken)
        {
            Guard.IsNotZeroOrNegative(tenantId, nameof(tenantId));
            Guard.IsNotNullOrWhiteSpace(tenantName, nameof(tenantName));

            using (var transaction = TransactionManager.Create())
            {
                // Aquire exclusive table lock on Tenants to be sure other transactions cannot modify or add new items till this transaction ends.
                await tenantRepository.LockAsync(cancellationToken);

                var tenant = await tenantRepository.GetAsync(tenantId, cancellationToken);

                if (tenant == null)
                {
                    return new OperationResult { Status = OperationResultStatus.NotFound, Message = $"Tenant with id = {tenantId} doesn't exist." };
                }

                if (await tenantRepository.TenantNameExistsInGroupAsync(tenantName, tenant.GroupId, cancellationToken))
                {
                    return new OperationResult { Status = OperationResultStatus.Failed, Message = $"Tenant with name '{tenantName}' already exists." };
                }

                tenant.Name = tenantName;

                await tenantRepository.UpdateAsync(tenant, cancellationToken);
                transaction.Complete();
            }

            return new OperationResult { Status = OperationResultStatus.Success, Message = $"Tenant name with id = {tenantId} updated successfuly" };
        }

        public async Task<OperationResult<Tenant>> CloneAsync(int tenantId, string newTenantName, CancellationToken cancellationToken)
        {
            if (tenantId <= 0)
            {
                return new OperationResult<Tenant> { Status = OperationResultStatus.Failed, Message = "Tenant id required." };
            }

            var tenantToClone = await tenantRepository.GetAsync(tenantId, cancellationToken);

            if (tenantToClone == null)
            {
                return new OperationResult<Tenant>
                {
                    Status = OperationResultStatus.Failed,
                    Message = $"There is no tenant with id = {tenantId}."
                };
            }

            var creationResult = await CreateAsync(new TenantCreationParameters() { GroupId = tenantToClone.GroupId, TenantName = newTenantName }, cancellationToken);

            if (!creationResult.IsSuccessful)
            {
                return creationResult;
            }

            var settings = await tenantSettingRepository.GetByTenantIdAsync(tenantToClone.Id, cancellationToken);

            if (settings == null)
            {
                return creationResult;
            }

            var newTenantId = creationResult.Data.Id;
            var settingToCreate = new TenantSetting() { TenantId = newTenantId, CultureId = settings.CultureId };

            await tenantSettingRepository.CreateAsync(settingToCreate, cancellationToken);

            return creationResult;
        }

        public async Task<string> GetTenantIdByEmailAsync(string uploadEmail, CancellationToken cancellationToken)
        {
            Guard.IsNotNullOrWhiteSpace(uploadEmail, nameof(uploadEmail));

            var tenant = await tenantRepository.GetIdByUploadEmailAsync(uploadEmail, cancellationToken);

            return tenant == null ? null : GetTenantId(tenant);
        }

        private bool IsNotCopiedDatabase(SqlDatabase sqlDatabase, Tenant tenant)
        {
            return sqlDatabase == null && tenant.Status != TenantDatabaseStatus.NotCopied;
        }

        private bool IsOnlineDatabase(SqlDatabase sqlDatabase, Tenant tenant)
        {
            if (sqlDatabase == null)
            {
                return false;
            }

            return sqlDatabase.State == SqlDatabaseState.Online && tenant.Status != TenantDatabaseStatus.Copied;
        }

        private bool IsFailedDatabase(SqlDatabase sqlDatabase, Tenant tenant)
        {
            if (sqlDatabase == null)
            {
                return false;
            }

            return
                sqlDatabase.State != SqlDatabaseState.Copying &&
                sqlDatabase.State != SqlDatabaseState.Online &&
                tenant.Status != TenantDatabaseStatus.FailedToCopy;
        }

        private async Task UpdateAsync(List<Tenant> tenants, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(tenants, nameof(tenants));

            await tenantRepository.UpdateAsync(tenants, cancellationToken);
        }

        //TODO: Refactor to use 1 instead of Default in the whole solution, or move this logic to common DatabaseNameProvider (Core project)
        private string GetTenantId(Tenant tenant)
        {
            if (string.Equals(tenant.DatabaseName, "Invoices_Default"))
            {
                //For developer database only.
                return "Default";
            }

            return tenant.Id.ToString();
        }

        private readonly ITenantRepository tenantRepository;
        private readonly ITenantSettingRepository tenantSettingRepository;
        private readonly ISqlManagementService sqlManagementService;
        private readonly IInvoicesDatabaseNameProvider invoicesDatabaseNameProvider;
        private readonly IEmailProvider emailProvider;
    }
}
