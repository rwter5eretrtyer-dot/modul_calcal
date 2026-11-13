using BudgetCalculator.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCalculator.Api.Controllers;

[ApiController]
[Route("api/budget")]
public sealed class BudgetController : ControllerBase
{
    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] BudgetRequest request)
    {
        // Ветка 1: не допускаем нулевое/отрицательное число дней.
        if (request.Days <= 0)
        {
            return BadRequest("Количество дней должно быть больше нуля.");
        }

        // Денежные суммы считаются в decimal, а не в float/double.
        var total = request.DailyCost * request.Days;

        // Ветка 2: страховка увеличивает стоимость на 5%, а не в 5 раз.
        if (request.IncludeInsurance)
        {
            total *= 1.05m;
        }

        return Ok(new { TotalAmount = total });
    }
}
