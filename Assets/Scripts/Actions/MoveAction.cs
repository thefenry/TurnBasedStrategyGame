using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] private float stoppingDistance = .1f;
    [SerializeField] private int maxMoveDistance = 4;

    private List<Vector3> _targetPositions;
    private int _currentPositionIndex;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    private void Update()
    {
        if (!IsActionActive) { return; }

        var targetPosition = _targetPositions[_currentPositionIndex];

        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float rotationSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);

        if (Vector3.Distance(targetPosition, transform.position) > stoppingDistance)
        {
            float moveSpeed = 4f;
            transform.position += moveDirection * (Time.deltaTime * moveSpeed);
        }
        else
        {
            _currentPositionIndex++;
            if (_currentPositionIndex >= _targetPositions.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition targetPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(Unit.GetCurrentGridPosition(), targetPosition, out int pathLength);
        _currentPositionIndex = 0;
        _targetPositions = new List<Vector3>();

        foreach (var pathGridPosition in pathGridPositionList)
        {
            _targetPositions.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        var validGridPositions = new List<GridPosition>();
        var currentGridPosition = Unit.GetCurrentGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                var offsetGridPosition = new GridPosition(x, z);
                var testGridPosition = currentGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (currentGridPosition == testGridPosition)
                {
                    // Same Grid Position where the unit is already at
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid Position already occupied with another Unit
                    continue;
                }

                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!Pathfinding.Instance.HasPath(currentGridPosition, testGridPosition))
                {
                    continue;
                }

                if (!IsPathLengthValid(currentGridPosition, testGridPosition))
                {
                    // Path length is too long
                    continue;
                }

                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }

    private bool IsPathLengthValid(GridPosition unitGridPosition, GridPosition testGridPosition)
    {
        int pathFindingDistanceMultiplier = 10; // this value should be accessed from the pathfinding movement cost
        var pathLength = Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition);
        var maxMoveDistanceMultiplied = maxMoveDistance * pathFindingDistanceMultiplier;
        var pathIsValidLength = pathLength <= maxMoveDistanceMultiplied;
     
        return pathIsValidLength;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        ShootAction shootAction = Unit.GetAction<ShootAction>();
        int targetCountAtGridPosition = shootAction.GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = targetCountAtGridPosition * 10 //THIS IS HOW MUCH THIS IS WORTH COMPARED TO OTHER ACTIONS. SHOULD NOT BE HARD CODED
        };
    }

    public override string GetActionName()
    {
        return "Move";
    }
}
