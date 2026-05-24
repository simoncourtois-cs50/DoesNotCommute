using Car.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

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
                    _carManager.ActivateCarControl();
                    break;
                case Phase.Rewind:
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
        private InputAction _continueInput;

        #endregion
    }
}
