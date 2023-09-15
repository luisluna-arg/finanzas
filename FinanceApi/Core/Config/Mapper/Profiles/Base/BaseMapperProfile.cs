using AutoMapper;
using FinanceApi.Application.Dtos;
using FinanceApi.Domain.Models.Interfaces;

namespace FinanceApi.Core.Config.Mapper.Profiles.Base;

public abstract class BaseMapperProfile<TEntity, TDto> : Profile
    where TEntity : IEntity
    where TDto : IDto
{
    protected BaseMapperProfile() => CreateMap<TEntity, TDto>();
}
