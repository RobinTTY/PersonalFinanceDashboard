using System.Text.RegularExpressions;
using RobinTTY.NordigenApiClient.Models.Responses;

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

        return Create($"{entityName} with id {id} does not exist.", code);
    }

    /// <summary>
    /// Creates an error for a failed request to a third party.
    /// </summary>
    /// <param name="error">The error returned by the third party.</param>
    /// <param name="fallbackMessage">The message to use if the third party didn't describe the error.</param>
    /// <param name="code">The error code from <see cref="ErrorCodes"/>.</param>
    public static GraphQLException ThirdPartyRequestFailed(BasicResponse error, string fallbackMessage, string code)
    {
        var message = error.Summary != null ? $"{error.Summary} {error.Detail}" : fallbackMessage;
        return Create(message, code);
    }

    /// <summary>
    /// Creates an error with the given message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="code">The error code from <see cref="ErrorCodes"/>.</param>
    public static GraphQLException Create(string message, string code) =>
        new(ErrorBuilder
            .New()
            .SetMessage(message)
            .SetCode(code)
            .Build());
}