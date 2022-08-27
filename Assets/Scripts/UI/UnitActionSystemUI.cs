using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private readonly List<ActionButtonUI> _activeButtons = new();

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

        CreateUnitActionButton();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }


    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButton();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
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

    private void UpdateActionPoints()
    {
        var currentUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointsText.text = $"Action Points: {currentUnit?.GetActionPoints()}";
    }
}
