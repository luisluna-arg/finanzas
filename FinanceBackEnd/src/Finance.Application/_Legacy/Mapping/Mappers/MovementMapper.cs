using Finance.Application.Legacy.Dtos.Movements;
using Finance.Application.Legacy.Mapping.Base;
using Finance.Domain.Models.Movements;

namespace Finance.Application.Legacy.Mapping.Mappers;

public class MovementMapper : BaseMapper<Movement, MovementDto>, IMovementMapper
{
    public MovementMapper(IMappingService mappingService) : base(mappingService)
    {
    }

    public override MovementDto Map(Movement source)
    {
        var target = base.Map(source);

        if (source.AppModule != null)
        {
            target.AppModuleId = source.AppModule.Id;
        }

        if (source.Bank != null)
        {
            target.BankId = source.Bank.Id;
        }

        return target;
    }
}

public interface IMovementMapper : IMapper<Movement, MovementDto>;
