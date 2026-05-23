using System;
using UnityEngine;

namespace Car.Runtime
{
    public class CarManager : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            _carsPool = new GameObject[2];
            for (int i = 0; i < _carQuantity; i++)
            {
                GameObject car = Instantiate(_carPrefab, Vector3.zero, Quaternion.identity);
                car.SetActive(false);
                if (!car.TryGetComponent<PlayerMotion>(out PlayerMotion carControl)) return;
                carControl.InitializeMission(_destination,_origin);
                carControl.OnDestinationReached += HandleOnDestinationReached;
                _carsPool[i] = car;
            }

            _startGame = true;
        }

        private void Update()
        {
            if (!_startGame) return;
            ActivateCarControl();
            _startGame = false;
        }

        #endregion
        
        
        #region Main API

        private void ActivateCarControl()
        {
            if (_currentIndex >= _carsPool.Length) return;
            
            GameObject currentControlledCar = _carsPool[_currentIndex];
            currentControlledCar.SetActive(true);
            if (!currentControlledCar.TryGetComponent<PlayerMotion>(out _currentPlayerController)) return;
            _currentPlayerController.StartPlayerControl();
            
            if (!currentControlledCar.TryGetComponent<GhostRecord>(out _currentGhostRecorder)) return;
            _currentGhostRecorder.StartRecording();
            
            if (!currentControlledCar.TryGetComponent<GhostMotion>(out _currentCarGhost)) return;
            
            ActivateGhosts();
        }

        private void HandleOnDestinationReached()
        {
            _currentIndex++;
            _currentPlayerController.StopPlayerControl();
            _currentGhostRecorder.StopRecording();
            _currentCarGhost.InitializeGhost();
            
            StopGhosts();
            ActivateCarControl();
        }

        private void ActivateGhosts()
        {
            if (_currentIndex <= 0) return;
            for (int i = _currentIndex-1; i >= 0; i--)
            {
                if (!_carsPool[i].TryGetComponent<GhostMotion>(out GhostMotion ghost)) return;
                ghost.StartMotion();
            }
        }

        private void StopGhosts()
        {
            if (_currentIndex <= 0) return;
            for (int i = _currentIndex-1; i > 0; i--)
            {
                if (!_carsPool[i].TryGetComponent<GhostMotion>(out GhostMotion ghost)) return;
                ghost.StopMotion();
            }
        }
        #endregion
        
        
        #region Private and Protected
        
        //TODO : Use multiple prefabs and Bigger Pool;
        [SerializeField] private GameObject _carPrefab;
        
        private GameObject[] _carsPool;
        private int _carQuantity = 2;
        private int _currentIndex;
        private PlayerMotion _currentPlayerController;
        private GhostRecord _currentGhostRecorder;
        private GhostMotion _currentCarGhost;
        [SerializeField] private Transform _origin;
        [SerializeField] private Collider _destination;
        //TODO : Optimize bool maybe with a gamemanager
        private bool _startGame;

        #endregion
    }
}
