using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.OCR.PerformanceTesting.App.Services
{
    internal class ProducerConsumer<TEntity>
    {
        public void Produce(List<TEntity> items) 
        {
            items.ForEach(item => 
                concurrentQueue.Enqueue(item));
        }

        public async Task ConsumeAsync(Func<TEntity, CancellationToken, Task> action, int consumerCount, CancellationToken cancellationToken) 
        {
            var consumerTasks = Enumerable
                .Range(1, consumerCount)
                .Select((number) => ConsumeActionAsync(action, cancellationToken))
                .ToList();

            await Task.WhenAll(consumerTasks);
        }

        private async Task ConsumeActionAsync(Func<TEntity, CancellationToken, Task> action, CancellationToken cancellationToken)
        {
            while (concurrentQueue.TryDequeue(out var entity))
            {
                await action(entity, cancellationToken);
            }
        }

        private readonly ConcurrentQueue<TEntity> concurrentQueue = new ConcurrentQueue<TEntity>();
    }
}
