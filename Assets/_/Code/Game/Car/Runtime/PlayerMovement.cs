using UnityEngine;
using UnityEngine.InputSystem;

namespace Car.Runtime
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.linearDamping = 1.5f;
            _rigidbody.angularDamping = 3.5f;
            _rigidbody.mass = 10f;
            _moveAction = InputSystem.actions.FindAction("Move");
            _currentSpeed = 0;
        }

        private void Update()
        {
            _moveValue = _moveAction.ReadValue<Vector2>().x;
        }

        private void FixedUpdate()
        {
            MoveForward();
            TurnCar();
        }

        #endregion


        #region Main API

        private void MoveForward()
        {
            if(_currentSpeed < _maxSpeed)
            {
                _currentSpeed += _accelerationValue * Time.fixedDeltaTime;
            }
            _rigidbody.AddForce(transform.forward * _currentSpeed);
        }
        
        private void TurnCar()
        {
            _rigidbody.AddTorque(Vector3.up * _moveValue * _rotationSpeed);
        }

        #endregion


        #region private and protected

        [Header("Parameters")]
        [Range(400,500),SerializeField]
        private float _maxSpeed;
        [Range(200,400f),SerializeField]
        private float _accelerationValue;
        [Range(400,500), SerializeField]
        private float _rotationSpeed;

        private float _currentSpeed;
        private Rigidbody _rigidbody;
        private InputAction _moveAction;
        private float _moveValue;

        #endregion
    }
}