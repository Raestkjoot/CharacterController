using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Pawn _pawn = null;
    
    private Controls _controls;

    public void Possess(Pawn pawn)
    {
        _pawn = pawn;
    }
    
    private void Awake()
    {
        _controls = new();
        _controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _controls.Gameplay.Disable();
    }

    private void FixedUpdate()
    {
        if (!_pawn)
        {
            return;
        }
        
        Vector2 moveDir = _controls.Gameplay.Move.ReadValue<Vector2>();
        _pawn.Move(moveDir);
    }
}