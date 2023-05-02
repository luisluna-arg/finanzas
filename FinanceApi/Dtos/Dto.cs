using FinanceApi.Models;

namespace FinanceApi.Dtos;

public abstract class Dto<TEntity>
    where TEntity : Entity
{
    public abstract TEntity BuildEntity();
}