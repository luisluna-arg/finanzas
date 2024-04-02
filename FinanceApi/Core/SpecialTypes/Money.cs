namespace FinanceApi.Core.SpecialTypes;

public struct Money : IComparable<Money>
{
    private readonly decimal value;

    public Money(decimal value)
    {
        this.value = value;
    }

    public Money(short value)
        : this(Convert.ToDecimal(value))
    {
    }

    public Money(ushort value)
        : this(Convert.ToDecimal(value))
    {
    }

    public Money(int value)
        : this(Convert.ToDecimal(value))
    {
    }

    public Money(uint value)
        : this(Convert.ToDecimal(value))
    {
    }

    public Money(long value)
        : this(Convert.ToDecimal(value))
    {
    }

    public Money(ulong value)
        : this(Convert.ToDecimal(value))
    {
    }

    public decimal Value { get => value; }

    public static implicit operator decimal(Money conversionStruct) => Convert.ToDecimal(conversionStruct.value);

    public static implicit operator short(Money conversionStruct) => Convert.ToInt16(conversionStruct.value);

    public static implicit operator int(Money conversionStruct) => Convert.ToInt32(conversionStruct.value);

    public static implicit operator long(Money conversionStruct) => Convert.ToInt64(conversionStruct.value);

    public static implicit operator Money(decimal amount) => new Money(amount);

    public static implicit operator Money(short amount) => new Money(amount);

    public static implicit operator Money(int amount) => new Money(amount);

    public static implicit operator Money(long amount) => new Money(amount);

    public static bool operator !=(Money left, Money right) => !left.value!.Equals(right.value);

    public static bool operator <(Money left, Money right) => CompareValues(left, right) < 0;

    public static bool operator >(Money left, Money right) => CompareValues(left, right) > 0;

    public static bool operator <=(Money left, Money right) => CompareValues(left, right) <= 0;

    public static bool operator >=(Money left, Money right) => CompareValues(left, right) >= 0;

    public static bool operator ==(Money left, Money right) => left.value!.Equals(right.value);

    public static Money operator *(Money left, Money right) => new Money(left.value * right.value);

    public static Money operator /(Money left, Money right) => new Money(left.value / right.value);

    public static Money operator +(Money left, Money right) => new Money(left.value + right.value);
    
    public static Money operator -(Money left, Money right) => new Money(left.value - right.value);

    public override bool Equals(object? obj) => obj is Money other && Equals(other);

    public bool Equals(Money other) => EqualityComparer<object>.Default.Equals(value, other.value);

    public override int GetHashCode() => value!.GetHashCode();

    public int CompareTo(Money obj) => this.Value.CompareTo(obj.Value);

    private static int CompareValues(Money left, Money right) => Comparer<object>.Default.Compare(left.value, right.value);
}
