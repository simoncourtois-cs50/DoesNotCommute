using System.Collections.Generic;
using UnityEngine;

namespace Car.Runtime
{
    public class RecordGhost : MonoBehaviour
    {
        #region Public

        public struct Data
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public float deltaTime;
            
            public Data(Vector3 currentPosition, Quaternion currentRotation, float timeElapsedSinceLastRecord)
            {
                Position = currentPosition;
                Rotation = currentRotation;
                deltaTime = timeElapsedSinceLastRecord;
            }
        }

        public List<Data> DataList = new();

        #endregion


        #region Unity API

        private void Awake()
        {
            _currentTime = 0;
            _recordInterval = 0.05f;
            Data startFrame = GetFrameRecord();
            DataList.Add(startFrame);
        }

        private void Update()
        {
            RegisterGhostFrames();
        }

        #endregion


        #region Main API

        private Data GetFrameRecord()
        {
            return new Data(transform.position, transform.rotation,_currentTime);
        }

        private void RegisterGhostFrames()
        {
            _currentTime += Time.deltaTime;

            if(_currentTime >= _recordInterval)
            {
                DataList.Add(GetFrameRecord());
                _currentTime = 0;
            }
        }

        #endregion


        #region Private and Protected

        private float _recordInterval;
        private float _currentTime;

        #endregion
    }
}