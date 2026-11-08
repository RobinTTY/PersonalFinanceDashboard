using System.Text.RegularExpressions;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Mutations;

/// <summary>
/// Errors returned by mutation resolvers.
/// </summary>
internal static class MutationErrors
{
    /// <summary>
    /// Creates an error for an entity that doesn't exist.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity, used for the error message.</typeparam>
    /// <param name="id">The id of the entity.</param>
    /// <param name="code">The error code from <see cref="ErrorCodes"/>.</param>
    public static GraphQLException NotFound<TEntity>(Guid id, string code)
    {
        // "BankAccount" -> "Bank account"
        var entityName = Regex.Replace(typeof(TEntity).Name, "(?<=[a-z])[A-Z]",
            match => " " + char.ToLowerInvariant(match.Value[0]));

        return new GraphQLException(ErrorBuilder
            .New()
            .SetMessage($"{entityName} with id {id} does not exist.")
            .SetCode(code)
            .Build());
    }
}