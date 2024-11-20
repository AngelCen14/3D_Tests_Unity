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
        }

        private void Update() {
            Move();
            Rotate();
            Animate();
        }

        private void OnDrawGizmos() {
            /*Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, _rotationDir);*/
        }
        #endregion

        #region Private Methods
        private void Move() {
            if (canMove) {
                // Calcular el movimiento en relacion a la camara (solo si se puede mover)
                _movement = _cameraTransform.forward * GameInput.Instance.MovementInput.y;
                _movement += _cameraTransform.right * GameInput.Instance.MovementInput.x;
                _movement.Normalize();
            } else _movement = Vector2.zero;

            _currentSpeed = _movement.magnitude * walkSpeed;
            _movement *= _currentSpeed;
            SetGravity();

            _characterController.Move(_movement * Time.deltaTime);
        }

        private void Rotate() {
            _rotation = _movement;
            _rotation.y = 0; // Ignorar la y, para que el personaje no mire hacia abajo
            if (_rotation == Vector3.zero) return;
            _rotation.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(_rotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            Debug.DrawRay(transform.position, _rotation, Color.red);
        }

        private void Animate() {
            _playerAnimator.SetAnimatorBool(PlayerAnimator.IsMoving, IsMoving());
            _playerAnimator.SetAnimatorBool(PlayerAnimator.IsFalling, IsFalling());
        }

        private void SetGravity() {
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
            if (_characterController.isGrounded && GameInput.Instance.Jump) {
                _verticalVelocity = jumpForce;
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
    }
}