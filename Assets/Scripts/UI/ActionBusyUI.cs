using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    private void Start()
    {
        UnitActionSystem.Instance.OnActionBusyChanged += UnitActionSystem_OnActionBusyChanged;
        ToggleVisibility(false);
    }

    private void UnitActionSystem_OnActionBusyChanged(object sender, bool isBusy)
    {
        ToggleVisibility(isBusy);
    }

    private void ToggleVisibility(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}
