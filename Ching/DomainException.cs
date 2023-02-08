namespace Ching;

public record DomainExceptionResult(string ErrorMessage);

public class DomainException : Exception
{
    public readonly string _userMessage;
    public DomainException(string userMessage) : base(userMessage) => _userMessage = userMessage;

    public DomainExceptionResult ToResult()
    {
        return new DomainExceptionResult(this._userMessage);
    }
}