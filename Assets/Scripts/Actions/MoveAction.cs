using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] private float stoppingDistance = .1f;
    [SerializeField] private int maxMoveDistance = 4;

    private Vector3 _targetPosition;
    
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    protected override void Awake()
    {
        _targetPosition = transform.position;
        base.Awake();
    }

    private void Update()
    {
        if (!IsActionActive) { return; }

        Vector3 moveDirection = (_targetPosition - transform.position).normalized;
        if (Vector3.Distance(_targetPosition, transform.position) > stoppingDistance)
        {
            float moveSpeed = 4f;
            transform.position += moveDirection * (Time.deltaTime * moveSpeed);
        }
        else
        {
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            ActionComplete();
        }

        float rotationSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
    }

    public override void TakeAction(GridPosition targetPosition, Action onActionComplete)
    {
        _targetPosition = LevelGrid.Instance.GetWorldPosition(targetPosition);

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

                if (currentGridPosition != testGridPosition &&
                    LevelGrid.Instance.IsValidGridPosition(testGridPosition) &&
                    !LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    validGridPositions.Add(testGridPosition);
                }
            }
        }

        return validGridPositions;
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
