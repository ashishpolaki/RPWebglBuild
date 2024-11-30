using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace HorseRace
{
    public class CatmullRomSpline : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private int splineIndex;
        [SerializeField] private List<Transform> controlPoints; // Define your control points in the Inspector
        [SerializeField] private List<Vector3> afterFinishLineControlPoints = new List<Vector3>();
        [SerializeField] private float segmentLength = 0.05f;
        #endregion

        #region Properties
        public List<Transform> ControlPoints { get { return controlPoints; } }
        public List<Vector3> AfterFinishLineControlPoints { get { return afterFinishLineControlPoints; } }
        public int SplineIndex { get { return splineIndex; } }
        public float SegmentLength { get { return segmentLength; } }
        #endregion

#if UNITY_EDITOR
        [SerializeField] private bool canDrawGizmos;
        [SerializeField] private bool canValidate;
        [SerializeField] private int controlPointsCount;
        [SerializeField] private int2 afterFinishLineControlPointsRange;
        private void OnValidate()
        {
            if (transform.childCount <= 0 || !canValidate)
                return;

            controlPoints = new List<Transform>(transform.childCount);
            int index = 0;
            for (int i = 0; i < controlPointsCount; i++)
            {
                if (index == transform.childCount)
                {
                    index = 0;
                }

                controlPoints.Add(transform.GetChild(index));

                //Name the control points
                if (i < transform.childCount)
                {
                    controlPoints[i].gameObject.name = $"ControlPoint ({splineIndex}) ({i + 1})";
                }
                index++;
            }

            //Finish Line Control Points
            afterFinishLineControlPoints = new List<Vector3>();
            for (int i = afterFinishLineControlPointsRange.x; i < afterFinishLineControlPointsRange.y; i++)
            {
                afterFinishLineControlPoints.Add(controlPoints[i].position);
            }
        }
        private void OnDrawGizmos()
        {
            if (canDrawGizmos)
            {
                SplineHelper splineHelper = new SplineHelper();
                Dictionary<int, List<Vector3>> splinePoints = splineHelper.GetSplinePoints(controlPoints.ToArray(), segmentLength);
                for (int i = 0; i < splinePoints.Values.Count; i++)
                {
                    int queueCount = splinePoints[i].Count;

                    for (int j = 0; j < queueCount - 1; j++)
                    {
                        var splinePoint = splinePoints[i][j];
                        Gizmos.DrawSphere(splinePoint, 0.1f); // Draw a sphere at the calculated position
                        Debug.DrawLine(splinePoint, splinePoints[i][j + 1], Color.red);
                    }
                }
            }
        }
        public struct SplineHelper
        {
            private Transform[] controlPoints;
            private Dictionary<int, List<Vector3>> splinePoints;

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
            public Dictionary<int, List<Vector3>> GetSplinePoints(Transform[] _controlPoints, float segmentLength)
            {
                splinePoints = new Dictionary<int, List<Vector3>>();
                controlPoints = _controlPoints;
                for (int i = 0; i < controlPoints.Length - 1; i++)
                {
                    Vector3 p0 = controlPoints[ClampListPos(i - 1)].position;
                    Vector3 p1 = controlPoints[i].position;
                    Vector3 p2 = controlPoints[ClampListPos(i + 1)].position;
                    Vector3 p3 = controlPoints[ClampListPos(i + 2)].position;
                    splinePoints[i] = new List<Vector3>();

                    for (float t = 0; t <= 1; t += segmentLength) // Adjust the step for visual representation
                    {
                        Vector3 newPosition = GetCatmullRomPoint(t, p0, p1, p2, p3);
                        if (!splinePoints[i].Contains(newPosition))
                        {
                            splinePoints[i].Add(newPosition);
                        }
                    }
                }
                return splinePoints;
            }
        }
#endif
    }

}