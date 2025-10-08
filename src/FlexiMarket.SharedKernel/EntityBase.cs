namespace FlexiMarket.SharedKernel;

public abstract class Entity
{
    public override bool Equals(object obj)
    {
        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return GetHashCode() == other.GetHashCode();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
