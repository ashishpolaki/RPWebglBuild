using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace HorseRace.Camera
{
    public class RiderFaceCameraAngle : CameraAngle
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera2;
        [SerializeField] private float changeRiderFaceTime;

        private float riderFaceTimer;
        [Tooltip("Key: RacePosition, (Value : Item1 = Horse Number , Item2 = Horse Transform)")]
        private Dictionary<int, (int, Transform)> horseTransformsInRaceOrder = new Dictionary<int, (int, Transform)>();
        private List<int> capturedHorsesList = new List<int>();

        public override void StartState()
        {
            base.StartState();
            riderFaceTimer = Time.fixedTime;
            ChangeRiderFace();
        }
        public override void UpdateState()
        {
            if (riderFaceTimer + changeRiderFaceTime < Time.fixedTime)
            {
                riderFaceTimer = Time.fixedTime;

                //If all horses are captured then finish the state
                if (capturedHorsesList.Count == horseTransformsInRaceOrder.Count)
                {
                    FinishState();
                    return;
                }

                //Change the camera angle to the next horse
                ChangeRiderFace();
            }
        }
        public override void FinishState()
        {
            horseTransformsInRaceOrder.Clear();
            horseTransformsInRaceOrder = null;
            virtualCamera.enabled = false;
            OnEndEvent?.Invoke();
            gameObject.SetActive(false);
        }

        private void ChangeRiderFace()
        {
            //Get the horse Transforms that are in race order.
            horseTransformsInRaceOrder = cameraController.HorsesTransformInRaceOrder;
            Transform horseTransform = horseTransformsInRaceOrder[1].Item2;

            //Get the horse that is not captured yet.
            for (int i = horseTransformsInRaceOrder.Count; i > 0; i--)
            {
                int horseNumber = horseTransformsInRaceOrder[i].Item1;
                if (!capturedHorsesList.Contains(horseNumber))
                {
                    horseTransform = horseTransformsInRaceOrder[i].Item2;
                    capturedHorsesList.Add(horseNumber);
                    break;
                }
            }

            //Update the camera angle
            if (virtualCamera.enabled)
            {
                virtualCamera2.enabled = true;
                virtualCamera.enabled = false;
                virtualCamera2.Follow = horseTransform;
                virtualCamera2.LookAt = horseTransform;
            }
            else
            {
                virtualCamera.enabled = true;
                virtualCamera2.enabled = false;
                virtualCamera.Follow = horseTransform;
                virtualCamera.LookAt = horseTransform;
            }
        }
    }
}
