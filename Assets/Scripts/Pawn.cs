using UnityEngine;

public class Pawn : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10.0f;
    
    public void Move(Vector2 direction)
    {
        Vector3 moveDirection = new(direction.x, 0.0f, direction.y);
        transform.Translate(moveDirection * (_moveSpeed * Time.deltaTime));
    }
}
