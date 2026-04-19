using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Pawn : MonoBehaviour
{
    [SerializeField] private float _maxMoveSpeed = 10.0f;
    [SerializeField] private float _acceleration = 10.0f;
    [SerializeField] private float _deceleration = 10.0f;
    [SerializeField] private float _gravity = 10.0f;
    [SerializeField] private float _skinWidth = 0.015f;
    
    // NOTE: This is cached. We can add a MarkDirty function to refresh if the collider changes size.
    private float _radius;
    private float _halfHeight;
    
    private bool _isGrounded = false;
    private Vector2 _horizontalVelocity = Vector2.zero;
    private float _verticalVelocity = 0.0f;

    private const uint MaxBounces = 5;
    
    private void Awake()
    {
        var collider = GetComponent<CapsuleCollider>();
        
        _radius = collider.radius;
        _halfHeight = collider.height * 0.5f - collider.radius;
    }

    public void Move(Vector2 direction)
    {
        Vector2.MoveTowards(_horizontalVelocity, Vector2.zero, _deceleration * Time.deltaTime);
        Vector3 accelerationLeft =  new(direction.x, 0.0f, direction.y);
        accelerationLeft *= _acceleration * Time.deltaTime;
        Vector3 velocityChange = Vector3.zero;
        
        Vector3 capsulePoint1 = transform.position + transform.up * _halfHeight;
        Vector3 capsulePoint2 = transform.position - transform.up * _halfHeight;
        RaycastHit hit;
        
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
                    accelerationLeft,
                    out hit,
                    accelerationLeft.magnitude,
                    -1,
                    QueryTriggerInteraction.Ignore))
            {
                Vector3 snapToSurface = accelerationLeft.normalized * (hit.distance - _skinWidth);

                if (snapToSurface.magnitude <= _skinWidth)
                {
                    snapToSurface = Vector3.zero;
                }
                
                velocityChange = snapToSurface;
                capsulePoint1 += snapToSurface;
                capsulePoint2 += snapToSurface;

                accelerationLeft -= snapToSurface;
                float mag = accelerationLeft.magnitude;
                accelerationLeft = Vector3.ProjectOnPlane(accelerationLeft, hit.normal).normalized;
                accelerationLeft *= mag;
            }
            else
            {
                velocityChange += accelerationLeft;
                break;
            }
        }

        velocityChange = Vector3.ClampMagnitude(velocityChange, _maxMoveSpeed);

        float maxDist = _skinWidth + _halfHeight;
        if (_verticalVelocity < 0.0f)
        {
            maxDist -= _verticalVelocity;
        }
        
        if (Physics.SphereCast(transform.position, _radius, Vector3.down, out hit, maxDist,
                -1, QueryTriggerInteraction.Ignore))
        {
            _isGrounded = true;
            _verticalVelocity = 0.0f;
            velocityChange.y = _skinWidth + _halfHeight - hit.distance;
        }
        else
        {
            _isGrounded = false;
            _verticalVelocity -= _gravity * Time.deltaTime;
            velocityChange.y = _verticalVelocity;
        }
        
        VisualDebug.Instance.DrawSphere(transform.position, _radius, _isGrounded ? Color.green : Color.red);
        VisualDebug.Instance.DrawSphere(transform.position + Vector3.down * maxDist, _radius, _isGrounded ? Color.green : Color.red);
        
        transform.Translate(velocityChange);
    }
}
