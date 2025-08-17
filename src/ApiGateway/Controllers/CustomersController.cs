using Microsoft.AspNetCore.Mvc;
using ApiGateway.Messaging;
using SofkaFinance.Contracts.Customers;
using ApiGateway.Dtos;
using System.Text.Json;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/clientes")]
public class CustomersController : ControllerBase
{
    private readonly IMessagingClient _client;
    public CustomersController(IMessagingClient client) => _client = client;

    [HttpPost]
    public async Task<ActionResult<Customer>> Create([FromBody] CreateCustomerDto body)
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

            var contractResponse = await _client.RequestAsync<CreateCustomerCommand, CreateCustomerResponse>(cmd);

            if (contractResponse.CustomerId.HasValue)
            {
                var gatewayResponse = new Customer(
                    Message: "Cliente creado exitosamente",
                    Cliente: new CustomerEntity(
                        Id: contractResponse.CustomerId.Value,
                        Nombre: body.Name ?? "",
                        Correo: body.Email ?? "",
                        Telefono: body.Phone ?? "",
                        Direccion: body.Address ?? "",
                        Identificacion: body.Identification ?? "",
                        Genero: body.Gender ?? "",
                        Edad: body.Age,
                        FechaCreacion: DateTime.UtcNow
                    )
                );

                return Ok(gatewayResponse);
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando creacion de cliente: {ex.Message}"));
        }
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Customer>> GetById([FromRoute] Guid id)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var query = new GetCustomerByIdQuery(operationId, id);

            var contractResponse = await _client.RequestAsync<GetCustomerByIdQuery, GetCustomerResponse>(query);

            if (contractResponse.Customer != null)
            {
                var json = JsonSerializer.Serialize(contractResponse.Customer);
                var customerData = JsonSerializer.Deserialize<CustomerEntity>(json);

                if (customerData != null)
                {
                    var gatewayResponse = new Customer(
                        Message: "Cliente obtenido exitosamente",
                        Cliente: customerData
                    );

                    return Ok(gatewayResponse);
                }
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando consulta de cliente: {ex.Message}"));
        }
    }

    [HttpGet]
    public async Task<ActionResult<CustomersList>> GetAll()
    {
        try
        {
            var operationId = Guid.NewGuid();
            var query = new GetAllCustomersQuery(operationId);

            var contractResponse = await _client.RequestAsync<GetAllCustomersQuery, GetAllCustomersResponse>(query);

            if (contractResponse.Customers != null)
            {
                var customerEntities = contractResponse.Customers.Select(customer =>
                {
                    var json = JsonSerializer.Serialize(customer);
                    var customerData = JsonSerializer.Deserialize<CustomerEntity>(json);

                    if (customerData == null) return null;

                    return customerData;
                }).Where(x => x != null).Cast<CustomerEntity>().ToArray();

                var gatewayResponse = new CustomersList(
                    Message: "Clientes obtenidos exitosamente",
                    Clientes: customerEntities
                );

                return Ok(gatewayResponse);
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando consulta de clientes: {ex.Message}"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Customer>> Update([FromRoute] Guid id, [FromBody] UpdateCustomerDto body)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new UpdateCustomerCommand(operationId, id, body.Name ?? "", body.Email ?? "", body.Phone ?? "", body.Address ?? "");

            var contractResponse = await _client.RequestAsync<UpdateCustomerCommand, UpdateCustomerResponse>(cmd);

            if (contractResponse.CustomerId.HasValue)
            {
                var gatewayResponse = new Customer(
                    Message: "Cliente actualizado exitosamente",
                    Cliente: new CustomerEntity(
                        Id: id,
                        Nombre: body.Name ?? "",
                        Correo: body.Email ?? "",
                        Telefono: body.Phone ?? "",
                        Direccion: body.Address ?? "",
                        Identificacion: "12345678",
                        Genero: "No especificado",
                        Edad: 0,
                        FechaCreacion: DateTime.UtcNow
                    )
                );

                return Ok(gatewayResponse);
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando actualizacion de cliente: {ex.Message}"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Response>> Delete([FromRoute] Guid id)
    {
        try
        {
            var operationId = Guid.NewGuid();
            var cmd = new DeleteCustomerCommand(operationId, id);

            var contractResponse = await _client.RequestAsync<DeleteCustomerCommand, DeleteCustomerResponse>(cmd);

            if (contractResponse.CustomerId.HasValue)
            {
                return Ok(new Response("Cliente eliminado exitosamente"));
            }

            return BadRequest(new Response(contractResponse.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Response($"Error procesando eliminacion de cliente: {ex.Message}"));
        }
    }
}
