using CKO.PaymentGateway.Models.Exceptions;

namespace CKO.PaymentGateway.Models;

/// <summary>
/// SmartEnum for <see cref="Currency"/> representation.
/// </summary>
/// <remarks>
/// The purpose of this record is mainly to counteract the Primitive Obsession code smell.
/// According to the <seealso href="https://en.wikipedia.org/wiki/ISO_4217">ISO 4217</seealso>
/// a currency can take up to 4 digits in precision and it's constituted by an alphabetic code,
/// a numeric code and minor unit (precision).
/// </remarks>
public readonly record struct Currency
{
    public static readonly Currency EUR = new("Euro", "EUR", 978, 2);
    public static readonly Currency USD = new("United states dollar", "USD", 840, 2);
    public static readonly Currency GBP = new("Pound sterling", "GBP", 826, 2);
    public static readonly Currency CHF = new("Swiss franc", "CHF", 756, 2);
    public static readonly Currency JPY = new("Japanese yen", "JPY", 392, 0);

    /// <summary>
    /// All currencies available under the enumeration.
    /// </summary>
    public static readonly IEnumerable<Currency> All = new[] { EUR, USD, GBP, CHF, JPY };

    /// <summary>
    /// The entity component of the currency.
    /// </summary>
    public string Entity { get; }

    /// <summary>
    /// The alphabetic code of the currency.
    /// </summary>
    public string AlphabeticCode { get; }

    /// <summary>
    /// The numeric code of the currency.
    /// </summary>
    public ushort NumericCode { get; }

    /// <summary>
    /// The minor unit (precision) of the currency.
    /// </summary>
    public byte MinorUnit { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Currency"/> record.
    /// Defaults to <see cref="EUR"/>.
    /// </summary>
    public Currency() : this(EUR.Entity, EUR.AlphabeticCode, EUR.NumericCode, EUR.MinorUnit)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Currency"/> record.
    /// </summary>
    /// <param name="entity">The well-known name of the currency.</param>
    /// <param name="alphabeticCode">The alphabetic code of the currency.</param>
    /// <param name="numericCode">The numeric code of the currency.</param>
    /// <param name="minorUnit">The minor unit (precision) of the currency.</param>
    private Currency(string entity, string alphabeticCode, ushort numericCode, byte minorUnit)
    {
        Entity = entity;
        AlphabeticCode = alphabeticCode;
        NumericCode = numericCode;
        MinorUnit = minorUnit;
    }

    public static implicit operator Currency(string currency) => FromAlphabeticCode(currency);
    public static implicit operator string(Currency currency) => currency.AlphabeticCode;
    public static implicit operator ushort(Currency currency) => currency.NumericCode;

    /// <summary>
    /// Gets the currency for the provided alphabetic code.
    /// </summary>
    /// <param name="alphabeticCode">The alphabetic code.</param>
    /// <returns>The corresponding currency.</returns>
    /// <exception cref="UnsupportedCurrencyException">Exception thrown if the supplied alphabetic code is unsupported.</exception>
    public static Currency FromAlphabeticCode(string alphabeticCode)
    {
        try
        {
            return All.Single(currency => currency.AlphabeticCode.Equals(alphabeticCode, StringComparison.InvariantCulture));
        }
        catch (InvalidOperationException)
        {
            var supportedCurrencies = string.Join(",", All);
            throw new UnsupportedCurrencyException(
                        $"Unsupported alphabetic code provided [{alphabeticCode}]. Supported currencies: {supportedCurrencies}");
        }
    }

    /// <summary>
    /// Gets the currency for the provided numeric code.
    /// </summary>
    /// <param name="numericCode">The numeric code.</param>
    /// <returns>The corresponding currency.</returns>
    /// <exception cref="UnsupportedCurrencyException">Exception thrown if the supplied numeric code is unsupported.</exception>
    public static Currency FromNumericCode(ushort numericCode)
    {
        try
        {
            return All.Single(currency => currency.NumericCode.Equals(numericCode));
        }
        catch (InvalidOperationException)
        {
            var supportedCurrencies = string.Join(",", All);
            throw new UnsupportedCurrencyException(
                        $"Unsupported numeric code provided [{numericCode}]. Supported currencies: {supportedCurrencies}");
        }
    }
}
