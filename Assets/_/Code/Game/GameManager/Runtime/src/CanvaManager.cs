using Timer.Runtime;
using TMPro;
using UnityEngine;

namespace GameManager.Runtime
{
    public class CanvaManager : MonoBehaviour
    {
        #region Main API
        private void Update()
        {
            DisplayTime();
        }
        private void DisplayTime()
        {
            float time = TimeManager.Instance.m_currentTime;
            float milliSeconds = (Mathf.Floor(time % 1 * 10));
            float seconds = Mathf.Floor(time);
            _secondDisplay.text = seconds.ToString();
            _millisecondDisplay.text = milliSeconds.ToString();
        }

        #endregion


        #region Private and Protected

        [SerializeField] private TMP_Text _secondDisplay;
        [SerializeField] private TMP_Text _millisecondDisplay;
        
        #endregion
    }
}