using System;
using System.Collections.Generic;
using UnityEngine;

namespace HorseRace.Camera
{
    public class CameraControllerLoad : CameraController
    {
        #region Inspector Variables
        [SerializeField] private CameraAngle[] basicCameraAngles;
        [SerializeField] private CameraAngle[] specialCameraAngles;
        #endregion

        #region Private Variables
        private CameraAngle currentBasicCamera;
        private CameraAngle currentSpecialCamera;

        private bool isSpecialCameraActive;
        private int basicCameraIndex = 0;
        #endregion

        #region Properties
        public Transform WinnerHorseTransform { get { return GameManager.Instance.RaceManager.RaceWinnerTransform(); } }
        public Transform OvertakingHorse { get { return GameManager.Instance.RaceManager.CurrentOvertakingHorse.transform; } }
        [Tooltip("Key: RacePosition, Value : (Horse Number, Transform)")]
        public Dictionary<int, (int, Transform)> HorsesTransformInRaceOrder { get { return GameManager.Instance.RaceManager.HorseTransformsInRaceOrder(); } }
        public Transform HorseInFirstPlace
        {
            get
            {
                if (HorsesTransformInRaceOrder.Count > 0)
                {
                    return HorsesTransformInRaceOrder[1].Item2;
                }
                else
                {
                    return WinnerHorseTransform;
                }
            }
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            GameManager.Instance.SetCameraController(this);
        }
        private void OnEnable()
        {
            EventManager.Instance.OnOvertakeCameraEvent += OnOvertakeCameraHandle;
            EventManager.Instance.OnCameraConfigureEvent += OnInitializeCamera;
        }
        private void OnDisable()
        {
            EventManager.Instance.OnOvertakeCameraEvent -= OnOvertakeCameraHandle;
            EventManager.Instance.OnCameraConfigureEvent -= OnInitializeCamera;
        }
        private void FixedUpdate()
        {
            //Update Normal Camera Angle
            currentBasicCamera?.UpdateState();

            //Update Speical Camera Angle 
            if (isSpecialCameraActive)
            {
                currentSpecialCamera?.UpdateState();
            }
        }
        #endregion

        #region Private Methods
        private void OnInitializeCamera()
        {
            SetBasicCameraAngle();
        }
        #endregion

        #region Special Camera Angle
        private void OnOvertakeCameraHandle(CameraType camera)
        {
            ActivateSpecialCamera(camera);
        }
        private void ActivateSpecialCamera(CameraType cameraType)
        {
            //Find Special Camera angle with camera type
            CameraAngle targetSpecialCamera = Array.Find(specialCameraAngles, x => x.CameraType == cameraType);

            //Change Special Camera Angle if the current special camera has a lower priority level
            if (!isSpecialCameraActive || this.currentSpecialCamera.PriorityLevel < targetSpecialCamera.PriorityLevel)
            {
                this.currentSpecialCamera?.FinishState();
                SetCurrentSpecialCamera(targetSpecialCamera);
            }
        }
        private void SetCurrentSpecialCamera(CameraAngle _specialCamera)
        {
            currentSpecialCamera = _specialCamera;
            currentSpecialCamera?.StartState();
            isSpecialCameraActive = true;
        }
        #endregion

        #region Basic Camera Angle
        public void ChangeBasicCameraAngle()
        {
            basicCameraIndex++;
            SetBasicCameraAngle();
        }
        private void SetBasicCameraAngle()
        {
            currentBasicCamera = basicCameraAngles[basicCameraIndex];
            currentBasicCamera.StartState();
        }
        #endregion

        #region CameraAngles UnityEvent Methods
        public void StartRace()
        {
            EventManager.Instance.StartRace();
        }
        public void OnActiveRiderFaceCamera()
        {
            ActivateSpecialCamera(CameraType.RiderCloseUp);
        }
        public void DeactivateSpecialCamera()
        {
            isSpecialCameraActive = false;
        }
        public void EnableRacePositionsUI()
        {
            UI.UIManager.Instance.EnableRacePositions(false);
        }
        public void IncreaseGallopSound()
        {
            SoundManager.Instance.IncreaseGallopSound();
        }
        public void ResetGallopSound()
        {
            SoundManager.Instance.ResetGallopSound();
        }
        #endregion

    }
}
