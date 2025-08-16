using Microsoft.AspNetCore.Mvc;
using ApiGateway.Messaging;
using SofkaFinance.Contracts.Customers;
using ApiGateway.Dtos;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/clientes")]
public class CustomersController : ControllerBase
{
    private readonly IMessagingClient _client;
    public CustomersController(IMessagingClient client) => _client = client;

    [HttpPost]
    public async Task<ActionResult<CustomerOperationResponse>> Create([FromBody] CreateCustomerDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new CreateCustomerCommand(
                operationId,
                body.Name ?? "",
                body.Gender ?? "",
                body.Age,
                body.Identification ?? "",
                body.Address ?? "",
                body.Phone ?? "",
                body.Email ?? "",
                body.Password ?? ""
            );
            await _client.SendAsync("finance.customers", "customer.create", cmd);

            var response = new CustomerOperationResponse(
                OperationId: operationId,
                Operation: "CreateCustomer",
                Message: $"Solicitud de creacion de cliente para '{body.Name}' fue exitosa"
            );

            return Accepted(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new CustomerOperationResponse(
                OperationId: Guid.Empty,
                Operation: "CreateCustomer",
                Message: $"Error procesando creacion de cliente: {ex.Message}",
                StatusCode: 500
            );
            return StatusCode(500, errorResponse);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerOperationResponse>> GetById([FromRoute] Guid id)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new GetCustomerByIdQuery(operationId, id);
            await _client.SendAsync("finance.customers", "customer.getById", cmd);

            var response = new CustomerOperationResponse(
                OperationId: operationId,
                Operation: "GetCustomerById",
                Message: $"Consulta de cliente para ID '{id}' fue exitosa"
            );

            return Accepted(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new CustomerOperationResponse(
                OperationId: Guid.Empty,
                Operation: "GetCustomerById",
                Message: $"Error procesando consulta de cliente: {ex.Message}",
                StatusCode: 500
            );
            return StatusCode(500, errorResponse);
        }
    }

    [HttpGet]
    public async Task<ActionResult<CustomerOperationResponse>> GetAll()
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new GetAllCustomersQuery(operationId);
            await _client.SendAsync("finance.customers", "customer.getAll", cmd);

            var response = new CustomerOperationResponse(
                OperationId: operationId,
                Operation: "GetAllCustomers",
                Message: "Solicitud para obtener todos los clientes fue exitosa"
            );

            return Accepted(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new CustomerOperationResponse(
                OperationId: Guid.Empty,
                Operation: "GetAllCustomers",
                Message: $"Error procesando consulta de clientes: {ex.Message}",
                StatusCode: 500
            );
            return StatusCode(500, errorResponse);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerOperationResponse>> Update([FromRoute] Guid id, [FromBody] UpdateCustomerDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new UpdateCustomerCommand(operationId, id, body.Name ?? "", body.Email ?? "", body.Phone ?? "", body.Address ?? "");
            await _client.SendAsync("finance.customers", "customer.update", cmd);

            var response = new CustomerOperationResponse(
                OperationId: operationId,
                Operation: "UpdateCustomer",
                Message: $"Solicitud de actualizacion de cliente para ID '{id}' fue exitosa"
            );

            return Accepted(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new CustomerOperationResponse(
                OperationId: Guid.Empty,
                Operation: "UpdateCustomer",
                Message: $"Error procesando actualizacion de cliente: {ex.Message}",
                StatusCode: 500
            );
            return StatusCode(500, errorResponse);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<CustomerOperationResponse>> Delete([FromRoute] Guid id)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new DeleteCustomerCommand(operationId, id);
            await _client.SendAsync("finance.customers", "customer.delete", cmd);

            var response = new CustomerOperationResponse(
                OperationId: operationId,
                Operation: "DeleteCustomer",
                Message: $"Solicitud de eliminacion de cliente para ID '{id}' fue exitosa"
            );

            return Accepted(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new CustomerOperationResponse(
                OperationId: Guid.Empty,
                Operation: "DeleteCustomer",
                Message: $"Error procesando eliminacion de cliente: {ex.Message}",
                StatusCode: 500
            );
            return StatusCode(500, errorResponse);
        }
    }
}
