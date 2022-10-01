using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    protected bool IsActionActive;
    protected Unit Unit;

    protected Action OnActionComplete;

    [SerializeField, Tooltip("How many action points are needed to perform action")] private int actionCost = 1;

    protected virtual void Awake()
    {
        Unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);
    
    public abstract List<GridPosition> GetValidActionGridPositions();

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        var validGridPositionList = GetValidActionGridPositions();
        return validGridPositionList.Contains(gridPosition);
    }

    public virtual int GetActionPointCost()
    {
        return actionCost;
    }

    protected void ActionStart(Action onActionComplete)
    {
        IsActionActive = true;
        OnActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        IsActionActive = false;
        OnActionComplete();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActions = new();

        var validActionGridPositions = GetValidActionGridPositions();

        foreach (var gridPosition in validActionGridPositions)
        {
            var enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActions.Add(enemyAIAction);
        }

        enemyAIActions.Sort((EnemyAIAction a, EnemyAIAction b) => b.ActionValue - a.ActionValue);
        return enemyAIActions.FirstOrDefault();
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

    public Unit GetUnit() => Unit;
}
