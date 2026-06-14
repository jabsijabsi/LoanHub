using LoanHub.Api.Contracts;
using LoanHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanHub.Api.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customers;

    public CustomersController(ICustomerService customers) => _customers = customers;

    /// <summary>Registers a new customer.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerView), StatusCodes.Status201Created)]
    public async Task<ActionResult<CustomerView>> Create([FromBody] NewCustomerRequest request, CancellationToken ct)
    {
        var created = await _customers.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Returns one customer.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CustomerView), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerView>> GetById(int id, CancellationToken ct)
        => Ok(await _customers.GetAsync(id, ct));

    /// <summary>Returns every loan belonging to a customer.</summary>
    [HttpGet("loans")]
    [ProducesResponseType(typeof(IReadOnlyList<LoanView>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<LoanView>>> Loans([FromQuery] int customerId, CancellationToken ct)
        => Ok(await _customers.GetLoanHistoryAsync(customerId, ct));
}
