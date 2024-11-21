using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    public static GameInput Instance { get; private set; }

    private InputActions _inputActions;

    public event Action EmotePerformed;
    public event Action JumpTriggered;

    #region Unity Methods
    private void Awake() {
        Instance = this;

        _inputActions = new InputActions();
        _inputActions.Enable();

        _inputActions.Player.Jump.performed += OnJumpPerformed;
        _inputActions.Player.Emote.performed += OnEmotePerformed;
    }

    private void OnDestroy() {
        _inputActions.Player.Jump.performed -= OnJumpPerformed;
        _inputActions.Player.Emote.performed -= OnEmotePerformed;

        _inputActions.Dispose();
    }
    #endregion

    #region Public Methods
    public Vector2 GetMovementInput() {
        return _inputActions.Player.Move.ReadValue<Vector2>().normalized;
    }
    #endregion

    #region Input Events
    private void OnJumpPerformed(InputAction.CallbackContext context) {
        JumpTriggered?.Invoke();
    }

    private void OnEmotePerformed(InputAction.CallbackContext context) {
        EmotePerformed?.Invoke();
    }
    #endregion
}