using System.Collections.Generic;
using UnityEngine;

#region Structs Container
public struct SplineSegment
{
    public int splineIndex;
    [Tooltip("Key : (ControlPointIndex),Value = Spline Points")]
    public Dictionary<int, SplineInfo> splineControlPoints;

    public List<Vector3> postFinishLineControlPoints;

    public SplineSegment(int splineIndex, Dictionary<int, SplineInfo> splineControlPoints)
    {
        this.splineIndex = splineIndex;
        this.splineControlPoints = splineControlPoints;
        this.postFinishLineControlPoints = new List<Vector3>();
    }
}
public struct SplineInfo
{
    public int splineIndex;
    public List<Vector3> splinePoints;
}
public struct ControlPointInfo
{
    public int splineIndex;
    public int controlPointIndex;
    public Transform controlPoint;
}

[System.Serializable]
public struct SplineHorseTracker
{
    public int splineIndex;
    public List<int> currentSplineHorses; // Horses that are in the current spline.
    public List<int> incomingHorses; // Horses that are going to join this spline from another spline.
    public List<int> outgoingHorses; // Horses that are going to leave this spline for another spline.
}
#endregion

namespace HorseRace
{
    public class SplineManager : MonoBehaviour
    {
        public static SplineManager Instance { get; private set; }

        #region Inspector Variables
        [SerializeField] private Spline[] splines;
        [SerializeField] private List<SplineHorseTracker> horsesTrackerList = new List<SplineHorseTracker>();
        [SerializeField] private bool canCreateSegments = true;
        #endregion

        #region Private Variables
        private List<SplineSegment> splineSegmentsList = new List<SplineSegment>();
        #endregion

        #region Unity Methods
        private void Awake()
        {
            Instance = this;
            CreateSplineSegments();
        }
        #endregion

        public bool CheckIfSplineExist(int index)
        {
            return index >= 0 && index < splineSegmentsList.Count;
        }

        public Vector3 GetPostFinishLineControlPoint(int splineIndex, int controlPoint)
        {
            return splineSegmentsList[splineIndex].postFinishLineControlPoints[controlPoint];
        }

        public Vector3 GetNearestControlPoint(int controlPointIndex, Vector3 pos)
        {
            float closestDistance = float.MaxValue;
            Vector3 closestPos = Vector3.zero;
            foreach (var spline in splines)
            {
                if (controlPointIndex < spline.ControlPoints.Count)
                {
                    float tempDistance = Vector3.Distance(spline.ControlPoints[controlPointIndex].position, pos);
                    if (tempDistance < closestDistance)
                    {
                        closestDistance = tempDistance;
                        closestPos = spline.ControlPoints[controlPointIndex].position;
                    }
                }
            }
            return closestPos;
        }

        private void CreateSplineSegments()
        {
            if (!canCreateSegments)
            {
                return;
            }

            foreach (var spline in splines)
            {
                //Get Control Points
                List<ControlPointInfo> controlPoints = new List<ControlPointInfo>();
                for (int controlPointIndex = 0; controlPointIndex < spline.ControlPoints.Count; controlPointIndex++)
                {
                    ControlPointInfo controlPointInfo = new ControlPointInfo
                    {
                        splineIndex = spline.SplineIndex,
                        controlPointIndex = controlPointIndex,
                        controlPoint = spline.ControlPoints[controlPointIndex],
                    };
                    controlPoints.Add(controlPointInfo);
                }

                //Get spline points
                Dictionary<int, SplineInfo> splineControlPointsDictionary = GetSplinePoints(controlPoints, spline.SegmentLength);
                SplineSegment splineSegment = new SplineSegment(spline.SplineIndex, splineControlPointsDictionary);
                splineSegment.postFinishLineControlPoints = spline.PostFinishLineControlPoints;

                splineSegmentsList.Add(splineSegment);
                horsesTrackerList.Add(new SplineHorseTracker
                {
                    splineIndex = spline.SplineIndex,
                    currentSplineHorses = new List<int>(),
                    incomingHorses = new List<int>(),
                    outgoingHorses = new List<int>(),
                });
            }
        }

        #region Horses Tracker In Spline
        public void ChangedSpline(int currentSplineIndex, int lastSplineIndex, int nextSplineIndex, int horseNumber)
        {
            horsesTrackerList[currentSplineIndex].currentSplineHorses.Add(horseNumber);
            horsesTrackerList[lastSplineIndex].currentSplineHorses.Remove(horseNumber);
            horsesTrackerList[lastSplineIndex].outgoingHorses.Remove(horseNumber);
            horsesTrackerList[nextSplineIndex].incomingHorses.Remove(horseNumber);
        }

        public List<int> GetCurrentSplineHorses(int splineINdex)
        {
            return horsesTrackerList[splineINdex].currentSplineHorses;
        }

        public List<int> GetIncomingHorses(int splineINdex)
        {
            return horsesTrackerList[splineINdex].incomingHorses;
        }

        public bool IsAnyHorseEnteringCurrentSpline(int splineIndex)
        {
            return horsesTrackerList[splineIndex].incomingHorses.Count > 0;
        }

        public List<int> GetOutgoingHorses(int splineINdex)
        {
            return horsesTrackerList[splineINdex].outgoingHorses;
        }

        public void HorseChangingSpline(int currentSplineIndex, int nextSplineIndex, int horseNumber)
        {
            horsesTrackerList[currentSplineIndex].outgoingHorses.Add(horseNumber);
            horsesTrackerList[nextSplineIndex].incomingHorses.Add(horseNumber);
        }
        #endregion

