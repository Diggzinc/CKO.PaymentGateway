using CKO.PaymentGateway.Models.Exceptions;

namespace CKO.PaymentGateway.Models;

/// <summary>
/// SmartEnum for <see cref="PaymentOperation"/> representation.
/// </summary>
/// <remarks>
/// The purpose of this record is mainly to counteract the Primitive Obsession code smell.
/// </remarks>
public readonly record struct PaymentOperation
{
    public static readonly PaymentOperation Issued = new(0, "Issued");
    public static readonly PaymentOperation Verifying = new(1, "Verifying");
    public static readonly PaymentOperation Verified = new(2, "Verified");
    public static readonly PaymentOperation Authorizing = new(3, "Authorizing");
    public static readonly PaymentOperation Authorized = new(4, "Authorized");
    public static readonly PaymentOperation Processing = new(5, "Processing");
    public static readonly PaymentOperation Processed = new(6, "Processed");
    public static readonly PaymentOperation Failed = new(7, "Failed");

    /// <summary>
    /// All payment operations available under the enumeration.
    /// </summary>
    public static readonly IEnumerable<PaymentOperation> All = new[]
    {
        Issued,
        Verifying,
        Verified,
        Authorizing,
        Authorized,
        Processing,
        Processed,
        Failed
    };

    /// <summary>
    /// Identifier for the payment operation.
    /// </summary>
    public uint Id { get; init; }

    /// <summary>
    /// Code of the payment operation.
    /// </summary>
    public string Code { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentOperation"/> record.
    /// Defaults to <see cref="Issued"/>.
    /// </summary>
    public PaymentOperation() : this(Issued.Id, Issued.Code)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentOperation"/> record.
    /// </summary>
    /// <param name="id">The id of the operations.</param>
    /// <param name="code">The code of the operation.</param>
    private PaymentOperation(uint id, string code)
    {
        Id = id;
        Code = code;
    }

    /// <summary>
    /// Gets the payment operation for the provided id.
    /// </summary>
    /// <param name="id">The operation id.</param>
    /// <returns>The corresponding currency.</returns>
    /// <exception cref="UnsupportedPaymentOperationException">Exception thrown if the supplied id is unsupported.</exception>
    public static PaymentOperation FromId(uint id)
    {
        try
        {
            return All.Single(operation => operation.Id.Equals(id));
        }
        catch (InvalidOperationException)
        {
            var supportedPaymentOperations = string.Join(",", All);
            throw new UnsupportedPaymentOperationException(
                        $"Unsupported id provided [{id}]. Supported payment operations: {supportedPaymentOperations}");
        }
    }
}

