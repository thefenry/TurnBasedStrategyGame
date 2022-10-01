using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    private Camera _camera;

    private bool _isBusy;
    private BaseAction _selectedAction;

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnActionBusyChanged;
    public event EventHandler OnActionStarted;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There's more than one UnitActionSystem. {transform} - {Instance}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _camera = Camera.main;
        SetSelectedUnit(selectedUnit);
    }

    private void Update()
    {
        if (_isBusy) { return; }

        if (!TurnSystem.Instance.IsPlayerTurn()) { return; }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (TryHandleUnitSelection()) { return; }

        HandleSelectedAction();
    }

    private bool TryHandleUnitSelection()
    {
        if (!Input.GetMouseButtonDown(0)) { return false; }

        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, unitLayerMask)) { return false; }

        hitInfo.transform.TryGetComponent(out Unit unit);
        
        if (selectedUnit == unit || unit.IsEnemy())
        {
            //Unit is already selected or is an enemy
            return false;
        }

        SetSelectedUnit(unit);
        return true;
    }

    private void HandleSelectedAction()
    {
        if (!Input.GetMouseButtonDown(0)) { return; }

        var mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

        if (!_selectedAction.IsValidActionGridPosition(mouseGridPosition)) { return; }

        if (!selectedUnit.TrySpendActionPointsToTakeAction(_selectedAction)) { return; }

        SetBusy();
        _selectedAction.TakeAction(mouseGridPosition, ClearBusy);

        OnActionStarted?.Invoke(this, EventArgs.Empty);
    }

    private void SetBusy()
    {
        _isBusy = true;
        OnActionBusyChanged?.Invoke(this, _isBusy);
    }

    private void ClearBusy()
    {
        _isBusy = false;
        OnActionBusyChanged?.Invoke(this, _isBusy);
    }

    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.MoveAction);

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        _selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() => selectedUnit;

    public BaseAction GetSelectedAction() => _selectedAction;
}
