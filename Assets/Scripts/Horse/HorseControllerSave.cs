using System.Collections.Generic;
using UnityEngine;

namespace HorseRace
{
    public class HorseControllerSave : HorseController, ISaveHorseData
    {
        [SerializeField] private List<ControlPointSave> controlPointsSaveList = new List<ControlPointSave>();

        //private float previousSavedTime;
        //private int overtakeVelocityIndex;
        //private int previousRacePosition = -1;

        //private List<int> overtakeVelocityIndexList = new List<int>();
        //[SerializeField] private int overtakeCameraWaypointGroupMin;
        //[SerializeField] private int overtakeCameraWaypointGroupMax;
        //[SerializeField] private float overtakeCheckTime = 3f;
        //[SerializeField] private int minHorsesOvertake = 2;
        private int previousSplineIndex = -1;
        private int nextSplineIndex = -1;

        public override void RaceStart()
        {
            base.RaceStart();
        }
        public override void UpdateState()
        {
            base.UpdateState();
        }
        public override void InitializeData(SplineData _splineData, float _speed, float _maxSpeed, float _accleration, float _thresHold)
        {
            controlPointsSaveList.Add(new ControlPointSave
            {
                speed = _speed,
                acceleration = _accleration,
                splineIndex = _splineData.splineIndex,
                controlPointIndex = CurrentControlPointIndex
            });
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

        #region Speed
        public override void SetSpeed(float speed, float aceleration)
        {
            base.SetSpeed(speed, aceleration);
            controlPointsSaveList.Add(new ControlPointSave
            {
                speed = speed,
                acceleration = aceleration,
                splineIndex = CurrentSplineData.splineIndex,
                controlPointIndex = CurrentControlPointIndex
            });
        }
        #endregion

        #region Save Info
        private void SaveOvertakeCameraData()
        {
            //    if (currentWaypointGroupIndex > overtakeCameraWaypointGroupMin &&
            //          currentWaypointGroupIndex < overtakeCameraWaypointGroupMax)
            //{
            //    if (previousRacePosition > 0)
            //    {
            //        if (Time.fixedTime - previousSavedTime > overtakeCheckTime)
            //        {
            //            if (RacePosition <= previousRacePosition - minHorsesOvertake)
            //            {
            //                //Save horsevelocityIndex
            //                overtakeVelocityIndexList.Add(overtakeVelocityIndex);
            //            }
            //            previousRacePosition = -1;
            //        }
            //    }

            //    if (previousRacePosition <= 0)
            //    {
            //        previousRacePosition = RacePosition;
            //        previousSavedTime = Time.fixedTime;
            //        overtakeVelocityIndex = horseVelocityList.Count;
            //    }
            //}
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
