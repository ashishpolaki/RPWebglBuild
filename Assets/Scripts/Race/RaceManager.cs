using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HorseRace
{
    public class RaceManager : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] protected HorseSplineManager horseSplineManager;
        [SerializeField] protected HorseSpeedSO horseSpeedSO;
        #endregion

        #region Protected Variables
        protected bool isRaceStart;
        [Tooltip("Key: HorseNumber, Value : HorseController(Monobehaviour Component)")]
        protected Dictionary<int, HorseController> horsesByNumber = new Dictionary<int, HorseController>();
        [Tooltip("Key: RacePosition, Value : HorseNumber")]
        protected Dictionary<int, int> horsesInRacePositions = new Dictionary<int, int>();
        [Tooltip("Key: RacePosition, (Value : Item1 = Horse Number , Item2 = Horse Transform)")]
        protected Dictionary<int, (int, Transform)> horsesTransformInRaceOrder = new Dictionary<int, (int, Transform)>();
        protected List<int> horsesInRaceFinishOrder = new List<int>();
        #endregion

        protected bool areRacePositionsCalculating;
        private List<float> horseFinishLineAccelerationValues = new List<float>();
        private List<int> finishLineSplinePattern = new List<int> { 0, -1, 1 };

        #region Properties
        public virtual HorseController CurrentOvertakingHorse { get; set; }
        #endregion

        #region Unity Methods
        protected virtual void OnEnable()
        {
            EventManager.Instance.OnRaceStartEvent += StartRace;
            EventManager.Instance.OnCrossFinishLineEvent += FinishLineCrossed;
        }
        protected virtual void OnDisable()
        {
            EventManager.Instance.OnRaceStartEvent -= StartRace;
            EventManager.Instance.OnCrossFinishLineEvent -= FinishLineCrossed;
        }
        protected virtual void FixedUpdate()
        {
            if (isRaceStart)
            {
                UpdateHorseRacePositions();
            }
        }
        #endregion

        public virtual void Initialize(HorseController[] _horses)
        {
            for (int i = 0; i < _horses.Length; i++)
            {
                horsesByNumber.Add(_horses[i].HorseNumber, _horses[i]);
            }
            for (int i = 1; i <= _horses.Length; i++)
            {
                horseFinishLineAccelerationValues.Add(horseSpeedSO.finishRaceAccelerationIncrement + (i * horseSpeedSO.finishRaceAccelerationMultiplier));
            }
        }
        private bool CheckIfHorseNumbersAreSame(int horseNumber1, int horseNumber2)
        {
            return horseNumber1 == horseNumber2;
        }
        private bool IsOtherHorseIsFrontOfCurrentHorse(int currentHorseNumber, int otherHorseNumber)
        {
            //Check dot product of the horse direction and other horse direction
            float3 directionVector = math.normalize(horsesByNumber[otherHorseNumber].transform.position - horsesByNumber[currentHorseNumber].transform.position);
            float dotProduct = math.dot(directionVector, horsesByNumber[currentHorseNumber].transform.forward);
            return dotProduct > 0;
        }

        private bool IsOtherHorseIsInTheRangeOfControlPoint(int otherHorseNumber, int controlPointIndex)
        {
            return horsesByNumber[otherHorseNumber].CurrentControlPointIndex == controlPointIndex ||
                horsesByNumber[otherHorseNumber].CurrentControlPointIndex + 1 == controlPointIndex ||
                horsesByNumber[otherHorseNumber].CurrentControlPointIndex - 1 == controlPointIndex;
        }

        public virtual void ChangeControlPoint(int horseNumber)
        {
            if (horseFinishLineAccelerationValues.Count <= 0)
            {
                return;
            }
            if (horseSpeedSO.finishRaceControlPointIndex <= horsesByNumber[horseNumber].CurrentControlPointIndex)
            {
                //Set Finish Line Spline Pattern
                bool isCollided = true;

                for (int i = 0; i < finishLineSplinePattern.Count; i++)
                {
                    int splineIndex = horsesByNumber[horseNumber].CurrentSplineIndex + finishLineSplinePattern[0];

                    if (splineIndex > 0 && splineIndex <= horseSplineManager.TotalSplinesCount)
                    {
                        List<int> horses = horseSplineManager.GetHorsesCurrentlyInSpline(splineIndex);
                        horses.AddRange(horseSplineManager.GetIncomingHorsesInSpline(splineIndex));
                        foreach (var otherhorseNumber in horses)
                        {
                            if (!CheckIfHorseNumbersAreSame(horseNumber, otherhorseNumber)
                                && IsOtherHorseIsFrontOfCurrentHorse(horseNumber, otherhorseNumber))
                            {
                                isCollided = CheckCollisionWithOtherHorse(horseNumber, otherhorseNumber, splineIndex, horsesByNumber[horseNumber].CurrentSplineIndex, 0);
                                if (!isCollided)
                                {
                                    if (horsesByNumber[horseNumber].CurrentSplineIndex != splineIndex)
                                    {
                                        horsesByNumber[horseNumber].SetSpline(horseSplineManager.GetSplineData(horsesByNumber[horseNumber].CurrentSplineIndex, splineIndex, horsesByNumber[horseNumber].CurrentControlPointIndex));
                                    }
                                    break;
                                }
                            }
                        }
                        if (!isCollided)
                            break;
                    }
                }
                horsesByNumber[horseNumber].SetSpeed(0, horseFinishLineAccelerationValues[0]);
                horseFinishLineAccelerationValues.RemoveAt(0);
                return;
            }

            //Get All the variables 
            HorseController horse = horsesByNumber[horseNumber];
            int currentSplineIndex = horse.CurrentSplineIndex;
            int leftSplineIndex = currentSplineIndex - 1;
            int rightSplineIndex = currentSplineIndex + 1;
            int controlPointIndex = horse.CurrentControlPointIndex;
            Direction direction = horseSplineManager.GetNextDirection(controlPointIndex);
            bool isCollisionWithLeftHorse = false;
            bool isCollisionWithRightHorse = false;
            bool isCollisionWithFrontHorse = false;
            bool isCollisionWithIncomingHorse = false;
            float targetSpeed = Utils.GenerateRandomNumber(horseSpeedSO.targetSpeedRange.x, horseSpeedSO.targetSpeedRange.y);

            if (direction == Direction.Left)
            {
                //Check for incoming collisions
                List<int> horsesIncomingIntoCurrentSpline = new List<int>(horseSplineManager.GetIncomingHorsesInSpline(currentSplineIndex));
                foreach (var otherHorseNumber in horsesIncomingIntoCurrentSpline)
                {
                    if (!CheckIfHorseNumbersAreSame(horseNumber, otherHorseNumber) &&
                        IsOtherHorseIsInTheRangeOfControlPoint(otherHorseNumber, controlPointIndex))
                    {
                        isCollisionWithIncomingHorse = CheckCollisionWithOtherHorse(horseNumber, otherHorseNumber, currentSplineIndex, currentSplineIndex, targetSpeed);
                        if (isCollisionWithIncomingHorse)
                        {
                            // Decrease the speed of the horse to avoid collision with the other horse.
                            targetSpeed = horse.TargetSpeed;
                            break;
                        }
                    }
                }

                if (isCollisionWithIncomingHorse || direction == Direction.None || leftSplineIndex <= 0)
                {
                    isCollisionWithLeftHorse = true;
                }

                if (isCollisionWithIncomingHorse || direction == Direction.None || rightSplineIndex >= horseSplineManager.TotalSplinesCount)
                {
                    isCollisionWithRightHorse = true;
                }

                //Check Collisions
                if (!isCollisionWithLeftHorse)
                {
                    if (direction == Direction.Left && leftSplineIndex > 0)
                    {
                        HandleSplineChange(horseNumber, leftSplineIndex, currentSplineIndex, controlPointIndex, targetSpeed, ref isCollisionWithLeftHorse);
                    }
                }

                if (isCollisionWithLeftHorse)
                {
                    //Check Collisions with front horse.
                    List<int> horsesInFrontOfCurrentHorse = new List<int>(horseSplineManager.GetHorsesCurrentlyInSpline(currentSplineIndex));
                    foreach (var otherHorseNumber in horsesInFrontOfCurrentHorse)
                    {
                        if (!CheckIfHorseNumbersAreSame(horseNumber, otherHorseNumber)
                       && IsOtherHorseIsInTheRangeOfControlPoint(otherHorseNumber, controlPointIndex)
                       && IsOtherHorseIsFrontOfCurrentHorse(horseNumber, otherHorseNumber))

                        {
                            isCollisionWithFrontHorse = CheckCollisionWithOtherHorse(horseNumber, otherHorseNumber, currentSplineIndex, currentSplineIndex, targetSpeed);
                            //  Debug.Log($"Front Collision:{isCollisionWithFrontHorse} {horseNumber} collides with {otherHorseNumber}");
                            if (isCollisionWithFrontHorse)
                            {
                                targetSpeed = horseSpeedSO.targetSpeedRange.x;
                                break;
                            }
                        }
                    }

                }

                //Check Collisions with right horse.
                if (!isCollisionWithRightHorse && isCollisionWithFrontHorse)
                {
                    targetSpeed = Utils.GenerateRandomNumber(horseSpeedSO.targetSpeedRange.x, horseSpeedSO.targetSpeedRange.y);

                    HandleSplineChange(horseNumber, rightSplineIndex, currentSplineIndex, controlPointIndex, targetSpeed, ref isCollisionWithRightHorse);

                    if (isCollisionWithRightHorse)
                    {
                        targetSpeed = horseSpeedSO.targetSpeedRange.x;
                    }
                }
            }
            else if (direction == Direction.Right)
            {
                //Check for incoming collisions
                List<int> horsesIncomingIntoCurrentSpline = new List<int>(horseSplineManager.GetIncomingHorsesInSpline(currentSplineIndex));
                foreach (var otherHorseNumber in horsesIncomingIntoCurrentSpline)
                {
                    if (!CheckIfHorseNumbersAreSame(horseNumber, otherHorseNumber) &&
                        IsOtherHorseIsInTheRangeOfControlPoint(otherHorseNumber, controlPointIndex))
                    {
                        isCollisionWithIncomingHorse = CheckCollisionWithOtherHorse(horseNumber, otherHorseNumber, currentSplineIndex, currentSplineIndex, targetSpeed);
                        if (isCollisionWithIncomingHorse)
                        {
                            // Decrease the speed of the horse to avoid collision with the other horse.
                            targetSpeed = horse.TargetSpeed;
                            break;
                        }
                    }
                }
                if (isCollisionWithIncomingHorse || direction == Direction.None || leftSplineIndex <= 0)
                {
                    isCollisionWithLeftHorse = true;
                }

                if (isCollisionWithIncomingHorse || direction == Direction.None || rightSplineIndex >= horseSplineManager.TotalSplinesCount)
                {
                    isCollisionWithRightHorse = true;
                }
                //Check Collisions with right Horses
                if (!isCollisionWithRightHorse)
                {
                    if (direction == Direction.Right && rightSplineIndex <= horseSplineManager.TotalSplinesCount)
                    {
                        HandleSplineChange(horseNumber, rightSplineIndex, currentSplineIndex, controlPointIndex, targetSpeed, ref isCollisionWithLeftHorse);
                    }
                }


                if (isCollisionWithRightHorse)
                {
                    //Check Collisions with front horse.
                    List<int> horsesInFrontOfCurrentHorse = new List<int>(horseSplineManager.GetHorsesCurrentlyInSpline(currentSplineIndex));
                    foreach (var otherHorseNumber in horsesInFrontOfCurrentHorse)
                    {
                        if (!CheckIfHorseNumbersAreSame(horseNumber, otherHorseNumber)
                       && IsOtherHorseIsInTheRangeOfControlPoint(otherHorseNumber, controlPointIndex)
                       && IsOtherHorseIsFrontOfCurrentHorse(horseNumber, otherHorseNumber))

                        {
                            isCollisionWithFrontHorse = CheckCollisionWithOtherHorse(horseNumber, otherHorseNumber, currentSplineIndex, currentSplineIndex, targetSpeed);
                            if (isCollisionWithFrontHorse)
                            {
                                //  Debug.Log($"Front Collision: {horseNumber} collides with {otherHorseNumber}");
                                targetSpeed = horseSpeedSO.targetSpeedRange.x;
                                break;
                            }
                        }
                    }

                }

                //Check Collisions with left horse.
                if (!isCollisionWithLeftHorse && isCollisionWithFrontHorse)
                {
                    targetSpeed = Utils.GenerateRandomNumber(horseSpeedSO.targetSpeedRange.x, horseSpeedSO.targetSpeedRange.y);

                    HandleSplineChange(horseNumber, leftSplineIndex, currentSplineIndex, controlPointIndex, targetSpeed, ref isCollisionWithRightHorse);

                    if (isCollisionWithLeftHorse)
                    {
                        targetSpeed = horseSpeedSO.targetSpeedRange.x;
                    }
                }

            }

            horsesByNumber[horseNumber].SetSpeed(targetSpeed, horseSpeedSO.acceleration);
            CalculateRacePositions();
        }

        protected void CalculateRacePositions()
        {
            if (areRacePositionsCalculating)
                return;

            areRacePositionsCalculating = true;
            List<KeyValuePair<int, float>> racePositionCalculator = new List<KeyValuePair<int, float>>(horsesByNumber.Count);

            foreach (var horse in horsesByNumber.Values)
            {
                racePositionCalculator.Add(new KeyValuePair<int, float>(horse.HorseNumber, horseSplineManager.GetDistanceCoveredAtSplinePoint(horse.CurrentSplinePointIndex)));
            }

            // Sort using a custom comparer to avoid LINQ overhead
            racePositionCalculator.Sort((x, y) => y.Value.CompareTo(x.Value));

            // SetRacePositions
            for (int i = 0; i < racePositionCalculator.Count; i++)
            {
                int horseNumber = racePositionCalculator[i].Key;
                int racePosition = i + 1;
                horsesInRacePositions[racePosition] = horseNumber;
                horsesByNumber[horseNumber].SetRacePosition(racePosition);
            }

            areRacePositionsCalculating = false;
        }

        private void HandleSplineChange(int horseNumber, int targetSplineIndex, int currentSplineIndex, int controlPointIndex, float targetSpeed, ref bool isCollisionDetected)
        {
            foreach (var otherHorseNumber in horsesByNumber.Keys)
            {
                if (!CheckIfHorseNumbersAreSame(horseNumber, otherHorseNumber)
                   && IsOtherHorseIsInTheRangeOfControlPoint(otherHorseNumber, controlPointIndex)
                    && horseSplineManager.GetSplineHorses(targetSplineIndex).Contains(otherHorseNumber))
                {
                    isCollisionDetected = CheckCollisionWithOtherHorse(horseNumber, otherHorseNumber, targetSplineIndex, currentSplineIndex, targetSpeed);
                    //  Debug.Log($"Side Collision:{isCollisionDetected} {horseNumber} collides with {otherHorseNumber}");
                    if (isCollisionDetected)
                    {
                        break;
                    }
                }
            }

            if (!isCollisionDetected)
            {
                horsesByNumber[horseNumber].SetSpline(horseSplineManager.GetSplineData(currentSplineIndex, targetSplineIndex, controlPointIndex));
            }
        }

        private bool CheckCollisionWithOtherHorse(int HorseNumber, int otherHorseNumber, int nextSplineIndex, int currentSplineIndex, float targetSpeed)
        {
            HorseController horse = horsesByNumber[HorseNumber];
            HorseController otherHorse = horsesByNumber[otherHorseNumber];

            SplineData horseSplineData = (nextSplineIndex == currentSplineIndex) ? horse.CurrentSplineData : horseSplineManager.GetSplineData(currentSplineIndex, nextSplineIndex, horse.CurrentControlPointIndex);
            SplineData otherHorseSplineData = otherHorse.CurrentSplineData;

            // Get horse spline points of control point 
            List<float3> horseSplinePositions = new List<float3>();
            for (int i = horse.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
            {
                horseSplinePositions.Add(horseSplineData.splinePoints[i].position);
            }

            // Get other horse spline points of control point 
            List<float3> otherHorseSplinePositions = new List<float3>();
            for (int i = otherHorse.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
            {
                otherHorseSplinePositions.Add(otherHorseSplineData.splinePoints[i].position);
            }

            //Convert List to NativeArray
            NativeArray<bool> collisionResult = new NativeArray<bool>(1, Allocator.Persistent);
            NativeArray<float3> corners1 = new NativeArray<float3>(8, Allocator.Persistent);
            NativeArray<float3> splinePoints1 = horseSplinePositions.ToNativeArray(Allocator.Persistent);

            NativeArray<float3> corners2 = new NativeArray<float3>(8, Allocator.Persistent);
            NativeArray<float3> splinePoints2 = otherHorseSplinePositions.ToNativeArray(Allocator.Persistent);

            //NativeList<float3> positions1 = new NativeList<float3>(Allocator.Persistent);
            //NativeList<quaternion> quaternions1 = new NativeList<quaternion>(Allocator.Persistent);
            //NativeList<float3> positions2 = new NativeList<float3>(Allocator.Persistent);
            //NativeList<quaternion> quaternions2 = new NativeList<quaternion>(Allocator.Persistent);

            //Schedule Job
            HorseBoundingBoxCalculationJob horseBoundingBoxCalculationJob = new HorseBoundingBoxCalculationJob
            {
                deltaTime = Time.fixedDeltaTime,
                isColliding = collisionResult,
                maxSpeed = horse.MaxSpeed,
                acceleration = horse.Acceleration,
                thresholdDistance = horseSplineManager.ChangeThresholdDistance,

                //Horse 1
                splinePoints1 = splinePoints1,
                corners1 = corners1,
                extents1 = horseSplineManager.Extents,
                lastPosition1 = horse.ColliderTransform.position,
                lastRotation1 = horse.ColliderTransform.rotation,
                currentSpeed1 = horse.CurrentSpeed,
                targetSpeed1 = targetSpeed,

                //Horse 2
                splinePoints2 = splinePoints2,
                corners2 = corners2,
                extents2 = horseSplineManager.Extents,
                lastPosition2 = otherHorse.ColliderTransform.position,
                lastRotation2 = otherHorse.ColliderTransform.rotation,
                currentSpeed2 = otherHorse.CurrentSpeed,
                targetSpeed2 = otherHorse.TargetSpeed,

                //Debug Variables
                //positions1 = positions1,
                //quaternions1 = quaternions1,
                //positions2 = positions2,
                //quaternions2 = quaternions2,
            };
            JobHandle jobHandle = horseBoundingBoxCalculationJob.Schedule();
            jobHandle.Complete();

            //Dispose Everything
            splinePoints1.Dispose();
            splinePoints2.Dispose();
            corners1.Dispose();
            corners2.Dispose();

            //if (!isVisualize)
            //{
            //    StartCoroutine(VisualizeBoundingBoxes(positions1, quaternions1, positions2, quaternions2, horseSplineManager.Extents));
            //    isVisualize = true;
            //}

            bool isCollided = collisionResult[0];
            collisionResult.Dispose();
            return isCollided;
        }
        public void HorseReachedSpline(int splineIndex, int previousSplineINdex, int horseNumber)
        {
            horseSplineManager.HorseReachedSpline(splineIndex, previousSplineINdex, horseNumber);
        }
        public void HorseChangingSpline(int currentSplineArrayIndex, int nextSplineArrayIndex, int horseNumber)
        {
            horseSplineManager.HorseChangingSpline(currentSplineArrayIndex, nextSplineArrayIndex, horseNumber);
        }

        #region Race Position Tracking
        protected virtual void UpdateHorseRacePositions()
        {
        }
        #endregion

        #region Horse Transforms
        public Dictionary<int, (int, Transform)> HorseTransformsInRaceOrder()
        {
            return horsesTransformInRaceOrder.Count > 0 ? horsesTransformInRaceOrder : new Dictionary<int, (int, Transform)>();
        }
        public virtual Transform RaceWinnerTransform()
        {
            return horsesByNumber[horsesInRaceFinishOrder[0]].transform;
        }
        #endregion

        #region Race Start/Finish Methods
        protected virtual void StartRace()
        {
            isRaceStart = true;
        }
        protected virtual void FinishLineCrossed(int _horseNumber)
        {
            horsesInRaceFinishOrder.Add(_horseNumber);
            horsesByNumber[_horseNumber].FinishLineCrossed();

            //Return True, if all Horses Crossed Finish Line
            if (horsesInRaceFinishOrder.Count >= horsesByNumber.Count)
            {
                RaceFinished();
            }
        }
        protected virtual void RaceFinished()
        {
            EventManager.Instance.RaceFinished();
        }
        #endregion
    }
}

