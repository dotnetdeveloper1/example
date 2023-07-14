using Microsoft.EntityFrameworkCore;
using PWP.InvoiceCapture.Core.Utilities;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Enumerations;
using PWP.InvoiceCapture.InvoiceManagement.Business.Contract.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PWP.InvoiceCapture.InvoiceManagement.DataAccess.Extensions
{
    internal static class QueryableExtensions
    {
        public static async Task<PaginatedResult<TEntity>> ToPaginatedResultAsync<TEntity, TSortField>(this IQueryable<TEntity> source, PaginatedRequest<TSortField> paginatedRequest, CancellationToken cancellationToken) where TSortField : struct
        {
            Guard.IsNotNull(paginatedRequest, nameof(paginatedRequest));
            Guard.IsNotZeroOrNegative(paginatedRequest.PageNumber, nameof(paginatedRequest.PageNumber));
            Guard.IsNotZeroOrNegative(paginatedRequest.ItemsPerPage, nameof(paginatedRequest.ItemsPerPage));

            var skipCount = (paginatedRequest.PageNumber - 1) * paginatedRequest.ItemsPerPage;

            var items = await source
                .Sort(paginatedRequest)
                .Skip(skipCount)
                .Take(paginatedRequest.ItemsPerPage)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<TEntity>
            {
                Items = items,
                TotalItemsCount = source.Count()
            };
        }

        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string property)
        {
            return ApplyOrder(source, property, "OrderBy");
        }

        public static IOrderedQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> source, string property)
        {
            return ApplyOrder(source, property, "OrderByDescending");
        }

        public static IOrderedQueryable<TEntity> ThenBy<TEntity>(this IOrderedQueryable<TEntity> source, string property)
        {
            return ApplyOrder(source, property, "ThenBy");
        }

        public static IOrderedQueryable<TEntity> ThenByDescending<TEntity>(this IOrderedQueryable<TEntity> source, string property)
        {
            return ApplyOrder(source, property, "ThenByDescending");
        }

        private static IOrderedQueryable<TEntity> ApplyOrder<TEntity>(IQueryable<TEntity> source, string propertyName, string methodName)
        {
            var type = typeof(TEntity);
            var argument = Expression.Parameter(type, "defaultName");
            var propertyInfo = type.GetProperty(propertyName);
            Expression expression = argument;
            var propertyExpression = Expression.Property(expression, propertyInfo);
            var propertyType = propertyInfo.PropertyType;
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), propertyType);
            var lambda = Expression.Lambda(delegateType, propertyExpression, argument);

            var result = typeof(Queryable).GetMethods()
                .Single(method =>
                    method.Name == methodName &&
                    method.IsGenericMethodDefinition &&
                    method.GetGenericArguments().Length == 2 &&
                    method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TEntity), propertyType)
                .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<TEntity>)result;
        }

        private static IOrderedQueryable<TEntity> Sort<TEntity, TSortField>(this IQueryable<TEntity> source, PaginatedRequest<TSortField> paginatedRequest) where TSortField : struct
        {
            var propertyName = paginatedRequest.SortBy.ToString();

            if (paginatedRequest.SortType == SortType.Ascending)
            {
                return source.OrderBy(propertyName);
            }

            if (paginatedRequest.SortType == SortType.Descending)
            {
                return source.OrderByDescending(propertyName);
            }

            throw new ArgumentException("Unknown sort type.");
        }
    }
}
