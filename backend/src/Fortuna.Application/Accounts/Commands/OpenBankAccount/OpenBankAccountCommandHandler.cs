using Fortuna.Application.Abstractions.Messaging;
using Fortuna.Application.Abstractions.Persistence;
using Fortuna.Application.Common.Exceptions;
using Fortuna.Application.Products;
using Fortuna.Domain.Accounts;
using Fortuna.Domain.Accounts.Repositories;
using Fortuna.Domain.Customers;
using Fortuna.Domain.Customers.Repositories;
using Fortuna.Domain.Products.Repositories;

namespace Fortuna.Application.Accounts.Commands.OpenBankAccount;

public sealed class OpenBankAccountCommandHandler : ICommandHandler<OpenBankAccountCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OpenBankAccountCommandHandler(
        ICustomerRepository customerRepository,
        IBankAccountRepository bankAccountRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _bankAccountRepository = bankAccountRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(OpenBankAccountCommand command, CancellationToken cancellationToken)
    {
        var customerId = new CustomerId(command.CustomerId);
        var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
            throw new NotFoundException("Customer not found.");

        var nextNumberSequence = await _productRepository.GetNextNumberSequenceAsync(cancellationToken);
        var bankAccount = BankAccount.Open(
            customerId,
            new AccountNumber(ProductNumberGenerator.GenerateAccountNumber(nextNumberSequence)),
            command.AccountName,
            nextNumberSequence,
            command.Currency,
            (BankAccountType)command.AccountType);

        await _bankAccountRepository.AddAsync(bankAccount, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return bankAccount.Id;
    }
}
