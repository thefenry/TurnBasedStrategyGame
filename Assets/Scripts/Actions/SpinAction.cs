using System;
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

        IsActionActive = false;
        OnActionComplete();
    }

    public void Spin(Action onActionComplete)
    {
        OnActionComplete = onActionComplete;
        IsActionActive = true;
        _totalSpinAmount = 0;
    }
}
