namespace StayFit.DAL.Exceptions;

public class BusinessValidationException : Exception
{
    public BusinessValidationException(string message) : base(message)
    {
    }
}