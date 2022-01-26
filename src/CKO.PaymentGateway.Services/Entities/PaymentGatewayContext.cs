using Microsoft.EntityFrameworkCore;

namespace CKO.PaymentGateway.Services.Entities;

#pragma warning disable CS8618
public class PaymentGatewayContext : DbContext
{
    public virtual DbSet<PaymentEntity> Payments { get; set; }
    public virtual DbSet<PaymentOperationRecordEntity> PaymentOperationRecords { get; set; }

    public PaymentGatewayContext(DbContextOptions<PaymentGatewayContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<PaymentEntity>()
            .HasMany(record => record.PaymentOperationRecords)
            .WithOne()
            .HasForeignKey(record => record.PaymentId);
    }
}
#pragma warning restore CS8618
