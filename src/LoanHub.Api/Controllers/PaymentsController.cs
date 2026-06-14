using LoanHub.Api.Contracts;
using LoanHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanHub.Api.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _payments;

    public PaymentsController(IPaymentService payments) => _payments = payments;

    /// <summary>Records a payment and refreshes the loan status.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentReceipt), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PaymentReceipt>> Pay([FromBody] PaymentRequest request, CancellationToken ct)
        => Ok(await _payments.PayAsync(request, ct));
}
