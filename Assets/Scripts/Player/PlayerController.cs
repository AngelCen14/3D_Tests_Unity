using UnityEngine;

namespace Player {
    [SelectionBase]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour {
        // Fields and Properties
        #region Components
        private PlayerAnimator _playerAnimator;
        private CharacterController _characterController;
        private Transform _cameraTransform;
        #endregion

        #region Movement
        [Header("Movement")]

        [SerializeField]
        private bool canMove;
        [SerializeField]
        private float walkSpeed = 5f;

        [SerializeField]
        private float rotationSpeed = 5f;

        private float _currentSpeed;
        private Vector3 _movement;
        private Vector3 _rotation;
        #endregion

        #region Gravity
        [Header("Gravity")]

        [SerializeField]
        [Range(1f, 30f)]
        private float mass = 10f;

        private float _verticalVelocity;
        private const float GRAVITY = -9.81f;
        #endregion

        #region Jump
        [Header("Jump")]

        [SerializeField]
        [Range(0f, 100f)]
        private float jumpForce = 40f;
        private bool _jumpTriggered;
        #endregion

        // Methods
        #region Unity Methods
        private void Awake() {
            _playerAnimator = GetComponentInChildren<PlayerAnimator>();
            _characterController = GetComponent<CharacterController>();
            _cameraTransform = Camera.main.gameObject.transform;
        }

        private void Start() {
            canMove = true;
            _jumpTriggered = false;
            GameInput.Instance.JumpTriggered += OnJumpTriggered;
        }

        private void Update() {
            Move();
            Rotate();
            Animate();
        }

        private void OnDisable() {
            GameInput.Instance.JumpTriggered -= OnJumpTriggered;
        }
        #endregion

        #region Private Methods
        private void Move() {
            // Obtener el input de movimiento
            Vector2 movementInput = GameInput.Instance.GetMovementInput();
            if (canMove) {
                // Calcular el movimiento en relacion a la camara (solo si se puede mover)
                _movement = _cameraTransform.forward * movementInput.y;
                _movement += _cameraTransform.right * movementInput.x;
                _movement.Normalize();
            } else _movement = Vector2.zero;

            _currentSpeed = _movement.magnitude * walkSpeed;
            _movement *= _currentSpeed;

            ApplyGravity(); // Aplicar la gravedad y los saltos

            _characterController.Move(_movement * Time.deltaTime);
        }

        private void Rotate() {
            _rotation = _movement;
            _rotation.y = 0; // Ignorar la y, para que el personaje no mire hacia abajo
            if (_rotation == Vector3.zero) return;
            _rotation.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(_rotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, _rotation, Color.red);
        }

        private void Animate() {
            _playerAnimator.SetAnimatorBool(PlayerAnimator.IsMoving, IsMoving());
            _playerAnimator.SetAnimatorBool(PlayerAnimator.IsFalling, IsFalling());
        }

        private void ApplyGravity() {
            if (_characterController.isGrounded) {
                // Empujar al jugador hacia el suelo para asegurar que no este cayendo todo el tiempo
                _verticalVelocity = -1;
            } else {
                _verticalVelocity += (GRAVITY * mass) * Time.deltaTime; // Aplicar la gravedad
            }
            Jump();
            _movement.y = _verticalVelocity;
        }

        private void Jump() {
            if (_characterController.isGrounded && _jumpTriggered) {
                _verticalVelocity = jumpForce;
                _jumpTriggered = false;
            }
        }
        #endregion

        #region Public Methods
        public bool IsMoving() {
            return _movement.x != 0 || _movement.z != 0;
        }
        public bool IsFalling() {
            return !_characterController.isGrounded;
        }
        #endregion

        #region Event Listeners
        private void OnJumpTriggered() {
            _jumpTriggered = true;
        }
        #endregion
    }
}