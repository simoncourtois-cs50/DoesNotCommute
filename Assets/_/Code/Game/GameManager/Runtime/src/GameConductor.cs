using Camera.Runtime;
using Car.Runtime;
using Timer.Runtime;
using UI.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameManager.Runtime
{
    public class GameConductor : MonoBehaviour
    {
        #region Unity API

        private void Awake() => _continueInput = InputSystem.actions.FindAction("Jump");

        private void Start()
        {
            _carManager.InitializePool(_carQuantity, _originObjectList, _destinationObjectList);
            _carManager.SetRewindSpeed(_rewindSpeed);
            _carManager.OnPlayEnd += OnPlayEndHandler;
            _carManager.OnRewindEnd += OnRewindHandler;
            _carManager.OnSuccess += OnSuccessHandler;
            TimeManager.Instance.OnEnd += OnTimerEndHandler;
            ChangePhase(Phase.Rest);
        }

        private void Update()
        {
            OnStayPhase();
        }

        #endregion


        #region StateMachine

        private void OnEnterPhase()
        {
            switch (_currentPhase)
            {
                case Phase.MenuStart:
                    break;
                case Phase.Rest:
                   
                    ActivateMission();
                    ResetCameraToRest();

                    TimeManager.Instance.PauseTimer();

                    break;

                case Phase.Play:

                    ActivateRewindButton();

                    _carManager.ActivateCarControl();

                    ResetCameraToPlayer(_playCameraDistance);
                    ActivateDirectionalArrow();
                    TimeManager.Instance.StartChrono();
                    TimeManager.Instance.PlayTimer();

                    break;

                case Phase.Rewind:

                    ResetCameraToPlayer(_rewindResetCameraDistance);

                    _carManager.Rewind();

                    TimeManager.Instance.RewindTimer(_rewindSpeed);

                    break;

                case Phase.End:
                    _canvaManager.ActivateEndMenu(_endtext);
                    break;
            }
        }


        private void OnStayPhase()
        {
            switch (_currentPhase)
            {
                case Phase.MenuStart:
                    break;
                case Phase.Rest:
                    CheckContinue();
                    break;
                case Phase.Play:
                    break;
                case Phase.Rewind:
                    break;
                case Phase.End:
                    break;
            }
        }

        private void OnExitPhase()
        {
            switch (_currentPhase)
            {
                case Phase.MenuStart:
                    break;
                case Phase.Rest:
                    break;
                case Phase.Play:
                    DeactivateRewindButton();
                    DeActivateMission();
                    DeActivateDirectionalArrow();
                    break;
                case Phase.Rewind:
                    
                    break;
                case Phase.End:
                    break;
            }
        }

        private void ChangePhase(Phase newPhase)
        {
            OnExitPhase();
            _currentPhase = newPhase;
            OnEnterPhase();
        }

        #endregion


        #region Main API

        private void CheckContinue()
        {
            if (!_continueInput.WasReleasedThisFrame()) return;
            ChangePhase(Phase.Play);
        }
        public void OnStartClick()
        {
            SceneManager.LoadScene(0);
        }

        private void OnPlayEndHandler()
        {
            ChangePhase(Phase.Rest);
        }
        private void OnRewindHandler()
        {
            ChangePhase(Phase.Rest);
        }

        private void OnTimerEndHandler()
        {
            _endtext = "Game Over";
            ChangePhase(Phase.End);
        }
        private void OnSuccessHandler()
        {
            _endtext = "Victory !";
            ChangePhase(Phase.End);
        }

        public void Rewind()
        {
            ChangePhase(Phase.Rewind);
        }

        private void ActivateRewindButton()
        {
            _RewindButton.gameObject.SetActive(true);
        }

        private void DeactivateRewindButton()
        {
            _RewindButton.gameObject.SetActive(false);
        }
        private void ActivateMission()
        {
            _currentIndex = _carManager.GetCurrentIndex();
            _originObjectList[CurrentIndex].SetActive(true);
            _destinationObjectList[CurrentIndex].SetActive(true);
        }
        private void DeActivateMission()
        {
            _originObjectList[CurrentIndex].SetActive(false);
            _destinationObjectList[CurrentIndex].SetActive(false);
        }
        private void ActivateDirectionalArrow()
        {
            _directionalArrow.gameObject.SetActive(true);
            _directionalArrow.SetParameters(_destinationObjectList[CurrentIndex].transform, _currentCar);
        }
        private void DeActivateDirectionalArrow()
        {
            _directionalArrow.gameObject.SetActive(false);
        }

        private void ResetCameraToPlayer(float distance)
        {
            _currentCar = _carManager.GetCurrentCar().transform;
            _cameraFollow.SetTarget(_currentCar, distance, _playResetCameraTime);
        }

        private void ResetCameraToRest()
        {
            _cameraFollow.SetTarget(_restCameraTransform, _restCameraDistance, _restResetCameraTime);
        }

       
        #endregion


        #region Private and Protected

        [Header("References")]
        [SerializeField] private CarManager _carManager;
        [SerializeField] private CanvaManager _canvaManager;

        [SerializeField] private Button _RewindButton;
        [SerializeField] Transform _restCameraTransform;
        [SerializeField] private CameraFollow _cameraFollow;
        [SerializeField] private GameObject[] _originObjectList;
        [SerializeField] private GameObject[] _destinationObjectList;
        [SerializeField] private DirectionalArrow _directionalArrow;

        [Header("Rewind Speed")]
        [SerializeField] private float _rewindSpeed;

        [Header("Camera")]
        [SerializeField] private float _restResetCameraTime;
        [SerializeField] private float _playResetCameraTime;
        [SerializeField] float _playCameraDistance;
        [SerializeField] float _restCameraDistance;
        [SerializeField] private float _rewindResetCameraDistance;

        [Header("Car Pool")]
        [SerializeField] private int _carQuantity;

        private InputAction _continueInput;
        private Transform _currentCar;
        private int _currentIndex;
        private int CurrentIndex
        {
            get { return _currentIndex % (_carQuantity - 1); }
            set { _currentIndex = value; }
        }
        private string _endtext;
        private enum Phase
        {
            MenuStart,
            Rest,
            Play,
            Rewind,
            End
        }

        private Phase _currentPhase;

        #endregion
    }
}