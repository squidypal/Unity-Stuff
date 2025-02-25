using UnityEngine;

[ExecuteInEditMode]
public class ColliderGizmo : MonoBehaviour
{
    // Customizable fields
    public Color colliderColor = Color.red;
    public bool showGizmo = true;

    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Collider collider = GetComponent<Collider>();

            if (collider != null)
            {
                Gizmos.color = colliderColor;

                if (collider is BoxCollider boxCollider)
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
                }
                else if (collider is SphereCollider sphereCollider)
                {
                    Gizmos.DrawWireSphere(transform.TransformPoint(sphereCollider.center), sphereCollider.radius);
                }
                else if (collider is CapsuleCollider capsuleCollider)
                {
                    Vector3 topSphere = capsuleCollider.center + Vector3.up * (capsuleCollider.height / 2 - capsuleCollider.radius);
                    Vector3 bottomSphere = capsuleCollider.center - Vector3.up * (capsuleCollider.height / 2 - capsuleCollider.radius);

                    Gizmos.DrawWireSphere(transform.TransformPoint(topSphere), capsuleCollider.radius);
                    Gizmos.DrawWireSphere(transform.TransformPoint(bottomSphere), capsuleCollider.radius);
                    Gizmos.DrawLine(transform.TransformPoint(topSphere), transform.TransformPoint(bottomSphere));
                }
            }
        }
    }
}