using System.Collections;
using UnityEngine;

namespace HorseRace.Camera
{
    public class FinishLineCameraAngle : CameraAngle
    {
        [SerializeField] private float slowDownFactor;
        [SerializeField] private float slowDownLength;
        [SerializeField] private float fixedSlowDownSpeed;
        [SerializeField] private float slowDownStartAfter = 1f;

        private float fixedDeltaTime;

        private void OnEnable()
        {
            EventManager.Instance.OnRaceFinishEvent += FinishState;
        }
        private void OnDisable()
        {
            EventManager.Instance.OnRaceFinishEvent -= FinishState;
        }

        public override void StartState()
        {
            base.StartState();
            StartCoroutine(IEStartSlowDown());
        }
        public override void UpdateState()
        {
        }
        public override void FinishState()
        {
            Time.timeScale = 1;
            //          Time.fixedDeltaTime = fixedDeltaTime;
            base.FinishState();
        }
        IEnumerator IEStartSlowDown()
        {
            fixedDeltaTime = Time.fixedDeltaTime;
            yield return new WaitForSeconds(slowDownStartAfter);
            Time.timeScale = slowDownFactor;
            //        Time.fixedDeltaTime = Time.timeScale * fixedSlowDownSpeed;
        }
    }
}