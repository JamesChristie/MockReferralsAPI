namespace MockReferralsAPI.Exceptions;

public class InvalidRedemptionException : Exception
{
    public InvalidRedemptionException(string message) : base(message) { }

    public InvalidRedemptionException(string message, Exception innerException)
        : base(message, innerException)
    {
        
    }
}