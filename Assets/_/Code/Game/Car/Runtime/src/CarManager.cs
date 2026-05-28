using System;
using UnityEngine;

namespace Car.Runtime
{
    public class CarManager : MonoBehaviour
    {
        #region Public

        public event Action OnPlayEnd;
        public event Action OnRewindEnd;
        public event Action OnSuccess;

        #endregion
        
        
        #region Main API
        public void InitializePool(int carQuantity, GameObject[] originCollection, GameObject[] destinationCollection)
        {
            _carsPool = new GameObject[carQuantity];

            for (int i = 0; i < carQuantity; i++)
            {
                GameObject carPrefab = _carPrefabArray[i % _carPrefabArray.Length];
                GameObject destination = destinationCollection[i % _carPrefabArray.Length];
                GameObject origin = originCollection[i % _carPrefabArray.Length];

                GameObject car = Instantiate(carPrefab, Vector3.zero, Quaternion.identity);
                car.SetActive(false);

                if (!car.TryGetComponent<PlayerMotion>(out PlayerMotion carControl)) return;
                if (!destination.TryGetComponent<Collider>(out Collider destinationCollider)) return;

                carControl.InitializeMission(destinationCollider, origin.transform);
                carControl.OnDestinationReached += HandleOnDestinationReached;
                _carsPool[i] = car;
            }
        }
        public void ActivateCarControl()
        {
            
            if (_currentCarControledIndex >= _carsPool.Length) return;
            
            GameObject currentControlledCar = _carsPool[_currentCarControledIndex];
            currentControlledCar.SetActive(true);
            
            if (!currentControlledCar.TryGetComponent<PlayerMotion>(out _currentPlayerController)) return;
            _currentPlayerController.StartPlayerControl();
            
            if (!currentControlledCar.TryGetComponent<GhostRecord>(out _currentGhostRecorder)) return;
            _currentGhostRecorder.StartRecording();
            
            if (!currentControlledCar.TryGetComponent<GhostMotion>(out _currentCarGhost)) return;
            _currentCarGhost.SetRewindSpeed(_rewindSpeed);
            ActivateGhosts();
        }

        public int GetCurrentIndex()
        {
            return _currentCarControledIndex;
        }

        private void HandleOnDestinationReached()
        {
            _carsPool[_currentCarControledIndex].SetActive(false);
            _currentPlayerController.StopPlayerControl();
            _currentGhostRecorder.StopRecording();
            _currentCarGhost.InitializeGhost();
            
            StopGhosts();
            
            _currentCarControledIndex++;

            CheckEndOfArray();

            if (_currentCarControledIndex >= _carsPool.Length) return;

            OnPlayEnd?.Invoke();
        }

        private void ActivateGhosts()
        {
            if (_currentCarControledIndex <= 0) return;
            for (int i = _currentCarControledIndex - 1; i >= 0; i--)
            {
                if (!_carsPool[i].TryGetComponent<GhostMotion>(out GhostMotion ghost)) return;
                ghost.gameObject.SetActive(true);
                ghost.StartMotion();
            }
        }

        private void StopGhosts()
        {
            if (_currentCarControledIndex <= 0) return;
            for (int i = 0 ; i < _carsPool.Length; i ++)
            {
                if (!_carsPool[i].TryGetComponent<GhostMotion>(out GhostMotion ghost)) return;
                ghost.gameObject.SetActive(false);
                ghost.StopMotion();
            }
        }

        public void Rewind()
        {
            _currentPlayerController.StopPlayerControl();
            _currentGhostRecorder.StopRecording();
            _currentCarGhost.OnRewindEnd += HandleOnRewindEnd;
            _currentCarGhost.InitializeGhost();
            _currentCarGhost.StartMotion();
            
            for (int i = 0; i < _carsPool.Length; i++)
            {
                if (!_carsPool[i].TryGetComponent<GhostMotion>(out GhostMotion ghost)) return;
                ghost.Rewind();
            }
        }

        private void HandleOnRewindEnd()
        {
            _currentGhostRecorder.ClearRecording();
            _currentCarGhost.OnRewindEnd -= HandleOnRewindEnd;
            StopGhosts();
            OnRewindEnd?.Invoke();
        }

        public void SetRewindSpeed(float speed)
        {
            _rewindSpeed = speed;
        }

        public GameObject GetCurrentCar()
        {
            return _carsPool[_currentCarControledIndex];
        }

        private void CheckEndOfArray()
        {
            if (_currentCarControledIndex < _carsPool.Length) return;
            
            OnSuccess?.Invoke();
        }

        public void SetMission(Collider destination, Transform origin)
        {
            _currentPlayerController.InitializeMission(destination, origin);
        }

        #endregion
        
        
        #region Private and Protected
       
        [SerializeField] private GameObject[] _carPrefabArray;

        private GameObject[] _carsPool;
        private float _rewindSpeed;

        // Current Car
        private int _currentCarControledIndex;
        private PlayerMotion _currentPlayerController;
        private GhostRecord _currentGhostRecorder;
        private GhostMotion _currentCarGhost;


        #endregion
    }
}
