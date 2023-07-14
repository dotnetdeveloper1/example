using PWP.InvoiceCapture.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.Business.Extensions
{
    public static class EnumerableExtensions
    {
        public static async Task ForEachAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task> action)
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(action, nameof(action));

            var tasks = source.Select(action);

            await Task.WhenAll(tasks);
        }

        public static async Task ForEachAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, CancellationToken, Task> action, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(action, nameof(action));
            Guard.IsNotNull(cancellationToken, nameof(cancellationToken));

            var tasks = source.Select(item => action(item, cancellationToken));

            await Task.WhenAll(tasks);
        }

        public static async Task<TResult[]> ForEachAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, Task<TResult>> func)
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(func, nameof(func));

            return await Task.WhenAll(
                source.Select(func));
        }

        public static async Task<TResult[]> ForEachAsync<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, CancellationToken, Task<TResult>> func, CancellationToken cancellationToken)
        {
            Guard.IsNotNull(source, nameof(source));
            Guard.IsNotNull(func, nameof(func));
            Guard.IsNotNull(cancellationToken, nameof(cancellationToken));

            var tasks = source.Select(item => func(item, cancellationToken));

            return await Task.WhenAll(tasks);
        }
    }
}
