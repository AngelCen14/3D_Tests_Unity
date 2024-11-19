using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    public static GameInput Instance { get; private set; }

    InputActions _inputActions;

    public Vector2 MovementInput { get; private set; }
    public bool Jump {  get; private set; }

    public event Action EmotePerformed;

    #region Unity Methods
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
        _inputActions = new InputActions();
    }

    public void Start() {
        _inputActions.Player.Move.performed += OnMovePerformed;
        _inputActions.Player.Move.canceled += OnMoveCancelled;
        _inputActions.Player.Jump.performed += OnJumpStatusChanged;
        _inputActions.Player.Jump.canceled += OnJumpStatusChanged;
        _inputActions.Player.Emote.performed += OnEmotePerformed;
    }

    private void OnEnable() {
        _inputActions.Enable();
    }

    private void OnDisable() {
        _inputActions.Disable();

        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCancelled;
        _inputActions.Player.Jump.performed -= OnJumpStatusChanged;
        _inputActions.Player.Jump.canceled -= OnJumpStatusChanged;
        _inputActions.Player.Emote.performed -= OnEmotePerformed;
    }
    #endregion

    #region Move Events
    private void OnMovePerformed(InputAction.CallbackContext context) {
        MovementInput = context.ReadValue<Vector2>().normalized;
    }

    private void OnMoveCancelled(InputAction.CallbackContext callback) {
        MovementInput = Vector2.zero;
    }
    #endregion

    private void OnJumpStatusChanged(InputAction.CallbackContext context) {
        Jump = context.ReadValueAsButton();
    }

    private void OnEmotePerformed(InputAction.CallbackContext callback) {
        EmotePerformed?.Invoke();
    }
}