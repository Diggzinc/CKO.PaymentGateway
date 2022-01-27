using AutoMapper;
using CKO.PaymentGateway.Models;
using CKO.PaymentGateway.Services.Entities;
using Newtonsoft.Json;

namespace CKO.PaymentGateway.Services.Profiles;

public class ServiceEntitiesProfile : Profile
{
    public ServiceEntitiesProfile()
    {
        CreateMap<PaymentEntity, Payment>()
            .ConstructUsing((entity, context) =>
            {
                var operations = context.Mapper.Map<IEnumerable<PaymentOperationRecord>>(entity.PaymentOperationRecords)
                                               .OrderBy(operation => operation.Timestamp);
                return new Payment(
                    entity.Id,
                    new PartialCardNumber(entity.PartialCardNumber, (byte)entity.CardNumberLength),
                    entity.CardHolder,
                    new CardExpiryDate((byte)entity.CardExpiryDateMonth, (byte)entity.CardExpiryDateYear),
                    new PaymentCharge(entity.ChargeAmount, entity.ChargeCurrency),
                    entity.Description,
                    operations);
            })
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<PaymentOperationRecordEntity, PaymentOperationRecord>()
            .ConstructUsing((entity, _) =>
            {
                var metaData = JsonConvert.DeserializeObject<Dictionary<string, string>>(entity.MetaData);
                return new PaymentOperationRecord(
                    entity.Id,
                    entity.Timestamp,
                    PaymentOperation.FromCode(entity.Operation),
                    metaData);
            })
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<(Guid PaymentId, PaymentOperationRecord Record), PaymentOperationRecordEntity>()
            .ConstructUsing((request, _) =>
            {
                var (paymentId, record) = request;
                var metaData = JsonConvert.SerializeObject(record.MetaData, Formatting.None);
                return new PaymentOperationRecordEntity
                {
                    PaymentId = paymentId,
                    Id = record.Id,
                    Timestamp = record.Timestamp,
                    Operation = record.Operation.Code,
                    MetaData = metaData
                };
            })
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<
                (Guid PaymentId, Guid MerchantId, Card Card, PaymentCharge Charge, PaymentDescription Description),
                PaymentEntity>()
            .ConstructUsing((request, _) =>
            {
                var (paymentId, merchantId, card, charge, description) = request;
                var partialCardNumber = new PartialCardNumber(card.Number);
                return new PaymentEntity
                {
                    Id = paymentId,
                    MerchantId = merchantId,
                    Description = description,
                    PartialCardNumber = partialCardNumber.PartialNumber,
                    CardNumberLength = partialCardNumber.NumberLength,
                    CardHolder = card.Holder,
                    CardExpiryDateMonth = card.ExpiryDate.Month,
                    CardExpiryDateYear = card.ExpiryDate.Year,
                    ChargeAmount = charge.Amount,
                    ChargeCurrency = charge.Currency
                };
            })
            .ForAllMembers(opt => opt.Ignore());
    }
}
