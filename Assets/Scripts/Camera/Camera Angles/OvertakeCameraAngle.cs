using UnityEngine;

namespace HorseRace.Camera
{
    public class OvertakeCameraAngle : CameraAngle
    {
        [SerializeField] private float overtakeTime;
        private float overtakeTimer;
       
        public override void StartState( )
        {
            base.StartState();
            virtualCamera.enabled = true;
            overtakeTimer = Time.fixedTime;
            virtualCamera.Follow = cameraController.OvertakingHorse;
            virtualCamera.LookAt = cameraController.OvertakingHorse;
        }
        public override void UpdateState()
        {
            //If timer is completed, Return Finish State.
            if (overtakeTimer + overtakeTime < Time.fixedTime)
            {
                FinishState();
            }
        }
        public override void FinishState()
        {
            virtualCamera.enabled = false;
            OnEndEvent?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
