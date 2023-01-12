using Microsoft.AspNetCore.Mvc;
using MediatR;

using Ching.Features.BudgetCategory;

[ApiController]
[Route("[controller]")]
public class BudgetCategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    public BudgetCategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<bool>> Get(int id)
    {
        var test = await _mediator.Send(new Create.Command() { Name = "Test " });

        return Ok(test);
    }
}
