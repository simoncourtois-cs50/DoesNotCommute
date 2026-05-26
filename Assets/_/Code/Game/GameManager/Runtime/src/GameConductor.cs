using Camera.Runtime;
using Car.Runtime;
using Timer.Runtime;
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
            _carManager.SetRewindSpeed(_rewindSpeed);
            _carManager.OnPlayEnd += OnPlayEndHandler;
            _carManager.OnRewindEnd += OnRewindHandler;
            TimeManager.Instance.OnEnd += OnTimerEndHandler;
            _currentPhase = Phase.Rest;
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

                    ResetCameraToRest();

                    TimeManager.Instance.PauseTimer();

                    break;

                case Phase.Play:

                    ActivateRewindButton();

                    _carManager.ActivateCarControl();

                    ResetCameraToPlayer(_playCameraDistance);

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
                    break;
                case Phase.Rewind:
                    TimeManager.Instance.SetTimerAfterRewind();
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

        private void OnPlayEndHandler()
        {
            ChangePhase(Phase.Rest);
        }
        private void OnRewindHandler()
        {
            ChangePhase(Phase.Play);
        }

        private void OnTimerEndHandler()
        {
            _endtext = "Game Over";
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
        private void ResetCameraToPlayer(float distance)
        {
            _currentCar = _carManager.GetCurrentCar().transform;
            _cameraFollow.SetTarget(_currentCar, distance, _playResetCameraTime);
        }

        private void ResetCameraToRest()
        {
            _cameraFollow.SetTarget(_restCameraTransform, _restCameraDistance, _restResetCameraTime);
        }

        public void OnStartClick()
        {
            SceneManager.LoadScene(0);
        }

        #endregion


        #region Private and Protected

        [Header("References")]
        [SerializeField] private CarManager _carManager;
        [SerializeField] private CanvaManager _canvaManager;

        [SerializeField] private Button _RewindButton;
        [SerializeField] Transform _restCameraTransform;
        [SerializeField] private CameraFollow _cameraFollow;

        [Header("Rewind Speed")]
        [SerializeField] private float _rewindSpeed;

        [Header("Camera")]
        [SerializeField] private float _restResetCameraTime;
        [SerializeField] private float _playResetCameraTime;
        [SerializeField] float _playCameraDistance;
        [SerializeField] float _restCameraDistance;
        [SerializeField] private float _rewindResetCameraDistance;

        private InputAction _continueInput;
        private Transform _currentCar;
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