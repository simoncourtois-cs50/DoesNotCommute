using System.Collections.Generic;
using UnityEngine;

namespace Car.Runtime
{
    public class GhostMotion : MonoBehaviour
    {
        #region Unity API

        private void Start()
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
            _rigidbody.isKinematic = true;
        }

        public void StartMotion()
        {
            _currentIndex = 0;
            _currentTime = 0;
            _recordInterval = _pathRecordsList[1].DeltaTime;
            _isInMotion = true;
            transform.position = _pathRecordsList[0].Position;
            transform.rotation = _pathRecordsList[0].Rotation;
            SetNewLerpBounds();
        }

        public void StopMotion()
        {
            _isInMotion = false;
        }

        private void GetPathRecordsList()
        {
            if (!_ghostRecord) return;
            _pathRecordsList = _ghostRecord.PathDataList;
        }
        
        private void UpdateMotion()
        {
            _currentTime += Time.deltaTime;
            float ratio = _currentTime / _recordInterval;
            
            transform.position = Vector3.Lerp(_positionA, _positionB, ratio);
            transform.rotation = Quaternion.Lerp(_rotationA, _rotationB, ratio);
            
            bool _isNextPointReached = _currentTime >= _recordInterval;
            if (!_isNextPointReached) return;
            
            bool _isLastPointReached = _currentIndex == _pathRecordsList.Count - 2;
            if(_isLastPointReached) StopMotion();
            
            SetNewLerpBounds();

            _currentTime -= _recordInterval;
            _recordInterval = _pathRecordsList[_currentIndex + 1].DeltaTime;
            _currentIndex++;
        }
        
        private void SetNewLerpBounds()
        {
            _positionA = _pathRecordsList[_currentIndex].Position;
            _positionB = _pathRecordsList[_currentIndex + 1].Position;
            _rotationA = _pathRecordsList[_currentIndex].Rotation;
            _rotationB = _pathRecordsList[_currentIndex + 1].Rotation;
        }
        
        #endregion
        
        
        #region Private and Protected

        private bool _isInMotion;
        
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

        #endregion
    }
}