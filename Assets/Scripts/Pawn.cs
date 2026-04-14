using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Pawn : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private float _skinWidth = 0.015f;
    
    // TODO: This is cached. We can add a MarkDirty function to refresh if the collider changes size.
    private float _radius;
    private float _halfHeight;
    
    private void Awake()
    {
        var collider = GetComponent<CapsuleCollider>();
        
        _radius = collider.radius;
        _halfHeight = collider.height * 0.5f - collider.radius;
    }

    public void Move(Vector2 direction)
    {
        Vector3 dir = new(direction.x, 0.0f, direction.y);
        float dist = dir.magnitude * _moveSpeed * Time.deltaTime;

        if (Physics.CapsuleCast(
                transform.position + transform.up * _halfHeight,
                transform.position - transform.up * _halfHeight,
                _radius,
                dir,
                out RaycastHit hit,
                dist,
                -1,
                QueryTriggerInteraction.Ignore))
        {
            Vector3 snapToSurface = dir * (hit.distance - _skinWidth);
            Vector3 leftover = dir * dist - snapToSurface;
            float mag = leftover.magnitude;
            leftover = Vector3.ProjectOnPlane(leftover, hit.normal).normalized;
            leftover *= mag;

            Vector3 slideDir = snapToSurface + leftover;
            
            transform.Translate(slideDir);
            return;
        }
        
        transform.Translate(dir * dist);
    }
}
