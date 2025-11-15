using System.ComponentModel.DataAnnotations;
using Finance.Domain.Models.Interfaces;

namespace Finance.Domain.Models.Base;

public abstract class Entity : IEntity
{
    public bool Deactivated { get; set; }

    public virtual void Update(IEntity newData)
    {
        var properties = newData.GetType().GetProperties().Where(o => o.CanRead && o.CanWrite).ToArray();

        foreach (var property in properties)
        {
            property.SetValue(this, property.GetValue(newData));
        }
    }
}


public abstract class Entity<TId> : Entity
{
    protected Entity()
    {
    }

    [Key]
    public TId Id { get; set; } = default!;
}
