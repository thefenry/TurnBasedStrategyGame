using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private const int MoveStraightCost = 10;
    private const int MoveDiagonalCost = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;

    private int _width;
    private int _height;
    private float _cellSize;
    private GridSystem<PathNode> _gridSystem;

    public static Pathfinding Instance { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There's more than one Pathfinding. {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Setup(int width, int height, float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;

        _gridSystem = new GridSystem<PathNode>(_width, _height, _cellSize,
            (GridSystem<PathNode> gridObject, GridPosition gridPosition) => new PathNode(gridPosition));

        _gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = _gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = _gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);


        for (int x = 0; x < _gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < _gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = _gridSystem.GetGridObject(gridPosition);

                pathNode.GCost = int.MaxValue;
                pathNode.HCost = 0;
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.GCost = 0;
        startNode.HCost = CalculateDistance(startGridPosition, endGridPosition);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)
            {
                //Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighborNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighborNode))
                {
                    continue;
                }

                int tentativeGCost =
                    currentNode.GCost + CalculateDistance(currentNode.GridPosition, neighborNode.GridPosition);

                if (tentativeGCost >= neighborNode.GCost) { continue; }

                neighborNode.CameFromPathNode = currentNode;
                neighborNode.GCost = tentativeGCost;
                neighborNode.HCost = CalculateDistance(startGridPosition, endGridPosition);
                neighborNode.CalculateFCost();

                if (!openList.Contains(neighborNode))
                {
                    openList.Add(neighborNode);
                }
            }
        }

        // No path found
        return null;
    }

    private int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;

        int xDistance = Mathf.Abs(gridPositionDistance.X);
        int zDistance = Mathf.Abs(gridPositionDistance.Z);

        int remaining = Mathf.Abs(xDistance - zDistance);

        return MoveDiagonalCost * Mathf.Min(xDistance, zDistance) + MoveStraightCost * remaining;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodes)
    {
        PathNode lowestFCostPathNode = pathNodes[0];
        foreach (var t in pathNodes.Where(t => t.FCost < lowestFCostPathNode.FCost))
        {
            lowestFCostPathNode = t;
        }

        return lowestFCostPathNode;
    }

    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GridPosition;


        if (gridPosition.X - 1 > 0)
        {
            // Left
            neighborList.Add(GetNode(gridPosition.X - 1, gridPosition.Z + 0));

            if (gridPosition.Z - 1 >= 0)
            {
                // Left Down
                neighborList.Add(GetNode(gridPosition.X - 1, gridPosition.Z - 1));
            }

            if (gridPosition.Z + 1 < _gridSystem.GetHeight())
            {
                // Left Up
                neighborList.Add(GetNode(gridPosition.X - 1, gridPosition.Z + 1));
            }
        }

        if (gridPosition.X + 1 < _gridSystem.GetWidth())
        {
            // Right
            neighborList.Add(GetNode(gridPosition.X + 1, gridPosition.Z + 0));

            if (gridPosition.Z - 1 >= 0)
            {
                // Right Down
                neighborList.Add(GetNode(gridPosition.X + 1, gridPosition.Z - 1));
            }

            if (gridPosition.Z + 1 < _gridSystem.GetHeight())
            {
                // Right Up
                neighborList.Add(GetNode(gridPosition.X + 1, gridPosition.Z + 1));
            }
        }

        if (gridPosition.Z - 1 >= 0)
        {
            // Down
            neighborList.Add(GetNode(gridPosition.X + 0, gridPosition.Z - 1));
        }

        if (gridPosition.Z + 1 < _gridSystem.GetHeight())
        {
            // Up
            neighborList.Add(GetNode(gridPosition.X + 0, gridPosition.Z + 1));
        }

        return neighborList;
    }

    private PathNode GetNode(int gridPositionX, int gridPositionZ)
    {
        return _gridSystem.GetGridObject(new GridPosition(gridPositionX, gridPositionZ));
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        //List<PathNode> pathNodes = new List<PathNode>();
        List<GridPosition> gridPositions = new List<GridPosition>();
        //pathNodes.Add(endNode);
        PathNode currentNode = endNode;
        gridPositions.Add(endNode.GridPosition);
        while (currentNode.CameFromPathNode != null)
        {
            //pathNodes.Add(currentNode.CameFromPathNode);
            currentNode = currentNode.CameFromPathNode;
            gridPositions.Add(currentNode.GridPosition);
        }

        //pathNodes.Reverse();
        gridPositions.Reverse();
        return gridPositions;

    }
}
