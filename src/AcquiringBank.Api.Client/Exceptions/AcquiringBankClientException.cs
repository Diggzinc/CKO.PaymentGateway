namespace AcquiringBank.Api.Client.Exceptions;

public class AcquiringBankClientException : Exception
{
    public string Reason { get; }

    public AcquiringBankClientException(string reason)
        : base($"Call to API failed because: {reason}")
    {
        Reason = reason;
    }
}
