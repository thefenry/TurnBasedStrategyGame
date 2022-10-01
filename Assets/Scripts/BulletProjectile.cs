using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 200f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfx;

    private Vector3 _targetPosition;

    public void Setup(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    private void Update()
    {
        var position = transform.position;
        Vector3 moveDir = (_targetPosition - position).normalized;

        float distanceBeforeMoving = Vector3.Distance(position, _targetPosition);

        position += moveDir * (moveSpeed * Time.deltaTime);
        transform.position = position;

        float distanceAfterMoving = Vector3.Distance(position, _targetPosition);

        if (!(distanceBeforeMoving < distanceAfterMoving)) { return; }
        transform.position = _targetPosition;

        trailRenderer.transform.parent = null;

        Destroy(gameObject);

        Instantiate(bulletHitVfx, _targetPosition, Quaternion.identity);

    }
}
