namespace FinanceApi.Models;

using System.ComponentModel.DataAnnotations;

public abstract class Entity : IEntity
{
    [Key]
    public Guid Id { get; set; }

    public virtual void Update(IEntity newData)
    {
        var properties = newData.GetType().GetProperties().Where(o => o.CanRead && o.CanWrite).ToArray();
        foreach (var property in properties)
        {
            property.SetValue(this, property.GetValue(newData));
        }
    }
}
