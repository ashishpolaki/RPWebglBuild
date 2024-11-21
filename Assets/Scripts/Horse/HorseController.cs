using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HorseRace
{
    public interface ISaveHorseData
    {
        public HorseData HorseSaveData();
    }
    public interface ILoadHorseData
    {
        public void SetVelocity(Vector3 _vel);
    }

    public class HorseController : MonoBehaviour
    {
        #region Inspector Variables
        //Inspector Variables
        [SerializeField] private HorseLeg[] horseLegs;
        [SerializeField] private TMPro.TextMeshProUGUI[] horseNumberTexts;
        [SerializeField] private SkinnedMeshRenderer[] horseMeshes;
        [SerializeField] private SkinnedMeshRenderer[] jockeyMeshes;
        [SerializeField] private Animator horseAnimator;
        [SerializeField] private Animator jockeyAnimator;
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] private float animationMinSpeed = 4;
        [SerializeField] protected float slowDownSpeedLerp = 1.4f;
        #endregion

        #region Protected Variables
        protected bool isFinishLineCross;
        protected float actualSpeed;
        public float currentPercentageInSpline;
        #endregion

        #region Private Variables
        private int horseNumber;
        private int racePosition;
        private float jockeyAnimDamp = 0;
        private float verticalAnimDamping;
        #endregion

        #region Properties
        public Material[] HorseMaterials { get; private set; }
        public Material[] JockeyMaterials { get; private set; }

        public int HorseNumber { get => horseNumber; }
        public int RacePosition { get => racePosition; }
        public float AgentCurrentSpeed { get => agent.speed; }
        public float AgentActualSpeed { get => actualSpeed; }
        public bool IsFinishLineCrossed { get => isFinishLineCross; }
        #endregion


        #region Public Methods
        public virtual void RaceStart()
        {
        }
        public void SetHorseNumber(int _horseNumber)
        {
            horseNumber = _horseNumber;
            //Set horse number Text on both sides
            for (int n = 0; n < horseNumberTexts.Length; n++)
            {
                horseNumberTexts[n].text = $"{horseNumber}";
            }
            //Set Horse legs Triggers
            for (int i = 0; i < horseLegs.Length; i++)
            {
                horseLegs[i].SetHorseNumber(horseNumber);
            }
        }
        public void InitializeMaterials(List<Material> _horseMaterials, List<Material> _jockeyMaterials)
        {
            HorseMaterials = _horseMaterials.ToArray();
            JockeyMaterials = _jockeyMaterials.ToArray();
            for (int i = 0; i < horseMeshes.Length; i++)
            {
                horseMeshes[i].materials = HorseMaterials;
            }
            for (int i = 0; i < jockeyMeshes.Length; i++)
            {
                jockeyMeshes[i].materials = JockeyMaterials;
            }
        }
        public virtual void UpdateState()
        {
            AnimationState();
        }
        public void FinishLineCrossed()
        {
            isFinishLineCross = true;
        }
        public void SetSpeed(float _speed)
        {
            agent.speed = _speed;
            actualSpeed = _speed;
        }
        public void SetRacePosition(int raceNumber)
        {
            racePosition = raceNumber;
        }
        #endregion

        #region Animation
        protected virtual void AnimationState()
        {
            //Vertical Movement Calculation
            float velocity = agent.velocity.magnitude;
            float animatorSpeed = 1;
            if (velocity > 0)
            {
                animatorSpeed = animationMinSpeed + Mathf.InverseLerp(14, 17, velocity);
                verticalAnimDamping = Mathf.InverseLerp(0, 16, velocity);
            }

          //  jockeyAnimDamp = Mathf.Lerp(jockeyAnimDamp, verticalAnimDamping + Mathf.InverseLerp(16, 17, velocity), Time.deltaTime);
            horseAnimator.speed = jockeyAnimator.speed = animatorSpeed;
            horseAnimator.SetFloat("Vertical", velocity == 0 ? velocity : verticalAnimDamping);
            jockeyAnimator.SetFloat("Vertical", velocity == 0 ? velocity : verticalAnimDamping);
        }
        #endregion
    }
}
