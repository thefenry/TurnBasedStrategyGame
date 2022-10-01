using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }

    private float _timer;
    private State _currentState;

    private void Awake()
    {
        _currentState = State.WaitingForEnemyTurn;
    }


    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (_currentState)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                _timer -= Time.deltaTime;

                if (_timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        _currentState = State.Busy;
                    }
                    else
                    {
                        // No more enemies have actions to take
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayerTurn()) { return; }

        _currentState = State.TakingTurn;
        _timer = 2f;
    }

    private void SetStateTakingTurn()
    {
        _timer = 0.5f;
        _currentState = State.TakingTurn;
    }

    private bool TryTakeEnemyAIAction(Action onEnemyActionComplete)
    {
        //TODO: Determine if we can make this call once rather than multiple times
        var enemyUnits = UnitManager.Instance.GetEnemyUnitList();
        foreach (var unit in enemyUnits)
        {
            if (TryTakeEnemyAIAction(unit, onEnemyActionComplete))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit unit, Action onEnemyActionComplete)
    {
        SpinAction spinAction = unit.SpinAction;

        var actionGridPosition = unit.GetCurrentGridPosition();

        if (!spinAction.IsValidActionGridPosition(actionGridPosition)) { return false; }

        if (!unit.TrySpendActionPointsToTakeAction(spinAction)) { return false; }

        spinAction.TakeAction(actionGridPosition, onEnemyActionComplete);

        return true;
    }
}
