using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GridPosition _gridPosition;

    [SerializeField] private int maxActionPoints = 2;
    private int _actionPointsRemaining;

    public MoveAction MoveAction { get; private set; }
    public SpinAction SpinAction { get; private set; }

    public BaseAction[] AvailableActions { get; private set; }

    public static event EventHandler OnAnyActionPointsChanged;


    private void Awake()
    {
        MoveAction = GetComponent<MoveAction>();
        SpinAction = GetComponent<SpinAction>();

        AvailableActions = GetComponents<BaseAction>();

        _actionPointsRemaining = maxActionPoints;
    }

    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }
    
    private void Update()
    {
        var newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition == _gridPosition) { return; }

        LevelGrid.Instance.UnitMovedGridPosition(this, _gridPosition, newGridPosition);
        _gridPosition = newGridPosition;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        _actionPointsRemaining = maxActionPoints;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetCurrentGridPosition()
    {
        return _gridPosition;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }

        return false;
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
}
