using System;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int WalkParameterHash = Animator.StringToHash("IsWalking");


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            throw new NullReferenceException("Child Character needs an animator assigned");
        }

        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool(WalkParameterHash, true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool(WalkParameterHash, false);
    }

}
