using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private const int MoveStraightCost = 10;
    private const int MoveDiagonalCost = 14;

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstacleLayerMask;

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

        _gridSystem = new GridSystem<PathNode>(width, height, cellSize,
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));

        // Shows the debug tiles 
        //_gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5f;
                if (Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance,
                    Vector3.up, raycastOffsetDistance * 2, obstacleLayerMask))
                {
                    GetNode(x, z).IsWalkable = false;
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
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
                // Reached final node
                pathLength = endNode.FCost;
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

                if (!neighborNode.IsWalkable)
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost =
                    currentNode.GCost + CalculateDistance(currentNode.GridPosition, neighborNode.GridPosition);

                if (tentativeGCost < neighborNode.GCost)
                {
                    neighborNode.CameFromPathNode = currentNode;
                    neighborNode.GCost = tentativeGCost;
                    neighborNode.HCost = CalculateDistance(neighborNode.GridPosition, endGridPosition);
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        // No path found
        pathLength = 0;
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

        if (gridPosition.X - 1 >= 0)
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
        List<PathNode> pathNodes = new List<PathNode> { endNode };

        PathNode currentNode = endNode;
        while (currentNode.CameFromPathNode != null)
        {
            pathNodes.Add(currentNode.CameFromPathNode);
            currentNode = currentNode.CameFromPathNode;
        }

        pathNodes.Reverse();

        return pathNodes.Select(pathNode => pathNode.GridPosition).ToList();
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return _gridSystem.GetGridObject(gridPosition).IsWalkable;
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }

}
