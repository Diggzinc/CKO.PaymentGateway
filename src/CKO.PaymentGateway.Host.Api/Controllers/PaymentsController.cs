using AutoMapper;
using CKO.PaymentGateway.Api.ViewModels.Requests;
using CKO.PaymentGateway.Api.ViewModels.Responses;
using CKO.PaymentGateway.Host.Api.Constants;
using CKO.PaymentGateway.Models;
using CKO.PaymentGateway.Services.Abstractions.Errors;
using CKO.PaymentGateway.Services.Abstractions.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CKO.PaymentGateway.Host.Api.Controllers
{
    /// <summary>
    /// The payments controller.
    /// </summary>
    [Authorize]
    [ApiVersion(ApiVersionName.V1)]
    [Route("api/v{version:apiVersion}/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentsController"/> class.
        /// </summary>
        /// <param name="mapper">The mapping service.</param>
        /// <param name="mediator">The mediator to issue requests for the payments use cases.</param>
        /// <param name="logger">The logging service.</param>
        public PaymentsController(
            IMapper mapper,
            IMediator mediator,
            ILogger<PaymentsController> logger)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{paymentId:guid}", Name = nameof(RetrievePaymentAsync))]
        [Authorize(Policy = PaymentGatewayPolicy.MerchantOnly)]
        public async Task<IActionResult> RetrievePaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            // Working under the assumption that the token provided by the 'IdentityProvider' is always a GUID
            // and always exists because it passed the policy check.
            var merchantId = new Guid(User.FindFirst(PaymentGatewayClaim.MerchantId)!.Value);

            var request = new RetrievePaymentRequest(merchantId, paymentId);

            var response = await _mediator.Send(request, cancellationToken);

            var actionResult = response.Match<IActionResult>(
            error =>
            {
                switch (error)
                {
                    case PaymentNotFoundError:
                        var problemDetails = ProblemDetailsFactory
                            .CreateProblemDetails(
                                HttpContext,
                                StatusCodes.Status404NotFound,
                                type: "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                                title: "Payment not found.");
                        return BadRequest(problemDetails);

                    default:
                        throw new ArgumentOutOfRangeException(nameof(error));
                }
            },
            retrievePaymentResponse =>
            {
                var payment = retrievePaymentResponse.PaymentReference;
                var result = _mapper.Map<PaymentJsonResponse>(payment);
                return Ok(result);
            });

            return actionResult;
        }

        [HttpPost]
        [Authorize(Policy = PaymentGatewayPolicy.MerchantOnly)]
        public async Task<IActionResult> ProcessPaymentAsync([FromBody] ProcessPaymentJsonRequest payload, CancellationToken cancellationToken = default)
        {
            // Working under the assumption that the token provided by the 'IdentityProvider' is always a GUID
            // and always exists because it passed the policy check.
            var merchantId = new Guid(User.FindFirst(PaymentGatewayClaim.MerchantId)!.Value);

            var (card, charge, description) = _mapper.Map<(Card card, PaymentCharge charge, PaymentDescription description)>(payload);

            var request = new ProcessPaymentRequest(merchantId, card, charge, description);

            var response = await _mediator.Send(request, cancellationToken);

            var actionResult = response.Match<IActionResult>(
            error =>
            {
                switch (error)
                {
                    default:
                        throw new ArgumentOutOfRangeException(nameof(error));
                }
            },
            processPaymentResponse =>
            {
                var payment = processPaymentResponse.PaymentId;
                return CreatedAtRoute(
                        nameof(RetrievePaymentAsync),
                        new
                        {
                            version = ApiVersionName.V1,
                            paymentId = payment
                        },
                        default);
            });

            return actionResult;
        }
    }
}
