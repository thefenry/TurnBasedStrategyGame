using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualColor gridVisualColor;
        public Material material;
    }
    public enum GridVisualColor
    {
        White,
        Blue,
        Red,
        RedLight,
        Yellow
    }


    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterials;

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

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPostion += LevelGrid_OnAnyUnitMovedGridPosition;

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

    public void ShowGridPositions(List<GridPosition> gridPositions, GridVisualColor gridVisualColor)
    {
        var gridMaterial = GetGridVisualColorMaterial(gridVisualColor);
        foreach (var gridPosition in gridPositions)
        {
            _gridSystemVisualSingles[gridPosition.X, gridPosition.Z].Show(gridMaterial);
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualColor gridVisualColor)
    {
        List<GridPosition> gridPositions = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z =  -range; z <= range; z++)
            {
                
                var gridPositionToAdd = gridPosition + new GridPosition(x, z);
                if (!LevelGrid.Instance.IsValidGridPosition(gridPositionToAdd)) { continue; }

                // This calculates the distance in a radius.
                // Not sure if this is what I want
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }
                gridPositions.Add(gridPositionToAdd);
            }
        }

        ShowGridPositions(gridPositions, gridVisualColor);
    }

    private void UpdateGridVisual()
    {
        var selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        var selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        if (selectedUnit == null) { return; }

        HideAllGridPositions();

        GridVisualColor gridVisualColor;
        switch (selectedAction)
        {
            case ShootAction shootingAction:
                gridVisualColor = GridVisualColor.Red;
                ShowGridPositionRange(selectedUnit.GetCurrentGridPosition(), shootingAction.GetMaxShootingRange(),
                    GridVisualColor.RedLight);
                break;
            case SpinAction:
                gridVisualColor = GridVisualColor.Blue;
                break;
            case MoveAction:
            default:
                gridVisualColor = GridVisualColor.White;
                break;
        }

        ShowGridPositions(selectedAction.GetValidActionGridPositions(), gridVisualColor);
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualColorMaterial(GridVisualColor gridVisualColor)
    {
        foreach (var gridVisualTypeMaterial in gridVisualTypeMaterials)
        {
            if (gridVisualTypeMaterial.gridVisualColor == gridVisualColor)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError($"Could not find Material for color: {gridVisualColor}");
        return null;
    }
}
