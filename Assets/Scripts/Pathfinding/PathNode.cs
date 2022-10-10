public class PathNode
{
    private readonly GridPosition _gridPosition;

    private PathNode _cameFromPathNode;
    
    public PathNode(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public override string ToString()
    {
        return _gridPosition.ToString();
    }

    public void ResetCameFromPathNode()
    {
        _cameFromPathNode = null;
    }

    public int GCost { get; set; }

    public int HCost { get; set; }

    private int _fCost;

    public int FCost => _fCost;

    public bool IsWalkable { get; set; } = true;

    public GridPosition GridPosition => _gridPosition;

    public PathNode CameFromPathNode
    {
        get => _cameFromPathNode;
        set => _cameFromPathNode = value;
    }

    public void CalculateFCost()
    {
        _fCost = GCost + HCost;
    }
}
