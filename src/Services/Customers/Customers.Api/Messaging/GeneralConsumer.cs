using Customers;
using MassTransit;
using Customers.Api.Controllers;
using SofkaFinance.Contracts.Customers;

namespace Customers.Api.Messaging;

public class GeneralConsumer :
    IConsumer<CreateCustomerCommand>,
    IConsumer<UpdateCustomerCommand>,
    IConsumer<DeleteCustomerCommand>,
    IConsumer<GetCustomerByIdQuery>,
    IConsumer<GetAllCustomersQuery>,
    IConsumer<LoginCommand>
{
    private readonly CustomersController _customersController;
    private readonly AuthController _authController;

    public GeneralConsumer(
        CustomersController customersController,
        AuthController authController)
    {
        _customersController = customersController;
        _authController = authController;
    }

    // Customer Commands
    public async Task Consume(ConsumeContext<CreateCustomerCommand> context)
    {
        var response = await _customersController.Create(context.Message);
        await context.RespondAsync(response);
    }

    public async Task Consume(ConsumeContext<UpdateCustomerCommand> context)
    {
        var response = await _customersController.Update(context.Message);
        await context.RespondAsync(response);
    }

    public async Task Consume(ConsumeContext<DeleteCustomerCommand> context)
    {
        var response = await _customersController.Delete(context.Message);
        await context.RespondAsync(response);
    }

    // Customer Queries
    public async Task Consume(ConsumeContext<GetCustomerByIdQuery> context)
    {
        var response = await _customersController.GetById(context.Message);
        await context.RespondAsync(response);
    }

    public async Task Consume(ConsumeContext<GetAllCustomersQuery> context)
    {
        var response = await _customersController.GetAll(context.Message);
        await context.RespondAsync(response);
    }

    // Auth Commands
    public async Task Consume(ConsumeContext<LoginCommand> context)
    {
        var response = await _authController.Login(context.Message);
        await context.RespondAsync(response);
    }
}
