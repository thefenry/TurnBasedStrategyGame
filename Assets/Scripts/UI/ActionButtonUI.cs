using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedVisualImage;

    private BaseAction _baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        text.text = baseAction.GetActionName().ToUpper();
        _baseAction = baseAction;
        button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }

    public void UpdateVisuals()
    {
        var selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedVisualImage.SetActive(selectedBaseAction == _baseAction);
    }
}
