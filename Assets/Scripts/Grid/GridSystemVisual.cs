using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }


    [SerializeField] private Transform gridSystemVisualSinglePrefab;

    private GridSystemVisualSingle[,] _gridSystemVisualSingles;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There's more than one GridSystemVisual. {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _gridSystemVisualSingles =
            new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];

        for (var x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (var z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                var gridPosition = new GridPosition(x, z);
                var gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab,
                    LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                _gridSystemVisualSingles[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
    }

    private void Update()
    {
        UpdateGridVisual();
    }

    public void HideAllGridPositions()
    {
        for (var x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (var z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                _gridSystemVisualSingles[x, z].Hide();
            }
        }
    }

    public void ShowGridPositions(List<GridPosition> gridPositions)
    {
        foreach (var gridPosition in gridPositions)
        {
            _gridSystemVisualSingles[gridPosition.X, gridPosition.Z].Show();
        }
    }

    private void UpdateGridVisual()
    {
        var selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        var selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        if (selectedUnit == null) { return; }
        
        HideAllGridPositions();
        
        ShowGridPositions(selectedAction.GetValidActionGridPositions());
    }
}
