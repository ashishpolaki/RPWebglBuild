using UnityEngine;

namespace HorseRace
{
    public class HorseControllerLoad : HorseController, ILoadHorseData
    {
        [SerializeField] private float splineChangeDistance = 3f;
        [SerializeField] private Character character;
        private int controlPointIndex = 0;
        private Vector3 currentSplinePos;

        private bool canSlowAgentSpeed;

        public Character Character => character;


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
            if (canSlowAgentSpeed || isFinishLineCross)
            {
                if (agent != null)
                {
                    agent.speed -= slowDownSpeedLerp;
                }
            }
        }

        private void CheckFinishLineStatus()
        {
            if (isFinishLineCross && agent != null)
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

        public void SetCharacter(CharacterCustomisationEconomy characterCustomisationEconomy)
        {
            character.Load(characterCustomisationEconomy);
        }
    }
}
