using UnityEngine;

namespace Player {
    public class PlayerAnimator : MonoBehaviour {
        private Animator _animator;

        // Parametros animator
        public static readonly int IsMoving = Animator.StringToHash("isMoving");
        public static readonly int IsFalling = Animator.StringToHash("isFalling");
        public static readonly int EmoteTrigger = Animator.StringToHash("EmoteTrigger");


        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            GameInput.Instance.EmotePerformed += OnEmotePerformed;
        }

        private void OnDisable() {
            GameInput.Instance.EmotePerformed -= OnEmotePerformed;
        }

        private void OnEmotePerformed() {
            _animator.SetTrigger(EmoteTrigger);
        }

        public void SetAnimatorBool(int param, bool value) {
            _animator.SetBool(param, value);
        }
    }
}

