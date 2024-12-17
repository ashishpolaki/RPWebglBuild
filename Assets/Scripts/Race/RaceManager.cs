using System.Collections.Generic;
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
        protected bool areRacePositionsCalculating;
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

        #region Initialize
        public virtual void Initialize(HorseController[] _horses)
        {
            for (int i = 0; i < _horses.Length; i++)
            {
                horsesByNumber.Add(_horses[i].HorseNumber, _horses[i]);
            }
        }
        #endregion

        #region Spline
        public virtual void ChangeControlPoint(int horsenumber)
        {

        }
        public void HorseReachedSpline(int splineIndex, int previousSplineINdex, int horseNumber)
        {
            horseSplineManager.HorseReachedSpline(splineIndex, previousSplineINdex, horseNumber);
        }
        public void HorseChangingSpline(int currentSplineArrayIndex, int nextSplineArrayIndex, int horseNumber)
        {
            horseSplineManager.HorseChangingSpline(currentSplineArrayIndex, nextSplineArrayIndex, horseNumber);
        }

        #endregion

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
