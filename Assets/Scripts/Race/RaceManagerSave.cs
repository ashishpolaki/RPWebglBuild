using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HorseRace
{
    public class RaceManagerSave : RaceManager
    {
        #region Inspector Variables
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private PreWinnerManager preWinnerManager;
        #endregion

        #region Private Variables
        private HorseController preWinnerHorse;
        private int preWinnerHorseNumber;
        private int speedGenerationCount = 10;
        private int preWinnerTargetRacePosition = -1;
        private bool isRaceFinish = false;
        #endregion

        //public List<float> slowSpeedsList = new List<float>();
        //public List<float> fastSpeedsList = new List<float>();

        #region Unity Methods
        private void Awake()
        {
            GameManager.Instance.SetRaceManager(this);
        }
        private void Start()
        {
            //  GenerateFastSpeedsList(speedGenerationCount);
            //  GenerateSlowSpeedsList(speedGenerationCount);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        protected override void FixedUpdate()
        {
            if (!isRaceStart)
            {
                return;
            }
            //Save Horses Velocity to update them in json file at race end
            base.FixedUpdate();

            if(isRaceFinish)
            {
                if (horsesByNumber[horsesInRaceFinishOrder[0]].CurrentSpeed == 0)
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

        public override Transform RaceWinnerTransform()
        {
            return preWinnerHorse.transform;
        }

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

        protected override void UpdateHorseRacePositions()
        {
            // base.UpdateHorseRacePositions();
            foreach (var item in horsesByNumber.Values)
            {
                item.UpdateState();
            }
        }

        #region Speed
        //private float GetRandomSlowSpeed()
        //{
        //    int randomIndex = Utils.GenerateRandomNumber(0, slowSpeedsList.Count);
        //    return slowSpeedsList[randomIndex];
        //}
        //private float GetRandomHighSpeed()
        //{
        //    int randomIndex = Utils.GenerateRandomNumber(0, fastSpeedsList.Count);
        //    return fastSpeedsList[randomIndex];
        //}
        //private void GenerateSlowSpeedsList(int count)
        //{
        //    slowSpeedsList.Clear();
        //    for (int i = 0; i < count; i++)
        //    {
        //        float speed = Utils.GenerateRandomNumber(horseSpeedSO.inRaceMinSpeed, horseSpeedSO.inRaceMaxSpeed);
        //        //Decrase 25 percent of the speed
        //        speed = speed - (speed * 0.10f);
        //        slowSpeedsList.Add(speed);
        //    }
        //}
        //private void GenerateFastSpeedsList(int count)
        //{
        //    fastSpeedsList.Clear();
        //    for (int i = 0; i < count; i++)
        //    {
        //        float speed = Utils.GenerateRandomNumber(horseSpeedSO.inRaceMinSpeed, horseSpeedSO.inRaceMaxSpeed);
        //        //Increase 25 percent of the speed
        //        speed = speed + (speed * 0.10f);
        //        fastSpeedsList.Add(speed);
        //    }
        //}

        //private void ChangeSpeedRequest(int HorseNumber)
        //{
        //    float speed = Utils.GenerateRandomNumber(horseSpeedSO.inRaceMinSpeed, horseSpeedSO.inRaceMaxSpeed);
        //    HorseControllerSave horse = (HorseControllerSave)horsesByNumber[HorseNumber];

        //    if (IsAnyHorseJoiningCurrentControlPoint(horse.currentSplineIndex, horse.controlPointIndex, out int incomingHorseIndex))
        //    {
        //        float incomingHorseSpeed = GetHorseSpeed(incomingHorseIndex);
        //        speed = Utils.GenerateRandomNumber(horseSpeedSO.inRaceMinSpeed, incomingHorseSpeed);
        //    }
        //    else if (preWinnerTargetRacePosition > 0)
        //    {
        //        if (IsPreWinnerHorse(HorseNumber))
        //        {
        //            if (IsHorsePositionAhead(preWinnerHorse.RacePosition, preWinnerTargetRacePosition))
        //            {
        //                speed = GetRandomSlowSpeed();
        //            }
        //            else
        //            {
        //                speed = GetRandomHighSpeed();
        //            }
        //        }
        //        else
        //        {
        //            //Check if the horse is in the preWinner horse's target race position.
        //            if (horsesByNumber[HorseNumber].RacePosition <= preWinnerTargetRacePosition && horsesByNumber[HorseNumber].RacePosition > preWinnerHorse.RacePosition)
        //            {
        //                if (IsHorsePositionAhead(horsesByNumber[HorseNumber].RacePosition, preWinnerTargetRacePosition))
        //                {
        //                    speed = GetRandomHighSpeed();
        //                }
        //                else
        //                {
        //                    speed = GetRandomSlowSpeed();
        //                }
        //            }
        //        }
        //    }

        //    SetHorseSpeed(speed, HorseNumber);
        //}
        //private bool IsPreWinnerHorse(int horseNumber)
        //{
        //    return preWinnerHorseNumber == horseNumber;
        //}

        //public void SetPreWinnerTargetRacePosition(int _targetRacePos)
        //{
        //    preWinnerTargetRacePosition = _targetRacePos;
        //}
        #endregion
    }
}