using System.Collections.Generic;
using UnityEngine;

namespace Car.Runtime
{
    public class GhostRecord : MonoBehaviour
    {
        #region Public

        public struct PathData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public float DeltaTime;
            public PathData(Vector3 currentPosition, Quaternion currentRotation, float timeElapsedSinceLastRecord)
            {
                Position = currentPosition;
                Rotation = currentRotation;
                DeltaTime = timeElapsedSinceLastRecord;
            }
        }

        public IReadOnlyList <PathData> PathDataList => _pathDataList;

        #endregion


        #region Unity API

        private void Awake()
        {
            _currentTime = 0;
            _recordInterval = 0.05f;
        }

        private void Update()
        {
            if (!_isRecording) return;
            RecordFullPath();
        }

        #endregion


        #region Main API

        public void StartRecording()
        {
            _currentTime = 0;
            _isRecording = true;
            PathData startFrame = GetPathData();
            _pathDataList.Add(startFrame);
        }

        public void StopRecording()
        {
            _isRecording = false;
        }
        
        private void RecordFullPath()
        {
            _currentTime += Time.deltaTime;

            if (_currentTime < _recordInterval) return;
            
            _pathDataList.Add(GetPathData());
            _currentTime = 0;
        }
        public void ClearRecording()
        {
            _pathDataList.Clear(); 
        }
        
        private PathData GetPathData()
        {
            return new PathData(transform.position, transform.rotation,_currentTime);
        }

        #endregion
        

        #region Private and Protected
        
        private List<PathData> _pathDataList = new();
        private float _recordInterval;
        private float _currentTime;
        private bool _isRecording;

        #endregion
    }
}