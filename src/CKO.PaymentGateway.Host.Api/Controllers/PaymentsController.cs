using CKO.PaymentGateway.Services.Abstractions.Errors;
using CKO.PaymentGateway.Services.Abstractions.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CKO.PaymentGateway.Host.Api.Controllers
{
    /// <summary>
    /// The payments controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentsController"/> class.
        /// </summary>
        /// <param name="service">The payments service.</param>
        /// <param name="logger">The logger.</param>
        public PaymentsController(
            IMediator mediator,
            ILogger<PaymentsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> RetrievePaymentAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var request = new RetrievePaymentRequest(id);
            var response = await _mediator.Send(request, cancellationToken);

            var actionResult = response.Match<IActionResult>(
                                            error => error switch
                                            {
                                                PaymentNotFoundError p => NotFound(),
                                                _ => throw new NotImplementedException()
                                            },
                                            payment => Ok()
                               );

            return actionResult;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPaymentAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}