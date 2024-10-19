using UnityEngine;
using Unity.Cinemachine;

namespace HorseRace.Camera
{
    public class SimpleFollowCameraAngle : CameraAngle
    {
        [SerializeField] private Transform _changeCameraTransform;
        [SerializeField] private ForwardType _forwardType;

        private Transform targetTransform;

        public enum ForwardType
        {
            Left,
            Right,
            Front,
            Back
        }
        public override void StartState()
        {
            base.StartState();
            targetTransform = cameraController.HorseInFirstPlace;
        }
        public override void UpdateState()
        {
            Vector3 normal = (_changeCameraTransform.position - targetTransform.position).normalized;
            Vector3 dotVector = Vector3.zero;

            //Choose Dot Forward
            switch (_forwardType)
            {
                case ForwardType.Left:
                    dotVector = -targetTransform.right;
                    break;
                case ForwardType.Right:
                    dotVector = targetTransform.right;
                    break;
                case ForwardType.Front:
                    dotVector = targetTransform.forward;
                    break;
                case ForwardType.Back:
                    dotVector = -targetTransform.forward;
                    break;
                default:
                    break;
            }
            float dot = Vector3.Dot(normal, dotVector);
            //Check if you have crossed the change transform point
            if (dot < 0)
            {
                FinishState();
            }
        }
    }
}
