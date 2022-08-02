using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GridPosition _gridPosition;

    public MoveAction MoveAction { get; private set; }

    private void Awake()
    {
        MoveAction = GetComponent<MoveAction>();
    }

    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this);
    }

    private void Update()
    {
        var newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition == _gridPosition) { return; }

        LevelGrid.Instance.UnitMovedGridPosition(this, _gridPosition, newGridPosition);
        _gridPosition = newGridPosition;
    }

    public GridPosition GetCurrentGridPosition()
    {
        return _gridPosition;
    }
}
