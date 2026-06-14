using LoanHub.Api.Contracts;
using LoanHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanHub.Api.Controllers;

[ApiController]
[Route("api/loans")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loans;

    public LoansController(ILoanService loans) => _loans = loans;

    /// <summary>Submits a loan application; approval is decided automatically.</summary>
    [HttpPost("CreateApplication")]
    [ProducesResponseType(typeof(LoanView), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoanView>> CreateApplication([FromBody] LoanApplicationRequest request, CancellationToken ct)
    {
        var loan = await _loans.ApplyAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    /// <summary>Returns a loan with its current status and schedule.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LoanView), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanView>> GetById(int id, CancellationToken ct)
        => Ok(await _loans.GetAsync(id, ct));
}
