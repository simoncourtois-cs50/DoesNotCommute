using Car.Runtime;
using Timer.Runtime;
using TMPro;
using UnityEngine;

namespace GameManager.Runtime
{
    public class CanvaManager : MonoBehaviour
    {
        #region Main API

        private void Start()
        {
            _carManager.OnRewindEnd += ActivateMalusDisplay;
        }

        private void Update()
        {
            DisplayTime();

            if (!_isMalusVisible) return;

            DisplayMalus();
        }

        private void DisplayTime()
        {
            float time = TimeManager.Instance.m_currentTime;
            float milliSeconds = (Mathf.Floor(time % 1 * 10));
            float seconds = Mathf.Floor(time);
            _secondDisplay.text = seconds.ToString();
            _millisecondDisplay.text = milliSeconds.ToString();
        }

        private void DisplayMalus()
        {
            _malusTimer += Time.deltaTime;
            float ratio = Mathf.Sin(_malusTimer / _malusAppearanceLength);
            _malusDisplay.alpha = Mathf.Lerp(0, 255, ratio);
            if (ratio > 0) return;

            _isMalusVisible = false;
            _malusTimer = 0;
        }

        private void ActivateMalusDisplay()
        {
            _isMalusVisible = true;
        }

        public void ActivateEndMenu(string EndText)
        {
            _endMenu.SetActive(true);
            _endText.text = EndText;
        }

        #endregion


        #region Private and Protected

        [SerializeField] private TMP_Text _secondDisplay;
        [SerializeField] private TMP_Text _millisecondDisplay;
        [SerializeField] private TMP_Text _malusDisplay;
        [SerializeField] private TMP_Text _endText;
        [SerializeField] private GameObject _endMenu;
        [SerializeField] private float _malusAppearanceLength;
        [SerializeField] private CarManager _carManager;

        private float _malusTimer;
        private bool _isMalusVisible;

        #endregion
    }
}