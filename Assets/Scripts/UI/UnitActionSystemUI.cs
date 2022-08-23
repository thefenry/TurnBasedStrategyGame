using System;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButton();
    }

    //TODO: Add this to pooling system
    private void CreateUnitActionButton()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        var selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        if (selectedUnit == null) { return; }

        foreach (var action in selectedUnit.AvailableActions)
        {
           var actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
           var actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
           actionButtonUI.SetBaseAction(action);
        }
    }
}
