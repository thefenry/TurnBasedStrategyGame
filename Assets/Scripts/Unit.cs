using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private float stoppingDistance = .1f;
    [SerializeField] private Animator unitAnimator;

    private Vector3 _targetPosition;

    private readonly int _walkParameterHash = Animator.StringToHash("IsWalking");

    private void Awake()
    {
        _targetPosition = transform.position;
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
}