        public SplineSegment GetNeighborSplineSegment(int currentSplineIndex, int neighborSplineIndex, int currentControlPointIndex)
        {
            Spline currentSpline = splines[currentSplineIndex];
            Spline neighborSpline = splines[neighborSplineIndex];
            List<ControlPointInfo> controlPoints = new List<ControlPointInfo>();

            for (int i = 0; i < neighborSpline.ControlPoints.Count; i++)
            {
                //Create Control Point Info
                ControlPointInfo controlPointInfo = new ControlPointInfo
                {
                    controlPointIndex = i,
                };

                //Add Current Spline Control Points
                if (i < currentControlPointIndex + 1)
                {
                    controlPointInfo.splineIndex = currentSpline.SplineIndex;
                    controlPointInfo.controlPoint = currentSpline.ControlPoints[i];
                    controlPoints.Add(controlPointInfo);
                }
                //Add Neighbor Spline Control Points
                else
                {
                    controlPointInfo.splineIndex = neighborSpline.SplineIndex;
                    controlPointInfo.controlPoint = neighborSpline.ControlPoints[i];
                    controlPoints.Add(controlPointInfo);
                }
            }
            Dictionary<int, SplineInfo> splineControlPointsDictionary = GetSplinePoints(controlPoints, neighborSpline.SegmentLength);
            SplineSegment splineSegment = new SplineSegment(neighborSpline.SplineIndex, splineControlPointsDictionary);
            return splineSegment;
        }

        public SplineSegment GetSplineSegment(int splineIndex, int horseNumber)
        {
            horsesTrackerList[splineIndex].currentSplineHorses.Add(horseNumber);
            return splineSegmentsList[splineIndex];
        }

        private Vector3 GetCatmullRomPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return 0.5f * (
                (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                (-p0 + p2) * t +
                2f * p1
            );
        }

        /// <summary>
        /// Get the closest Position on the spline
        /// </summary>
        /// <param name="position"></param>
        /// <param name="splineIndex"></param>
        /// <param name="controlPointINdex"></param>
        /// <returns></returns>
        public float GetClosestT(Vector3 position, int splineIndex, int controlPointINdex)
        {
            float closestT = 0f;
            float closestDistance = float.MaxValue;
            float totalLength = 0f;
            float accumulatedLength = 0f;

            // Calculate the total length of the spline
            foreach (var kvp in splineSegmentsList[splineIndex].splineControlPoints)
            {
                List<Vector3> splinePoints = kvp.Value.splinePoints;
                for (int i = 0; i < splinePoints.Count - 1; i++)
                {
                    totalLength += Vector3.Distance(splinePoints[i], splinePoints[i + 1]);
                }
            }

            // Find the closest point on the spline
            foreach (var kvp in splineSegmentsList[splineIndex].splineControlPoints)
            {
                int controlPointKey = kvp.Key;
                List<Vector3> splinePoints = kvp.Value.splinePoints;
                for (int i = 0; i < splinePoints.Count - 1; i++)
                {
                    Vector3 p0 = splinePoints[i];
                    Vector3 p1 = splinePoints[i + 1];

                    // Project the position onto the line segment p0-p1
                    Vector3 direction = p1 - p0;
                    float length = direction.magnitude;
                    direction.Normalize();

                    float projection = Vector3.Dot(position - p0, direction);
                    projection = Mathf.Clamp(projection, 0, length);

                    Vector3 closestPoint = p0 + direction * projection;
                    float distance = Vector3.Distance(position, closestPoint);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestT = (accumulatedLength + projection) / totalLength;
                    }
                    accumulatedLength += length;
                }
            }
            return closestT;
        }

        private int ClampListPos(int pos, List<ControlPointInfo> controlPoints)
        {
            if (pos < 0) return 0;
            if (pos >= controlPoints.Count) return controlPoints.Count - 1;
            return pos;
        }

        public Dictionary<int, SplineInfo> GetSplinePoints(List<ControlPointInfo> controlPoints, float segmentLength)
        {
            var splinePoints = new Dictionary<int, SplineInfo>();

            foreach (var controlPointInfo in controlPoints)
            {
                int splineIndex = controlPointInfo.splineIndex;
                int controlPointIndex = controlPointInfo.controlPointIndex;
                Transform point = controlPointInfo.controlPoint;

                Vector3 p0 = controlPoints[ClampListPos(controlPointIndex - 1, controlPoints)].controlPoint.position;
                Vector3 p1 = point.position;
                Vector3 p2 = controlPoints[ClampListPos(controlPointIndex + 1, controlPoints)].controlPoint.position;
                Vector3 p3 = controlPoints[ClampListPos(controlPointIndex + 2, controlPoints)].controlPoint.position;

                if (!splinePoints.ContainsKey(controlPointIndex))
                {
                    splinePoints[controlPointIndex] = new SplineInfo
                    {
                        splineIndex = splineIndex,
                        splinePoints = new List<Vector3>()
                    };
                }

                for (float t = 0; t <= 1; t += segmentLength)
                {
                    Vector3 newPosition = GetCatmullRomPoint(t, p0, p1, p2, p3);
                    if (!splinePoints[controlPointIndex].splinePoints.Contains(newPosition))
                    {
                        splinePoints[controlPointIndex].splinePoints.Add(newPosition);
                    }
                }
            }

            return splinePoints;
        }
    }
}
