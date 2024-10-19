using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Random = Unity.Mathematics.Random;
using Unity.Mathematics;

namespace HorseRace
{
    [System.Serializable]
    public struct RaceTargetPosition
    {
        public int controlPointIndex;
        public int targetRacePosition;
    }

    public class PreWinnerManager : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private List<RaceTargetPosition> raceTargetPositionsList = new List<RaceTargetPosition>();
        [SerializeField] private CatmullRomSpline catmullRomSpline;
        [SerializeField] private HorseSpeedSO horsesSpeedSO;

        [SerializeField] private int startCheckControlPointIdx = 2;
        [SerializeField] private int controlPointsMinGap = 12;
        [SerializeField] private Vector2 controlPointsRange = new Vector2(3, 4);
        #endregion

        #region Private Variables
        private HorseController preWinnerHorse;
        private int savedControlPointIndex = -1;
        private int nextTargetRacePosition;
        private int prevTargetRacePosition;
        #endregion

        #region Unity Methods
        private void Awake()
        {
            InitializeData();
        }
        private void OnEnable()
        {
            EventManager.Instance.OnControlPointChangeEvent += HandleControlPointChange;
        }
        private void OnDisable()
        {
            EventManager.Instance.OnControlPointChangeEvent -= HandleControlPointChange;
        }
        #endregion


        #region Public Methods
        public void SetPreWinner(HorseController horseController)
        {
            preWinnerHorse = horseController;
        }
        #endregion

        #region Subscribed Methods
        private void HandleControlPointChange(int HorseNumber, int _controlPointIndex)
        {
            if (preWinnerHorse == null)
            {
                return;
            }

            if (preWinnerHorse.IsFinishLineCrossed)
            {
                return;
            }

            if (HorseNumber == preWinnerHorse.HorseNumber)
            {
                if (!IsTargetRacePositionAvailable(_controlPointIndex))
                {
                    return;
                }
                SetTargetRacePosition(_controlPointIndex);
                SetPreWinnerTargetPosition();
            }
        }
        #endregion

        #region Private Methods
        private void SetPreWinnerTargetPosition()
        {
            int currentRacePosition = preWinnerHorse.RacePosition;

            //Return True, if Winner's TargetRacePosition != CurrentRacePosition.
            if ((nextTargetRacePosition != prevTargetRacePosition) && (nextTargetRacePosition != currentRacePosition))
            {
                prevTargetRacePosition = nextTargetRacePosition;

                RaceManagerSave raceManager = (RaceManagerSave)GameManager.Instance.RaceManager;
                raceManager.SetPreWinnerTargetRacePosition(nextTargetRacePosition);
            }
        }

        /// <summary>
        /// Get the targeted race position in the current waypoint Group.
        /// </summary>
        /// <param name="_preWinnerWaypointIndex"></param>
        /// <returns></returns>
        private void SetTargetRacePosition(int currentControlPointIndex)
        {
            int targetControlPointIndex = raceTargetPositionsList[0].controlPointIndex;
            if (targetControlPointIndex > savedControlPointIndex && (currentControlPointIndex >= savedControlPointIndex))
            {
                nextTargetRacePosition = raceTargetPositionsList[0].targetRacePosition;
                savedControlPointIndex = targetControlPointIndex;
                raceTargetPositionsList.RemoveAt(0);
            }
        }

        /// <summary>
        /// CHeck if Target Race Position is available for PreWinner Horse.
        /// </summary>
        /// <returns></returns>
        private bool IsTargetRacePositionAvailable(int controlPointIndex)
        {
            return (raceTargetPositionsList.Count > 0) && (controlPointIndex >= startCheckControlPointIdx);
        }


        #region Initialize Winner Data
        private void InitializeData()
        {
            SetupTargetRacePositions();
        }
        /// <summary>
        /// Set Target Race positions for prewinner horse at waypoint groups.
        /// </summary>
        private void SetupTargetRacePositions()
        {
            List<int> selectedControlPoints = GenerateRandomControlPoints();

            //Generate random control points for Winner's Race Position
            for (int i = 0; i < selectedControlPoints.Count; i++)
            {
                if (raceTargetPositionsList.Exists(x => x.controlPointIndex == selectedControlPoints[i]))
                    continue;

                RaceTargetPosition raceTargetPosition = new RaceTargetPosition
                {
                    controlPointIndex = selectedControlPoints[i],
                    targetRacePosition = Utils.GenerateRandomNumber(1, GameManager.Instance.HorsesToSpawnList.Count)
                };
                raceTargetPositionsList.Add(raceTargetPosition);
            }

            //Sort in ascending order with waypointGroup Index
            raceTargetPositionsList.Sort((x, y) => x.controlPointIndex.CompareTo(y.controlPointIndex));
        }

        private List<int> GenerateRandomControlPoints()
        {
            int numberOfPoints = Utils.GenerateRandomNumber((int)controlPointsRange.x, (int)controlPointsRange.y);
            NativeArray<int> selectedControlPoints = new NativeArray<int>(numberOfPoints, Allocator.TempJob);
            GetRandomControlPointsJob job = new GetRandomControlPointsJob
            {
                totalPoints = catmullRomSpline.ControlPoints.Count,
                numberOfPoints = numberOfPoints,
                minGap = controlPointsMinGap,
                seed = (uint)UnityEngine.Random.Range(1, int.MaxValue),
                selectedPoints = selectedControlPoints
            };
            JobHandle handle = job.Schedule();
            handle.Complete();

            List<int> controlPoints = new List<int>(selectedControlPoints);

            //Dispose 
            selectedControlPoints.Dispose();

            return controlPoints;
        }

        #endregion

        #endregion
    }

    public struct GetRandomControlPointsJob : IJob
    {
        public int totalPoints;
        public int numberOfPoints;
        public int minGap;
        public uint seed;

        public NativeArray<int> selectedPoints;

        public void Execute()
        {
            Random random = new Random(seed);
            List<int> tempSelectedPoints = new List<int>();

            while (tempSelectedPoints.Count < numberOfPoints)
            {
                int point = random.NextInt(totalPoints);

                // Ensure the point is not too close to any already selected points
                bool isValid = true;
                foreach (int selectedPoint in tempSelectedPoints)
                {
                    if (math.abs(selectedPoint - point) < minGap)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    tempSelectedPoints.Add(point);
                }
            }

            tempSelectedPoints.Sort();

            for (int i = 0; i < tempSelectedPoints.Count; i++)
            {
                selectedPoints[i] = tempSelectedPoints[i];
            }
        }
    }
}



