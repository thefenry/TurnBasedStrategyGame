using System;
using UnityEngine;
using static ShootAction;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform projectileSpawnPosition;

    private static readonly int WalkParameterHash = Animator.StringToHash("IsWalking");
    private static readonly int ShootParameterHash = Animator.StringToHash("Shoot");


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

        if (TryGetComponent(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
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

    private void ShootAction_OnShoot(object sender, OnShootEventArgs e)
    {
        animator.SetTrigger(ShootParameterHash);

        Transform bulletProjectileTransform =
            Instantiate(bulletProjectilePrefab, projectileSpawnPosition.position, Quaternion.identity);

        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.TargetUnit.GetWorldPosition();

        targetUnitShootAtPosition.y = projectileSpawnPosition.position.y;

        bulletProjectile.Setup(targetUnitShootAtPosition);

    }
}
