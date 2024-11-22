using UnityEngine;

namespace Player {
    public class PlayerAnimator : MonoBehaviour {
        private Animator _animator;

        // Parametros animator
        public static readonly int IsFalling = Animator.StringToHash("IsFalling");
        public static readonly int EmoteTrigger = Animator.StringToHash("EmoteTrigger");
        public static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");
        public static readonly int Speed = Animator.StringToHash("Speed");

        private void Awake() {
            _animator = GetComponent<Animator>();
        }

        private void Start() {
            GameInput.Instance.EmotePerformed += OnEmotePerformed;
        }

        private void OnDestroy() {
            GameInput.Instance.EmotePerformed -= OnEmotePerformed;
        }

        private void OnEmotePerformed() {
            /* Esta animacion utiliza una Avatar Mask para reproducirse a la vez que 
             * el resto de animaciones, pero solo en la parte superior del cuerpo */
            _animator.SetTrigger(EmoteTrigger);
        }

        public void SetAnimatorBool(int param, bool value) {
            _animator.SetBool(param, value);
        }

        public void SetAnimatorTrigger(int trigger) {
            _animator.SetTrigger(trigger);
        }

        public void SetAnimatorFloat(int param, float value) {
            _animator.SetFloat(param, value);
        }
    }
}

