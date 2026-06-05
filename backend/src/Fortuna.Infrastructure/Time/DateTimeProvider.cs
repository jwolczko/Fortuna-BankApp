using Fortuna.Application.Abstractions.Clock;

namespace Fortuna.Infrastructure.Time;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
