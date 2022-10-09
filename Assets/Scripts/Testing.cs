using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            GridPosition startGridPosition = new GridPosition(0, 0);

            var gridPositions = Pathfinding.Instance.FindPath(startGridPosition, mouseGridPosition);

            if (gridPositions == null) { return; }

            for (int i = 0; i < gridPositions.Count - 1; i++)
            {
                Debug.DrawLine(LevelGrid.Instance.GetWorldPosition(gridPositions[i]),
                    LevelGrid.Instance.GetWorldPosition(gridPositions[i + 1]), Color.magenta, 10);
            }
        }
    }
}
