using FluentValidation;
using MediatR;

namespace WebhookManager.Application.Subscriptions.Commands;

public class CreateSubscriptionCommand : IRequest<long>
{

}

public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{

}

public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, long>
{
    Task<long> IRequestHandler<CreateSubscriptionCommand, long>.Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}