[BurstCompile]
public struct HorseBoundingBoxCalculationJob : IJob
{
    public NativeArray<bool> isColliding;
    [ReadOnly] public float deltaTime;
    [ReadOnly] public float thresholdDistance;
    [ReadOnly] public float maxSpeed;
    [ReadOnly] public float acceleration;

    //Horse 1 
    public NativeArray<float3> splinePoints1;
    public NativeArray<float3> corners1;
    public float3 extents1;
    public float3 lastPosition1;
    public quaternion lastRotation1;
    public float currentSpeed1;
    public float targetSpeed1;

    //Horse 2
    public NativeArray<float3> splinePoints2;
    public NativeArray<float3> corners2;
    public float3 extents2;
    public float3 lastPosition2;
    public quaternion lastRotation2;
    public float currentSpeed2;
    public float targetSpeed2;

    //Debug 
    //public NativeList<float3> positions1;
    //public NativeList<quaternion> quaternions1;
    //public NativeList<float3> positions2;
    //public NativeList<quaternion> quaternions2;

    public void Execute()
    {
        int splinePointIndex1 = 0;
        int splinePointIndex2 = 0;
        isColliding[0] = false;

        while (splinePointIndex1 < splinePoints1.Length && splinePointIndex2 < splinePoints2.Length)
        {
            //If both horses speed is zero, then break the loop
            if ((currentSpeed1 <= 0 && currentSpeed2 <= 0))
            {
                break;
            }
            // Horse 1
            if (splinePointIndex1 < splinePoints1.Length)
            {
                float speedChange1 = acceleration * deltaTime;
                currentSpeed1 = currentSpeed1 < targetSpeed1 ? math.clamp(currentSpeed1 + speedChange1, 0, targetSpeed1) : math.clamp(currentSpeed1 - speedChange1, targetSpeed1, maxSpeed);
                float3 targetPosition1 = splinePoints1[splinePointIndex1];
                float distance1 = math.distance(lastPosition1, targetPosition1);
                float timeToReach1 = distance1 / currentSpeed1;

                if (timeToReach1 > 0)
                {
                    // Update Position
                    float3 direction = math.normalize(targetPosition1 - lastPosition1);
                    float3 newPosition = lastPosition1 + direction * currentSpeed1 * deltaTime;
                    lastPosition1 = newPosition;
                    // positions1.Add(newPosition);

                    // Apply rotation to bounding box 1
                    quaternion targetRotation1 = quaternion.LookRotationSafe(direction, new float3(0, 1, 0));
                    quaternion smoothRotation1 = math.slerp(lastRotation1, targetRotation1, deltaTime * currentSpeed1);
                    float3x3 rotationMatrix1 = new float3x3(smoothRotation1);
                    lastRotation1 = smoothRotation1;
                    // quaternions1.Add(smoothRotation1);


                    // Calculate the corners of bounding box 1
                    float3 newCenter1 = newPosition;
                    NativeArray<float3> localCorners1 = new NativeArray<float3>(8, Allocator.Temp)
                    {
                        [0] = new float3(-extents1.x, -extents1.y, -extents1.z),
                        [1] = new float3(extents1.x, -extents1.y, -extents1.z),
                        [2] = new float3(-extents1.x, extents1.y, -extents1.z),
                        [3] = new float3(extents1.x, extents1.y, -extents1.z),
                        [4] = new float3(-extents1.x, -extents1.y, extents1.z),
                        [5] = new float3(extents1.x, -extents1.y, extents1.z),
                        [6] = new float3(-extents1.x, extents1.y, extents1.z),
                        [7] = new float3(extents1.x, extents1.y, extents1.z)
                    };
                    for (int i = 0; i < 8; i++)
                    {
                        corners1[i] = math.mul(rotationMatrix1, localCorners1[i]) + newCenter1;
                    }
                }
                if (distance1 < thresholdDistance)
                {
                    splinePointIndex1++;
                    if (splinePointIndex1 + 1 == splinePoints1.Length)
                    {
                        thresholdDistance = 0.3f;
                    }
                }
            }

            // Horse 2
            if (splinePointIndex2 < splinePoints2.Length)
            {
                float speedChange2 = acceleration * deltaTime;
                currentSpeed2 = currentSpeed2 < targetSpeed2 ? math.clamp(currentSpeed2 + speedChange2, 0, targetSpeed2) : math.clamp(currentSpeed2 - speedChange2, targetSpeed2, maxSpeed);
                float3 targetPosition2 = splinePoints2[splinePointIndex2];
                float distance2 = math.distance(lastPosition2, targetPosition2);
                float timeToReach2 = distance2 / currentSpeed2;

                if (timeToReach2 > 0)
                {
                    // Update Position
                    float3 direction = math.normalize(targetPosition2 - lastPosition2);
                    float3 newPosition = lastPosition2 + direction * currentSpeed2 * deltaTime;
                    lastPosition2 = newPosition;
                    // positions2.Add(newPosition);

                    // Apply rotation to bounding box 2
                    quaternion targetRotation2 = quaternion.LookRotationSafe(direction, new float3(0, 1, 0));
                    quaternion smoothRotation2 = math.slerp(lastRotation2, targetRotation2, deltaTime * currentSpeed2);
                    float3x3 rotationMatrix2 = new float3x3(smoothRotation2);
                    lastRotation2 = smoothRotation2;
                    // quaternions2.Add(smoothRotation2);

                    // Calculate the corners of bounding box 2
                    float3 newCenter2 = newPosition;
                    NativeArray<float3> localCorners2 = new NativeArray<float3>(8, Allocator.Temp)
                    {
                        [0] = new float3(-extents2.x, -extents2.y, -extents2.z),
                        [1] = new float3(extents2.x, -extents2.y, -extents2.z),
                        [2] = new float3(-extents2.x, extents2.y, -extents2.z),
                        [3] = new float3(extents2.x, extents2.y, -extents2.z),
                        [4] = new float3(-extents2.x, -extents2.y, extents2.z),
                        [5] = new float3(extents2.x, -extents2.y, extents2.z),
                        [6] = new float3(-extents2.x, extents2.y, extents2.z),
                        [7] = new float3(extents2.x, extents2.y, extents2.z)
                    };
                    for (int i = 0; i < 8; i++)
                    {
                        corners2[i] = math.mul(rotationMatrix2, localCorners2[i]) + newCenter2;
                    }
                }
                if (distance2 < thresholdDistance)
                {
                    splinePointIndex2++;
                    if (splinePointIndex1 + 1 == splinePoints1.Length)
                    {
                        thresholdDistance = 0.3f;
                    }
                }
            }
            // Check for collision
            isColliding[0] = AreBoundingBoxesColliding(corners1, corners2);
            if (isColliding[0] == true)
            {
                break;
            }
        }
    }


