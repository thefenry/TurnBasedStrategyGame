using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Show(Material material)
    {
        _meshRenderer.enabled = true;
        _meshRenderer.material = material;
    }

    public void Hide()
    {
        _meshRenderer.enabled = false;
    }
}
