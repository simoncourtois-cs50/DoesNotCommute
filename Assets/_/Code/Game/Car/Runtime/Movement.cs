using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Car.Runtime
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.linearDamping = 1.5f;
            _rigidbody.angularDamping = 2.5f;
            _rigidbody.mass = 10f;
            _moveAction = InputSystem.actions.FindAction("Move");
            _currentSpeed = 0;
        }
        private void LateUpdate()
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
                _currentSpeed += _accelerationValue * Time.deltaTime;
            }
            _rigidbody.AddForce(transform.forward * _currentSpeed);
        }
        
        private void TurnCar()
        {
            float _movevalue = _moveAction.ReadValue<Vector2>().x;
            _rigidbody.AddTorque(Vector3.up * _movevalue * _rotationSpeed);
        }

        #endregion


        #region private and protected

        [Header("Parameters")]
        [Range(80f,100f),SerializeField]
        private float _maxSpeed;
        [Range(20f,40f),SerializeField]
        private float _accelerationValue;
        [Range(80f,100f), SerializeField]
        private float _rotationSpeed;

        private float _currentSpeed;
        private Rigidbody _rigidbody;
        private InputAction _moveAction;

        #endregion
    }
}