using Customers;
using MassTransit;
using Customers.Application.Interfaces;
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
    private readonly ICustomerService _customerService;
    private readonly IAuthService _authService;

    public GeneralConsumer(
        ICustomerService customerService,
        IAuthService authService)
    {
        _customerService = customerService;
        _authService = authService;
    }

    // Customer Commands
    public async Task Consume(ConsumeContext<CreateCustomerCommand> context)
    {
        var response = await _customerService.CreateAsync(context.Message);
        await context.RespondAsync(response);
    }

    public async Task Consume(ConsumeContext<UpdateCustomerCommand> context)
    {
        var response = await _customerService.UpdateAsync(context.Message);
        await context.RespondAsync(response);
    }

    public async Task Consume(ConsumeContext<DeleteCustomerCommand> context)
    {
        var response = await _customerService.DeleteAsync(context.Message);
        await context.RespondAsync(response);
    }

    // Customer Queries
    public async Task Consume(ConsumeContext<GetCustomerByIdQuery> context)
    {
        var response = await _customerService.GetByIdAsync(context.Message);
        await context.RespondAsync(response);
    }

    public async Task Consume(ConsumeContext<GetAllCustomersQuery> context)
    {
        var response = await _customerService.GetAllAsync(context.Message);
        await context.RespondAsync(response);
    }

    // Auth Commands
    public async Task Consume(ConsumeContext<LoginCommand> context)
    {
        var response = await _authService.LoginAsync(context.Message);
        await context.RespondAsync(response);
    }
}
