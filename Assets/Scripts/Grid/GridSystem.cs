using UnityEngine;

public class GridSystem
{
    private readonly int _width;
    private readonly int _height;
    private readonly float _cellSize;
    private GridObject[,] _gridObjectsArray;

    public GridSystem(int width, int height, float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;

        _gridObjectsArray = new GridObject[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                _gridObjectsArray[x, z] = new GridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.X, 0, gridPosition.Z) * _cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / _cellSize),
            Mathf.RoundToInt(worldPosition.z / _cellSize));
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                var gridPosition = new GridPosition(x, z);
                var debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                var gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    public GridObject GetGridObject(GridPosition gridPosition)
    {
        return _gridObjectsArray[gridPosition.X, gridPosition.Z];
    }
}
