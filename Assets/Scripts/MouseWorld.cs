using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    [SerializeField] private LayerMask mousePlaneLayerMask;

    private Camera _camera;

    private static MouseWorld _instance;

    private void Start()
    {
        _camera = Camera.main;
        _instance = this;
    }
    
    public static Vector3 GetPosition()
    {
        var ray = _instance._camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, _instance.mousePlaneLayerMask);

        return hitInfo.point;
    }
}
