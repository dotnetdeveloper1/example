using System;
using System.Transactions;

namespace PWP.InvoiceCapture.Core.Utilities
{
    public static class TransactionManager
    {
        public static TransactionScope Create() 
        {
            var options = new TransactionOptions
            {
                IsolationLevel = defaultIsolationLevel,
                Timeout = defaultTimeout
            };

            return Create(options);
        }

        public static TransactionScope Create(IsolationLevel? isolationLevel, TimeSpan? timeout)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = isolationLevel ?? defaultIsolationLevel,
                Timeout = timeout?? defaultTimeout
            };

            return Create(options);
        }

        public static TransactionScope Create(TransactionOptions options) => 
            new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled);

        private static readonly TimeSpan defaultTimeout = TimeSpan.FromMinutes(2);
        private static readonly IsolationLevel defaultIsolationLevel = IsolationLevel.ReadCommitted;
    }
}
