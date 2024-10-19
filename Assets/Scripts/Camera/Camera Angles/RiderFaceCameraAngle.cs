using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace HorseRace.Camera
{
    public class RiderFaceCameraAngle : CameraAngle
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera2;
        [SerializeField] private float changeRiderFaceTime;

        private int racePositionIndex;
        private float riderFaceTimer;
        private List<Transform> horseTransforms = new List<Transform>();

        public override void StartState()
        {
            base.StartState();
            riderFaceTimer = Time.fixedTime;
            horseTransforms = cameraController.HorsesInRaceOrderList;
            racePositionIndex = horseTransforms.Count - 1;
            ChangeRiderFace();
        }
        public override void UpdateState()
        {
            if (riderFaceTimer + changeRiderFaceTime < Time.fixedTime)
            {
                riderFaceTimer = Time.fixedTime;
                racePositionIndex--;
                if (racePositionIndex < 0)
                {
                    FinishState();
                    return;
                }
                ChangeRiderFace();
            }
        }
        public override void FinishState()
        {
            virtualCamera.enabled = false;
            OnEndEvent?.Invoke();
            gameObject.SetActive(false);
        }

        private void ChangeRiderFace()
        {
            if (virtualCamera.enabled)
            {
                virtualCamera2.enabled = true;
                virtualCamera.enabled = false;
                virtualCamera2.Follow = horseTransforms[racePositionIndex];
                virtualCamera2.LookAt = horseTransforms[racePositionIndex];
            }
            else
            {
                virtualCamera.enabled = true;
                virtualCamera2.enabled = false;
                virtualCamera.Follow = horseTransforms[racePositionIndex];
                virtualCamera.LookAt = horseTransforms[racePositionIndex];
            }
        }
    }
}
