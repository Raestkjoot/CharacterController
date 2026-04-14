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

    private const uint MaxBounces = 5;
    
    private void Awake()
    {
        var collider = GetComponent<CapsuleCollider>();
        
        _radius = collider.radius;
        _halfHeight = collider.height * 0.5f - collider.radius;
    }

    public void Move(Vector2 direction)
    {
        Vector3 capsulePoint1 = transform.position + transform.up * _halfHeight;
        Vector3 capsulePoint2 = transform.position - transform.up * _halfHeight;
        Vector3 movementLeft =  new(direction.x, 0.0f, direction.y);
        movementLeft *= _moveSpeed * Time.deltaTime;
        Vector3 move = Vector3.zero;
        
        for (int i = 0;; ++i)
        {
            if (i > MaxBounces)
            {
                return;
            }
            
            if (Physics.CapsuleCast(
                    capsulePoint1,
                    capsulePoint2,
                    _radius,
                    movementLeft,
                    out RaycastHit hit,
                    movementLeft.magnitude,
                    -1,
                    QueryTriggerInteraction.Ignore))
            {
                Vector3 snapToSurface = movementLeft.normalized * (hit.distance - _skinWidth);

                if (snapToSurface.magnitude <= _skinWidth)
                {
                    snapToSurface = Vector3.zero;
                }
                
                move = snapToSurface;
                capsulePoint1 += snapToSurface;
                capsulePoint2 += snapToSurface;

                movementLeft -= snapToSurface;
                float mag = movementLeft.magnitude;
                movementLeft = Vector3.ProjectOnPlane(movementLeft, hit.normal).normalized;
                movementLeft *= mag;
            }
            else
            {
                move += movementLeft;
                break;
            }
        }

        transform.Translate(move);
    }
}
