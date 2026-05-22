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
            
            public Data(Vector3 currentPosition, Quaternion currentRotation)
            {
                Position = currentPosition;
                Rotation = currentRotation;
            }
        }

        public List<Data> DataList = new();

        #endregion


        #region Main API

        private void SaveFrame()
        {
            Data currentFrame = new Data(transform.position, transform.rotation);
        }

        #endregion


        #region Private and Protected

        private int _currentIndex;

        #endregion
    }
}