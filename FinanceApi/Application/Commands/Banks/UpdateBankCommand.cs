using System.ComponentModel.DataAnnotations;
using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Commands.Banks;

public class UpdateBankCommand : IRequest<Bank>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}
