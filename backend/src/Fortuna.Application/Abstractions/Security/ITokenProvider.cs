namespace Fortuna.Application.Abstractions.Security;

public interface ITokenProvider
{
    AuthenticationToken Create(Guid customerId, string email);
}
