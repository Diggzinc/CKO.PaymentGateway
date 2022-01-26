using System.Text.RegularExpressions;
using AutoMapper;
using CKO.PaymentGateway.Api.ViewModels.Requests;
using CKO.PaymentGateway.Api.ViewModels.Responses;
using CKO.PaymentGateway.Models;

namespace CKO.PaymentGateway.Api.ViewModels.Profiles;

public class ApiViewModelsProfile : Profile
{
    public ApiViewModelsProfile()
    {
        CreateMap<ProcessPaymentJsonRequest, (Card card, PaymentCharge charge, PaymentDescription description)>()
            .ConstructUsing((request, _) =>
            {
                var (month, year) = (byte.Parse(request.Card.ExpiryDate[..2]), byte.Parse(request.Card.ExpiryDate[3..]));
                var card = new Card(
                    request.Card.Number,
                    request.Card.SecurityCode,
                    new(month, year),
                    request.Card.Holder);
                var charge = new PaymentCharge(request.Charge.Amount, request.Charge.Currency);
                PaymentDescription description = request.Description;
                return (card, charge, description);
            })
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<Payment, PaymentJsonResponse>()
            .ForMember(dst => dst.Card, opt => opt.MapFrom((src, _) => ToMaskedCardJsonResponse(src)));

        CreateMap<PaymentCharge, PaymentChargeJsonResponse>();

        CreateMap<PaymentDescription, string>()
            .ConstructUsing(src => src);

        CreateMap<Currency, string>()
            .ConstructUsing(src => src.AlphabeticCode);

        CreateMap<PaymentCharge, PaymentChargeJsonResponse>();

        CreateMap<PaymentOperationRecord, PaymentOperationRecordJsonResponse>()
            .ForMember(dst => dst.Timestamp, opt => opt.MapFrom((src, _) => src.Timestamp.ToString("O")))
            .ForMember(dst => dst.Operation, opt => opt.MapFrom((src, _) => src.Operation.Code));
    }

    public static MaskedCardJsonResponse ToMaskedCardJsonResponse(Payment payment)
    {
        // Converts the partial card number representation into a pretty masked value.
        // Not optimized for performance in any way.
        //
        // Example:
        //      Length: 16, PartialNumber: 1234.
        // Result:
        //      **** **** **** 1234

        const int groupSize = 4;
        const char maskedCharacter = '*';
        var maskedNumber = string.Join(
                                    string.Empty,
                                    Enumerable.Repeat(
                                        maskedCharacter,
                                        payment.PartialCardNumber.NumberLength - PartialCardNumber.ExactAmountOfDigits));
        maskedNumber += payment.PartialCardNumber.PartialNumber;
        maskedNumber = Regex.Replace(maskedNumber, $".{{{groupSize}}}", "$0 ").TrimEnd();

        return new MaskedCardJsonResponse
        {
            Number = maskedNumber,
            Holder = payment.CardHolder,
            ExpiryDate = $"{payment.CardExpiryDate.Month:00}/{payment.CardExpiryDate.Year:00}",
        };
    }

    public static MaskedCardJsonResponse BetterToMaskedCardJsonResponse(Payment payment)
    {
        // Converts the partial card number representation into a pretty masked value.
        // A better version of ToMaskedCardJsonResponse this will be used to compare on the performance tests
        // Example:
        //      Length: 16, PartialNumber: 1234.
        // Result:
        //      **** **** **** 1234

        const int groupSize = 4;
        const char maskedCharacter = '*';
        var length = payment.PartialCardNumber.NumberLength;
        var spaces = (byte)(Math.Ceiling(length / (double)groupSize)-1);
        int lastMaskedPosition = -1; 
        if (length % groupSize == 0)
        {
            lastMaskedPosition = 0;
        }
        length += spaces;
        var strArray = new char[length];
        var partialNumber = payment.PartialCardNumber.PartialNumber;
        lastMaskedPosition += length - PartialCardNumber.ExactAmountOfDigits;
        var counter = 0;
        for (var i = 0; i < lastMaskedPosition; i++)
        {
            if (counter == 4)
            {
                strArray[i] = ' ';
                counter = 0;
                continue;
            }

            strArray[i] = maskedCharacter;
            counter++;
        }
        for (int i = 0, j=lastMaskedPosition; i < PartialCardNumber.ExactAmountOfDigits; i++, j++)
        {
            if (counter == 4)
            {
                strArray[j] = ' ';
                counter = 0;
                j++;
            }
            strArray[j] = partialNumber[i];
            counter++;
        }

        var maskedNumber = new string(strArray);

        return new MaskedCardJsonResponse
        {
            Number = maskedNumber,
            Holder = payment.CardHolder,
            ExpiryDate = $"{payment.CardExpiryDate.Month:00}/{payment.CardExpiryDate.Year:00}",
        };
    }
}
