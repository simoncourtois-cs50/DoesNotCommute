using System;
using UnityEngine;

namespace Timer.Runtime
{
    public class TimeManager : MonoBehaviour
    {
        #region Public

        public bool m_isPlaying { get; private set; }
        public float m_currentTime { get; private set; }

        public event Action OnPause;
        public event Action OnPlay;
        public event Action OnEnd;
        public static TimeManager Instance { get; private set; }

        #endregion


        #region Unity API

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }
            m_currentTime = _timerValue;
        }

        private void Update()
        {
            if (m_isPlaying) m_currentTime -= Time.deltaTime;

            if (_isRewinding) HandleRewind();

            CheckEndGame();
        }

        #endregion


        #region Main API

        public void PauseTimer()
        {
            m_isPlaying = false;
            OnPause?.Invoke();
        }

        public void PlayTimer()
        {
            m_isPlaying = true;
            _isRewinding = false;

            OnPlay?.Invoke();
        }

        public void ResetTimer()
        {
            m_currentTime = _timerValue;
        }

        public void StartChrono()
        {
            _startStamp = m_currentTime;
        }

        public void RewindTimer(float rewindSpeed)
        {
            _isRewinding = true;
            m_isPlaying = false;
            _rewindSpeed = rewindSpeed;
        }
        private void HandleRewind()
        {
            m_currentTime += Time.deltaTime * _rewindSpeed;

            if (m_currentTime < _startStamp) return;
            
           
            m_isPlaying = false;
            _isRewinding = false;
            SetTimerAfterRewind();
        }
        public void SetTimerAfterRewind()
        {
            m_currentTime = _startStamp - 1f;
        }
        
        private void CheckEndGame()
        {
            if (m_currentTime > 0) return;
            ResetTimer();
            PauseTimer();
            OnEnd?.Invoke();
        }

        #endregion


        #region

        [SerializeField] private float _timerValue;
        private float _startStamp;
        private float _rewindSpeed;
        private bool _isRewinding;

        #endregion
    }
}