using System;
using UnityEngine;

namespace Player {
    public class GameInput : MonoBehaviour {
        public static GameInput Instance { get; private set; }

        public Vector2 MovementInput { get; private set; }

        public event Action EmotePerformed;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
            } else {
                Instance = this;
            }
        }

        private void Update() {
            // TODO: Refactorizar para usar el Input System
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            MovementInput = new Vector2(x, y).normalized;

            if (Input.GetKeyDown(KeyCode.B)) {
                EmotePerformed?.Invoke();
            }
        }
    }
}