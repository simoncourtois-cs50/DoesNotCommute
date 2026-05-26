using System;
using System.Collections.Generic;
using UnityEngine;

namespace Car.Runtime
{
    [RequireComponent(typeof(Rigidbody),typeof(GhostRecord))]
    public class GhostMotion : MonoBehaviour
    {
        #region Public

        public event Action OnRewindEnd;

        #endregion
        
        
        #region Unity API

        private void Awake()
        {
            if (!TryGetComponent<GhostRecord>(out _ghostRecord)) return;
            _currentIndex = 0;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!_isInMotion) return;
            UpdateMotion();
        }

        #endregion
        
        
        #region Main API
        
        public void InitializeGhost()
        {
            GetPathRecordsList();
        }

        public void StartMotion()
        {
            if (_pathRecordsList == null) return;
            _currentIndex = 0;
            _currentTime = 0;
            _recordInterval = _pathRecordsList[1].DeltaTime;
            _isInMotion = true;
            _isRewinding = false;
            _rigidbody.isKinematic = true;
            transform.position = _pathRecordsList[0].Position;
            transform.rotation = _pathRecordsList[0].Rotation;
            SetNewIntervalParameters();
        }

        public void StopMotion()
        {
            _isInMotion = false;
            _rigidbody.isKinematic = false;
        }

        private void GetPathRecordsList()
        {
            if (!_ghostRecord) return;
            
            _pathRecordsList = _ghostRecord.PathDataList;
        }
        
        private void UpdateMotion()
        {
            _currentTime += _isRewinding
                ? - Time.deltaTime * _speedRewind
                : Time.deltaTime;
            
            float ratio = _currentTime / _recordInterval;
            
            transform.position = Vector3.Lerp(_positionA, _positionB, ratio);
            transform.rotation = Quaternion.Lerp(_rotationA, _rotationB, ratio);
            
            bool _isNextPointReached = _isRewinding
                ? _currentTime <= 0
                : _currentTime >= _recordInterval;
            
            if (!_isNextPointReached) return;
            
            bool _isLastPointReached = _isRewinding
                ? _currentIndex == 1
                : _currentIndex == _pathRecordsList.Count - 2;
            if (_isLastPointReached)
            {
                StopMotion();
                OnRewindEnd?.Invoke();
                return;
            }
            
            SetNewIntervalParameters();
            
            _currentTime += _isRewinding
                ? _recordInterval
                : -_recordInterval;
            
            int recordIntervalIndex = _currentIndex + 1;
            
            _recordInterval = _pathRecordsList[recordIntervalIndex].DeltaTime;
            
            _currentIndex += _isRewinding
                ? -1
                : 1;
        }
        
        private void SetNewIntervalParameters()
        {
                _positionA = _pathRecordsList[_currentIndex].Position;
                _positionB = _pathRecordsList[_currentIndex + 1].Position;
                _rotationA = _pathRecordsList[_currentIndex].Rotation;
                _rotationB = _pathRecordsList[_currentIndex + 1].Rotation;
        }

        public void Rewind()
        {
            //Rewind only Cars with a recorded List
            if (_pathRecordsList == null || _pathRecordsList.Count < 2) return;
            _isRewinding = true;
            _isInMotion = true;
            _currentIndex--;
            
            if (_currentIndex >= 0) return;
            
            // Initialize Ghost for the currentControlled Car
            _currentIndex = _pathRecordsList.Count - 2;
            _recordInterval = _pathRecordsList[_pathRecordsList.Count - 1].DeltaTime;
            SetNewIntervalParameters();
        }

        public void SetRewindSpeed(float speed)
        {
            _speedRewind = speed;
        }

        #endregion
        
        
        #region Private and Protected

        [SerializeField] bool _isInMotion;
        [SerializeField] private bool _isRewinding;
        private float _currentTime;
        private float _recordInterval;
        private int _currentIndex;
        
        private Vector3 _positionA;
        private Vector3 _positionB;
        private Quaternion _rotationA;
        private Quaternion _rotationB;
        
        private IReadOnlyList<GhostRecord.PathData> _pathRecordsList;
        private GhostRecord _ghostRecord;
        private Rigidbody _rigidbody;
        private float _speedRewind;

        #endregion
    }
}