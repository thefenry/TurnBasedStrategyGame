using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    private GridObject _gridObject;
    private TextMeshPro _textMeshPro;

    private void Start()
    {
        _textMeshPro = GetComponentInChildren<TextMeshPro>();
    }

    public void SetGridObject(GridObject gridObject)
    {
        _gridObject = gridObject;
    }

    private void Update()
    {
        _textMeshPro.text = _gridObject.ToString();
    }
}
