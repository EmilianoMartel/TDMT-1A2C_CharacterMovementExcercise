using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public event Action<Vector2> onMovementInput = delegate { };
    public event Action onJumpInput = delegate { };
    public event Action<Vector2> onLookInput = delegate { };

    public void HandleMovementInput(InputAction.CallbackContext ctx)
    {
        onMovementInput.Invoke(ctx.ReadValue<Vector2>());
    }

    public void HandleJumpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            onJumpInput.Invoke();

    }

    public void HandleLook(InputAction.CallbackContext ctx)
    {
        Vector2 temp = new Vector2(ctx.ReadValue<Vector2>().x,0);
        onLookInput.Invoke(temp);
    }
}
