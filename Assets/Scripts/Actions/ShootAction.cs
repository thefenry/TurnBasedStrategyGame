using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    [SerializeField]
    private int maxShootDistance = 7;

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

    public event EventHandler OnShoot;


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
        OnShoot?.Invoke(this, EventArgs.Empty);
        _targetUnit.TakeDamage();
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
        ActionStart(onActionComplete);

        _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        _currentState = State.Aiming;
        float aimingStateTime = 1f;
        _stateTimer = aimingStateTime;

        _canShoot = true;
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        var validGridPositions = new List<GridPosition>();

        var currentGridPosition = Unit.GetCurrentGridPosition();

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
}
