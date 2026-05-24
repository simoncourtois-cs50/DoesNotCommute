using System;
using UnityEngine;

namespace Car.Runtime
{
    public class CarManager : MonoBehaviour
    {
        #region Unity API

        private void Awake()
        {
            _carsPool = new GameObject[_carQuantity];
            
            for (int i = 0; i < _carQuantity; i++)
            {
                GameObject car = Instantiate(_carPrefab, Vector3.zero, Quaternion.identity);
                car.SetActive(false);
                if (!car.TryGetComponent<PlayerMotion>(out PlayerMotion carControl)) return;
                carControl.InitializeMission(_destinationCollection[i],_originCollection[i]);
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
            if (_currentCarControledIndex >= _carsPool.Length) return;
            
            GameObject currentControlledCar = _carsPool[_currentCarControledIndex];
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
            _currentPlayerController.StopPlayerControl();
            _currentGhostRecorder.StopRecording();
            _currentCarGhost.InitializeGhost();
            StopGhosts();
            
            _currentCarControledIndex++;
            ActivateCarControl();
        }

        private void ActivateGhosts()
        {
            if (_currentCarControledIndex <= 0) return;
            for (int i = _currentCarControledIndex-1; i >= 0; i--)
            {
                if (!_carsPool[i].TryGetComponent<GhostMotion>(out GhostMotion ghost)) return;
                ghost.StartMotion();
            }
        }

        private void StopGhosts()
        {
            if (_currentCarControledIndex <= 0) return;
            for (int i = _currentCarControledIndex-1; i > 0; i--)
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
        private int _carQuantity = 5;
        private int _currentCarControledIndex;
        private PlayerMotion _currentPlayerController;
        private GhostRecord _currentGhostRecorder;
        private GhostMotion _currentCarGhost;
        //TODO : Protect the minimum possible = _carQuantity;
        [SerializeField] private Transform[] _originCollection;
        [SerializeField] private Collider[] _destinationCollection;
        //TODO : Optimize bool maybe with a gamemanager
        private bool _startGame;

        #endregion
    }
}
