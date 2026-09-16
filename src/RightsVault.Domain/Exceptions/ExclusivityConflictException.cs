namespace RightsVault.Domain.Exceptions;

public sealed class ExclusivityConflictException(string message)
    : DomainException(message);

public abstract class DomainException(string message)
    : Exception(message);
