namespace HorseRace.Camera
{
    public class RaceWinnerCameraAngle : CameraAngle
    {
        private void OnEnable()
        {
            EventManager.Instance.OnWinnersMedalEvent += FinishState;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnWinnersMedalEvent -= FinishState;
        }

        public override void StartState()
        {
            base.StartState();
            virtualCamera.LookAt = (cameraController.WinnerHorseTransform);
            virtualCamera.Follow = (cameraController.WinnerHorseTransform);
        }
    }
}