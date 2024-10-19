using UnityEngine;
using Unity.Cinemachine;

namespace HorseRace.Camera
{
    public class DollyCartCameraAngle : CameraAngle
    {
        [SerializeField] private CinemachineSmoothPath cinemachineSmoothPath;
        [SerializeField] private CinemachineDollyCart cineMachineDollyCart;

        [SerializeField] private float startSpeed;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float lerpSpeed;

        private float pathLength;

        public override void StartState()
        {
            base.StartState();
            pathLength = cinemachineSmoothPath.m_Waypoints.Length - 1;
            cineMachineDollyCart.m_Speed = startSpeed;
            virtualCamera.LookAt = cameraController.HorseInFirstPlace ?? null;
        }
        public override void UpdateState()
        {
            if (cineMachineDollyCart.m_Speed < maxSpeed)
            {
                cineMachineDollyCart.m_Speed += lerpSpeed * Time.fixedDeltaTime;
            }

            //Return Finishstate, if the dolly cart has reached its final destination
            if (cineMachineDollyCart.m_Position >= pathLength)
            {
                FinishState();
            }
        }
    }
}
