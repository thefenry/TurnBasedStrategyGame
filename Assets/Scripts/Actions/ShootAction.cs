using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    [SerializeField] private int maxShootDistance = 7;
    [SerializeField] private int damageAmount = 40;

    private State _currentState;
    private float _stateTimer;

    private Unit _targetUnit;
    private bool _canShoot;

    private enum State
    {
        Aiming,
        Shooting,
        CoolOff
    }

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit TargetUnit;
        public Unit ShootingUnit;
    }

    private void Update()
    {
        if (!IsActionActive) { return; }


        _stateTimer -= Time.deltaTime;
        switch (_currentState)
        {
            case State.Aiming:
                LookAtTarget();
                break;
            case State.Shooting:
                if (_canShoot)
                {
                    Shoot();
                    _canShoot = false;
                }
                break;
            case State.CoolOff:
            default:
                break;
        }

        if (_stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void LookAtTarget()
    {
        Vector3 aimDirection = (_targetUnit.GetWorldPosition() - Unit.GetWorldPosition()).normalized;
        float rotationSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotationSpeed);
    }

    private void Shoot()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs { TargetUnit = _targetUnit, ShootingUnit = Unit });
        _targetUnit.TakeDamage(damageAmount);
    }

    private void NextState()
    {
        switch (_currentState)
        {
            case State.Aiming:
                _currentState = State.Shooting;
                float shootingStateTime = 0.1f;
                _stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                _currentState = State.CoolOff;
                float coolOffStateTime = 0.5f;
                _stateTimer = coolOffStateTime;
                break;
            case State.CoolOff:
            default:
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        _currentState = State.Aiming;
        float aimingStateTime = 1f;
        _stateTimer = aimingStateTime;

        _canShoot = true;

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        var currentGridPosition = Unit.GetCurrentGridPosition();
        return GetValidActionGridPositions(currentGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositions(GridPosition currentGridPosition)
    {
        var validGridPositions = new List<GridPosition>();
        
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                var offsetGridPosition = new GridPosition(x, z);
                var testGridPosition = currentGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

                // This calculates the distance in a radius.
                // Not sure if this is what I want
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance)
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) { continue; }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                // Are both units in the same team?
                if (targetUnit.IsEnemy() == Unit.IsEnemy())
                {
                    continue;
                }

                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = 100 //THIS IS HOW MUCH THIS IS WORTH COMPARED TO OTHER ACTIONS. SHOULD NOT BE HARD CODED
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositions(gridPosition).Count;
    }

    public Unit GetTargetUnit() => _targetUnit;

    public int GetMaxShootingRange() => maxShootDistance;
}
