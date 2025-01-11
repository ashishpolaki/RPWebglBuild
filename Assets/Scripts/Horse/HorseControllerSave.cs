using System.Collections.Generic;
using UnityEngine;

namespace HorseRace
{
    public class HorseControllerSave : HorseController, ISaveHorseData
    {
        private List<ControlPointSave> controlPointsSaveList = new List<ControlPointSave>();
        private List<int> overtakeControlPointGroupIndices = new List<int>();
        private float overtakeCheckTime = 3f;
        private float previousSavedTime;
        private int minHorsesOvertake = 2;
        private int previousRacePosition = -1;
        private int overtakeControlPointIndex;
        private int minOvertakeControlPointNumber;
        private int maxOvertakeControlPointNumber;
        private int previousSplineIndex = -1;
        private int nextSplineIndex = -1;

        public override void RaceStart()
        {
            base.RaceStart();
        }
        public override void UpdateState()
        {
            base.UpdateState();
            SaveOvertakeCameraData();
        }

        public override void InitializeData(SplineData _splineData, float _speed, float _maxSpeed, float _accleration, float _thresHold)
        {
            AddControlPointData(_speed, _accleration, _splineData.splineIndex, CurrentControlPointIndex);
            base.InitializeData(_splineData, _speed, _maxSpeed, _accleration, _thresHold);
        }

        protected override void OnControlPointChange()
        {
            base.OnControlPointChange();
            if (currentSplineData.splineIndex == nextSplineIndex)
            {
                //Reached the spline.
                GameManager.Instance.RaceManager.HorseReachedSpline(nextSplineIndex, previousSplineIndex, HorseNumber);
                currentSplineIndex = nextSplineIndex;
                nextSplineIndex = -1;
                previousSplineIndex = -1;
            }
            EventManager.Instance.OnControlPointChange(HorseNumber, CurrentControlPointIndex);
        }

        public void SetOvertakeControlPointsData(int totalControlPoints, int startOffset, int endOffset, int minOvertakeHorses,float overtakeCheckTime)
        {
            minOvertakeControlPointNumber = startOffset;
            maxOvertakeControlPointNumber = totalControlPoints - endOffset;
            minHorsesOvertake = minOvertakeHorses;
            this.overtakeCheckTime = overtakeCheckTime;
        }
        public override void SetSpline(SplineData splineData)
        {
            if (splineData.splineIndex != currentSplineIndex)
            {
                previousSplineIndex = currentSplineData.splineIndex;
                nextSplineIndex = splineData.splineIndex;
            }
            GameManager.Instance.RaceManager.HorseChangingSpline(currentSplineIndex - 1, nextSplineIndex - 1, HorseNumber);
            base.SetSpline(splineData);
        }

        private void AddControlPointData(float speed, float aceleration, int splineIndex, int currentControLpointIndex)
        {
            controlPointsSaveList.Add(new ControlPointSave
            {
                speed = speed,
                acceleration = aceleration,
                splineIndex = splineIndex,
                controlPointIndex = currentControLpointIndex
            });
        }

        #region Speed
        public override void SetSpeed(float speed, float aceleration)
        {
            base.SetSpeed(speed, aceleration);
            AddControlPointData(speed, aceleration, CurrentSplineData.splineIndex, CurrentControlPointIndex);
        }
        #endregion

        #region Save Info
        private void SaveOvertakeCameraData()
        {
            if (CurrentControlPointIndex > minOvertakeControlPointNumber &&
                 CurrentControlPointIndex < maxOvertakeControlPointNumber)
            {
                if (previousRacePosition > 0)
                {
                    if (Time.fixedTime - previousSavedTime > overtakeCheckTime)
                    {
                        if (RacePosition <= previousRacePosition - minHorsesOvertake)
                        {
                            //Save horsevelocityIndex
                            overtakeControlPointGroupIndices.Add(overtakeControlPointIndex);
                        }
                        previousRacePosition = -1;
                    }
                }

                if (previousRacePosition <= 0)
                {
                    previousRacePosition = RacePosition;
                    previousSavedTime = Time.fixedTime;
                    overtakeControlPointIndex = CurrentControlPointIndex;
                }
            }
        }

        public HorseData HorseSaveData()
        {
            HorseData horseData = new HorseData();
            horseData.horseNumber = HorseNumber.ToString();
            horseData.controlPoints = controlPointsSaveList.ToArray();
            //StringBuilder overtakeHorsesStringBuilder = new StringBuilder();
            //for (int i = 0; i < overtakeVelocityIndexList.Count; i++)
            //{
            //    overtakeHorsesStringBuilder.Append($"{overtakeVelocityIndexList[i]},");
            //}
            //horseData.overtakeData = overtakeHorsesStringBuilder.ToString();
            return horseData;
        }
        #endregion
    }
}
