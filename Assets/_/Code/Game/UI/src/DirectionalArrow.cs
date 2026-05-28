using UnityEngine;

namespace UI.Runtime
{
    public class DirectionalArrow : MonoBehaviour
    {
        #region UnityAPI

        private void Update()
        {
            SetPosition();
            SetRotation();
        }

        #endregion


        #region Main API

        private void SetPosition()
        {
            if (!_carAnchor) return;
            GetDirection();
            transform.position = _carAnchor.position + (_direction * _offsetDistance);
        }

        private void SetRotation()
        {
            if (!_objective) return;

            transform.forward = _direction;
        }

        private void GetDirection()
        {
            _direction = (_objective.position - _carAnchor.position).normalized;
        }
        
        public void SetParameters(Transform objective, Transform car)
        {
            _objective = objective;
            _carAnchor = car;
        }

        #endregion


        #region Private and Protecter

        [SerializeField] private float _offsetDistance;
        private Transform _objective;
        private Transform _carAnchor;
        private Vector3 _direction;

        #endregion
    }
}