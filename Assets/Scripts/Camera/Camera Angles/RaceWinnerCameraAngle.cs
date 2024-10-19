namespace HorseRace.Camera
{
    public class RaceWinnerCameraAngle : CameraAngle
    {
        public override void StartState()
        {
            base.StartState();
            virtualCamera.LookAt = (cameraController.WinnerHorseTransform);
            virtualCamera.Follow = (cameraController.WinnerHorseTransform);
        }
    }
}