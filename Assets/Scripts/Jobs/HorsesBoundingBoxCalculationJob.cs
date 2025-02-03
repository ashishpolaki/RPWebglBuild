using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HorseRace
{
    public struct HorseSplineCollisionData
    {
        public void Dispose()
        {
            //Horse 1
            horse1SplinePoints.Dispose();
            horse1Corners.Dispose();
            horse1Positions.Dispose();
            horse1Rotations.Dispose();

            //Horse 2
            horse2SplinePoints.Dispose();
            horse2Corners.Dispose();
            horse2Positions.Dispose();
            horse2Rotations.Dispose();

            //Horse 3
            horse3SplinePoints.Dispose();
            horse3Corners.Dispose();
            horse3Positions.Dispose();
            horse3Rotations.Dispose();

            //Horse 4
            horse4SplinePoints.Dispose();
            horse4Corners.Dispose();
            horse4Positions.Dispose();
            horse4Rotations.Dispose();
        }

        public int GetMaxPositionsLength()
        {
            int maxCount = 0;

            // Check each horse's positions count and update maxCount if necessary
            maxCount = math.max(maxCount, horse1Positions.Length);
            maxCount = math.max(maxCount, horse2Positions.Length);
            maxCount = math.max(maxCount, horse3Positions.Length);
            maxCount = math.max(maxCount, horse4Positions.Length);

            maxCount = math.max(maxCount, horse5Positions.Length);
            maxCount = math.max(maxCount, horse6Positions.Length);
            maxCount = math.max(maxCount, horse7Positions.Length);
            maxCount = math.max(maxCount, horse8Positions.Length);
            maxCount = math.max(maxCount, horse9Positions.Length);
            maxCount = math.max(maxCount, horse10Positions.Length);
            maxCount = math.max(maxCount, horse11Positions.Length);
            maxCount = math.max(maxCount, horse12Positions.Length);
            return maxCount;
        }

        #region Horse Variables
        //Horse 1
        public NativeArray<float3> horse1SplinePoints;
        public NativeArray<float3> horse1Corners;
        public NativeList<float3> horse1Positions;
        public NativeList<quaternion> horse1Rotations;
        public float3 horse1CurrentPosition;
        public quaternion horse1CurrentRotation;
        public float horse1CurrentSpeed;
        public float horse1TargetSpeed;
        public int horse1SplinePointIndex;

        //Horse 2
        public NativeArray<float3> horse2SplinePoints;
        public NativeArray<float3> horse2Corners;
        public NativeList<float3> horse2Positions;
        public NativeList<quaternion> horse2Rotations;
        public float3 horse2CurrentPosition;
        public quaternion horse2CurrentRotation;
        public float horse2CurrentSpeed;
        public float horse2TargetSpeed;
        public int horse2SplinePointIndex;

        //Horse 3
        public NativeArray<float3> horse3SplinePoints;
        public NativeArray<float3> horse3Corners;
        public NativeList<float3> horse3Positions;
        public NativeList<quaternion> horse3Rotations;
        public float3 horse3CurrentPosition;
        public quaternion horse3CurrentRotation;
        public float horse3CurrentSpeed;
        public float horse3TargetSpeed;
        public int horse3SplinePointIndex;

        //Horse 4
        public NativeArray<float3> horse4SplinePoints;
        public NativeArray<float3> horse4Corners;
        public NativeList<float3> horse4Positions;
        public NativeList<quaternion> horse4Rotations;
        public float3 horse4CurrentPosition;
        public quaternion horse4CurrentRotation;
        public float horse4CurrentSpeed;
        public float horse4TargetSpeed;
        public int horse4SplinePointIndex;


        //Horse 5
        public NativeArray<float3> horse5SplinePoints;
        public NativeArray<float3> horse5Corners;
        public NativeList<float3> horse5Positions;
        public NativeList<quaternion> horse5Rotations;
        public float3 horse5CurrentPosition;
        public quaternion horse5CurrentRotation;
        public float horse5CurrentSpeed;
        public float horse5TargetSpeed;
        public int horse5SplinePointIndex;

        //Horse 6
        public NativeArray<float3> horse6SplinePoints;
        public NativeArray<float3> horse6Corners;
        public NativeList<float3> horse6Positions;
        public NativeList<quaternion> horse6Rotations;
        public float3 horse6CurrentPosition;
        public quaternion horse6CurrentRotation;
        public float horse6CurrentSpeed;
        public float horse6TargetSpeed;
        public int horse6SplinePointIndex;

        //Horse 7
        public NativeArray<float3> horse7SplinePoints;
        public NativeArray<float3> horse7Corners;
        public NativeList<float3> horse7Positions;
        public NativeList<quaternion> horse7Rotations;
        public float3 horse7CurrentPosition;
        public quaternion horse7CurrentRotation;
        public float horse7CurrentSpeed;
        public float horse7TargetSpeed;
        public int horse7SplinePointIndex;

        //Horse 8
        public NativeArray<float3> horse8SplinePoints;
        public NativeArray<float3> horse8Corners;
        public NativeList<float3> horse8Positions;
        public NativeList<quaternion> horse8Rotations;
        public float3 horse8CurrentPosition;
        public quaternion horse8CurrentRotation;
        public float horse8CurrentSpeed;
        public float horse8TargetSpeed;
        public int horse8SplinePointIndex;

        //Horse 9
        public NativeArray<float3> horse9SplinePoints;
        public NativeArray<float3> horse9Corners;
        public NativeList<float3> horse9Positions;
        public NativeList<quaternion> horse9Rotations;
        public float3 horse9CurrentPosition;
        public quaternion horse9CurrentRotation;
        public float horse9CurrentSpeed;
        public float horse9TargetSpeed;
        public int horse9SplinePointIndex;

        //Horse 10
        public NativeArray<float3> horse10SplinePoints;
        public NativeArray<float3> horse10Corners;
        public NativeList<float3> horse10Positions;
        public NativeList<quaternion> horse10Rotations;
        public float3 horse10CurrentPosition;
        public quaternion horse10CurrentRotation;
        public float horse10CurrentSpeed;
        public float horse10TargetSpeed;
        public int horse10SplinePointIndex;

        //Horse 11
        public NativeArray<float3> horse11SplinePoints;
        public NativeArray<float3> horse11Corners;
        public NativeList<float3> horse11Positions;
        public NativeList<quaternion> horse11Rotations;
        public float3 horse11CurrentPosition;
        public quaternion horse11CurrentRotation;
        public float horse11CurrentSpeed;
        public float horse11TargetSpeed;
        public int horse11SplinePointIndex;

        //Horse 12
        public NativeArray<float3> horse12SplinePoints;
        public NativeArray<float3> horse12Corners;
        public NativeList<float3> horse12Positions;
        public NativeList<quaternion> horse12Rotations;
        public float3 horse12CurrentPosition;
        public quaternion horse12CurrentRotation;
        public float horse12CurrentSpeed;
        public float horse12TargetSpeed;
        public int horse12SplinePointIndex;
        #endregion
    }

    [BurstCompile]
    public struct HorseBoundingBoxCalculationJob : IJob
    {
        public NativeReference<bool> isColliding;
        [ReadOnly] public float deltaTime;
        [ReadOnly] public float thresholdDistance;
        [ReadOnly] public float maxSpeed;
        [ReadOnly] public float acceleration;
        [ReadOnly] public float3 extents;

        //Horses Data
        public HorseSplineCollisionData horseSplineCollisionData;

        public void HorseMovement(ref int splinePointIndex, ref NativeArray<float3> splinePoints, ref NativeList<float3> positions, ref NativeList<quaternion> rotations,
            ref float currentSpeed, float targetSpeed, float maxSpeed, float acceleration, float deltaTime,
           ref float3 currentPosition, ref quaternion currentRotation, float3 boundingBoxExtents, ref NativeArray<float3> boundingBoxCorners)
        {
            if (splinePointIndex < splinePoints.Length)
            {
                float speedChange = acceleration * deltaTime;
                currentSpeed = currentSpeed < targetSpeed ? math.clamp(currentSpeed + speedChange, 0, targetSpeed) : math.clamp(currentSpeed - speedChange, targetSpeed, maxSpeed);
                float3 targetPosition = splinePoints[splinePointIndex];
                float distance = math.distance(currentPosition, targetPosition);
                float timeToReach = distance / currentSpeed;

                if (timeToReach > 0)
                {
                    // Update Position
                    float3 direction = math.normalize(targetPosition - currentPosition);
                    float3 newPosition = currentPosition + direction * currentSpeed * deltaTime;
                    currentPosition = newPosition;
                    positions.Add(newPosition);

                    //Update Rotation
                    quaternion targetRotation = quaternion.LookRotationSafe(direction, new float3(0, 1, 0));
                    quaternion smoothRotation = math.slerp(currentRotation, targetRotation, deltaTime * currentSpeed);
                    float3x3 rotationMatrix1 = new float3x3(smoothRotation);
                    currentRotation = smoothRotation;
                    rotations.Add(smoothRotation);

                    // Calculate the corners 
                    float3 newCenter1 = newPosition;
                    NativeArray<float3> localCorners1 = new NativeArray<float3>(8, Allocator.Temp)
                    {
                        [0] = new float3(-boundingBoxExtents.x, -boundingBoxExtents.y, -boundingBoxExtents.z),
                        [1] = new float3(boundingBoxExtents.x, -boundingBoxExtents.y, -boundingBoxExtents.z),
                        [2] = new float3(-boundingBoxExtents.x, boundingBoxExtents.y, -boundingBoxExtents.z),
                        [3] = new float3(boundingBoxExtents.x, boundingBoxExtents.y, -boundingBoxExtents.z),
                        [4] = new float3(-boundingBoxExtents.x, -boundingBoxExtents.y, boundingBoxExtents.z),
                        [5] = new float3(boundingBoxExtents.x, -boundingBoxExtents.y, boundingBoxExtents.z),
                        [6] = new float3(-boundingBoxExtents.x, boundingBoxExtents.y, boundingBoxExtents.z),
                        [7] = new float3(boundingBoxExtents.x, boundingBoxExtents.y, boundingBoxExtents.z)
                    };
                    for (int i = 0; i < 8; i++)
                    {
                        boundingBoxCorners[i] = math.mul(rotationMatrix1, localCorners1[i]) + newCenter1;
                    }
                }
                if (distance < thresholdDistance)
                {
                    splinePointIndex++;
                    if (splinePointIndex + 1 == splinePoints.Length)
                    {
                        thresholdDistance = 0.3f;
                    }
                }
            }
        }

        public void Execute()
        {
            isColliding.Value = false;

            while (horseSplineCollisionData.horse1SplinePointIndex < horseSplineCollisionData.horse1SplinePoints.Length || horseSplineCollisionData.horse1CurrentSpeed <= 0)
            {
                //If current horse speed is zero, then break the loop.
                if (horseSplineCollisionData.horse1CurrentSpeed <= 0)
                {
                    break;
                }

                #region Horse Movement

                // Horse 1
                HorseMovement(ref horseSplineCollisionData.horse1SplinePointIndex, ref horseSplineCollisionData.horse1SplinePoints, ref horseSplineCollisionData.horse1Positions, ref horseSplineCollisionData.horse1Rotations,
                    ref horseSplineCollisionData.horse1CurrentSpeed, horseSplineCollisionData.horse1TargetSpeed, maxSpeed, acceleration, deltaTime,
                    ref horseSplineCollisionData.horse1CurrentPosition, ref horseSplineCollisionData.horse1CurrentRotation, extents, ref horseSplineCollisionData.horse1Corners);

                //Horse 2
                HorseMovement(ref horseSplineCollisionData.horse2SplinePointIndex, ref horseSplineCollisionData.horse2SplinePoints, ref horseSplineCollisionData.horse2Positions, ref horseSplineCollisionData.horse2Rotations,
                ref horseSplineCollisionData.horse2CurrentSpeed, horseSplineCollisionData.horse2TargetSpeed, maxSpeed, acceleration, deltaTime,
                ref horseSplineCollisionData.horse2CurrentPosition, ref horseSplineCollisionData.horse2CurrentRotation, extents, ref horseSplineCollisionData.horse2Corners);

                //Horse 3
                HorseMovement(ref horseSplineCollisionData.horse3SplinePointIndex, ref horseSplineCollisionData.horse3SplinePoints, ref horseSplineCollisionData.horse3Positions, ref horseSplineCollisionData.horse3Rotations,
                ref horseSplineCollisionData.horse3CurrentSpeed, horseSplineCollisionData.horse3TargetSpeed, maxSpeed, acceleration, deltaTime,
                ref horseSplineCollisionData.horse3CurrentPosition, ref horseSplineCollisionData.horse3CurrentRotation, extents, ref horseSplineCollisionData.horse3Corners);

                //Horse 4
                HorseMovement(ref horseSplineCollisionData.horse4SplinePointIndex, ref horseSplineCollisionData.horse4SplinePoints, ref horseSplineCollisionData.horse4Positions, ref horseSplineCollisionData.horse4Rotations,
                    ref horseSplineCollisionData.horse4CurrentSpeed, horseSplineCollisionData.horse4TargetSpeed, maxSpeed, acceleration, deltaTime,
                    ref horseSplineCollisionData.horse4CurrentPosition, ref horseSplineCollisionData.horse4CurrentRotation, extents, ref horseSplineCollisionData.horse4Corners);

                //Horse 5 
                HorseMovement(ref horseSplineCollisionData.horse5SplinePointIndex, ref horseSplineCollisionData.horse5SplinePoints, ref horseSplineCollisionData.horse5Positions, ref horseSplineCollisionData.horse5Rotations,
                ref horseSplineCollisionData.horse5CurrentSpeed, horseSplineCollisionData.horse5TargetSpeed, maxSpeed, acceleration, deltaTime,
                ref horseSplineCollisionData.horse5CurrentPosition, ref horseSplineCollisionData.horse5CurrentRotation, extents, ref horseSplineCollisionData.horse5Corners);

                //Horse 6
                HorseMovement(ref horseSplineCollisionData.horse6SplinePointIndex, ref horseSplineCollisionData.horse6SplinePoints, ref horseSplineCollisionData.horse6Positions, ref horseSplineCollisionData.horse6Rotations,
                  ref horseSplineCollisionData.horse6CurrentSpeed, horseSplineCollisionData.horse6TargetSpeed, maxSpeed, acceleration, deltaTime,
                  ref horseSplineCollisionData.horse6CurrentPosition, ref horseSplineCollisionData.horse6CurrentRotation, extents, ref horseSplineCollisionData.horse6Corners);
                // Horse 7
                HorseMovement(ref horseSplineCollisionData.horse7SplinePointIndex, ref horseSplineCollisionData.horse7SplinePoints, ref horseSplineCollisionData.horse7Positions, ref horseSplineCollisionData.horse7Rotations,
                              ref horseSplineCollisionData.horse7CurrentSpeed, horseSplineCollisionData.horse7TargetSpeed, maxSpeed, acceleration, deltaTime,
                              ref horseSplineCollisionData.horse7CurrentPosition, ref horseSplineCollisionData.horse7CurrentRotation, extents, ref horseSplineCollisionData.horse7Corners);

                // Horse 8
                HorseMovement(ref horseSplineCollisionData.horse8SplinePointIndex, ref horseSplineCollisionData.horse8SplinePoints, ref horseSplineCollisionData.horse8Positions, ref horseSplineCollisionData.horse8Rotations,
                              ref horseSplineCollisionData.horse8CurrentSpeed, horseSplineCollisionData.horse8TargetSpeed, maxSpeed, acceleration, deltaTime,
                              ref horseSplineCollisionData.horse8CurrentPosition, ref horseSplineCollisionData.horse8CurrentRotation, extents, ref horseSplineCollisionData.horse8Corners);

                // Horse 9
                HorseMovement(ref horseSplineCollisionData.horse9SplinePointIndex, ref horseSplineCollisionData.horse9SplinePoints, ref horseSplineCollisionData.horse9Positions, ref horseSplineCollisionData.horse9Rotations,
                              ref horseSplineCollisionData.horse9CurrentSpeed, horseSplineCollisionData.horse9TargetSpeed, maxSpeed, acceleration, deltaTime,
                              ref horseSplineCollisionData.horse9CurrentPosition, ref horseSplineCollisionData.horse9CurrentRotation, extents, ref horseSplineCollisionData.horse9Corners);

                // Horse 10
                HorseMovement(ref horseSplineCollisionData.horse10SplinePointIndex, ref horseSplineCollisionData.horse10SplinePoints, ref horseSplineCollisionData.horse10Positions, ref horseSplineCollisionData.horse10Rotations,
                              ref horseSplineCollisionData.horse10CurrentSpeed, horseSplineCollisionData.horse10TargetSpeed, maxSpeed, acceleration, deltaTime,
                              ref horseSplineCollisionData.horse10CurrentPosition, ref horseSplineCollisionData.horse10CurrentRotation, extents, ref horseSplineCollisionData.horse10Corners);

                // Horse 11
                HorseMovement(ref horseSplineCollisionData.horse11SplinePointIndex, ref horseSplineCollisionData.horse11SplinePoints, ref horseSplineCollisionData.horse11Positions, ref horseSplineCollisionData.horse11Rotations,
                              ref horseSplineCollisionData.horse11CurrentSpeed, horseSplineCollisionData.horse11TargetSpeed, maxSpeed, acceleration, deltaTime,
                              ref horseSplineCollisionData.horse11CurrentPosition, ref horseSplineCollisionData.horse11CurrentRotation, extents, ref horseSplineCollisionData.horse11Corners);

                // Horse 12
                HorseMovement(ref horseSplineCollisionData.horse12SplinePointIndex, ref horseSplineCollisionData.horse12SplinePoints, ref horseSplineCollisionData.horse12Positions, ref horseSplineCollisionData.horse12Rotations,
                              ref horseSplineCollisionData.horse12CurrentSpeed, horseSplineCollisionData.horse12TargetSpeed, maxSpeed, acceleration, deltaTime,
                              ref horseSplineCollisionData.horse12CurrentPosition, ref horseSplineCollisionData.horse12CurrentRotation, extents, ref horseSplineCollisionData.horse12Corners);

                #endregion

                // Check for collision
                for (int i = 2; i <= 12; i++)
                {
                    NativeArray<float3> horseCorners = i switch
                    {
                        2 => horseSplineCollisionData.horse2Corners,
                        3 => horseSplineCollisionData.horse3Corners,
                        4 => horseSplineCollisionData.horse4Corners,
                        5 => horseSplineCollisionData.horse5Corners,
                        6 => horseSplineCollisionData.horse6Corners,
                        7 => horseSplineCollisionData.horse7Corners,
                        8 => horseSplineCollisionData.horse8Corners,
                        9 => horseSplineCollisionData.horse9Corners,
                        10 => horseSplineCollisionData.horse10Corners,
                        11 => horseSplineCollisionData.horse11Corners,
                        12 => horseSplineCollisionData.horse12Corners,
                        _ => throw new ArgumentOutOfRangeException(nameof(i), "Invalid horse index")
                    };

                    isColliding.Value = AreBoundingBoxesColliding(horseSplineCollisionData.horse1Corners, horseCorners);
                    if (isColliding.Value)
                    {
                        break;
                    }
                }
                if (isColliding.Value)
                {
                    break;
                }
            }
        }


        private bool AreBoundingBoxesColliding(NativeArray<float3> corners1, NativeArray<float3> corners2)
        {
            // Get the axes to test (normals of the faces of the bounding boxes)
            NativeArray<float3> axes = new NativeArray<float3>(15, Allocator.Temp);
            axes[0] = math.normalize(corners1[1] - corners1[0]);
            axes[1] = math.normalize(corners1[2] - corners1[0]);
            axes[2] = math.normalize(corners1[4] - corners1[0]);
            axes[3] = math.normalize(corners2[1] - corners2[0]);
            axes[4] = math.normalize(corners2[2] - corners2[0]);
            axes[5] = math.normalize(corners2[4] - corners2[0]);

            // Cross products of edges to get additional axes
            int index = 6;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 3; j < 6; j++)
                {
                    float3 crossProduct = math.cross(axes[i], axes[j]);
                    if (!math.all(crossProduct == float3.zero))
                    {
                        axes[index++] = math.normalize(crossProduct);
                    }
                }
            }

            // Check for overlap on all axes
            foreach (float3 axis in axes)
            {
                if (!IsOverlappingOnAxis(corners1, corners2, axis))
                {
                    return false; // Separating axis found, no collision
                }
            }

            return true; // No separating axis found, collision detected
        }

        private bool IsOverlappingOnAxis(NativeArray<float3> corners1, NativeArray<float3> corners2, float3 axis)
        {
            // Project all corners onto the axis and find the min and max values
            float min1 = float.MaxValue, max1 = float.MinValue;
            float min2 = float.MaxValue, max2 = float.MinValue;

            for (int i = 0; i < 8; i++)
            {
                float projection1 = math.dot(corners1[i], axis);
                min1 = math.min(min1, projection1);
                max1 = math.max(max1, projection1);

                float projection2 = math.dot(corners2[i], axis);
                min2 = math.min(min2, projection2);
                max2 = math.max(max2, projection2);
            }

            // Check for overlap
            return max1 >= min2 && max2 >= min1;
        }
    }
}