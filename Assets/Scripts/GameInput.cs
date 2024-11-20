using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    public static GameInput Instance { get; private set; }

    private InputActions _inputActions;

    public Vector2 MovementInput { get; private set; }
    public bool JumpTriggered { get; private set; }

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

    private void LateUpdate() {
        /* Reiniciar el estado del salto despues de cada frame
         * para evitar que el jugador salte mientras el boton
         * este presionado, y hacer que salte solo cuando se pulsa
         */
        if (JumpTriggered) JumpTriggered = false;
    }

    private void OnEnable() {
        _inputActions.Enable();

        _inputActions.Player.Move.performed += OnMovePerformed;
        _inputActions.Player.Move.canceled += OnMoveCancelled;
        _inputActions.Player.Jump.performed += OnJumpButtonStateChanged;
        _inputActions.Player.Jump.canceled += OnJumpButtonStateChanged;
        _inputActions.Player.Emote.performed += OnEmotePerformed;
    }

    private void OnDisable() {
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCancelled;
        _inputActions.Player.Jump.performed -= OnJumpButtonStateChanged;
        _inputActions.Player.Jump.canceled -= OnJumpButtonStateChanged;
        _inputActions.Player.Emote.performed -= OnEmotePerformed;

        _inputActions.Disable();
    }
    #endregion

    #region Move Events
    private void OnMovePerformed(InputAction.CallbackContext context) {
        MovementInput = context.ReadValue<Vector2>().normalized;
    }

    private void OnMoveCancelled(InputAction.CallbackContext context) {
        MovementInput = Vector2.zero;
    }
    #endregion

    #region Jump Events
    private void OnJumpButtonStateChanged(InputAction.CallbackContext context) {
        JumpTriggered = context.ReadValueAsButton();
    }
    #endregion

    private void OnEmotePerformed(InputAction.CallbackContext context) {
        EmotePerformed?.Invoke();
    }
}