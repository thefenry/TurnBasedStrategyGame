using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit parentUnit;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += HandleSelectedUnit;
        UpdateVisual();
    }

    private void HandleSelectedUnit(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        var currentSelectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        _meshRenderer.enabled = currentSelectedUnit == parentUnit;
    }
}
