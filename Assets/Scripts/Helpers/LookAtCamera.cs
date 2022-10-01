using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool invert;
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (invert)
        {
            var position = transform.position;
            Vector3 directionToCamera = (_cameraTransform.position - position).normalized;
            transform.LookAt(position + directionToCamera * -1);
        }
        else
        {
            transform.LookAt(_cameraTransform);
        }
    }
}
