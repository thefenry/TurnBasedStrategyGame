public class PathNode
{
    private readonly GridPosition _gridPosition;

    private int _gCost;
    private int _hCost;
    private int _fCost;

    private PathNode _cameFromPathNode;

    public PathNode(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public override string ToString()
    {
        return _gridPosition.ToString();
    }

    public int GCost => _gCost;
    public int HCost => _hCost;
    public int FCost => _fCost;
}
