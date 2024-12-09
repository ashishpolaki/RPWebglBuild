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
        [Tooltip("Key: HorseNumber, Value : HorseController(Monobehaviour Component)")]
        protected Dictionary<int, HorseController> horsesByNumber = new Dictionary<int, HorseController>();
        [Tooltip("Key: RacePosition, Value : HorseNumber")]
        protected Dictionary<int, int> horsesInRacePositions = new Dictionary<int, int>();
        [Tooltip("Key: RacePosition, (Value : Item1 = Horse Number , Item2 = Horse Transform)")]
        protected Dictionary<int, (int, Transform)> horsesTransformInRaceOrder = new Dictionary<int, (int, Transform)>();
        protected List<int> horsesInRaceFinishOrder = new List<int>();
        #endregion

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
        }

        #region Race Position Tracking
        protected virtual void UpdateHorseRacePositions()
        {
            //Update Horses InRace
            var horsesSplinePercentages = new Dictionary<int, float>();
            foreach (var horse in horsesByNumber.Values)
            {
                horse.UpdateState();
                horsesSplinePercentages[horse.HorseNumber] = horse.currentPercentageInSpline;
            }

            //Sort Horse Racepositions by Spline Percentages
            var horsesInRaceOrder = new List<KeyValuePair<int, float>>(horsesSplinePercentages);
            horsesInRaceOrder.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            for (int i = 1; i <= horsesByNumber.Count; i++)
            {
                int horseNumber = horsesInRaceOrder.ElementAt(i - 1).Key;
                int raceposition = i;
                horsesInRacePositions[raceposition] = horseNumber;
                horsesByNumber[horseNumber].SetRacePosition(raceposition);
            }
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
