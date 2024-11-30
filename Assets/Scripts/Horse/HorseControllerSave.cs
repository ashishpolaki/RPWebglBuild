using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HorseRace
{
    public class HorseControllerSave : HorseController, ISaveHorseData
    {
        [SerializeField] private float splineChangeDistance = 3f;
        [SerializeField] private int overtakeCameraWaypointGroupMin;
        [SerializeField] private int overtakeCameraWaypointGroupMax;
        [SerializeField] private float overtakeCheckTime = 3f;
        [SerializeField] private int minHorsesOvertake = 2;

        private List<HorseVelocity> horseVelocityList = new List<HorseVelocity>();
        private List<int> overtakeVelocityIndexList = new List<int>();
        private float previousSavedTime;
        private int overtakeVelocityIndex;
        private int previousRacePosition = -1;

        public override void RaceStart()
        {
            base.RaceStart();
            agent.updatePosition = false;
            currentSplineSegment = SplineManager.Instance.GetSplineSegment(HorseNumber, HorseNumber);
        }
        public override void UpdateState()
        {
            base.UpdateState();
            SpeedState();
            SplineState();
        }

        #region Speed
        private void SpeedState()
        {
            //If Finish Line Crossed
            if (isFinishLineCross)
            {
                agent.speed -= Time.fixedDeltaTime * slowDownSpeedLerp;
                agent.stoppingDistance = 1f;
                return;
            }
        }
        #endregion

        #region Splines
        public SplineSegment currentSplineSegment;
        public int splinePointIndex;
        public int controlPointIndex;
        public int lastSplineIndex;
        public int currentSplineIndex;
        public int nextSplineIndex = -1;
        public int speedRequestInterval = 2;

        public bool isChangingSplineRightSide;
        public bool isChangingSplineLeftSide;
        public int speedRequestCountDown;
        private int finishLineControlPointIndex;

        private void SetHorseDestination()
        {
            SplineInfo splineInfo = currentSplineSegment.splineControlPoints[controlPointIndex];
            Vector3 splinePoint = splineInfo.splinePoints[splinePointIndex];
            currentSplineIndex = currentSplineSegment.splineControlPoints[controlPointIndex].splineIndex;
            agent.SetDestination(splinePoint);
        }

        private void CheckIfControlPointChanged()
        {
            SplineInfo splineInfo = currentSplineSegment.splineControlPoints[controlPointIndex];
            splinePointIndex++;

            //If spline control points in current control point are finished. Move to next control point.
            if (splinePointIndex >= splineInfo.splinePoints.Count)
            {
                ControlPointChange();
                VerifyAndChangeSpline();
            }
        }

        private void ControlPointChange()
        {
            //Control Point Changed
            EventManager.Instance.OnControlPointChange(HorseNumber, controlPointIndex);
            splinePointIndex = 0;
            controlPointIndex++;
        }

        private void VerifyAndChangeSpline()
        {
            if (controlPointIndex >= currentSplineSegment.splineControlPoints.Count)
            {
                return;
            }
            speedRequestCountDown--;

            //Check if horse Changed Spline Successfully.
            int splineIndex = currentSplineSegment.splineControlPoints[controlPointIndex].splineIndex;
            if (splineIndex == nextSplineIndex)
            {
                lastSplineIndex = currentSplineIndex;
                currentSplineIndex = splineIndex;
                SplineManager.Instance.ChangedSpline(nextSplineIndex, lastSplineIndex, nextSplineIndex, HorseNumber);
                ResetValues();
            }

             //Check if horse can change spline
             ((RaceManagerSave)GameManager.Instance.RaceManager).CheckForSplineChange(HorseNumber);
            if (speedRequestCountDown <= 0)
            {
                speedRequestCountDown = speedRequestInterval;
            }
        }

        private void ResetValues()
        {
            //Reset Values
            nextSplineIndex = -1;
            isChangingSplineLeftSide = false;
            isChangingSplineRightSide = false;
        }

        private void SplineState()
        {
            if (agent.remainingDistance <= splineChangeDistance)
            {
                if (controlPointIndex >= currentSplineSegment.splineControlPoints.Count)
                {
                    Vector3 destination = SplineManager.Instance.GetPostFinishLineControlPoint(currentSplineIndex, finishLineControlPointIndex);
                    finishLineControlPointIndex++;
                    agent.SetDestination(destination);
                    return;
                }
                SetHorseDestination();
                CheckIfControlPointChanged();
            }

            //If finish line is not crossed then update the percentage in spline.
            if (!isFinishLineCross)
            {
                currentPercentageInSpline = SplineManager.Instance.GetClosestT(transform.position, 6, controlPointIndex);
                currentPercentageInSpline = currentPercentageInSpline * 100;
            }
        }
        #endregion

        public void MaintainOtherHorseSpeed(float speed)
        {
            actualSpeed = agent.speed;
            agent.speed = speed;
        }

        public void MaintainActualSpeed()
        {
            agent.speed = actualSpeed;
        }

        public void TurnLeft()
        {
            nextSplineIndex = currentSplineIndex - 1;
            currentSplineSegment = SplineManager.Instance.GetNeighborSplineSegment(currentSplineIndex, nextSplineIndex, controlPointIndex);
            SplineManager.Instance.HorseChangingSpline(currentSplineIndex, nextSplineIndex, HorseNumber);
            isChangingSplineLeftSide = true;
        }
        public void TurnRight()
        {
            nextSplineIndex = currentSplineIndex + 1;
            currentSplineSegment = SplineManager.Instance.GetNeighborSplineSegment(currentSplineIndex, nextSplineIndex, controlPointIndex);
            SplineManager.Instance.HorseChangingSpline(currentSplineIndex, nextSplineIndex, HorseNumber);
            isChangingSplineRightSide = true;
        }

        #region Waypoint

        private void WaypointsState()
        {

        }

        private void OnWayPointCrossed()
        {
            //if (currentWaypointGroupIndex >= 1 && !isFinishLineCross)
            //{
            //    EventManager.Instance.OnWaypointCrossed(HorseNumber, currentWaypointGroupIndex);
            //}
        }

        #endregion

        #region Animation
        protected override void AnimationState()
        {
            base.AnimationState();
            //Update horse position
            transform.position = agent.nextPosition;
            SaveHorseVelocity();
            SaveOvertakeCameraData();
        }
        #endregion

        #region Save Info
        private void SaveHorseVelocity()
        {
            HorseVelocity horseDetail = new HorseVelocity();
            horseDetail.x = agent.velocity.x.ToString("F2");
            horseDetail.z = agent.velocity.z.ToString("F2");
            horseVelocityList.Add(horseDetail);
        }

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
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < horseVelocityList.Count; i++)
            {
                stringBuilder.Append($"{horseVelocityList[i].x},{horseVelocityList[i].z}!");
            }

            StringBuilder overtakeHorsesStringBuilder = new StringBuilder();
            for (int i = 0; i < overtakeVelocityIndexList.Count; i++)
            {
                overtakeHorsesStringBuilder.Append($"{overtakeVelocityIndexList[i]},");
            }
            horseData.overtakeData = overtakeHorsesStringBuilder.ToString();
            horseData.raceData = stringBuilder.ToString();
            return horseData;
        }
        #endregion
    }
}
