using System.Linq.Expressions;

namespace Ecommerce.Api.Infrastructure;

/// <summary>
/// Extension methods for IQueryable to support pagination, filtering, and sorting
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies pagination to a queryable collection
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <returns>Paginated queryable</returns>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        return query.Skip((page - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Applies sorting to a queryable collection
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="sortBy">Property name to sort by</param>
    /// <param name="sortDirection">Sort direction (asc or desc)</param>
    /// <returns>Sorted queryable</returns>
    public static IQueryable<T> SortBy<T>(this IQueryable<T> query, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var isDescending = sortDirection?.ToLowerInvariant() == "desc";

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = GetNestedProperty(parameter, sortBy);
        
        if (property == null)
            return query;

        var lambda = Expression.Lambda(property, parameter);
        
        var methodName = isDescending ? "OrderByDescending" : "OrderBy";
        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
    }

    /// <summary>
    /// Applies multiple sorting criteria to a queryable collection
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="sortCriteria">Dictionary of property names and sort directions</param>
    /// <returns>Sorted queryable</returns>
    public static IQueryable<T> SortBy<T>(this IQueryable<T> query, Dictionary<string, string> sortCriteria)
    {
        if (sortCriteria == null || !sortCriteria.Any())
            return query;

        var isFirst = true;
        foreach (var criteria in sortCriteria)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = GetNestedProperty(parameter, criteria.Key);
            
            if (property == null)
                continue;

            var lambda = Expression.Lambda(property, parameter);
            var isDescending = criteria.Value?.ToLowerInvariant() == "desc";
            
            string methodName;
            if (isFirst)
            {
                methodName = isDescending ? "OrderByDescending" : "OrderBy";
                isFirst = false;
            }
            else
            {
                methodName = isDescending ? "ThenByDescending" : "ThenBy";
            }

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type);

            query = (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
        }

        return query;
    }

    /// <summary>
    /// Applies a filter condition to a queryable collection
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="predicate">Filter condition</param>
    /// <returns>Filtered queryable</returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Applies a filter condition to a queryable collection
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="condition">Condition to check</param>
    /// <param name="predicate">Filter condition</param>
    /// <returns>Filtered queryable</returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate, Expression<Func<T, bool>> elsePredicate)
    {
        return condition ? query.Where(predicate) : query.Where(elsePredicate);
    }

    /// <summary>
    /// Searches for text in specified properties
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="query">The queryable collection</param>
    /// <param name="searchTerm">Term to search for</param>
    /// <param name="properties">Properties to search in</param>
    /// <returns>Filtered queryable</returns>
    public static IQueryable<T> SearchIn<T>(this IQueryable<T> query, string? searchTerm, params Expression<Func<T, string?>>[] properties)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || !properties.Any())
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var property in properties)
        {
            var propertyAccess = Expression.Invoke(property, parameter);
            var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null));
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var containsCall = Expression.Call(propertyAccess, containsMethod!, Expression.Constant(searchTerm));
            var notNullAndContains = Expression.AndAlso(nullCheck, containsCall);

            combinedExpression = combinedExpression == null 
                ? notNullAndContains 
                : Expression.OrElse(combinedExpression, notNullAndContains);
        }

        if (combinedExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

    /// <summary>
    /// Gets a nested property from an expression
    /// </summary>
    /// <param name="parameter">The parameter expression</param>
    /// <param name="propertyPath">The property path (e.g., "Category.Name")</param>
    /// <returns>Property expression or null if not found</returns>
    private static Expression? GetNestedProperty(Expression parameter, string propertyPath)
    {
        if (string.IsNullOrWhiteSpace(propertyPath))
            return null;

        var properties = propertyPath.Split('.');
        Expression? property = parameter;

        foreach (var propName in properties)
        {
            var propertyInfo = property!.Type.GetProperty(propName);
            if (propertyInfo == null)
                return null;

            property = Expression.Property(property, propertyInfo);
        }

        return property;
    }
}