    private bool AreBoundingBoxesColliding(NativeArray<float3> corners1, NativeArray<float3> corners2)
    {
        // Get the axes to test (normals of the faces of the bounding boxes)
        NativeArray<float3> axes = new NativeArray<float3>(15, Allocator.Temp);
        axes[0] = math.normalize(corners1[1] - corners1[0]);
        axes[1] = math.normalize(corners1[2] - corners1[0]);
        axes[2] = math.normalize(corners1[4] - corners1[0]);
        axes[3] = math.normalize(corners2[1] - corners2[0]);
        axes[4] = math.normalize(corners2[2] - corners2[0]);
        axes[5] = math.normalize(corners2[4] - corners2[0]);

        // Cross products of edges to get additional axes
        int index = 6;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                axes[index++] = math.normalize(math.cross(axes[i], axes[j]));
            }
        }

        // Check for overlap on all axes
        foreach (float3 axis in axes)
        {
            if (!IsOverlappingOnAxis(corners1, corners2, axis))
            {
                return false; // Separating axis found, no collision
            }
        }

        return true; // No separating axis found, collision detected
    }

    private bool IsOverlappingOnAxis(NativeArray<float3> corners1, NativeArray<float3> corners2, float3 axis)
    {
        // Project all corners onto the axis and find the min and max values
        float min1 = float.MaxValue, max1 = float.MinValue;
        float min2 = float.MaxValue, max2 = float.MinValue;

        for (int i = 0; i < 8; i++)
        {
            float projection1 = math.dot(corners1[i], axis);
            min1 = math.min(min1, projection1);
            max1 = math.max(max1, projection1);

            float projection2 = math.dot(corners2[i], axis);
            min2 = math.min(min2, projection2);
            max2 = math.max(max2, projection2);
        }

        // Check for overlap
        return max1 >= min2 && max2 >= min1;
    }
}