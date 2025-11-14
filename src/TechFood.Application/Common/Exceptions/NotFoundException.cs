
namespace TechFood.Application.Common.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message)
        : base(message)
    { }

    public NotFoundException(string message, System.Exception innerException)
        : base(message, innerException)
    { }
}
