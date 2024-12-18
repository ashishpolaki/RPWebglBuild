using Unity.Mathematics;
using UnityEngine;

namespace HorseRace
{
    public interface ISaveHorseData
    {
        public HorseData HorseSaveData();
    }

    public class HorseController : MonoBehaviour
    {
        #region Inspector Variables
        //Inspector Variables
        [SerializeField] private HorseLeg[] horseLegs;
        [SerializeField] private TMPro.TextMeshProUGUI[] horseNumberTexts;
        [SerializeField] private SkinnedMeshRenderer[] horseMeshes;
        [SerializeField] private Animator horseAnimator;
        [SerializeField] private Animator jockeyAnimator;
        [SerializeField] private Transform colliderTransform;
        #endregion

        #region Private Variables
        private bool isFinishLineCross;
        private int horseNumber;
        private int racePosition;
        private int currentControlPointIndex;
        private int currentSplinePointIndex;
        [Tooltip("Spline Change Threshold")]
        private float thresholdDistance;
        private float targetSpeed;
        private float maxSpeed;
        private float currentSpeed;
        private float acceleration;
        #endregion

        #region Protected Variables
        protected SplineData currentSplineData;
        protected int currentSplineIndex;
        #endregion

        #region Properties
        public Material HorseMaterial { get; private set; }
        public int HorseNumber { get => horseNumber; }
        public int RacePosition { get => racePosition; }
        public bool IsControlPointChange { get; private set; }
        public bool IsFinishLineCrossed { get => isFinishLineCross; }
        public Transform ColliderTransform { get => colliderTransform; }

        public SplineData CurrentSplineData { get => currentSplineData; }
        public int CurrentControlPointIndex { get => currentControlPointIndex; }
        public int CurrentSplinePointIndex { get => currentSplinePointIndex; }
        public int CurrentSplineIndex { get => currentSplineIndex; }
        public float Acceleration { get => acceleration; }
        public float TargetSpeed { get => targetSpeed; }
        public float MaxSpeed { get => maxSpeed; }
        public float CurrentSpeed { get => currentSpeed; }
        #endregion

#if UNITY_EDITOR
        public bool canDrawGizmos;
        private void OnDrawGizmos()
        {
            if (canDrawGizmos)
            {
                var splinePoints = currentSplineData.splinePoints;
                for (int i = 0; i < splinePoints.Count - 1; i++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(splinePoints[i].position, 0.1f); // Draw a sphere at the calculated position
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(splinePoints[i].position, splinePoints[i + 1].position);
                }
            }
        }
#endif
        #region Initialize
        public virtual void InitializeData(SplineData _splineData, float _speed, float _maxSpeed, float _accleration, float _thresHold)
        {
            currentSplineData = _splineData;
            currentSplineIndex = _splineData.splineIndex;
            targetSpeed = _speed;
            maxSpeed = _maxSpeed;
            acceleration = _accleration;
            thresholdDistance = _thresHold;
        }
        #endregion

        #region Spline Code
        public virtual void UpdateState()
        {
            MoveAlongSpline();
        }
        public void MoveAlongSpline()
        {
            if (currentSplinePointIndex >= currentSplineData.splinePoints.Count)
            {
                return;
            }

            Vector3 targetPosition = currentSplineData.splinePoints[currentSplinePointIndex].position;
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 newPosition = transform.position + direction * currentSpeed * Time.fixedDeltaTime;

            // Check if the horse is within the threshold distance to the target splinePoint position
            if (Vector3.Distance(transform.position, targetPosition) < thresholdDistance)
            {
                currentSplinePointIndex++;
                if (currentSplinePointIndex < currentSplineData.splinePoints.Count)
                {
                    //Search for upcoming control Point Index
                    if (currentControlPointIndex + 1 == currentSplineData.splinePoints[currentSplinePointIndex].controlPointIndex)
                    {
                        OnControlPointChange();
                    }
                }
            }

            //update position
            transform.position = newPosition;

            //Update Rotation
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * currentSpeed);
            }

            SpeedState();
            AnimationState();
        }
       
        public virtual void SetSpline(SplineData splineData)
        {
            currentSplineData = splineData;
        }
        protected virtual void OnControlPointChange()
        {
            currentControlPointIndex++;
            IsControlPointChange = true;
        }
        public void OnControlPointChangeSuccessful()
        {
            IsControlPointChange = false;
        }
        #endregion

        #region Speed
        public virtual void SetSpeed(float speed, float aceleration)
        {
            acceleration = aceleration;
            targetSpeed = speed;
        }
        public virtual void SpeedState()
        {
            float speedChange = acceleration * Time.fixedDeltaTime;
            currentSpeed = currentSpeed < targetSpeed
                ? Mathf.Clamp(currentSpeed + speedChange, 0, targetSpeed)
                : Mathf.Clamp(currentSpeed - speedChange, targetSpeed, maxSpeed);
        }
        #endregion

        #region Animation
        public virtual void AnimationState()
        {
            if (horseAnimator != null)
            {
                horseAnimator.SetFloat("Vertical", currentSpeed / maxSpeed);
                jockeyAnimator.SetFloat("Vertical", currentSpeed / maxSpeed);
                horseAnimator.speed = (Time.timeScale == 1) ? math.max(currentSpeed / maxSpeed, 1) : Time.timeScale;
                jockeyAnimator.speed = (Time.timeScale == 1) ? math.max(currentSpeed / maxSpeed, 1) : Time.timeScale;
            }
        }
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
        public void InitializeMaterials(Material _horseMaterial)
        {
            HorseMaterial = _horseMaterial;
            for (int i = 0; i < horseMeshes.Length; i++)
            {
                horseMeshes[i].material = HorseMaterial;
            }
        }
        public void FinishLineCrossed()
        {
            isFinishLineCross = true;
        }
        public void SetRacePosition(int raceNumber)
        {
            racePosition = raceNumber;
        }
        #endregion
    }


}
