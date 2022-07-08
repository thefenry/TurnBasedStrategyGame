using System;

public readonly struct GridPosition: IEquatable<GridPosition>
{
    public GridPosition(int x, int z)
    {
        X = x;
        Z = z;
    }

    public int X { get; }

    public int Z { get; }

    public override string ToString()
    {
        return $"x: {X}; z: {Z}";
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Z);
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.X == b.X && a.Z == b.Z;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }
}
