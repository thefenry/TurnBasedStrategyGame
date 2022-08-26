using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;

    private readonly List<ActionButtonUI> _activeButtons = new();

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;

        CreateUnitActionButton();
        UpdateSelectedVisual();
    }
    
    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButton();
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    //TODO: Add this to pooling system
    private void CreateUnitActionButton()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        _activeButtons.Clear();

        var selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if (selectedUnit == null) { return; }

        foreach (var action in selectedUnit.AvailableActions)
        {
            var actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            var actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(action);

            _activeButtons.Add(actionButtonUI);
        }
    }

    public void UpdateSelectedVisual()
    {
        foreach (var actionButton in _activeButtons)
        {
            actionButton.UpdateVisuals();
        }
    }
}
