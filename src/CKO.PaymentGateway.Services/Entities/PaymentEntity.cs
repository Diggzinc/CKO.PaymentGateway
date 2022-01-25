namespace CKO.PaymentGateway.Services.Entities;
#pragma warning disable CS8618
public record PaymentEntity
{
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public string Description { get; set; }
    public string PartialCardNumber { get; set; }
    public int CardNumberLength { get; set; }
    public int CardExpiryDateMonth { get; set; }
    public int CardExpiryDateYear { get; set; }
    public string CardHolder { get; set; }
    public decimal ChargeAmount { get; set; }
    public string ChargeCurrency { get; set; }
    public List<PaymentOperationRecordEntity> PaymentOperationRecords { get; set; } = new();
}
#pragma warning restore CS8618

