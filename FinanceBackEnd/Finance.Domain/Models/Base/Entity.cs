using System.ComponentModel.DataAnnotations;
using Finance.Domain.Models.Interfaces;

namespace Finance.Domain.Models.Base;

public abstract class Entity<TId> : IEntity
{
    protected Entity()
    {
    }

    [Key]
    public TId Id { get; set; } = default!;

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
