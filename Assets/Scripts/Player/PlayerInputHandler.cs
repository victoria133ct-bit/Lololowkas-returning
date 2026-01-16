using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput controls;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool attackPressed;

    private void Awake()
    {
        controls = new PlayerInput();
    }

    private void OnEnable()
    {
        Debug.Log("Enable");
        controls.Player.Enable();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Attack.performed += ctx => attackPressed = true;
        controls.Player.Jump.performed += ctx => jumpPressed = true;
    }

    private void OnDisable()
    {
        Debug.Log("Disable");
        controls.Player.Disable();
    }

    void Update()
    {
        // ?????? ??????
        //Debug.Log($"Move: {moveInput}, Jump: {jumpPressed}");
    }

    public Vector2 GetMoveInput() => moveInput;

    public bool ConsumeJump()
    {
        if (!jumpPressed) return false;
        jumpPressed = false;
        return true;
    }
    public bool ConsumeAttack()
    {
        if (!attackPressed) return false;
        attackPressed = false;
        return true;
    }
}