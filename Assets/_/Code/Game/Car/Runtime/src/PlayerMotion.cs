using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Car.Runtime
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMotion : MonoBehaviour
    {
        #region Public

        public event Action OnDestinationReached;

        #endregion
        
        #region Unity API

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            //TODO : Personnalize with Scriptable Object for different type of vehicle
            _moveAction = InputSystem.actions.FindAction("Move");
            _currentSpeed = 0;
        }

        private void Update()
        {
            if (!_enablePlayerControl) return;
            _moveValue = _moveAction.ReadValue<Vector2>().x;
        }

        private void FixedUpdate()
        { 
            if (!_enablePlayerControl) return;
            MoveForward();
            TurnCar();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != _destination) return;
            
            OnDestinationReached?.Invoke();
        }

        #endregion


        #region Main API

        public void InitializeMission(Collider destination, Transform origin)
        {
            transform.position = origin.position;
            _destination = destination;
        }

        public void StartPlayerControl()
        {
            _enablePlayerControl = true;
        }

        public void StopPlayerControl()
        {
            _enablePlayerControl = false;
        }
        private void MoveForward()
        {
            if(_currentSpeed < _maxSpeed) _currentSpeed += _accelerationValue * Time.fixedDeltaTime;
            
            _rigidbody.AddForce(transform.forward * _currentSpeed);
        }
        
        private void TurnCar()
        {
            _rigidbody.AddTorque(Vector3.up * _moveValue * _rotationSpeed);
        }

        #endregion


        #region private and protected

        [Header("Parameters")]
        [Range(100,500),SerializeField]
        private float _maxSpeed;
        [Range(50,400f),SerializeField]
        private float _accelerationValue;
        [Range(100,500), SerializeField]
        private float _rotationSpeed;

        private float _currentSpeed;
        private Rigidbody _rigidbody;
        private InputAction _moveAction;
        private float _moveValue;
        private bool _enablePlayerControl;
        
        private Collider _destination;

        #endregion
    }
}