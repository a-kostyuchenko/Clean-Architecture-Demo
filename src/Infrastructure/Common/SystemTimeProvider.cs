using SharedKernel;

namespace Infrastructure.Common;

public class SystemTimeProvider : ISystemTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}