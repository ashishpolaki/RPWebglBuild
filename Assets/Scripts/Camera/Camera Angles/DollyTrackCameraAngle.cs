using UnityEngine;
using Unity.Cinemachine;

namespace HorseRace.Camera
{
    public class DollyTrackCameraAngle : CameraAngle
    {
        [SerializeField] private CinemachineSmoothPath cinemachineSmoothPath;

        private CinemachineTrackedDolly cinemachineTrackedDolly;
        private float pathLength;

        public override void StartState()
        {
            base.StartState();
            cinemachineTrackedDolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            pathLength = cinemachineSmoothPath.m_Waypoints.Length - 1;
            virtualCamera.LookAt = cameraController.HorseInFirstPlace;
            virtualCamera.Follow = cameraController.HorseInFirstPlace;
        }
        public override void UpdateState()
        {
            //Return Finishstate, if the dolly cart has reached its final destination
            if (cinemachineTrackedDolly.m_PathPosition >= pathLength)
            {
                FinishState();
            }
        }
    }
}