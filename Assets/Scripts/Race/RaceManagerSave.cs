using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HorseRace
{
    public class RaceManagerSave : RaceManager
    {
        #region Inspector Variables
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private PreWinnerManager preWinnerManager;
        [SerializeField] private int startOvertakeControlPointOffset;
        [SerializeField] private int endOvertakeControlPointOffset;
        [SerializeField] private float overtakeCheckTime = 3f;
        [SerializeField] private int minHorsesOvertake = 3;
        #endregion

        #region Private Variables
        private HorseController preWinnerHorse;
        private int preWinnerHorseNumber;
        private int speedGenerationCount = 10;
        private int preWinnerTargetRacePosition = -1;
        private bool isRaceFinish = false;
        private List<float> slowSpeedsList = new List<float>();
        private List<float> fastSpeedsList = new List<float>();
        private List<float> horseFinishLineAccelerationValues = new List<float>();
        private List<int> finishLineSplinePattern = new List<int> { 0, -1, 1, 2, -2, 3, -3 };
        #endregion

        public bool debugCollision;

        #region Unity Methods
        private void Awake()
        {
            GameManager.Instance.SetRaceManager(this);
            GenerateFastSpeedsList(speedGenerationCount);
            GenerateSlowSpeedsList(speedGenerationCount);
        }


        protected override void FixedUpdate()
        {
            if (!isRaceStart)
            {
                return;
            }
            if (isRaceStart)
            {
                foreach (var item in horsesByNumber.Values)
                {
                    item.UpdateState();
                    if (item.IsControlPointChange == true)
                    {
                        if (Time.timeScale != 0)
                        {
                            //Stop the Game and make calculation for horse to change the spline.
                            Time.timeScale = 0;
                            StartCoroutine(IEChangeControlPoint(item.HorseNumber));
                            break;
                        }
                    }
                }
                CalculateRacePositions();
            }

            if (isRaceFinish)
            {
                if (horsesByNumber[horsesInRaceFinishOrder[0]].CurrentSpeed <= 0)
                {
                    isRaceFinish = false;
                    //Save Race.
                    //Get Horse Velocity Data to save in json file.
                    List<HorseData> horseDatasList = new List<HorseData>();
                    for (int i = 1; i <= horsesByNumber.Count; i++)
                    {
                        ISaveHorseData savedHorseData = (ISaveHorseData)horsesByNumber[i];
                        horseDatasList.Add(savedHorseData.HorseSaveData());
                    }
                    saveManager.SetPreDeterminedWinner(horsesInRaceFinishOrder[0]);
                    saveManager.SetWinnersList(horsesInRaceFinishOrder);
                    saveManager.SetHorseData(horseDatasList);
                    base.RaceFinished();

                    //Reload Same Active Scene
                    SceneLoadingManager.Instance.ReloadCurrentScene();
                }
            }
        }
        #endregion

        #region Visualize Bounding Boxes

        private void VisualizeBoundingBox(int i, NativeList<float3> positions, NativeList<quaternion> quaternions, float3 extents, Color color)
        {
            if (i >= positions.Length)
            {
                return;
            }

            float3 position = positions[i];
            quaternion rotation = quaternions[i];
            float3x3 rotationMatrix = new float3x3(rotation);

            // Calculate the corners of the bounding box
            NativeArray<float3> localCorners = new NativeArray<float3>(8, Allocator.Temp)
            {
                [0] = new float3(-extents.x, -extents.y, -extents.z),
                [1] = new float3(extents.x, -extents.y, -extents.z),
                [2] = new float3(-extents.x, extents.y, -extents.z),
                [3] = new float3(extents.x, extents.y, -extents.z),
                [4] = new float3(-extents.x, -extents.y, extents.z),
                [5] = new float3(extents.x, -extents.y, extents.z),
                [6] = new float3(-extents.x, extents.y, extents.z),
                [7] = new float3(extents.x, extents.y, extents.z)
            };
            float3[] worldCorners = new float3[8];
            for (int j = 0; j < 8; j++)
            {
                worldCorners[j] = math.mul(rotationMatrix, localCorners[j]) + position;
            }
            // Draw the bounding box
            DrawBoundingBox(worldCorners, color);
            localCorners.Dispose();
        }

        private IEnumerator IEVisualizeBoundingBoxes(HorseSplineCollisionData horseSplineCollisionData, float3 extents)
        {
            int maxCount = horseSplineCollisionData.GetMaxPositionsLength();
            for (int i = 0; i < maxCount; i++)
            {
                VisualizeBoundingBox(i, horseSplineCollisionData.horse1Positions, horseSplineCollisionData.horse1Rotations, extents, Color.green);
                VisualizeBoundingBox(i, horseSplineCollisionData.horse2Positions, horseSplineCollisionData.horse2Rotations, extents, Color.red);
                VisualizeBoundingBox(i, horseSplineCollisionData.horse3Positions, horseSplineCollisionData.horse3Rotations, extents, Color.red);
                VisualizeBoundingBox(i, horseSplineCollisionData.horse4Positions, horseSplineCollisionData.horse4Rotations, extents, Color.red);
                VisualizeBoundingBox(i, horseSplineCollisionData.horse5Positions, horseSplineCollisionData.horse5Rotations, extents, Color.red);
                VisualizeBoundingBox(i, horseSplineCollisionData.horse6Positions, horseSplineCollisionData.horse6Rotations, extents, Color.red);
                VisualizeBoundingBox(i, horseSplineCollisionData.horse7Positions, horseSplineCollisionData.horse7Rotations, extents, Color.red);
                VisualizeBoundingBox(i, horseSplineCollisionData.horse8Positions, horseSplineCollisionData.horse8Rotations, extents, Color.red);
                VisualizeBoundingBox(i, horseSplineCollisionData.horse9Positions, horseSplineCollisionData.horse9Rotations, extents, Color.red);
                VisualizeBoundingBox(i, horseSplineCollisionData.horse10Positions, horseSplineCollisionData.horse10Rotations, extents, Color.red);
                VisualizeBoundingBox(i, horseSplineCollisionData.horse11Positions, horseSplineCollisionData.horse11Rotations, extents, Color.red);
                VisualizeBoundingBox(i, horseSplineCollisionData.horse12Positions, horseSplineCollisionData.horse12Rotations, extents, Color.red);

                yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
            }
            yield return null;
        }

        private void DrawBoundingBox(float3[] corners, Color color)
        {
            Debug.DrawLine(corners[0], corners[1], color);
            Debug.DrawLine(corners[1], corners[3], color);
            Debug.DrawLine(corners[3], corners[2], color);
            Debug.DrawLine(corners[2], corners[0], color);

            Debug.DrawLine(corners[4], corners[5], color);
            Debug.DrawLine(corners[5], corners[7], color);
            Debug.DrawLine(corners[7], corners[6], color);
            Debug.DrawLine(corners[6], corners[4], color);

            Debug.DrawLine(corners[0], corners[4], color);
            Debug.DrawLine(corners[1], corners[5], color);
            Debug.DrawLine(corners[2], corners[6], color);
            Debug.DrawLine(corners[3], corners[7], color);
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize Race Setup.
        /// </summary>
        /// <param name="_horses"></param>
        public override void Initialize(HorseController[] _horses)
        {
            base.Initialize(_horses);
            OnSetPreWinner();
            EventManager.Instance.OnCameraSetup();
            for (int i = 1; i <= _horses.Length; i++)
            {
                horseFinishLineAccelerationValues.Add(horseSpeedSO.finishRaceAccelerationIncrement + (i * horseSpeedSO.finishRaceAccelerationMultiplier));
                if (_horses[i - 1] is HorseControllerSave horseControllerSave)
                {
                    horseControllerSave.SetOvertakeControlPointsData(horseSplineManager.TotalSplinePointsCount, startOvertakeControlPointOffset, endOvertakeControlPointOffset, minHorsesOvertake, overtakeCheckTime);
                }
            }
        }
        /// <summary>
        /// Set prewinner horse for the current race.
        /// </summary>
        private void OnSetPreWinner()
        {
            int randomIndex = Utils.GenerateRandomNumber(0, horsesByNumber.Count);
            preWinnerHorseNumber = horsesByNumber.ElementAt(randomIndex).Key;
            preWinnerHorse = horsesByNumber[preWinnerHorseNumber];
            preWinnerManager.SetPreWinner(preWinnerHorse);
        }
        #endregion

        #region Race Start/Finished Methods
        protected override void StartRace()
        {
            //Initialize Horses
            foreach (var item in horsesByNumber.Values)
            {
                float initialAccelerationSpeed = Utils.GenerateRandomNumber(horseSpeedSO.initialAccelerationRange.x, horseSpeedSO.initialAccelerationRange.y);
                float speed = Utils.GenerateRandomNumber(horseSpeedSO.targetSpeedRange.x, horseSpeedSO.targetSpeedRange.y);
                SplineData splineData = horseSplineManager.InitializeSpline(item.HorseNumber, item.HorseNumber);
                item.InitializeData(splineData, speed, horseSpeedSO.maxSpeed, initialAccelerationSpeed, horseSplineManager.ChangeThresholdDistance);
            }
            base.StartRace();
        }

        protected override void RaceFinished()
        {
            isRaceFinish = true;
        }
        #endregion

        #region Speed
        private float GetRandomSlowSpeed()
        {
            int randomIndex = Utils.GenerateRandomNumber(0, slowSpeedsList.Count);
            return slowSpeedsList[randomIndex];
        }
        private float GetRandomHighSpeed()
        {
            int randomIndex = Utils.GenerateRandomNumber(0, fastSpeedsList.Count);
            return fastSpeedsList[randomIndex];
        }
        private void GenerateSlowSpeedsList(int count)
        {
            slowSpeedsList.Clear();
            for (int i = 0; i < count; i++)
            {
                float speed = Utils.GenerateRandomNumber(horseSpeedSO.targetSpeedRange.x, horseSpeedSO.targetSpeedRange.y);
                //Decrase 25 percent of the speed
                speed = speed - (speed * 0.10f);
                slowSpeedsList.Add(speed);
            }
            slowSpeedsList = slowSpeedsList.OrderBy(x => x).ToList();
        }
        private void GenerateFastSpeedsList(int count)
        {
            fastSpeedsList.Clear();
            for (int i = 0; i < count; i++)
            {
                float speed = Utils.GenerateRandomNumber(horseSpeedSO.targetSpeedRange.x, horseSpeedSO.targetSpeedRange.y);
                //Increase 25 percent of the speed
                speed = speed + (speed * 0.10f);
                fastSpeedsList.Add(speed);
            }
            fastSpeedsList = fastSpeedsList.OrderBy(x => x).ToList();
        }

        private float GetSpeed(int HorseNumber)
        {
            float speed = Utils.GenerateRandomNumber(horseSpeedSO.targetSpeedRange.x, horseSpeedSO.targetSpeedRange.y);
            if (preWinnerTargetRacePosition == -1 || preWinnerTargetRacePosition == horsesByNumber[preWinnerHorseNumber].RacePosition)
            {
                return speed;
            }
            if (IsPreWinnerHorse(HorseNumber))
            {
                if (IsHorsePositionAhead(preWinnerHorse.RacePosition, preWinnerTargetRacePosition))
                {
                    speed = GetRandomSlowSpeed();
                }
                else
                {
                    speed = GetRandomHighSpeed();
                }
            }
            //Check if the Normal horse is in the preWinner horse's target race position.
            else
            {
                int currentPosition = horsesByNumber[HorseNumber].RacePosition;
                int startPosition = preWinnerHorse.RacePosition + 1;

                if ((currentPosition >= startPosition && currentPosition <= preWinnerTargetRacePosition) ||
                    (currentPosition < startPosition && currentPosition >= preWinnerTargetRacePosition))
                {
                    if (IsHorsePositionAhead(horsesByNumber[HorseNumber].RacePosition, preWinnerTargetRacePosition))
                    {
                        speed = GetRandomHighSpeed();
                    }
                    else
                    {
                        speed = GetRandomSlowSpeed();
                    }
                }
            }
            return speed;
        }
        #endregion

        #region Splines 
        IEnumerator IEChangeControlPoint(int horseNumber)
        {
            int targetLeftSpline = horsesByNumber[horseNumber].CurrentSplineIndex - 2;
            if (targetLeftSpline <= 0)
            {

                //Set Horse Spline and Speed
                horsesByNumber[horseNumber].SetSpeed(horsesByNumber[horseNumber].CurrentSpeed, horsesByNumber[horseNumber].Acceleration);
                horsesByNumber[horseNumber].OnControlPointChangeSuccessful();
                yield return null;
            }
            else
            {
                yield return StartCoroutine(IECheckCollisionWithOtherHorses(horseNumber, horsesByNumber[horseNumber].CurrentSplineIndex, targetLeftSpline, horsesByNumber[horseNumber].TargetSpeed));
            }

            //Resume Game
            Time.timeScale = 1;
        }

        IEnumerator IECheckCollisionWithOtherHorses(int HorseNumber, int currentSplineIndex, int nextSplineIndex, float targetSpeed)
        {
            List<int> horsesList = horsesByNumber.Keys.ToList();

            #region Horse Spline Data

            //Current HorseData
            horsesList.Remove(HorseNumber);
            HorseController horse = horsesByNumber[HorseNumber];
            SplineData horseSplineData = (nextSplineIndex == currentSplineIndex) ? horse.CurrentSplineData : horseSplineManager.GetSplineData(currentSplineIndex, nextSplineIndex, horse.CurrentControlPointIndex);
            // Get horse spline points of control point 
            List<float3> horseSplinePositions = new List<float3>();
            for (int i = horse.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
            {
                horseSplinePositions.Add(horseSplineData.splinePoints[i].position);
            }

            //Other Horse - 2 Data
            int horse2Number = horsesList[0];
            horsesList.Remove(horse2Number);
            HorseController horse2 = horsesByNumber[horse2Number];
            List<float3> otherHorseSplinePositions = new List<float3>();
            bool isOtherHorseInRange2 = IsOtherHorseIsInTheRangeOfControlPoint(horse2Number, horse.CurrentControlPointIndex);
            float horse2CurrentSpeed = isOtherHorseInRange2 ? horse2.CurrentSpeed : 0;
            float horse2TargetSpeed = isOtherHorseInRange2 ? horse2.TargetSpeed : 0;
            if (isOtherHorseInRange2)
            {
                for (int i = horse2.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
                {
                    otherHorseSplinePositions.Add(horse2.CurrentSplineData.splinePoints[i].position);
                }
            }

            //Other Horse - 3 Data
            int horse3Number = horsesList[0];
            horsesList.Remove(horse3Number);
            HorseController horse3 = horsesByNumber[horse3Number];
            List<float3> otherHorseSplinePositions2 = new List<float3>();
            bool isOtherHorseInRange3 = IsOtherHorseIsInTheRangeOfControlPoint(horse3Number, horse.CurrentControlPointIndex);
            float horse3CurrentSpeed = isOtherHorseInRange3 ? horse3.CurrentSpeed : 0;
            float horse3TargetSpeed = isOtherHorseInRange3 ? horse3.TargetSpeed : 0;
            if (isOtherHorseInRange3)
                for (int i = horse3.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
                {
                    otherHorseSplinePositions2.Add(horse3.CurrentSplineData.splinePoints[i].position);
                }

            //Other Horse - 4 Data
            int horse4Number = horsesList[0];
            horsesList.Remove(horse4Number);
            HorseController horse4 = horsesByNumber[horse4Number];
            List<float3> otherHorseSplinePositions3 = new List<float3>();
            bool isOtherHorseInRange4 = IsOtherHorseIsInTheRangeOfControlPoint(horse4Number, horse.CurrentControlPointIndex);
            float horse4CurrentSpeed = isOtherHorseInRange4 ? horse4.CurrentSpeed : 0;
            float horse4TargetSpeed = isOtherHorseInRange4 ? horse4.TargetSpeed : 0;
            if (isOtherHorseInRange4)
                for (int i = horse4.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
                {
                    otherHorseSplinePositions3.Add(horse4.CurrentSplineData.splinePoints[i].position);
                }

            // Other Horse - 5 Data
            int horse5Number = horsesList[0];
            horsesList.Remove(horse5Number);
            HorseController horse5 = horsesByNumber[horse5Number];
            List<float3> otherHorseSplinePositions4 = new List<float3>();
            bool isOtherHorseInRange5 = IsOtherHorseIsInTheRangeOfControlPoint(horse5Number, horse.CurrentControlPointIndex);
            float horse5CurrentSpeed = isOtherHorseInRange5 ? horse5.CurrentSpeed : 0;
            float horse5TargetSpeed = isOtherHorseInRange5 ? horse5.TargetSpeed : 0;
            if (isOtherHorseInRange5)
                for (int i = horse5.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
                {
                    otherHorseSplinePositions4.Add(horse5.CurrentSplineData.splinePoints[i].position);
                }

            // Other Horse - 6 Data
            int horse6Number = horsesList[0];
            horsesList.Remove(horse6Number);
            HorseController horse6 = horsesByNumber[horse6Number];
            List<float3> otherHorseSplinePositions5 = new List<float3>();
            bool isOtherHorseInRange6 = IsOtherHorseIsInTheRangeOfControlPoint(horse6Number, horse.CurrentControlPointIndex);
            float horse6CurrentSpeed = isOtherHorseInRange6 ? horse6.CurrentSpeed : 0;
            float horse6TargetSpeed = isOtherHorseInRange6 ? horse6.TargetSpeed : 0;
            if (isOtherHorseInRange6)
                for (int i = horse6.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
                {
                    otherHorseSplinePositions5.Add(horse6.CurrentSplineData.splinePoints[i].position);
                }

            // Other Horse - 7 Data
            int horse7Number = horsesList[0];
            horsesList.Remove(horse7Number);
            HorseController horse7 = horsesByNumber[horse7Number];
            List<float3> otherHorseSplinePositions6 = new List<float3>();
            bool isOtherHorseInRange7 = IsOtherHorseIsInTheRangeOfControlPoint(horse7Number, horse.CurrentControlPointIndex);
            float horse7CurrentSpeed = isOtherHorseInRange7 ? horse7.CurrentSpeed : 0;
            float horse7TargetSpeed = isOtherHorseInRange7 ? horse7.TargetSpeed : 0;
            if (isOtherHorseInRange7)
                for (int i = horse7.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
                {
                    otherHorseSplinePositions6.Add(horse7.CurrentSplineData.splinePoints[i].position);
                }

            // Other Horse - 8 Data
            int horse8Number = horsesList[0];
            horsesList.Remove(horse8Number);
            HorseController horse8 = horsesByNumber[horse8Number];
            List<float3> otherHorseSplinePositions7 = new List<float3>();
            bool isOtherHorseInRange8 = IsOtherHorseIsInTheRangeOfControlPoint(horse8Number, horse.CurrentControlPointIndex);
            float horse8CurrentSpeed = isOtherHorseInRange8 ? horse8.CurrentSpeed : 0;
            float horse8TargetSpeed = isOtherHorseInRange8 ? horse8.TargetSpeed : 0;
            if (isOtherHorseInRange8)
                for (int i = horse8.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
                {
                    otherHorseSplinePositions7.Add(horse8.CurrentSplineData.splinePoints[i].position);
                }

            // Other Horse - 9 Data
            int horse9Number = horsesList[0];
            horsesList.Remove(horse9Number);
            HorseController horse9 = horsesByNumber[horse9Number];
            List<float3> otherHorseSplinePositions8 = new List<float3>();
            bool isOtherHorseInRange9 = IsOtherHorseIsInTheRangeOfControlPoint(horse9Number, horse.CurrentControlPointIndex);
            float horse9CurrentSpeed = isOtherHorseInRange9 ? horse9.CurrentSpeed : 0;
            float horse9TargetSpeed = isOtherHorseInRange9 ? horse9.TargetSpeed : 0;
            if (isOtherHorseInRange9)
                for (int i = horse9.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
                {
                    otherHorseSplinePositions8.Add(horse9.CurrentSplineData.splinePoints[i].position);
                }

            // Other Horse - 10 Data
            int horse10Number = horsesList[0];
            horsesList.Remove(horse10Number);
            HorseController horse10 = horsesByNumber[horse10Number];
            List<float3> otherHorseSplinePositions9 = new List<float3>();
            bool isOtherHorseInRange10 = IsOtherHorseIsInTheRangeOfControlPoint(horse10Number, horse.CurrentControlPointIndex);
            float horse10CurrentSpeed = isOtherHorseInRange10 ? horse10.CurrentSpeed : 0;
            float horse10TargetSpeed = isOtherHorseInRange10 ? horse10.TargetSpeed : 0;
            if (isOtherHorseInRange10)
                for (int i = horse10.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
                {
                    otherHorseSplinePositions9.Add(horse10.CurrentSplineData.splinePoints[i].position);
                }

            // Other Horse - 11 Data
            int horse11Number = horsesList[0];
            horsesList.Remove(horse11Number);
            HorseController horse11 = horsesByNumber[horse11Number];
            List<float3> otherHorseSplinePositions10 = new List<float3>();
            bool isOtherHorseInRange11 = IsOtherHorseIsInTheRangeOfControlPoint(horse11Number, horse.CurrentControlPointIndex);
            float horse11CurrentSpeed = isOtherHorseInRange11 ? horse11.CurrentSpeed : 0;
            float horse11TargetSpeed = isOtherHorseInRange11 ? horse11.TargetSpeed : 0;
            if (isOtherHorseInRange11)
                for (int i = horse11.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
                {
                    otherHorseSplinePositions10.Add(horse11.CurrentSplineData.splinePoints[i].position);
                }

            // Other Horse - 12 Data
            int horse12Number = horsesList[0];
            horsesList.Remove(horse12Number);
            HorseController horse12 = horsesByNumber[horse12Number];
            List<float3> otherHorseSplinePositions11 = new List<float3>();
            bool isOtherHorseInRange12 = IsOtherHorseIsInTheRangeOfControlPoint(horse12Number, horse.CurrentControlPointIndex);
            float horse12CurrentSpeed = isOtherHorseInRange12 ? horse12.CurrentSpeed : 0;
            float horse12TargetSpeed = isOtherHorseInRange12 ? horse12.TargetSpeed : 0;
            if (isOtherHorseInRange12)
                for (int i = horse12.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
                {
                    otherHorseSplinePositions11.Add(horse12.CurrentSplineData.splinePoints[i].position);
                }
            #endregion

            #region Job Collision
            HorseSplineCollisionData horseSplineCollisionData = new HorseSplineCollisionData()
            {
                //Main Horse Collision
                horse1SplinePoints = horseSplinePositions.ToNativeArray(Allocator.Persistent),
                horse1Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse1Positions = new NativeList<float3>(Allocator.Persistent),
                horse1Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse1CurrentPosition = horse.ColliderTransform.position,
                horse1CurrentRotation = horse.ColliderTransform.rotation,
                horse1CurrentSpeed = horse.CurrentSpeed,
                horse1TargetSpeed = targetSpeed,
                horse1SplinePointIndex = 0,

                //Other Horse 2
                horse2SplinePoints = otherHorseSplinePositions.ToNativeArray(Allocator.Persistent),
                horse2Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse2Positions = new NativeList<float3>(Allocator.Persistent),
                horse2Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse2CurrentPosition = horse2.ColliderTransform.position,
                horse2CurrentRotation = horse2.ColliderTransform.rotation,
                horse2CurrentSpeed = horse2CurrentSpeed,
                horse2TargetSpeed = horse2TargetSpeed,
                horse2SplinePointIndex = 0,

                //Other Horse 3
                horse3SplinePoints = otherHorseSplinePositions2.ToNativeArray(Allocator.Persistent),
                horse3Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse3Positions = new NativeList<float3>(Allocator.Persistent),
                horse3Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse3CurrentPosition = horse3.ColliderTransform.position,
                horse3CurrentRotation = horse3.ColliderTransform.rotation,
                horse3CurrentSpeed = horse3CurrentSpeed,
                horse3TargetSpeed = horse3TargetSpeed,
                horse3SplinePointIndex = 0,

                //Other Horse 4
                horse4SplinePoints = otherHorseSplinePositions3.ToNativeArray(Allocator.Persistent),
                horse4Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse4Positions = new NativeList<float3>(Allocator.Persistent),
                horse4Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse4CurrentPosition = horse4.ColliderTransform.position,
                horse4CurrentRotation = horse4.ColliderTransform.rotation,
                horse4CurrentSpeed = horse4CurrentSpeed,
                horse4TargetSpeed = horse4TargetSpeed,
                horse4SplinePointIndex = 0,

                // Other Horse 5
                horse5SplinePoints = otherHorseSplinePositions4.ToNativeArray(Allocator.Persistent),
                horse5Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse5Positions = new NativeList<float3>(Allocator.Persistent),
                horse5Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse5CurrentPosition = horse5.ColliderTransform.position,
                horse5CurrentRotation = horse5.ColliderTransform.rotation,
                horse5CurrentSpeed = horse5CurrentSpeed,
                horse5TargetSpeed = horse5TargetSpeed,
                horse5SplinePointIndex = 0,

                // Other Horse 6
                horse6SplinePoints = otherHorseSplinePositions5.ToNativeArray(Allocator.Persistent),
                horse6Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse6Positions = new NativeList<float3>(Allocator.Persistent),
                horse6Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse6CurrentPosition = horse6.ColliderTransform.position,
                horse6CurrentRotation = horse6.ColliderTransform.rotation,
                horse6CurrentSpeed = horse6CurrentSpeed,
                horse6TargetSpeed = horse6TargetSpeed,
                horse6SplinePointIndex = 0,

                // Other Horse 7
                horse7SplinePoints = otherHorseSplinePositions6.ToNativeArray(Allocator.Persistent),
                horse7Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse7Positions = new NativeList<float3>(Allocator.Persistent),
                horse7Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse7CurrentPosition = horse7.ColliderTransform.position,
                horse7CurrentRotation = horse7.ColliderTransform.rotation,
                horse7CurrentSpeed = horse7CurrentSpeed,
                horse7TargetSpeed = horse7TargetSpeed,
                horse7SplinePointIndex = 0,

                // Other Horse 8
                horse8SplinePoints = otherHorseSplinePositions7.ToNativeArray(Allocator.Persistent),
                horse8Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse8Positions = new NativeList<float3>(Allocator.Persistent),
                horse8Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse8CurrentPosition = horse8.ColliderTransform.position,
                horse8CurrentRotation = horse8.ColliderTransform.rotation,
                horse8CurrentSpeed = horse8CurrentSpeed,
                horse8TargetSpeed = horse8TargetSpeed,
                horse8SplinePointIndex = 0,

                // Other Horse 9
                horse9SplinePoints = otherHorseSplinePositions8.ToNativeArray(Allocator.Persistent),
                horse9Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse9Positions = new NativeList<float3>(Allocator.Persistent),
                horse9Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse9CurrentPosition = horse9.ColliderTransform.position,
                horse9CurrentRotation = horse9.ColliderTransform.rotation,
                horse9CurrentSpeed = horse9CurrentSpeed,
                horse9TargetSpeed = horse9TargetSpeed,
                horse9SplinePointIndex = 0,

                // Other Horse 10
                horse10SplinePoints = otherHorseSplinePositions9.ToNativeArray(Allocator.Persistent),
                horse10Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse10Positions = new NativeList<float3>(Allocator.Persistent),
                horse10Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse10CurrentPosition = horse10.ColliderTransform.position,
                horse10CurrentRotation = horse10.ColliderTransform.rotation,
                horse10CurrentSpeed = horse10CurrentSpeed,
                horse10TargetSpeed = horse10TargetSpeed,
                horse10SplinePointIndex = 0,

                // Other Horse 11
                horse11SplinePoints = otherHorseSplinePositions10.ToNativeArray(Allocator.Persistent),
                horse11Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse11Positions = new NativeList<float3>(Allocator.Persistent),
                horse11Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse11CurrentPosition = horse11.ColliderTransform.position,
                horse11CurrentRotation = horse11.ColliderTransform.rotation,
                horse11CurrentSpeed = horse11CurrentSpeed,
                horse11TargetSpeed = horse11TargetSpeed,
                horse11SplinePointIndex = 0,

                // Other Horse 12
                horse12SplinePoints = otherHorseSplinePositions11.ToNativeArray(Allocator.Persistent),
                horse12Corners = new NativeArray<float3>(8, Allocator.Persistent),
                horse12Positions = new NativeList<float3>(Allocator.Persistent),
                horse12Rotations = new NativeList<quaternion>(Allocator.Persistent),

                horse12CurrentPosition = horse12.ColliderTransform.position,
                horse12CurrentRotation = horse12.ColliderTransform.rotation,
                horse12CurrentSpeed = horse12CurrentSpeed,
                horse12TargetSpeed = horse12TargetSpeed,
                horse12SplinePointIndex = 0,
            };
            NativeReference<bool> collisionResult = new NativeReference<bool>(Allocator.Persistent);

            //Schedule Job
            HorseBoundingBoxCalculationJob horseBoundingBoxCalculationJob = new HorseBoundingBoxCalculationJob
            {
                deltaTime = Time.fixedDeltaTime,
                maxSpeed = horse.MaxSpeed,
                acceleration = horse.Acceleration,
                thresholdDistance = horseSplineManager.ChangeThresholdDistance,
                isColliding = collisionResult,
                extents = horseSplineManager.Extents,

                horseSplineCollisionData = horseSplineCollisionData,
            };
            JobHandle jobHandle = horseBoundingBoxCalculationJob.Schedule();
            jobHandle.Complete();
            yield return new WaitWhile(() => !jobHandle.IsCompleted);
            if (!collisionResult.Value)
            {
                horseSplineCollisionData.Dispose();
                collisionResult.Dispose();
            }
            Debug.Log("Job Completed");
            #endregion

            //Visualize Bounding Boxes
            if (debugCollision)
                yield return StartCoroutine(IEVisualizeBoundingBoxes(horseSplineCollisionData, horseSplineManager.Extents));

            //Set Horse Spline 
            if (!collisionResult.Value)
            {
                horsesByNumber[HorseNumber].SetSpline(horseSplineManager.GetSplineData(horsesByNumber[HorseNumber].CurrentSplineIndex, nextSplineIndex, horsesByNumber[HorseNumber].CurrentControlPointIndex));
            }
            horsesByNumber[HorseNumber].SetSpeed(targetSpeed, horsesByNumber[HorseNumber].Acceleration);
            horsesByNumber[HorseNumber].OnControlPointChangeSuccessful();

            //Dispose 
            horseSplineCollisionData.Dispose();
            collisionResult.Dispose();

            yield return null;
        }

        private void SetFinishLineSplinePattern(int horseNumber)
        {
            bool isCollided = true;

            for (int i = 0; i < finishLineSplinePattern.Count; i++)
            {
                int splineIndex = horsesByNumber[horseNumber].CurrentSplineIndex + finishLineSplinePattern[i];

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
            horsesByNumber[horseNumber].OnControlPointChangeSuccessful();
            horseFinishLineAccelerationValues.RemoveAt(0);
        }

        public override void ChangeControlPoint(int horseNumber)
        {
            //Check if all the horses has crossed the finish line.
            if (horseFinishLineAccelerationValues.Count <= 0)
            {
                return;
            }

            //Set Acceleration Values for the horses after finish Line.
            if (horseSpeedSO.finishRaceControlPointIndex <= horsesByNumber[horseNumber].CurrentControlPointIndex)
            {
                SetFinishLineSplinePattern(horseNumber);
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
            float targetSpeed = GetSpeed(horseNumber);

            if (direction == Direction.Left)
            {
                //Check for incoming collisions
                isCollisionWithIncomingHorse = CheckIncomingCollisions(horseNumber, currentSplineIndex, controlPointIndex, ref targetSpeed);

                //Set Left Collision
                if (isCollisionWithIncomingHorse || direction == Direction.None || leftSplineIndex <= 0 || !CanAddHorseToSpline(leftSplineIndex))
                {
                    isCollisionWithLeftHorse = true;
                }

                //Set Right Collision
                if (isCollisionWithIncomingHorse || direction == Direction.None || rightSplineIndex >= horseSplineManager.TotalSplinesCount)
                {
                    isCollisionWithRightHorse = true;
                }

                //Check  Collisions with Left Horse.
                if (!isCollisionWithLeftHorse)
                {
                    HandleSplineChange(horseNumber, leftSplineIndex, currentSplineIndex, controlPointIndex, targetSpeed, ref isCollisionWithLeftHorse);
                }

                //Check for Front Collisions
                if (isCollisionWithLeftHorse)
                {
                    isCollisionWithFrontHorse = CheckFrontCollisions(horseNumber, currentSplineIndex, controlPointIndex, ref targetSpeed);
                }

                //Check Collisions with right horse.
                if (!isCollisionWithRightHorse && isCollisionWithFrontHorse)
                {
                    targetSpeed = Utils.GenerateRandomNumber(horseSpeedSO.targetSpeedRange.x, horseSpeedSO.targetSpeedRange.y);

                    HandleSplineChange(horseNumber, rightSplineIndex, currentSplineIndex, controlPointIndex, targetSpeed, ref isCollisionWithRightHorse);

                    if (isCollisionWithRightHorse)
                    {
                        for (int i = slowSpeedsList.Count - 1; i >= 0; i--)
                        {
                            targetSpeed = slowSpeedsList[i];
                            isCollisionWithFrontHorse = CheckFrontCollisions(horseNumber, currentSplineIndex, controlPointIndex, ref targetSpeed);
                            if (!isCollisionWithFrontHorse)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else if (direction == Direction.Right)
            {
                isCollisionWithIncomingHorse = CheckIncomingCollisions(horseNumber, currentSplineIndex, controlPointIndex, ref targetSpeed);

                if (isCollisionWithIncomingHorse || direction == Direction.None || leftSplineIndex <= 0)
                {
                    isCollisionWithLeftHorse = true;
                }

                if (isCollisionWithIncomingHorse || direction == Direction.None || rightSplineIndex >= horseSplineManager.TotalSplinesCount)
                {
                    isCollisionWithRightHorse = true;
                }

                if (!isCollisionWithRightHorse)
                {
                    HandleSplineChange(horseNumber, rightSplineIndex, currentSplineIndex, controlPointIndex, targetSpeed, ref isCollisionWithLeftHorse);
                }

                if (isCollisionWithRightHorse)
                {
                    isCollisionWithFrontHorse = CheckFrontCollisions(horseNumber, currentSplineIndex, controlPointIndex, ref targetSpeed);
                }

                if (!isCollisionWithLeftHorse && isCollisionWithFrontHorse)
                {
                    targetSpeed = Utils.GenerateRandomNumber(horseSpeedSO.targetSpeedRange.x, horseSpeedSO.targetSpeedRange.y);
                    HandleSplineChange(horseNumber, leftSplineIndex, currentSplineIndex, controlPointIndex, targetSpeed, ref isCollisionWithRightHorse);

                    if (isCollisionWithLeftHorse)
                    {
                        for (int i = slowSpeedsList.Count - 1; i >= 0; i--)
                        {
                            targetSpeed = slowSpeedsList[i];
                            isCollisionWithFrontHorse = CheckFrontCollisions(horseNumber, currentSplineIndex, controlPointIndex, ref targetSpeed);
                            if (!isCollisionWithFrontHorse)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else if (direction == Direction.None)
            {
                isCollisionWithFrontHorse = CheckFrontCollisions(horseNumber, currentSplineIndex, controlPointIndex, ref targetSpeed);
                if (isCollisionWithFrontHorse)
                {
                    HandleSplineChange(horseNumber, rightSplineIndex, currentSplineIndex, controlPointIndex, targetSpeed, ref isCollisionWithRightHorse);

                    if (isCollisionWithRightHorse)
                    {
                        for (int i = 0; i < speedGenerationCount; i++)
                        {
                            targetSpeed = GetRandomSlowSpeed();
                            isCollisionWithFrontHorse = CheckFrontCollisions(horseNumber, currentSplineIndex, controlPointIndex, ref targetSpeed);
                            if (!isCollisionWithFrontHorse)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            horsesByNumber[horseNumber].SetSpeed(targetSpeed, horseSpeedSO.acceleration);
            horsesByNumber[horseNumber].OnControlPointChangeSuccessful();
            Time.timeScale = 1;
        }

        private bool IsOtherHorseIsInTheRangeOfControlPoint(int otherHorseNumber, int controlPointIndex)
        {
            return horsesByNumber[otherHorseNumber].CurrentControlPointIndex == controlPointIndex ||
                horsesByNumber[otherHorseNumber].CurrentControlPointIndex + 1 == controlPointIndex
               /*|| horsesByNumber[otherHorseNumber].CurrentControlPointIndex - 1 == controlPointIndex*/;
        }

        #region No use
        private bool CheckIncomingCollisions(int horseNumber, int currentSplineIndex, int controlPointIndex, ref float targetSpeed)
        {
            List<int> horsesIncomingIntoCurrentSpline = new List<int>(horseSplineManager.GetIncomingHorsesInSpline(currentSplineIndex));
            foreach (var otherHorseNumber in horsesIncomingIntoCurrentSpline)
            {
                if (!CheckIfHorseNumbersAreSame(horseNumber, otherHorseNumber) &&
                    IsOtherHorseIsInTheRangeOfControlPoint(otherHorseNumber, controlPointIndex))
                {
                    bool isCollisionWithIncomingHorse = CheckCollisionWithOtherHorse(horseNumber, otherHorseNumber, currentSplineIndex, currentSplineIndex, targetSpeed);
                    if (isCollisionWithIncomingHorse)
                    {
                        targetSpeed = horsesByNumber[horseNumber].TargetSpeed;
                        return true;
                    }
                }
            }
            return false;
        }
        private bool CheckFrontCollisions(int horseNumber, int currentSplineIndex, int controlPointIndex, ref float targetSpeed)
        {
            List<int> horsesInFrontOfCurrentHorse = new List<int>(horseSplineManager.GetHorsesCurrentlyInSpline(currentSplineIndex));
            foreach (var otherHorseNumber in horsesInFrontOfCurrentHorse)
            {
                if (!CheckIfHorseNumbersAreSame(horseNumber, otherHorseNumber)
                    && IsOtherHorseIsInTheRangeOfControlPoint(otherHorseNumber, controlPointIndex)
                    && IsOtherHorseIsFrontOfCurrentHorse(horseNumber, otherHorseNumber))
                {
                    bool isCollisionWithFrontHorse = CheckCollisionWithOtherHorse(horseNumber, otherHorseNumber, currentSplineIndex, currentSplineIndex, targetSpeed);
                    if (isCollisionWithFrontHorse)
                    {
                        targetSpeed = horseSpeedSO.targetSpeedRange.x;
                        return true;
                    }
                }
            }
            return false;
        }
        private bool CanAddHorseToSpline(int splineIndex)
        {
            return horseSplineManager.CanAddHorseToSpline(splineIndex);
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
            //HorseController horse = horsesByNumber[HorseNumber];
            //HorseController otherHorse = horsesByNumber[otherHorseNumber];

            //SplineData horseSplineData = (nextSplineIndex == currentSplineIndex) ? horse.CurrentSplineData : horseSplineManager.GetSplineData(currentSplineIndex, nextSplineIndex, horse.CurrentControlPointIndex);
            //SplineData otherHorseSplineData = otherHorse.CurrentSplineData;

            //// Get horse spline points of control point 
            //List<float3> horseSplinePositions = new List<float3>();
            //for (int i = horse.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
            //{
            //    horseSplinePositions.Add(horseSplineData.splinePoints[i].position);
            //}

            //// Get other horse spline points of control point 
            //List<float3> otherHorseSplinePositions = new List<float3>();
            //for (int i = otherHorse.CurrentSplinePointIndex + 1; i < horse.CurrentControlPointIndex * horseSplineManager.ControlPointLength; i++)
            //{
            //    otherHorseSplinePositions.Add(otherHorseSplineData.splinePoints[i].position);
            //}

            ////Convert List to NativeArray
            //NativeArray<bool> collisionResult = new NativeArray<bool>(1, Allocator.Persistent);
            //NativeArray<float3> corners1 = new NativeArray<float3>(8, Allocator.Persistent);
            //NativeArray<float3> splinePoints1 = horseSplinePositions.ToNativeArray(Allocator.Persistent);

            //NativeArray<float3> corners2 = new NativeArray<float3>(8, Allocator.Persistent);
            //NativeArray<float3> splinePoints2 = otherHorseSplinePositions.ToNativeArray(Allocator.Persistent);

            //NativeList<float3> positions1 = new NativeList<float3>(Allocator.Persistent);
            //NativeList<quaternion> quaternions1 = new NativeList<quaternion>(Allocator.Persistent);
            //NativeList<float3> positions2 = new NativeList<float3>(Allocator.Persistent);
            //NativeList<quaternion> quaternions2 = new NativeList<quaternion>(Allocator.Persistent);

            ////Schedule Job
            //HorseBoundingBoxCalculationJob horseBoundingBoxCalculationJob = new HorseBoundingBoxCalculationJob
            //{
            //    deltaTime = Time.fixedDeltaTime,
            //    //  isColliding = collisionResult,
            //    maxSpeed = horse.MaxSpeed,
            //    acceleration = horse.Acceleration,
            //    thresholdDistance = horseSplineManager.ChangeThresholdDistance,

            //};
            //JobHandle jobHandle = horseBoundingBoxCalculationJob.Schedule();
            //jobHandle.Complete();

            ////Dispose Everything
            //splinePoints1.Dispose();
            //splinePoints2.Dispose();
            //corners1.Dispose();
            //corners2.Dispose();

            ////  StartCoroutine(IEVisualizeBoundingBoxes(positions1, quaternions1, positions2, quaternions2, horseSplineManager.Extents));

            //bool isCollided = collisionResult[0];
            //collisionResult.Dispose();
            //return isCollided;
            return false;
        }
        #endregion

        #endregion

        #region Calculate Race Positions
        private void CalculateRacePositions()
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
        #endregion

        #region PreWinner
        public void SetPreWinnerTargetRacePosition(int _targetRacePos)
        {
            preWinnerTargetRacePosition = _targetRacePos;
        }
        private bool IsPreWinnerHorse(int horseNumber)
        {
            return preWinnerHorseNumber == horseNumber;
        }

        private bool IsHorsePositionAhead(int racePosition, int otherRacePosition)
        {
            return racePosition < otherRacePosition;
        }

        #endregion

        #region Horse Transforms
        public override Transform RaceWinnerTransform()
        {
            return preWinnerHorse.transform;
        }
        #endregion

    }
}
