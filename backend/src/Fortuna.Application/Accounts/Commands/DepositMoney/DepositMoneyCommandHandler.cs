using Fortuna.Application.Abstractions.Messaging;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Application.Common.Exceptions;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Cards;
using Fortuna.Domain.Products.Repositories;

namespace Fortuna.Application.Accounts.Commands.DepositMoney;

public sealed class DepositMoneyCommandHandler : ICommandHandler<DepositMoneyCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DepositMoneyCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(DepositMoneyCommand command, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(command.AccountId, cancellationToken);
        if (product is null)
            throw new NotFoundException("Product not found.");

        var amount = new Money(command.Amount, command.Currency);

        switch (product)
        {
            case BankAccount bankAccount:
                bankAccount.Deposit(amount, command.Title);
                break;
            case Card card:
                card.Deposit(amount, command.Title);
                break;
            default:
                throw new ValidationException("Deposits are supported only for bank accounts and cards.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
