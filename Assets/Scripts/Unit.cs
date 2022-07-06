using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private float stoppingDistance = .1f;

    private Vector3 _targetPosition;

    private void Update()
    {

        if (Vector3.Distance(_targetPosition, transform.position) > stoppingDistance)
        {
            Vector3 moveDirection = (_targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += moveDirection * (Time.deltaTime * moveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Move(new Vector3(4, 0, 4));
        }
    }

    private void Move(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }
}
