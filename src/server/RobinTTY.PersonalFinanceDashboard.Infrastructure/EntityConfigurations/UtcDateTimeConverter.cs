using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

/// <summary>
/// Marks <see cref="DateTime"/> values read from the database as UTC, since SQLite doesn't store the kind.
/// </summary>
public class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    value => value,
    value => DateTime.SpecifyKind(value, DateTimeKind.Utc));