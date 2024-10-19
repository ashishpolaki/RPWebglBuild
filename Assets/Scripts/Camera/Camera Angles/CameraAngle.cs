using AarquieSolutions.InspectorAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace HorseRace.Camera
{
    public class CameraAngle : MonoBehaviour, ICameraSetup
    {
        #region Inspector Variables
        [SerializeField] private CameraMode cameraMode;
        [SerializeField][HideIf("cameraMode", CameraMode.Basic)] private CameraType cameraType;
        [SerializeField][HideIf("cameraMode", CameraMode.Basic)] private int priorityLevel;
        [SerializeField] protected CinemachineVirtualCamera virtualCamera;
        [SerializeField] protected UnityEvent OnStartEvent;
        [SerializeField] protected UnityEvent OnEndEvent;
        #endregion

        #region Protected Variables
        protected CameraControllerLoad cameraController;
        #endregion

        #region Properties
        public CameraMode CameraAngleType => cameraMode;
        public CameraType CameraType => cameraType;
        public int PriorityLevel => priorityLevel;
        #endregion

        #region Virtual Methods
        public virtual void StartState()
        {
            gameObject.SetActive(true);
            this.cameraController = (CameraControllerLoad)GameManager.Instance.CameraController;
            OnStartEvent?.Invoke();
        }
        public virtual void UpdateState() { }
        public virtual void FinishState()
        {
            virtualCamera.enabled = false;
            OnEndEvent?.Invoke();
            cameraController.ChangeBasicCameraAngle();
            gameObject.SetActive(false);
        }
        #endregion

    }
    public interface ICameraSetup
    {
        //Properties
        public CameraMode CameraAngleType { get; }
        public CameraType CameraType { get; }
        public int PriorityLevel { get; }

        //Abstract Methods
        public abstract void StartState();
        public abstract void UpdateState();
        public abstract void FinishState();
    }

}
