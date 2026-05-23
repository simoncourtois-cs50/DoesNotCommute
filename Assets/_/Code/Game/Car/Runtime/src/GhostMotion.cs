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
            _currentIndex = 0;
            _currentTime = 0;
            _recordInterval = _pathRecordsList[1].DeltaTime;
            _isInMotion = true;
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
            //TODO : Optimize the IFs
            if (_currentIndex > _pathRecordsList.Count) return;
            
            _currentTime += Time.deltaTime;
            if (_currentTime >= _recordInterval)
            {
            transform.position = _pathRecordsList[_currentIndex].Position;
            transform.rotation = _pathRecordsList[_currentIndex].Rotation;

            _currentTime -= _recordInterval;
            if (_currentIndex == _pathRecordsList.Count - 1) return;
            _recordInterval = _pathRecordsList[_currentIndex + 1].DeltaTime;
            _currentIndex++;
            }
        }

        #endregion
        
        
        #region Private and Protected

        private float _currentTime;
        private float _recordInterval;
        private int _currentIndex;
        private bool _isInMotion;
        
        private IReadOnlyList<GhostRecord.PathData> _pathRecordsList;
        private GhostRecord _ghostRecord;
        //TODO : Car manager should tell when it's initialize,
        //TODO : when it will be, no need to ref player motion anymore;
        #endregion
    }
}
