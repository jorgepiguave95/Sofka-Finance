using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/clientes")]
public class CustomersController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] object body)
        => throw new NotImplementedException();

    [HttpGet("{id:guid}")]
    public ActionResult<object> GetOne([FromRoute] Guid id)
        => throw new NotImplementedException();

    [HttpGet]
    public ActionResult<object> GetAll(
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        => throw new NotImplementedException();

    [HttpPut("{id:guid}")]
    public IActionResult Update([FromRoute] Guid id, [FromBody] object body)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        throw new NotImplementedException();
    }
}
