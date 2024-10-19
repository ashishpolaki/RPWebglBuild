using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

namespace HorseRace.Camera
{
    public abstract class CameraController : MonoBehaviour
    {
        [SerializeField] protected CinemachineBrain cinemachineBrain;
        [SerializeField] protected CinemachineTargetGroup targetGroup;

        public void SetTargetGroup(List<Transform> _transforms)
        {
            if (targetGroup != null)
            {
                for (int i = 0; i < _transforms.Count; i++)
                {
                    targetGroup.AddMember(_transforms[i], 1, 0);
                }
            }
        }
    }
}
