using FinanceApi.Domain.Models;

public class MovementComparer : IEqualityComparer<Movement>
{
    public bool Equals(Movement? x, Movement? y) =>
        (x == null && y == null) ||
        (x != null &&
        y != null &&
        x.AppModuleId == y.AppModuleId &&
        x.CreatedAt == y.CreatedAt &&
        x.TimeStamp == y.TimeStamp &&
        x.TimeStamp.Date.Subtract(y.TimeStamp.Date).TotalDays == 1 &&
        x.Amount == y.Amount &&
        x.Total == y.Total &&
        x.Concept1 == y.Concept1 &&
        x.Concept2 == y.Concept2);

    public int GetHashCode(Movement obj)
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            int mixerConst = 23;
            hash = (hash * mixerConst) + obj.AppModuleId.GetHashCode();
            hash = (hash * mixerConst) + obj.CreatedAt.GetHashCode();
            hash = (hash * mixerConst) + obj.TimeStamp.GetHashCode();
            hash = (hash * mixerConst) + obj.TimeStamp.GetHashCode();
            hash = (hash * mixerConst) + obj.Amount.GetHashCode();
            hash = (hash * mixerConst) + obj.Total.GetHashCode();
            hash = (hash * mixerConst) + obj.Concept1.GetHashCode();
            hash = (hash * mixerConst) + (obj.Concept2?.GetHashCode() ?? 0);
            return hash;
        }
    }
}
