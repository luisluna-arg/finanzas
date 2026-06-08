using Finance.Application.Queries.CreditCards;
using Finance.Application.Tests.Queries.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Identities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace Finance.Application.Tests.Queries.CreditCards;

public class GetCreditCardStatementImportTemplatesQueryHandlerTests : QueryHandlerBaseTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

    public GetCreditCardStatementImportTemplatesQueryHandlerTests()
    {
        _httpContextAccessor = new Mock<IHttpContextAccessor>();
    }

    private GetCreditCardStatementImportTemplatesQueryHandler CreateHandler() =>
        new(_dbContext, _httpContextAccessor.Object);

    private void SetupIdentity(string? name)
    {
        var identity = new Mock<IIdentity>();
        identity.Setup(i => i.Name).Returns(name);
        var principal = new Mock<ClaimsPrincipal>();
        principal.Setup(p => p.Identity).Returns(identity.Object);
        var ctx = new Mock<HttpContext>();
        ctx.Setup(c => c.User).Returns(principal.Object);
        _httpContextAccessor.Setup(a => a.HttpContext).Returns(ctx.Object);
    }

    private async Task<User> SeedCurrentUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "u",
            FirstName = "F",
            LastName = "L",
            Identities = [new Identity { SourceId = "IdentityNotFound" }],
        };
        await _dbContext.User.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task GetAll_WithNoIdentity_ReturnsOnlySystemTemplates()
    {
        _httpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        var systemTemplate = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "System",
            IsSystem = true,
            ConfigJson = "{}",
        };
        var userTemplate = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "User",
            IsSystem = false,
            UserId = Guid.NewGuid(),
            ConfigJson = "{}",
        };
        await _dbContext.CreditCardStatementImportTemplate.AddRangeAsync(systemTemplate, userTemplate);
        await _dbContext.SaveChangesAsync();

        var result = await CreateHandler().ExecuteAsync(new GetCreditCardStatementImportTemplatesQuery(), default);

        Assert.True(result.IsSuccess);
        var mine = result.Data.Where(t => t.Id == systemTemplate.Id || t.Id == userTemplate.Id).ToList();
        Assert.Single(mine);
        Assert.Equal(systemTemplate.Id, mine[0].Id);
    }

    [Fact]
    public async Task GetAll_WithUserIdentity_ReturnsSystemAndOwnTemplates()
    {
        var user = await SeedCurrentUser();
        SetupIdentity("IdentityNotFound");

        var systemTemplate = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "System",
            IsSystem = true,
            ConfigJson = "{}",
        };
        var myTemplate = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Mine",
            IsSystem = false,
            UserId = user.Id,
            ConfigJson = "{}",
        };
        var otherTemplate = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Other",
            IsSystem = false,
            UserId = Guid.NewGuid(),
            ConfigJson = "{}",
        };
        await _dbContext.CreditCardStatementImportTemplate.AddRangeAsync(systemTemplate, myTemplate, otherTemplate);
        await _dbContext.SaveChangesAsync();

        var result = await CreateHandler().ExecuteAsync(new GetCreditCardStatementImportTemplatesQuery(), default);

        Assert.True(result.IsSuccess);
        var relevant = result.Data
            .Where(t => t.Id == systemTemplate.Id || t.Id == myTemplate.Id || t.Id == otherTemplate.Id)
            .ToList();
        Assert.Equal(2, relevant.Count);
        Assert.Contains(relevant, t => t.Id == systemTemplate.Id);
        Assert.Contains(relevant, t => t.Id == myTemplate.Id);
        Assert.DoesNotContain(relevant, t => t.Id == otherTemplate.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsSystemTemplatesFirst_ThenAlphabetical()
    {
        var user = await SeedCurrentUser();
        SetupIdentity("IdentityNotFound");

        var templateB = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Banana",
            IsSystem = false,
            UserId = user.Id,
            ConfigJson = "{}",
        };
        var templateSys = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Alpha System",
            IsSystem = true,
            ConfigJson = "{}",
        };
        var templateA = new CreditCardStatementImportTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Apple",
            IsSystem = false,
            UserId = user.Id,
            ConfigJson = "{}",
        };
        await _dbContext.CreditCardStatementImportTemplate.AddRangeAsync(templateB, templateSys, templateA);
        await _dbContext.SaveChangesAsync();

        var result = await CreateHandler().ExecuteAsync(new GetCreditCardStatementImportTemplatesQuery(), default);

        var relevant = result.Data
            .Where(t => t.Id == templateB.Id || t.Id == templateSys.Id || t.Id == templateA.Id)
            .ToList();
        Assert.Equal(3, relevant.Count);
        Assert.Equal(templateSys.Id, relevant[0].Id);
        Assert.Equal(templateA.Id, relevant[1].Id);
        Assert.Equal(templateB.Id, relevant[2].Id);
    }
}
