using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

namespace HorseRace.Camera
{
    public class CameraControllerSave : CameraController
    {
        #region Inspector Variables
        [SerializeField] private CinemachineVirtualCamera zoomInCamera;
        [SerializeField] private CinemachineVirtualCamera zoomOutCamera;
        [SerializeField] private float zoomOutWaitTime = 2;
        #endregion

        #region Private Variables
        private Transform winnerTransform;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            GameManager.Instance.SetCameraController(this);
        }
        private void OnEnable()
        {
            EventManager.Instance.OnCameraConfigureEvent += OnInitializeCamera;
        }
        private void OnDisable()
        {
            EventManager.Instance.OnCameraConfigureEvent -= OnInitializeCamera;
        }
        #endregion

        #region Private Methods
        private void OnInitializeCamera()
        {
            winnerTransform = GameManager.Instance.RaceManager.RaceWinnerTransform();
            SetZoomInCamera(winnerTransform);
            StartCoroutine(IBlendCameraZoom());
        }
        /// <summary>
        /// Zooming in to the transform 
        /// </summary>
        /// <param name="_transform"></param>
        private void SetZoomInCamera(Transform _transform)
        {
            zoomOutCamera.enabled = false;
            zoomInCamera.enabled = true;
            zoomInCamera.LookAt = _transform;
            zoomInCamera.Follow = _transform;
        }
        /// <summary>
        /// Zoom out to the transform 
        /// </summary>
        /// <param name="_transform"></param>
        private void SetZoomOutCamera(Transform _transform)
        {
            zoomInCamera.enabled = false;
            zoomOutCamera.enabled = true;
            zoomOutCamera.LookAt = _transform;
            zoomOutCamera.Follow = _transform;
        }
        IEnumerator IBlendCameraZoom()
        {
            yield return new WaitForSeconds(zoomOutWaitTime);
            SetZoomOutCamera(winnerTransform);
            while (cinemachineBrain.IsBlending)
            {
                yield return new WaitForSeconds(Time.smoothDeltaTime);
            }
            EventManager.Instance.StartRace();
        }
        #endregion
    }
}
