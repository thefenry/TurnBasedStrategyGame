using System;
using UnityEngine;

public class Unit : MonoBehaviour
{

    [SerializeField] private int maxActionPoints = 2;

    [SerializeField] private bool isEnemy;

    private GridPosition _gridPosition;

    private int _actionPointsRemaining;
    private HealthSystem _healthSystem;

    public MoveAction MoveAction { get; private set; }
    public SpinAction SpinAction { get; private set; }

    public BaseAction[] AvailableActions { get; private set; }

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnySpawned;
    public static event EventHandler OnAnyUnitDead;

    private void Awake()
    {
        MoveAction = GetComponent<MoveAction>();
        SpinAction = GetComponent<SpinAction>();

        AvailableActions = GetComponents<BaseAction>();

        _healthSystem = GetComponent<HealthSystem>();
        _actionPointsRemaining = maxActionPoints;
    }

    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        _healthSystem.OnDeath += HealthSystem_OnDeath;

        OnAnySpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        var newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition == _gridPosition) { return; }

        var previousGridPosition = _gridPosition;
        _gridPosition = newGridPosition;
        LevelGrid.Instance.UnitMovedGridPosition(this, previousGridPosition, newGridPosition);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((!IsEnemy() || TurnSystem.Instance.IsPlayerTurn()) &&
            (IsEnemy() || !TurnSystem.Instance.IsPlayerTurn())) { return; }

        _actionPointsRemaining = maxActionPoints;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetCurrentGridPosition()
    {
        return _gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (!CanSpendActionPointsToTakeAction(baseAction)) { return false; }

        SpendActionPoints(baseAction.GetActionPointCost());
        return true;

    }

    private bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        return baseAction.GetActionPointCost() <= _actionPointsRemaining;
    }

    private void SpendActionPoints(int amount)
    {
        _actionPointsRemaining -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return _actionPointsRemaining;
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void TakeDamage(int damageAmount)
    {
        _healthSystem.TakeDamage(damageAmount);
    }

    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(_gridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

}
