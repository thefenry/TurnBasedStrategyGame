using System;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    private Camera _camera;

    private bool _isBusy;
    private BaseAction _selectedAction;

    public event EventHandler OnSelectedUnitChanged;

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

        if (TryHandleUnitSelection()) { return; }

        HandleSelectedAction();
    }

    private bool TryHandleUnitSelection()
    {
        if (!Input.GetMouseButtonDown(0)) { return false; }

        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, unitLayerMask)) { return false; }

        hitInfo.transform.TryGetComponent(out Unit unit);
        SetSelectedUnit(unit);
        return true;
    }

    private void HandleSelectedAction()
    {
        if (!Input.GetMouseButtonDown(0)) { return; }

        var mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        
        if (!_selectedAction.IsValidActionGridPosition(mouseGridPosition)) { return; }

        SetBusy();
        _selectedAction.TakeAction(mouseGridPosition, ClearBusy);
    }

    private void SetBusy() => _isBusy = true;

    private void ClearBusy() => _isBusy = false;


    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.MoveAction);

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        _selectedAction = baseAction;
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
}
