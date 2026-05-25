using Car.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GameManager.Runtime
{
    public class GameConductor : MonoBehaviour
    {
        #region Unity API

        private void Awake() => _continueInput = InputSystem.actions.FindAction("Jump");

        private void Start() => _carManager._OnPlayEnd += OnPlayEndHandler;
        
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
                case Phase.Rest:
                    break;
                case Phase.Play:
                    ActivateRewindButton();
                    _carManager.ActivateCarControl();
                    break;
                case Phase.Rewind:
                    _carManager.Rewind();
                    break;
            }
        }
        
        private void OnStayPhase()
        {
            switch (_currentPhase)
            {
                case Phase.Rest:
                    CheckContinue();
                    break;
                case Phase.Play:
                    break;
                case Phase.Rewind:
                    break;
            }
        }
        
        private void OnExitPhase()
        {
            switch (_currentPhase)
            {
                case Phase.Rest:
                    break;
                case Phase.Play:
                    DeactivateRewindButton();
                    break;
                case Phase.Rewind:
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

        #endregion
        
        #region Private and Protected

        private enum Phase
        {
            Rest,
            Play,
            Rewind
        }
        
        private Phase _currentPhase;
        
        [SerializeField] private CarManager _carManager;
        [SerializeField] private Button _RewindButton;
        private InputAction _continueInput;

        #endregion
    }
}
