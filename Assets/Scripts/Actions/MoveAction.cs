using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] private float stoppingDistance = .1f;
    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private Animator unitAnimator;

    private Vector3 _targetPosition;

    private readonly int _walkParameterHash = Animator.StringToHash("IsWalking");

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

            unitAnimator.SetBool(_walkParameterHash, true);
        }
        else
        {
            unitAnimator.SetBool(_walkParameterHash, false);
            IsActionActive = false;
            OnActionComplete();
        }

        float rotationSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
    }

    public void Move(GridPosition targetPosition, Action onActionComplete)
    {
        _targetPosition = LevelGrid.Instance.GetWorldPosition(targetPosition);
        IsActionActive = true;
        OnActionComplete = onActionComplete;
    }

    public bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        var validGridPositionList = GetValidActionGridPositions();
        return validGridPositionList.Contains(gridPosition);
    }

    public List<GridPosition> GetValidActionGridPositions()
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

    public override string GetActionName()
    {
        return "Move";
    }
}
