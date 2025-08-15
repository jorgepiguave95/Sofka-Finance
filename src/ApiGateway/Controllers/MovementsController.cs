using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/movimientos")]
public class MovementsController : ControllerBase
{
    [HttpPost("{id:guid}/deposito")]
    public IActionResult Deposit([FromRoute] Guid id, [FromBody] object body)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{id:guid}/retirar")]
    public IActionResult Withdraw([FromRoute] Guid id, [FromBody] object body)
    {
        throw new NotImplementedException();
    }

    [HttpPost("transferencia")]
    public IActionResult Transfer([FromBody] object body)
        => throw new NotImplementedException();

    [HttpGet("{idCliente:guid}/movimientos")]
    public ActionResult<object> Movements(
        [FromRoute] Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
        => throw new NotImplementedException();

     [HttpGet("{idCliente:guid}/reporte")]
    public ActionResult<object> Report(
        [FromRoute] Guid id,
        [FromQuery] DateTime fechaInicio = default,
        [FromQuery] DateTime fechaFin = default)
        => throw new NotImplementedException();
}
