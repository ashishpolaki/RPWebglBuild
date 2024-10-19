using System;
using UnityEngine;

namespace HorseRace
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance;

        #region Delegates
        public event Action OnRaceStartEvent;
        public event Action OnRaceFinishEvent;
        public event Action<int> OnCrossFinishLineEvent;
        public event Action<CameraType> OnOvertakeCameraEvent;
        public event Action OnCameraConfigureEvent;
        public event Action<int,int> OnControlPointChangeEvent;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        #endregion

        public void StartRace()
        {
            OnRaceStartEvent?.Invoke();
        }
        public void CrossFinishLine(int _horseNumber)
        {
            OnCrossFinishLineEvent?.Invoke(_horseNumber);
        }
        public void RaceFinished()
        {
            OnRaceFinishEvent?.Invoke();
        }
        public void OnOvertakeCamera(CameraType _cameraType)
        {
            OnOvertakeCameraEvent?.Invoke(_cameraType);
        }
        public void OnCameraSetup()
        {
            OnCameraConfigureEvent?.Invoke();
        }
        public void OnControlPointChange(int horseNumber,int controlPointNumber)
        {
            OnControlPointChangeEvent?.Invoke(horseNumber, controlPointNumber);
        }
    }
}
