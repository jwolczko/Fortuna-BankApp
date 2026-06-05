using Fortuna.Application.Abstractions.Messaging;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Application.Common.Exceptions;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Cards;
using Fortuna.Domain.Products;
using Fortuna.Domain.Products.Repositories;
using Fortuna.Domain.Transfers;
using Fortuna.Domain.Transfers.Repositories;

namespace Fortuna.Application.Transfers.Commands.CreateTransfer;

public sealed class CreateTransferCommandHandler : ICommandHandler<CreateTransferCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly ITransferRepository _transferRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTransferCommandHandler(
        IProductRepository productRepository,
        ITransferRepository transferRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _transferRepository = transferRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateTransferCommand command, CancellationToken cancellationToken)
    {
        var transferType = ParseTransferType(command.TransferType);
        var amount = new Money(command.Amount, command.Currency);
        var source = await _productRepository.GetByIdAsync(command.SourceAccountId, cancellationToken);

        if (source is null || source.CustomerId.Value != command.CustomerId)
            throw new NotFoundException("Source product not found.");

        if (transferType == TransferType.External)
        {
            if (string.IsNullOrWhiteSpace(command.TargetAccountNumber))
                throw new ValidationException("Target account number is required for external transfers.");

            if (string.IsNullOrWhiteSpace(command.RecipientName))
                throw new ValidationException("Recipient name is required for external transfers.");

            var externalTransfer = Transfer.CreateExternal(
                source.Id,
                amount,
                command.Title,
                command.TargetAccountNumber.Trim(),
                command.RecipientName.Trim());

            WithdrawFromProduct(source, amount, command.Title, externalTransfer.Id.Value);
            externalTransfer.MarkCompleted();

            await _transferRepository.AddAsync(externalTransfer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return externalTransfer.Id.Value;
        }

        if (!command.TargetAccountId.HasValue)
            throw new ValidationException("Target account is required for own transfers.");

        if (command.SourceAccountId == command.TargetAccountId.Value)
            throw new ValidationException("Source and target account must be different.");

        var target = await _productRepository.GetByIdAsync(command.TargetAccountId.Value, cancellationToken);

        if (target is null || target.CustomerId.Value != command.CustomerId)
            throw new NotFoundException("Target product not found.");

        var ownTransfer = Transfer.CreateOwn(source.Id, target.Id, amount, command.Title);

        WithdrawFromProduct(source, amount, command.Title, ownTransfer.Id.Value);
        DepositToProduct(target, amount, command.Title, ownTransfer.Id.Value);
        ownTransfer.MarkCompleted();

        await _transferRepository.AddAsync(ownTransfer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ownTransfer.Id.Value;
    }

    private static TransferType ParseTransferType(string transferType)
    {
        if (Enum.TryParse<TransferType>(transferType, true, out var parsedTransferType))
            return parsedTransferType;

        throw new ValidationException("Transfer type must be 'Own' or 'External'.");
    }

    private static void WithdrawFromProduct(Product product, Money amount, string title, Guid transferId)
    {
        switch (product)
        {
            case BankAccount bankAccount:
                bankAccount.Withdraw(amount, title, transferId);
                break;
            case Card card:
                card.Withdraw(amount, title, transferId);
                break;
            default:
                throw new ValidationException("Transfers are supported only for bank accounts and cards.");
        }
    }

    private static void DepositToProduct(Product product, Money amount, string title, Guid transferId)
    {
        switch (product)
        {
            case BankAccount bankAccount:
                bankAccount.Deposit(amount, title, transferId);
                break;
            case Card card:
                card.Deposit(amount, title, transferId);
                break;
            default:
                throw new ValidationException("Transfers are supported only for bank accounts and cards.");
        }
    }
}
