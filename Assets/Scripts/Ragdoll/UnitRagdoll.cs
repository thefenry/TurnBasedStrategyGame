using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;
    [SerializeField] private Transform explosionPosition;
    [SerializeField] private float explosionForce = 300f;
    [SerializeField] private float explosionRadius = 10f;

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);
        ApplyExplosionToRagdoll(ragdollRootBone);
    }

    private static void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            var cloneChild = clone.Find(child.name);
            if (cloneChild == null) { continue; }

            cloneChild.position = child.position;
            cloneChild.rotation = child.rotation;

            MatchAllChildTransforms(child, cloneChild);
        }
    }

    //TODO: Made the position be from where the unit gets hit
    //TODO: Make the explosion force better
    private void ApplyExplosionToRagdoll(Transform root)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition.position, explosionRadius);
            }

            ApplyExplosionToRagdoll(child);
        }
    }
}
