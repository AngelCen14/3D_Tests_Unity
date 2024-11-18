using UnityEngine;

namespace Player {
    [SelectionBase]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour {
        // Fields and Properties
        #region Components
        private PlayerAnimator _playerAnimator;
        private CharacterController _characterController;
        private Transform _cameraTransform;
        #endregion

        #region Movement
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 10f;
        [SerializeField] private float rotationSpeed = 5f;
        private Vector3 _movementDir;
        private Vector3 _rotationDir;
        [SerializeField]
        private bool _canMove;
        #endregion

        #region Gravity
        [Header("Gravity")]
        [SerializeField] private float mass = 4f;
        [SerializeField] private float fallSpeed = 6f;
        private float _verticalVelocity;
        private const float GRAVITY = -9.81f;
        #endregion

        // Methods
        #region Unity Methods
        private void Awake() {
            _playerAnimator = GetComponentInChildren<PlayerAnimator>();
            _characterController = GetComponent<CharacterController>();
            _cameraTransform = Camera.main.gameObject.transform;
        }

        private void Start() {
            _canMove = true;
        }

        private void Update() {
            HandleMovement();
            HandleRotation();
            HandleAnimation();
        }

        private void OnDrawGizmos() {
            /*Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, _rotationDir);*/
        }
        #endregion

        #region Private Methods
        private void HandleMovement() {
            if (_canMove) {
                // Calcular el movimiento en relacion a la camara (solo si se puede mover)
                _movementDir = _cameraTransform.forward * GameInput.Instance.MovementInput.y;
                _movementDir += _cameraTransform.right * GameInput.Instance.MovementInput.x;
            }

            // Calcular la gravedad
            _movementDir.y = CalculateVerticalForce();
            _movementDir.Normalize();

            // Ajustar la velocidad segun el estado del jugador
            float finalSpeed = IsFalling() ? fallSpeed * mass : walkSpeed;
            _characterController.Move(_movementDir * (finalSpeed * Time.deltaTime));
        }

        private void HandleRotation() {
            _rotationDir = _movementDir;
            _rotationDir.y = 0; // Ignorar la y, para que el personaje no mire hacia abajo
            if (_rotationDir == Vector3.zero) return;
            _rotationDir.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(_rotationDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            Debug.DrawRay(transform.position, _rotationDir, Color.red);
        }

        private void HandleAnimation() {
            _playerAnimator.SetAnimatorBool(PlayerAnimator.IsMoving, IsMoving());
            _playerAnimator.SetAnimatorBool(PlayerAnimator.IsFalling, IsFalling());
        }

        private float CalculateVerticalForce() {
            if (_characterController.isGrounded) {
                // Empujar al jugador hacia el suelo para asegurar que no este cayendo todo el tiempo
                _verticalVelocity = -1;
            } else {
                _verticalVelocity += GRAVITY * Time.deltaTime; // Aplicar la gravedad
            }
            return _verticalVelocity;
        }
        #endregion

        #region Public Methods
        public bool IsMoving() {
            return _movementDir.x != 0 || _movementDir.z != 0;
        }
        public bool IsFalling() {
            return !_characterController.isGrounded;
            
        }
        #endregion
    }
}