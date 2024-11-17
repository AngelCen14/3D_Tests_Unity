using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Player {
    public class GameInput : MonoBehaviour {
        public static GameInput Instance { get; private set; }

        InputActions _inputActions;

        public Vector2 MovementInput { get; private set; }

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
            _inputActions.Player.Move.canceled += OnMoveCancelled; ;
            _inputActions.Player.Emote.performed += OnEmotePerformed;
        }

        private void OnEnable() {
            _inputActions.Enable();
        }

        private void OnDisable() {
            _inputActions.Disable();

            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCancelled;
            _inputActions.Player.Emote.performed -= OnEmotePerformed;
        }
        #endregion

        #region Move Events
        private void OnMovePerformed(CallbackContext context) {
            MovementInput = context.ReadValue<Vector2>().normalized;
        }

        private void OnMoveCancelled(CallbackContext callback) {
            MovementInput = Vector2.zero;
        }
        #endregion

        private void OnEmotePerformed(CallbackContext callback) {
            EmotePerformed?.Invoke();
        }
    }
}