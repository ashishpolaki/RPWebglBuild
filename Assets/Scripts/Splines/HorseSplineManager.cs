using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HorseSplineManager : MonoBehaviour
{
    public static HorseSplineManager Instance;

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Inspector Variables
    [SerializeField] private int Laps;
    [SerializeField] private float segmentLength = 0.2f;
    [SerializeField] private int controlPointLength;
    [Tooltip("Horse Collider Extents")]
    [SerializeField] private float3 extents;
    [SerializeField] private float changeThresholdDistance = 3;
    [SerializeField] private float forwardHorseThresholdDistance = 6;
    [SerializeField] private List<HorseSpline> HorseSplinesList;
    [SerializeField] private List<SplineTracker> SplineHorseTrackerList = new List<SplineTracker>();
    [SerializeField] private List<SplineData> splineDataList = new List<SplineData>();
    #endregion

    #region Properties
    public float ChangeThresholdDistance => changeThresholdDistance;
    public float ForwardHorseThresholdDistance => forwardHorseThresholdDistance;
    public float3 Extents => extents;
    public int TotalSplinesCount => splineDataList.Count;
    public int ControlPointLength => controlPointLength;
    #endregion

#if UNITY_EDITOR
    [Space(10), Header("Editor")]
    public bool validateControlPointGroups;
    public bool validateControlPointsInSplines;
    public bool validateCreateSplinePoints;
    public bool canDrawGizmos;
    public GameObject controlPointsParent;

    private void OnValidate()
    {
        if (validateControlPointGroups)
        {
            //Set Horse Control Point index
            for (int i = 0; i < controlPointsParent.transform.childCount; i++)
            {
                HorseControlPoint[] horseControlPoints = controlPointsParent.transform.GetComponentsInChildren<HorseControlPoint>();
                for (int j = 0; j < horseControlPoints.Length; j++)
                {
                    //Set Control PointData
                    HorseControlPoint horseControlPoint = horseControlPoints[j];
                    horseControlPoint.gameObject.name = $"ControlPoint Group ({j + 1})";
                    horseControlPoint.controlPoint.index = j + 1;
                }
            }
        }

        if (validateControlPointsInSplines)
        {
            for (int i = 0; i < HorseSplinesList.Count; i++)
            {
                HorseSplinesList[i].controlPoints.Clear();
                HorseSplinesList[i].splineID = i + 1;
            }

            HorseControlPoint[] horseControlPoints = controlPointsParent.transform.GetComponentsInChildren<HorseControlPoint>();
            //Add childrens of transform with controlpoint data to splines
            foreach (HorseControlPoint horseControlPoint in horseControlPoints)
            {
                foreach (HorseSpline horseSpline in HorseSplinesList)
                {
                    horseSpline.controlPoints.Add(new ControlPoint()
                    {
                        index = horseControlPoint.controlPoint.index,
                        Position = horseControlPoint.transform.GetChild(horseSpline.splineID - 1).position,
                        PriorityDirection = horseControlPoint.controlPoint.PriorityDirection
                    });
                }
            }
        }

        // Draw splines
        if (validateCreateSplinePoints)
        {
            //Set Control Point Length
            controlPointLength = 0;
            for (float t = 0; t <= 1; t += segmentLength)
            {
                controlPointLength++;
            }

            //Create spline points for each spline
            splineDataList.Clear();
            foreach (HorseSpline horseSpline in HorseSplinesList)
            {
                SplineHelper splineHelper = new SplineHelper();
                List<SplineData.SplinePointData> splinePoints = splineHelper.GetSplinePoints(horseSpline.controlPoints.ToArray(), segmentLength, Laps);

                SplineData splineData = new SplineData
                {
                    splineIndex = horseSpline.splineID,
                    controlPoints = horseSpline.controlPoints,
                    splinePoints = splinePoints
                };

                splineDataList.Add(splineData);
            }

            //Create spline tracker list with spline index
            SplineHorseTrackerList.Clear();
            foreach (HorseSpline horseSpline in HorseSplinesList)
            {
                SplineTracker splineTracker = new SplineTracker
                {
                    splineIndex = horseSpline.splineID,
                    horsesInCurrentSpline = new List<int>(),
                    horsesIncomingToSpline = new List<int>(),
                    horsesOutgoingFromSpline = new List<int>()
                };
                SplineHorseTrackerList.Add(splineTracker);
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (!canDrawGizmos)
        {
            return;
        }
        foreach (var splineData in splineDataList)
        {
            var splinePoints = splineData.splinePoints;
            for (int i = 0; i < splinePoints.Count - 1; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(splinePoints[i].position, 0.1f); // Draw a sphere at the calculated position
                Gizmos.color = Color.red;
                Gizmos.DrawLine(splinePoints[i].position, splinePoints[i + 1].position);
            }
        }
    }

#endif

    public SplineData SetSplineWithCustomControlPoints(List<int> splineIndexesPerControlPoint)
    {
        SplineData splineData = new SplineData();
        SplineHelper splineHelper = new SplineHelper();
        ControlPoint[] controlPoints = new ControlPoint[splineIndexesPerControlPoint.Count];
        for (int i = 0; i < splineIndexesPerControlPoint.Count; i++)
        {
            int splineIndex = splineIndexesPerControlPoint[i];
            int splineArrayIndex = splineIndex - 1;

            controlPoints[i] = (HorseSplinesList[splineArrayIndex].controlPoints[i]);
        }
        List<SplineData.SplinePointData> splinePoints = splineHelper.GetSplinePoints(controlPoints, segmentLength, Laps);
        splineData.splineIndex = splineIndexesPerControlPoint[0];
        splineData.splinePoints = splinePoints;
        return splineData;
    }

    public Direction GetNextDirection(int controlPointIndex)
    {
        //Take any spline because priority direction is same for all splines
        int splineArrayIndex = 0;
        int controlPointINdexArray = controlPointIndex - 1;
        return splineDataList[splineArrayIndex].controlPoints[controlPointINdexArray].PriorityDirection;
    }


    public SplineData InitializeSpline(int requestSplineIndex, int horseNumber)
    {
        int splineArrayIndex = requestSplineIndex - 1;
        //Add horse to the current spline
        SplineHorseTrackerList[splineArrayIndex].horsesInCurrentSpline.Add(horseNumber);
        return splineDataList[splineArrayIndex];
    }

    public SplineData GetSplineData(int currentSplineIndex, int nextSplineIndex, int controlPointIndex)
    {
        int currentSplineArrayIndex = currentSplineIndex - 1;
        int nextSplineArrayIndex = nextSplineIndex - 1;

        // Get the current control points up to the specified index
        List<ControlPoint> currentControlPoints = splineDataList[currentSplineArrayIndex].controlPoints.GetRange(0, controlPointIndex);

        // Get the future control points from the specified index onward
        List<ControlPoint> futureControlPoints = splineDataList[nextSplineArrayIndex].controlPoints.GetRange(controlPointIndex, splineDataList[nextSplineArrayIndex].controlPoints.Count - controlPointIndex);

        // Combine the current and future control points
        List<ControlPoint> combinedControlPoints = new List<ControlPoint>();
        combinedControlPoints.AddRange(currentControlPoints);
        combinedControlPoints.AddRange(futureControlPoints);

        // Use the SplineHelper class to generate the new spline points
        SplineHelper splineHelper = new SplineHelper();
        List<SplineData.SplinePointData> newSplinePoints = splineHelper.GetSplinePoints(combinedControlPoints.ToArray(), segmentLength, Laps);

        // Create a new SplineData object with the combined control points and new spline points
        SplineData newSplineData = new SplineData
        {
            splineIndex = nextSplineIndex, // Assign a new index for the new spline
            controlPoints = combinedControlPoints,
            splinePoints = newSplinePoints
        };

        return newSplineData;
    }

    public float GetDistanceCoveredAtSplinePoint(int splinePointIndex)
    {
        int splineArrayIndex = 7;
        if (splinePointIndex < splineDataList[splineArrayIndex].splinePoints.Count)
        {
            return splineDataList[splineArrayIndex].splinePoints[splinePointIndex].distanceCovered;
        }
        else
        {
            //Return the last spline point distance covered
            return splineDataList[splineArrayIndex].splinePoints[splineDataList[splineArrayIndex].splinePoints.Count - 1].distanceCovered;
        }
    }

    #region Spline Trackers
    public void HorseChangingSpline(int currentSplineArrayIndex, int nextSplineArrayIndex, int horseNumber)
    {
        // If the current spline is the same as the next spline, do nothing.
        if (currentSplineArrayIndex == nextSplineArrayIndex)
        {
            return;
        }

        //Current Spline
        SplineHorseTrackerList[currentSplineArrayIndex].horsesInCurrentSpline.Remove(horseNumber);
        SplineHorseTrackerList[currentSplineArrayIndex].horsesOutgoingFromSpline.Add(horseNumber);

        // Next Spline
        SplineHorseTrackerList[nextSplineArrayIndex].horsesIncomingToSpline.Add(horseNumber);
    }

    /// <summary>
    /// Reached New Spline from previous spline successfully.
    /// </summary>
    /// <param name="splineIndex"></param>
    /// <param name="previousSplineINdex"></param>
    /// <param name="horseNumber"></param>
    public void HorseReachedSpline(int splineIndex, int previousSplineINdex, int horseNumber)
    {
        int splineArrayIndex = splineIndex - 1;
        int previousSplineArrayIndex = previousSplineINdex - 1;

        //Add Horse to the current spline tracker
        SplineHorseTrackerList[splineArrayIndex].horsesInCurrentSpline.Add(horseNumber);
        SplineHorseTrackerList[splineArrayIndex].horsesIncomingToSpline.Remove(horseNumber);

        //Remove Data from previous spline tracker
        SplineHorseTrackerList[previousSplineArrayIndex].horsesOutgoingFromSpline.Remove(horseNumber);
    }

    public List<int> GetSplineHorses(int splineIndex)
    {
        int splineArrayIndex = splineIndex - 1;
        if (splineArrayIndex < 0 || splineArrayIndex >= splineDataList.Count)
        {
            return new List<int>();
        }
        List<int> horsesList = new List<int>(SplineHorseTrackerList[splineArrayIndex].horsesInCurrentSpline);
        horsesList.AddRange(SplineHorseTrackerList[splineArrayIndex].horsesIncomingToSpline);
        horsesList.AddRange(SplineHorseTrackerList[splineArrayIndex].horsesOutgoingFromSpline);
        return horsesList;
    }

    public List<int> GetHorsesCurrentlyInSpline(int splineIndex)
    {
        int splineArrayIndex = splineIndex - 1;
        if (splineArrayIndex < 0 || splineArrayIndex >= splineDataList.Count)
        {
            return new List<int>();
        }
        return SplineHorseTrackerList[splineArrayIndex].horsesInCurrentSpline;
    }
    public List<int> GetIncomingHorsesInSpline(int splineIndex)
    {
        int splineArrayIndex = splineIndex - 1;
        if (splineArrayIndex < 0 || splineArrayIndex >= splineDataList.Count)
        {
            return new List<int>();
        }
        return SplineHorseTrackerList[splineArrayIndex].horsesIncomingToSpline;
    }
    public List<int> GetOutgoingHorsesInSpline(int splineIndex)
    {
        int splineArrayIndex = splineIndex - 1;
        if (splineArrayIndex < 0 || splineArrayIndex >= splineDataList.Count)
        {
            return new List<int>();
        }
        return SplineHorseTrackerList[splineArrayIndex].horsesOutgoingFromSpline;
    }
    #endregion

}

#region Utils

[System.Serializable]
public struct SplineTracker
{
    public int splineIndex;
    public List<int> horsesInCurrentSpline; // Horses that are in the current spline.
    public List<int> horsesIncomingToSpline; // Horses that are going to join this spline from another spline.
    public List<int> horsesOutgoingFromSpline; // Horses that are going to leave this spline for another spline.
}

[System.Serializable]
public struct SplineData
{
    public int splineIndex;
    public List<ControlPoint> controlPoints;
    public List<SplinePointData> splinePoints;

    [System.Serializable]
    public struct SplinePointData
    {
        public int lap;
        public int controlPointIndex;
        public int splinePointIndex;
        public Vector3 position;
        public float distanceCovered;
    }
}

[System.Serializable]
public struct ControlPoint
{
    public int index;
    public Vector3 Position;
    public Direction PriorityDirection;
}
public enum Direction
{
    None,
    Left,
    Right,
    Either // or LeftOrRight
}
public class SplineHelper
{
    private ControlPoint[] controlPoints;

    private int ClampListPos(int pos)
    {
        if (pos < 0) return 0;
        if (pos >= controlPoints.Length) return controlPoints.Length - 1;
        return pos;
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


    public List<SplineData.SplinePointData> GetSplinePoints(ControlPoint[] _controlPoints, float segmentLength, int laps)
    {
        List<SplineData.SplinePointData> splinePoints = new List<SplineData.SplinePointData>();
        controlPoints = _controlPoints;
        float totalDistance = 0f;

        for (int lap = 0; lap < laps; lap++)
        {
            for (int i = 0; i < controlPoints.Length - 1; i++)
            {
                Vector3 p0 = controlPoints[ClampListPos(i - 1)].Position;
                Vector3 p1 = controlPoints[i].Position;
                Vector3 p2 = controlPoints[ClampListPos(i + 1)].Position;
                Vector3 p3 = controlPoints[ClampListPos(i + 2)].Position;

                Vector3 previousPosition = p1;

                for (float t = 0; t <= 1; t += segmentLength) // Adjust the step for visual representation
                {
                    Vector3 newPosition = GetCatmullRomPoint(t, p0, p1, p2, p3);
                    float distance = Vector3.Distance(previousPosition, newPosition);
                    totalDistance += distance;

                    splinePoints.Add(new SplineData.SplinePointData
                    {
                        lap = lap + 1,
                        controlPointIndex = i + 1,
                        splinePointIndex = splinePoints.Count,
                        position = newPosition,
                        distanceCovered = totalDistance
                    });

                    previousPosition = newPosition;
                }
            }
        }
        return splinePoints;
    }
    #endregion

}