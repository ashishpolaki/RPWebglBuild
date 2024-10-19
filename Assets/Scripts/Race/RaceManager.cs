using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HorseRace
{
    public class RaceManager : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] protected SplineManager splineManager;
        [SerializeField] protected HorseSpeedSO horseSpeedSO;
        #endregion

        #region Protected Variables
        protected bool isRaceStart;
        protected Dictionary<int, HorseController> horsesByNumber = new Dictionary<int, HorseController>();
        [Tooltip("Key: RacePosition, Value : Horse")]
        protected Dictionary<int, int> horsesInRacePositions = new Dictionary<int, int>();
        protected List<int> horsesInFinishOrder = new List<int>();
        #endregion

        #region Properties
        public virtual HorseController CurrentOvertakingHorse { get; set; }
        #endregion

        #region Unity Methods
        protected virtual void OnEnable()
        {
            EventManager.Instance.OnRaceStartEvent += StartRace;
            //    EventManager.Instance.OnChangeWaypointGroupEvent += OnWaypointGroupChange;
            EventManager.Instance.OnCrossFinishLineEvent += FinishLineCrossed;
        }
        protected virtual void OnDisable()
        {
            EventManager.Instance.OnRaceStartEvent -= StartRace;
            //     EventManager.Instance.OnChangeWaypointGroupEvent -= OnWaypointGroupChange;
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

        #region Race Position Tracking
        protected virtual void UpdateHorseRacePositions()
        {
            //Update Horses InRace
            Dictionary<int,float> horsesSplinePercentages = new Dictionary<int, float>();
            for (int i = 1; i <= horsesByNumber.Count; i++)
            {
                int horseNumber = horsesByNumber.ElementAt(i - 1).Key;
                horsesByNumber[horseNumber].UpdateState();
                horsesSplinePercentages[i] = horsesByNumber[i].currentPercentageInSpline;
            }

            //Sort Horse Racepositions by Spline Percentages
            var horsesInRaceOrder =  horsesSplinePercentages.OrderByDescending(horsesSplinePercentages => horsesSplinePercentages.Value);
            for (int i = 1; i <= horsesByNumber.Count; i++)
            {
                int horseNumber = horsesInRaceOrder.ElementAt(i - 1).Key;
                int raceposition = i;
                horsesInRacePositions[raceposition] = horseNumber;
                horsesByNumber[horseNumber].SetRacePosition(raceposition);
            }
        }
        #endregion

        public virtual void Initialize(HorseController[] _horses)
        {
            for (int i = 0; i < _horses.Length; i++)
            {
                horsesByNumber.Add(_horses[i].HorseNumber, _horses[i]);
            }
        }

        public List<Transform> HorseTransformsInRaceOrder()
        {
            if (horsesInRacePositions.Count > 0)
            {
                return horsesInRacePositions.Select(kvp => horsesByNumber[kvp.Value].transform).ToList();
            }
            else
            {
                return new List<Transform>();
            }
        }
        public virtual Transform RaceWinnerTransform()
        {
            return horsesByNumber[horsesInFinishOrder[0]].transform;
        }


        #region Race Start/Finish Methods
        protected virtual void StartRace()
        {
            isRaceStart = true;
        }
        protected virtual void FinishLineCrossed(int _horseNumber)
        {
            horsesInFinishOrder.Add(_horseNumber);
            horsesByNumber[_horseNumber].FinishLineCrossed();

            //Return True, if all Horses Crossed Finish Line
            if (horsesInFinishOrder.Count >= horsesByNumber.Count)
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
