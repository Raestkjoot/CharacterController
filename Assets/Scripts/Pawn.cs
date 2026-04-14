using UnityEngine;

public class Pawn : MonoBehaviour
{
    public void Move(Vector2 direction)
    {
        Debug.Log("Direction: " + direction);
    }
}
