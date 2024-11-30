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
        [SerializeField] private ActivateFinishLine finishLine;

        [SerializeField] private int changeSpeedPerControlPoint = 2;
        #endregion

        #region Private Variables
        private HorseController preWinnerHorse;
        private int preWinnerHorseNumber;
        private int speedGenerationCount = 10;
        #endregion

        public Vector2 frontHorseMaintainDistance = new Vector2(0.34f, 0.5f);

        public float avoidClashDistance = 0.5f;
        public float sideHorseCheckDistance = 0.1f;
        public float sideHorseCheckAngle = 65f;

        public List<float> slowSpeedsList = new List<float>();
        public List<float> fastSpeedsList = new List<float>();
        public int preWinnerTargetRacePosition = -1;



        #region Unity Methods
        private void Awake()
        {
            GameManager.Instance.SetRaceManager(this);
        }
        private void Start()
        {
            GenerateFastSpeedsList(speedGenerationCount);
            GenerateSlowSpeedsList(speedGenerationCount);
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
            finishLine.SetPreWinner(preWinnerHorse);
        }
        #endregion

        public override Transform RaceWinnerTransform()
        {
            return preWinnerHorse.transform;
        }

        #region Race Start/Finished Methods
        protected override void StartRace()
        {
            base.StartRace();

            //Assign Random Speeds for horsesInSameSpline
            for (int i = 0; i < horsesByNumber.Count; i++)
            {
                int horseNumber = i + 1;
                float speed = Utils.GenerateRandomNumber(horseSpeedSO.startMinSpeed, horseSpeedSO.startMaxSpeed);
                SetHorseSpeed(speed, horseNumber);
                horsesByNumber[horseNumber].RaceStart();
            }
        }
        protected override void RaceFinished()
        {
            //Get Horse Velocity Data to save in json file.
            List<HorseData> horseDatasList = new List<HorseData>();
            for (int i = 1; i <= horsesByNumber.Count; i++)
            {
                ISaveHorseData savedHorseData = (ISaveHorseData)horsesByNumber[i];
                horseDatasList.Add(savedHorseData.HorseSaveData());
            }

            //Save Race.
            saveManager.SetPreDeterminedWinner(preWinnerHorseNumber);
            saveManager.SetWinnersList(horsesInFinishOrder);
            saveManager.SetHorseData(horseDatasList);

            //Reload the currentScene Again, play infinite loops
            SceneLoadingManager.Instance.ReloadActiveScene(3f);
            base.RaceFinished();
        }
        #endregion

        protected override void UpdateHorseRacePositions()
        {
            base.UpdateHorseRacePositions();
            foreach (var item in horsesByNumber.Values)
            {
                HorseControllerSave horseController = item as HorseControllerSave;
                AdjustSpeedBasedOnFrontHorses(horseController);
            }
        }

        #region Speed
        private void AdjustSpeedBasedOnFrontHorses(HorseControllerSave horse)
        {
            //Check if other horses are in same spline
            List<int> horsesInSameSpline = splineManager.GetCurrentSplineHorses(horse.currentSplineIndex);
            if (horsesInSameSpline.Count > 1)
            {
                foreach (var otherHorseNumber in horsesInSameSpline)
                {
                    //Skip the current horse
                    if (CheckIfHorseNumbersAreSame(horse.HorseNumber, otherHorseNumber))
                    {
                        continue;
                    }

                    //if the other horse crossed the finish line, then no need to check.
                    if (horsesByNumber[otherHorseNumber].IsFinishLineCrossed)
                    {
                        continue;
                    }

                    //Return true,If the other horse is ahead of the current horse
                    if (IsHorsePositionAhead(horsesByNumber[otherHorseNumber].RacePosition, horse.RacePosition))
                    {
                        var forwardHorse = (HorseControllerSave)horsesByNumber[otherHorseNumber];
                        float distanceBetweenHorses = Mathf.Abs(horse.currentPercentageInSpline - forwardHorse.currentPercentageInSpline);
                        int speedIndex = Utils.GenerateRandomNumber((int)horseSpeedSO.inRaceMinSpeed, (int)forwardHorse.AgentCurrentSpeed);
                        float speed = Utils.GenerateRandomNumber((float)speedIndex - 1, (float)speedIndex);

                        if (distanceBetweenHorses < avoidClashDistance)
                        {
                            horse.MaintainOtherHorseSpeed(speed - 2);
                            return;
                        }
                        else if (distanceBetweenHorses < Utils.GenerateRandomNumber(frontHorseMaintainDistance.x, frontHorseMaintainDistance.y))
                        {
                            horse.MaintainOtherHorseSpeed(speed);
                            return;
                        }
                    }
                }
            }
            horse.MaintainActualSpeed();
        }

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
                float speed = Utils.GenerateRandomNumber(horseSpeedSO.inRaceMinSpeed, horseSpeedSO.inRaceMaxSpeed);
                //Decrase 25 percent of the speed
                speed = speed - (speed * 0.10f);
                slowSpeedsList.Add(speed);
            }
        }
        private void GenerateFastSpeedsList(int count)
        {
            fastSpeedsList.Clear();
            for (int i = 0; i < count; i++)
            {
                float speed = Utils.GenerateRandomNumber(horseSpeedSO.inRaceMinSpeed, horseSpeedSO.inRaceMaxSpeed);
                //Increase 25 percent of the speed
                speed = speed + (speed * 0.10f);
                fastSpeedsList.Add(speed);
            }
        }

        private void ChangeSpeedRequest(int HorseNumber)
        {
            float speed = Utils.GenerateRandomNumber(horseSpeedSO.inRaceMinSpeed, horseSpeedSO.inRaceMaxSpeed);
            HorseControllerSave horse = (HorseControllerSave)horsesByNumber[HorseNumber];

            if (IsAnyHorseJoiningCurrentControlPoint(horse.currentSplineIndex, horse.controlPointIndex, out int incomingHorseIndex))
            {
                float incomingHorseSpeed = GetHorseSpeed(incomingHorseIndex);
                speed = Utils.GenerateRandomNumber(horseSpeedSO.inRaceMinSpeed, incomingHorseSpeed);
            }
            else if (preWinnerTargetRacePosition > 0)
            {
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
                else
                {
                    //Check if the horse is in the preWinner horse's target race position.
                    if (horsesByNumber[HorseNumber].RacePosition <= preWinnerTargetRacePosition && horsesByNumber[HorseNumber].RacePosition > preWinnerHorse.RacePosition)
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
            }

            SetHorseSpeed(speed, HorseNumber);
        }

        private void SetHorseSpeed(float speed, int horseNumber)
        {
            horsesByNumber[horseNumber].SetSpeed(speed);
        }
        #endregion


        private bool CheckIfHorseNumbersAreSame(int horseNumber1, int horseNumber2)
        {
            return horseNumber1 == horseNumber2;
        }

        private bool IsHorsePositionAhead(int racePosition, int otherRacePosition)
        {
            return racePosition < otherRacePosition;
        }

        private bool IsAnyHorseJoiningCurrentControlPoint(int splineIndex, int controlPointIndex, out int horseIndex)
        {
            horseIndex = -1;
            if (splineManager.IsAnyHorseEnteringCurrentSpline(splineIndex))
            {
                List<int> incomingHorsesList = splineManager.GetIncomingHorses(splineIndex);
                foreach (var incomingHorseNumber in incomingHorsesList)
                {
                    HorseControllerSave incomingHorse = (HorseControllerSave)horsesByNumber[incomingHorseNumber];
                    if (incomingHorse.controlPointIndex == controlPointIndex)
                    {
                        horseIndex = incomingHorseNumber;
                        return true;
                    }
                }
            }
            return false;
        }
        private bool IsPreWinnerHorse(int horseNumber)
        {
            return preWinnerHorseNumber == horseNumber;
        }

        private float GetHorseSpeed(int horseNumber)
        {
            float speed = horsesByNumber[horseNumber].AgentCurrentSpeed;
            return speed;
        }

        public void SetPreWinnerTargetRacePosition(int _targetRacePos)
        {
            preWinnerTargetRacePosition = _targetRacePos;
        }

        public void CheckForSplineChange(int horseNumber)
        {
            HorseControllerSave horseController = (HorseControllerSave)horsesByNumber[horseNumber];

            if (horseController.speedRequestCountDown > 0)
            {
                ChangeSpeedRequest(horseNumber);
            }

            if (CheckIfHorseIsChangingSpline(horseController))
            {
                return;
            }

            if (CanTakeLeftTurn(horseController))
            {
                horseController.TurnLeft();
            }
            else if (CanTakeRightTurn(horseController))
            {
                horseController.TurnRight();
            }
        }

        private bool CanTakeLeftTurn(HorseControllerSave horse)
        {
            int leftSplineIndex = horse.currentSplineIndex - 1;
            bool isLeftSplineExist = splineManager.CheckIfSplineExist(leftSplineIndex);
            if (isLeftSplineExist)
            {
                List<int> leftSplineHorses = splineManager.GetCurrentSplineHorses(leftSplineIndex);

                if (leftSplineHorses.Count > 0)
                {
                    foreach (var leftHorseNumber in leftSplineHorses)
                    {
                        //check if the left horse is not giving a chance to change spline
                        HorseControllerSave leftHorse = (HorseControllerSave)horsesByNumber[leftHorseNumber];
                        float distance = Mathf.Abs(leftHorse.currentPercentageInSpline - horse.currentPercentageInSpline);

                        if (distance <= sideHorseCheckDistance)
                        {
                            if (leftHorse.isChangingSplineLeftSide)
                            {
                                return true;
                            }
                            return false;
                        }

                    }
                }
                return true;
            }
            return false;
        }

        private bool CheckIfHorseIsChangingSpline(HorseControllerSave horse)
        {
            if (horse.isChangingSplineLeftSide || horse.isChangingSplineRightSide)
            {
                return true;
            }
            return false;
        }

        private bool CanTakeRightTurn(HorseControllerSave horse)
        {
            bool canTurnRight = false;
            int currentSplineIndex = horse.currentSplineIndex;
            List<int> currentSplineHorses = splineManager.GetCurrentSplineHorses(currentSplineIndex);

            if (currentSplineHorses.Count > 1)
            {
                foreach (var otherHorseNumber in currentSplineHorses)
                {
                    //Skip the current horse
                    if (horse.HorseNumber == otherHorseNumber)
                    {
                        continue;
                    }

                    //Return true,If the other horse is ahead of the current horse
                    if (horse.RacePosition > horsesByNumber[otherHorseNumber].RacePosition)
                    {
                        var forwardHorse = (HorseControllerSave)horsesByNumber[otherHorseNumber];
                        if (horse.AgentActualSpeed > forwardHorse.AgentActualSpeed)
                        {
                            float distanceBetweenHorses = Mathf.Abs(horse.currentPercentageInSpline - forwardHorse.currentPercentageInSpline);
                            if (distanceBetweenHorses <= Utils.GenerateRandomNumber(frontHorseMaintainDistance.x, frontHorseMaintainDistance.y))
                            {
                                canTurnRight = true;
                                break;
                            }
                        }
                    }
                }
            }

            int rightSplineIndex = horse.currentSplineIndex + 1;
            bool isRightSplineExist = splineManager.CheckIfSplineExist(rightSplineIndex);
            if (isRightSplineExist && canTurnRight)
            {
                List<int> rightSplineHorses = splineManager.GetCurrentSplineHorses(rightSplineIndex);
                if (rightSplineHorses.Count > 0)
                {
                    foreach (var rightHorseNumber in rightSplineHorses)
                    {
                        //check if the right horse is not giving a chance to change spline
                        HorseControllerSave rightHorse = (HorseControllerSave)horsesByNumber[rightHorseNumber];
                        float distance = Mathf.Abs(rightHorse.currentPercentageInSpline - horse.currentPercentageInSpline);
                        if (distance <= sideHorseCheckDistance)
                        {
                            if (rightHorse.isChangingSplineRightSide)
                            {
                                return true;
                            }

                            return false;
                        }
                    }
                }
                return true;
            }

            return false;
        }

    }
}