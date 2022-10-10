using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    private object _gridObject;
    [SerializeField] private TextMeshPro textMeshPro;

    public virtual void SetGridObject(object gridObject)
    {
        _gridObject = gridObject;
    }

    protected  virtual void Update()
    {
        textMeshPro.text = _gridObject.ToString();
    }
}
