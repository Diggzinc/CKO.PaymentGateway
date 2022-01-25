using System.ComponentModel.DataAnnotations.Schema;

namespace CKO.PaymentGateway.Services.Entities;
#pragma warning disable CS8618
public record PaymentOperationRecordEntity
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Operation { get; set; }

    [Column(TypeName = "jsonb")]
    public string MetaData { get; set; }
}
#pragma warning restore CS8618
