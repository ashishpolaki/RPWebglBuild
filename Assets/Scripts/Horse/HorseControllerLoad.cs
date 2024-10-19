using UnityEngine;

namespace HorseRace
{
    public class HorseControllerLoad : HorseController, ILoadHorseData
    {
        private bool canSlowAgentSpeed;

        public override void UpdateState()
        {
            base.UpdateState();
            SpeedState();
            CheckFinishLineStatus();
        }
        protected override void AnimationState()
        {
            //Load Data for animation State
            base.AnimationState();
        }
        public void SetVelocity(Vector3 _vel)
        {
            if (!isFinishLineCross)
            {
                agent.velocity = _vel;
            }
            else
            {
                //If the horse crossed the finish line, then slow down the nav agent speed.
                if (!canSlowAgentSpeed)
                {
                    canSlowAgentSpeed = true;
                    agent.speed = agent.velocity.magnitude;
                    agent.radius = 1.3f;
                }
            }
        }

        /// <summary>
        /// Slow down speed after finish line cross
        /// </summary>
        private void SpeedState()
        {
            if (canSlowAgentSpeed)
            {
                agent.speed -= slowDownSpeedLerp;
            }
        }

        [SerializeField] private float splineChangeDistance = 3f;
        private int controlPointIndex = 0;
        private Vector3 currentSplinePos;

        private void CheckFinishLineStatus()
        {
            if (isFinishLineCross)
            {
                float distance = Vector3.Distance(transform.position, currentSplinePos);
                if (!agent.hasPath || distance <= splineChangeDistance)
                {
                    Vector3 destination = SplineManager.Instance.GetNearestControlPoint(controlPointIndex, transform.position);
                    currentSplinePos = destination;
                    agent.SetDestination(destination);
                    controlPointIndex++;
                }
            }
        }
    }
}
