using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour
{
    [SerializeField] private float stoppingDistance = .1f;
    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private Animator unitAnimator;

    private Vector3 _targetPosition;

    private readonly int _walkParameterHash = Animator.StringToHash("IsWalking");
    private Unit _unit;

    private void Awake()
    {
        _targetPosition = transform.position;
        _unit = GetComponent<Unit>();
    }

    private void Update()
    {
        if (Vector3.Distance(_targetPosition, transform.position) > stoppingDistance)
        {
            Vector3 moveDirection = (_targetPosition - transform.position).normalized;
            float moveSpeed = 4f;
            transform.position += moveDirection * (Time.deltaTime * moveSpeed);

            float rotationSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
            unitAnimator.SetBool(_walkParameterHash, true);
        }
        else
        {
            unitAnimator.SetBool(_walkParameterHash, false);
        }
    }

    public void Move(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    public List<GridPosition> GetValidActionGridPositions()
    {
        var validGridPositions = new List<GridPosition>();

        var currentGridPosition = _unit.GetCurrentGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                var offsetGridPosition = new GridPosition(x, z);
                var testGridPosition = currentGridPosition + offsetGridPosition;
                Debug.Log(testGridPosition);
            }
        }

        return validGridPositions;
    }
}
