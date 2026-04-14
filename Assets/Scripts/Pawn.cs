using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Pawn : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10.0f;
    [SerializeField] private float _skinWidth = 0.015f;
    
    // TODO: This is cached. Add a MarkDirty function to refresh if the collider changes size.
    private float _radius;
    private float _halfHeight;
    
    private void Awake()
    {
        var collider = GetComponent<CapsuleCollider>();
        
        _radius = collider.radius - _skinWidth;
        _halfHeight = collider.height * 0.5f - (collider.radius + _skinWidth);
    }

    public void Move(Vector2 direction)
    {
        Vector3 moveDirection = new(direction.x, 0.0f, direction.y);

        if (Physics.CapsuleCast(
                transform.position + transform.up * _halfHeight,
                transform.position - transform.up * _halfHeight,
                _radius,
                moveDirection,
                out RaycastHit hitInfo,
                moveDirection.magnitude,
                -1,
                QueryTriggerInteraction.Ignore))
        {
            Debug.Log($"Hit: {hitInfo.transform.gameObject.name}");
        }
        
        transform.Translate(moveDirection * (_moveSpeed * Time.deltaTime));
    }
}
