using Finance.Application.Commands.CreditCards;
using Finance.Application.Repositories;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.CreditCards;

namespace Finance.Application.Tests.Commands.CreditCards;

public class SetCreditCardDefaultImportTemplateCommandHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IRepository<CreditCard, Guid>> _creditCardRepository;

    public SetCreditCardDefaultImportTemplateCommandHandlerTests()
    {
        _creditCardRepository = new Mock<IRepository<CreditCard, Guid>>();
    }

    private SetCreditCardDefaultImportTemplateCommandHandler CreateHandler() =>
        new(_dbContext, _creditCardRepository.Object);

    [Fact]
    public async Task Set_WhenCreditCardExists_UpdatesDefaultTemplateId()
    {
        var templateId = Guid.NewGuid();
        var creditCard = new CreditCard { Id = Guid.NewGuid(), Name = "Visa" };

        _creditCardRepository
            .Setup(r => r.GetByIdAsync(creditCard.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creditCard);

        var command = new SetCreditCardDefaultImportTemplateCommand
        {
            CreditCardId = creditCard.Id,
            TemplateId = templateId,
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(templateId, creditCard.DefaultImportTemplateId);
        _creditCardRepository.Verify(r => r.UpdateAsync(creditCard, It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        _creditCardRepository.Verify(r => r.PersistAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Set_WithNullTemplateId_ClearsDefaultTemplate()
    {
        var creditCard = new CreditCard
        {
            Id = Guid.NewGuid(),
            Name = "Mastercard",
            DefaultImportTemplateId = Guid.NewGuid(),
        };

        _creditCardRepository
            .Setup(r => r.GetByIdAsync(creditCard.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creditCard);

        var command = new SetCreditCardDefaultImportTemplateCommand
        {
            CreditCardId = creditCard.Id,
            TemplateId = null,
        };

        var result = await CreateHandler().ExecuteAsync(command, default);

        Assert.True(result.IsSuccess);
        Assert.Null(creditCard.DefaultImportTemplateId);
    }

    [Fact]
    public async Task Set_WhenCreditCardNotFound_ThrowsException()
    {
        var cardId = Guid.NewGuid();
        _creditCardRepository
            .Setup(r => r.GetByIdAsync(cardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreditCard?)null);

        var command = new SetCreditCardDefaultImportTemplateCommand
        {
            CreditCardId = cardId,
            TemplateId = Guid.NewGuid(),
        };

        await Assert.ThrowsAsync<Exception>(() => CreateHandler().ExecuteAsync(command, default));
    }
}
