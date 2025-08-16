using Customers;
using MassTransit;
using Customers.Api.Controllers;
using SofkaFinance.Contracts.Customers;

namespace Customers.Api.Messaging;

public class GeneralConsumer :
    IConsumer<CreateCustomerCommand>,
    IConsumer<UpdateCustomerCommand>,
    IConsumer<DeleteCustomerCommand>,
    IConsumer<ActivateCustomerCommand>,
    IConsumer<DeactivateCustomerCommand>,
    IConsumer<GetCustomerByIdQuery>,
    IConsumer<GetAllCustomersQuery>,
    IConsumer<SearchCustomersQuery>,
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

        Console.WriteLine("🚀 GeneralConsumer initialized - Ready to receive commands and queries");
    }    // Customer Commands
    public async Task Consume(ConsumeContext<CreateCustomerCommand> context)
    {
        Console.WriteLine($"Processing CreateCustomerCommand: {context.Message.OperationId}");
        await _customersController.Create(context.Message);
    }

    public async Task Consume(ConsumeContext<UpdateCustomerCommand> context)
    {
        Console.WriteLine($"Processing UpdateCustomerCommand: {context.Message.OperationId}");
        await _customersController.Update(context.Message);
    }

    public async Task Consume(ConsumeContext<DeleteCustomerCommand> context)
    {
        Console.WriteLine($"Processing DeleteCustomerCommand: {context.Message.OperationId}");
        await _customersController.Delete(context.Message);
    }

    public async Task Consume(ConsumeContext<ActivateCustomerCommand> context)
    {
        Console.WriteLine($"Processing ActivateCustomerCommand: {context.Message.OperationId}");
        await _customersController.Activate(context.Message);
    }

    public async Task Consume(ConsumeContext<DeactivateCustomerCommand> context)
    {
        Console.WriteLine($"Processing DeactivateCustomerCommand: {context.Message.OperationId}");
        await _customersController.Deactivate(context.Message);
    }

    // Customer Queries
    public async Task Consume(ConsumeContext<GetCustomerByIdQuery> context)
    {
        Console.WriteLine($"Processing GetCustomerByIdQuery");
        await _customersController.GetById(context.Message);
    }

    public async Task Consume(ConsumeContext<GetAllCustomersQuery> context)
    {
        Console.WriteLine($"Processing GetAllCustomersQuery");
        await _customersController.GetAll(context.Message);
    }

    public async Task Consume(ConsumeContext<SearchCustomersQuery> context)
    {
        Console.WriteLine($"Processing SearchCustomersQuery");
        await _customersController.Search(context.Message);
    }

    // Auth Commands
    public async Task Consume(ConsumeContext<LoginCommand> context)
    {
        Console.WriteLine($"Processing LoginCommand: {context.Message.OperationId}");
        await _authController.Login(context.Message);
    }
}
