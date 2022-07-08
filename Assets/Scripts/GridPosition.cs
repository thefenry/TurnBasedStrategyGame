public readonly struct GridPosition
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
}
