using UnityEngine;

namespace Camera.Runtime
{
    public class CameraFollow : MonoBehaviour
    {
        #region Unity API

        private void Update()
        {
            if (!_isTargeting) return;
            MoveToTarget();
        }

        private void LateUpdate()
        {
            if (!_isTargetSet) return;
            FollowTarget();
        }

        #endregion


        #region Main API

        public void SetTarget(Transform target, float distanceFromtarget, float transitionTime)
        {
            _isTargetSet = false;
            _originPosition = transform.position;
            _target = target;
            _offset = Vector3.up * distanceFromtarget;
            _transitionTime = transitionTime;
            _isTargeting = true;
            _currentTime = 0;
            
        }

        private void MoveToTarget()
        {
            _currentTime += Time.deltaTime;
            float ratio = _currentTime / _transitionTime;
            Vector3 newPosition = Vector3.Lerp(_originPosition, _target.position + _offset, ratio);
            transform.position = newPosition;

            if (ratio < 1) return;

            _isTargetSet = true;
            _isTargeting = false;
            _currentTime = 0;
        }

        private void FollowTarget()
        {
            transform.position = _target.position + _offset;
        }

        #endregion


        #region Private and Protected
        
        private Transform _target;
        private Vector3 _originPosition;
        private Vector3 _currentPosition;
        private float _transitionTime;
        private float _currentTime;

        private Vector3 _offset;

        private bool _isTargetSet;
        private bool _isTargeting;

        #endregion
    }
}