using System.ComponentModel.DataAnnotations;
using FinanceApi.Domain.Models;
using MediatR;

namespace FinanceApi.Application.Commands.Banks;

public class CreateBankCommand : IRequest<Bank>
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
