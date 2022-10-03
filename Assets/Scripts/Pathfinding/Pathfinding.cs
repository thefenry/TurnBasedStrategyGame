using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private Transform gridDebugObjectPrefab;

    private int width;
    private int height;
    private float cellSize;
    private GridSystem<PathNode> _gridSystem;

    private void Awake()
    {
       _gridSystem = new GridSystem<PathNode>(10, 10, 2f,
            (GridSystem<PathNode> gridObject, GridPosition gridPosition) => new PathNode(gridPosition));

       _gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }
}
