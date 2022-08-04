using System;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    private Camera _camera;

    private bool _isBusy;

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
    }

    private void Update()
    {
        if (_isBusy) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            if (TryHandleUnitSelection())
            {
                return;
            }

            if (selectedUnit != null)
            {
                var mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
                if (selectedUnit.MoveAction.IsValidActionGridPosition(mouseGridPosition))
                {
                    SetBusy();
                    selectedUnit.MoveAction.Move(mouseGridPosition, ClearBusy);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            SetBusy();
            selectedUnit.SpinAction.Spin(ClearBusy);
        }
    }

    private bool TryHandleUnitSelection()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, unitLayerMask)) { return false; }

        hitInfo.transform.TryGetComponent(out Unit unit);
        SetSelectedUnit(unit);
        return true;

    }

    private void SetBusy() => _isBusy = true;

    private void ClearBusy() => _isBusy = false;


    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
}
