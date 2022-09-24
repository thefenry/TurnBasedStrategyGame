using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private const float SpinAmount = 360f;
    private float _totalSpinAmount;

    private void Update()
    {
        if (!IsActionActive) { return; }

        float spinAddAmount = SpinAmount * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

        _totalSpinAmount += spinAddAmount;

        if (!(_totalSpinAmount >= SpinAmount)) { return; }

        ActionComplete();
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        _totalSpinAmount = 0;
        ActionStart(onActionComplete);
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        var currentGridPosition = Unit.GetCurrentGridPosition();

        return new List<GridPosition> { currentGridPosition };
    }
}
