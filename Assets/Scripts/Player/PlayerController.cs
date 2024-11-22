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

        [SerializeField]
        PlayerState _state;

        #region Movement
        [Header("Movement")]
        [SerializeField]
        private bool canMove;

        [SerializeField]
        private float walkSpeed = 5f;

        [SerializeField]
        private float runSpeed = 10f;

        [SerializeField]
        private float rotationSpeed = 5f;

        [SerializeField]
        private float speedChangeRate = 7f;

        private float _speed;
        private float _animationBlendSpeed;
        private Vector3 _movement;
        private Vector3 _rotation;
        #endregion

        #region Gravity
        [Header("Gravity")]
        [SerializeField]
        [Range(1f, 30f)]
        private float mass = 10f;

        [SerializeField]
        private float fallTimeout = 0.15f;

        private bool _isFalling;
        private float _fallTimer;
        private float _verticalVelocity;
        private const float GRAVITY = -9.81f;
        #endregion

        #region Jump
        [Header("Jump")]
        [SerializeField]
        [Range(0f, 100f)]
        private float jumpForce = 30f;

        [SerializeField]
        private float jumpTimeout = 0.5f;

        private float _jumpTimer;
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
            _fallTimer = fallTimeout;
            _jumpTimer = jumpTimeout;
            _state = PlayerState.Idle;
            GameInput.Instance.JumpTriggered += OnJumpTriggered;
        }

        private void Update() {
            Move();
            Rotate();
            UpdateState();
            Animate();
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, _rotation);
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
                // Calcular el movimiento en relacion a la camara
                Vector3 forward = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
                Vector3 right = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up).normalized;

                _movement = (forward * movementInput.y + right * movementInput.x);
                _movement.Normalize();
            } else {
                _movement = Vector3.zero;
            }

            ApplySpeed(movementInput); // Calcular la velocidad y aplicarsela al movimiento
            ApplyGravity(); // Aplicar la gravedad y los saltos

            _characterController.Move(_movement * Time.deltaTime);
        }

        private void ApplySpeed(Vector2 movementInput) {
            // Obtener la velocidad objetivo en funcion del input
            float targetSpeed = GameInput.Instance.Sprint ? runSpeed : walkSpeed;
            if (movementInput == Vector2.zero) targetSpeed = 0f;

            // Obtener la velocidad actual
            float currentSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;
            float speedOffset = 0.1f; // Offset para asegurar que se llega a la velocidad objetivo

            // Aumentar la velocidad progresivamente hasta llegar a la velocidad objetivo
            if (currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset) {
                _speed = Mathf.Lerp(currentSpeed, targetSpeed * movementInput.magnitude,
                    Time.deltaTime * speedChangeRate);
                // Redondear a 3 decimales
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            } else {
                _speed = targetSpeed;
            }

            // Calcular la velocidad de blend entre animaciones
            _animationBlendSpeed = Mathf.Lerp(_animationBlendSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlendSpeed < 0.01f) _animationBlendSpeed = 0f;

            _movement *= _speed;
        }

        private void Rotate() {
            _rotation = _movement;
            _rotation.y = 0; // Ignorar la y, para que el personaje no mire hacia abajo
            if (_rotation == Vector3.zero) return;
            _rotation.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(_rotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void Animate() {
            _playerAnimator.SetAnimatorFloat(PlayerAnimator.Speed, _animationBlendSpeed);
            _playerAnimator.SetAnimatorBool(PlayerAnimator.IsFalling, _isFalling);
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
            HandleFall();
        }

        private void Jump() {
            if (_characterController.isGrounded && _jumpTriggered && _jumpTimer <= 0.0f) {
                _verticalVelocity = jumpForce;
                _jumpTimer = jumpTimeout;
                _playerAnimator.SetAnimatorTrigger(PlayerAnimator.JumpTrigger);
            }

            if (_jumpTimer >= 0.0f && _characterController.isGrounded) {
                _jumpTimer -= Time.deltaTime;
            }

            _jumpTriggered = false;
        }

        private void HandleFall() {
            if (_characterController.isGrounded) {
                _fallTimer = fallTimeout;
                _isFalling = false;
            } else {
                // Timeout para que el jugador no caiga cuando baja escaleras o rampas
                if (_fallTimer >= 0.0f) {
                    _fallTimer -= Time.deltaTime;
                    _isFalling = false;
                } else {
                    _isFalling = true;
                }
            }
        }

        private void UpdateState() {
            if (_speed == 0) _state = PlayerState.Idle;
            if (_speed <= walkSpeed && _speed > 0) _state = PlayerState.Walking;
            if (_speed <= runSpeed && _speed > walkSpeed) _state = PlayerState.Running;

            if (_isFalling) _state = PlayerState.Falling;
        }
        #endregion

        #region Public Methods
        public bool IsMoving() {
            return _movement.x != 0 || _movement.z != 0;
        }
        #endregion

        #region Event Listeners
        private void OnJumpTriggered() {
            _jumpTriggered = true;
        }
        #endregion
    }
}