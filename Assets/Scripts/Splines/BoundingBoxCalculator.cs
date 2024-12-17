using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

//Test Script
public class BoundingBoxCalculator : MonoBehaviour
{
    public bool onValidate = false;

    public NativeArray<bool> collisionResult;
    public float coroutineSpeed = 0.1f;
    public float acceleration;
    public float maxSpeed;
    public float thresholdDistance;
    public float3 extents;

    [Space(10), Header("Horse 1")]
    public List<Vector3> splinePointsList1;
    public Color colliderColor1 = Color.green;
    public NativeArray<float3> splinePoints1;
    private NativeArray<float3> corners1;
    public NativeList<float3> positions1;
    public NativeList<quaternion> quaternions1;
    public float targetSpeed;
    public float currentSpeed;
    public Vector3 startPosition1;

    [Space(10), Header("Horse 2")]
    public List<Vector3> splinePointsList2;
    public Color colliderColor2 = Color.green;
    public NativeArray<float3> splinePoints2;
    private NativeArray<float3> corners2;
    public NativeList<float3> positions2;
    public NativeList<quaternion> quaternions2;
    public float targetSpeed2;
    public float currentSpeed2;
    public Vector3 startPosition2;
    public float timescale;

    void Start()
    {
        Time.timeScale = timescale;
        corners1 = new NativeArray<float3>(8, Allocator.Persistent);
        positions1 = new NativeList<float3>(Allocator.Persistent);
        quaternions1 = new NativeList<quaternion>(Allocator.Persistent);
        splinePoints1 = new NativeArray<float3>(splinePointsList1.Count, Allocator.Persistent);

        corners2 = new NativeArray<float3>(8, Allocator.Persistent);
        positions2 = new NativeList<float3>(Allocator.Persistent);
        quaternions2 = new NativeList<quaternion>(Allocator.Persistent);
        splinePoints2 = new NativeArray<float3>(splinePointsList2.Count, Allocator.Persistent);

        collisionResult = new NativeArray<bool>(1, Allocator.Persistent);

        for (int i = 0; i < splinePointsList1.Count; i++)
        {
            splinePoints1[i] = splinePointsList1[i];
        }

        for (int i = 0; i < splinePointsList2.Count; i++)
        {
            splinePoints2[i] = splinePointsList2[i];
        }

        TestHorseBoundingBoxCalculationJob horseBoundingBoxCalculationJob = new TestHorseBoundingBoxCalculationJob
        {
            deltaTime = Time.fixedDeltaTime,
            isColliding = collisionResult,
            acceleration = acceleration,
            thresholdDistance = thresholdDistance,
            maxSpeed = maxSpeed,

            //Horse 1
            splinePoints1 = splinePoints1,
            corners1 = corners1,
            positions1 = positions1,
            quaternions1 = quaternions1,
            extents1 = extents,
            lastPosition1 = startPosition1,
            lastRotation1 = quaternion.identity,
            currentSpeed1 = currentSpeed,
            targetSpeed1 = targetSpeed,

            //Horse 2
            splinePoints2 = splinePoints2,
            corners2 = corners2,
            positions2 = positions2,
            quaternions2 = quaternions2,
            extents2 = extents,
            lastPosition2 = startPosition2,
            lastRotation2 = quaternion.identity,
            currentSpeed2 = currentSpeed2,
            targetSpeed2 = targetSpeed2,
        };
        JobHandle jobHandle = horseBoundingBoxCalculationJob.Schedule();
        jobHandle.Complete();
        // Check for collision
        if (collisionResult[0])
        {
            Debug.Log("Bounding boxes are colliding!");
        }
        Time.timeScale = 1f;
        StartCoroutine(VisualizeBoundingBoxes(positions1, quaternions1, positions2, quaternions2));
    }
    private void OnDestroy()
    {
        if (corners1.IsCreated)
        {
            corners1.Dispose();
        }
        if (positions1.IsCreated)
        {
            positions1.Dispose();
        }
        if (quaternions1.IsCreated)
        {
            quaternions1.Dispose();
        }
        if (splinePoints1.IsCreated)
        {
            splinePoints1.Dispose();
        }
        if (collisionResult.IsCreated)
        {
            collisionResult.Dispose();
        }
        if (corners2.IsCreated)
        {
            corners2.Dispose();
        }
        if (positions2.IsCreated)
        {
            positions2.Dispose();
        }
        if (quaternions2.IsCreated)
        {
            quaternions2.Dispose();
        }
        if (splinePoints2.IsCreated)
        {
            splinePoints2.Dispose();
        }
    }

    private IEnumerator VisualizeBoundingBoxes(NativeList<float3> positions1, NativeList<quaternion> quaternions1,
        NativeList<float3> positions2, NativeList<quaternion> quaternions2)
    {
        int maxCount = math.max(positions1.Length, positions1.Length);
        for (int i = 0; i < maxCount; i++)
        {
            //Horse 1 Visualisation
            if (i < positions1.Length)
            {
                float3 position = positions1[i];
                quaternion rotation = quaternions1[i];
                float3x3 rotationMatrix = new float3x3(rotation);

                // Calculate the corners of the bounding box
                NativeArray<float3> localCorners = new NativeArray<float3>(8, Allocator.Temp)
                {
                    [0] = new float3(-extents.x, -extents.y, -extents.z),
                    [1] = new float3(extents.x, -extents.y, -extents.z),
                    [2] = new float3(-extents.x, extents.y, -extents.z),
                    [3] = new float3(extents.x, extents.y, -extents.z),
                    [4] = new float3(-extents.x, -extents.y, extents.z),
                    [5] = new float3(extents.x, -extents.y, extents.z),
                    [6] = new float3(-extents.x, extents.y, extents.z),
                    [7] = new float3(extents.x, extents.y, extents.z)
                };
                float3[] worldCorners = new float3[8];
                for (int j = 0; j < 8; j++)
                {
                    worldCorners[j] = math.mul(rotationMatrix, localCorners[j]) + position;
                }
                // Draw the bounding box
                DrawBoundingBox(worldCorners, Color.green);
            }

            //Horse 2 Visualisation
            if (i < positions2.Length)
            {
                float3 position = positions2[i];
                quaternion rotation = quaternions2[i];
                float3x3 rotationMatrix = new float3x3(rotation);

                // Calculate the corners of the bounding box
                NativeArray<float3> localCorners = new NativeArray<float3>(8, Allocator.Temp)
                {
                    [0] = new float3(-extents.x, -extents.y, -extents.z),
                    [1] = new float3(extents.x, -extents.y, -extents.z),
                    [2] = new float3(-extents.x, extents.y, -extents.z),
                    [3] = new float3(extents.x, extents.y, -extents.z),
                    [4] = new float3(-extents.x, -extents.y, extents.z),
                    [5] = new float3(extents.x, -extents.y, extents.z),
                    [6] = new float3(-extents.x, extents.y, extents.z),
                    [7] = new float3(extents.x, extents.y, extents.z)
                };
                float3[] worldCorners = new float3[8];
                for (int j = 0; j < 8; j++)
                {
                    worldCorners[j] = math.mul(rotationMatrix, localCorners[j]) + position;
                }
                // Draw the bounding box
                DrawBoundingBox(worldCorners, Color.green);
            }

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
    private void DrawBoundingBox(float3[] corners, Color color)
    {
        Debug.DrawLine(corners[0], corners[1], color);
        Debug.DrawLine(corners[1], corners[3], color);
        Debug.DrawLine(corners[3], corners[2], color);
        Debug.DrawLine(corners[2], corners[0], color);

        Debug.DrawLine(corners[4], corners[5], color);
        Debug.DrawLine(corners[5], corners[7], color);
        Debug.DrawLine(corners[7], corners[6], color);
        Debug.DrawLine(corners[6], corners[4], color);

        Debug.DrawLine(corners[0], corners[4], color);
        Debug.DrawLine(corners[1], corners[5], color);
        Debug.DrawLine(corners[2], corners[6], color);
        Debug.DrawLine(corners[3], corners[7], color);
    }

}


[BurstCompile]
public struct TestHorseBoundingBoxCalculationJob : IJob
{
    public NativeArray<bool> isColliding;
    [ReadOnly] public float deltaTime;
    [ReadOnly] public float thresholdDistance;
    [ReadOnly] public float maxSpeed;
    [ReadOnly] public float acceleration;

    //Horse 1 
    public NativeArray<float3> splinePoints1;
    public NativeArray<float3> corners1;
    public NativeList<float3> positions1;
    public NativeList<quaternion> quaternions1;
    public float3 extents1;
    public float3 lastPosition1;
    public quaternion lastRotation1;
    public float currentSpeed1;
    public float targetSpeed1;

    //Horse 2
    public NativeArray<float3> splinePoints2;
    public NativeArray<float3> corners2;
    public NativeList<float3> positions2;
    public NativeList<quaternion> quaternions2;
    public float3 extents2;
    public float3 lastPosition2;
    public quaternion lastRotation2;
    public float currentSpeed2;
    public float targetSpeed2;

    public void Execute()
    {
        int splinePointIndex1 = 0;
        int splinePointIndex2 = 0;
        isColliding[0] = false;

        while (splinePointIndex1 < splinePoints1.Length && splinePointIndex2 < splinePoints2.Length)
        {
            //If both horses speed is zero, then break the loop
            if ((currentSpeed1 <= 0 && currentSpeed2 <= 0))
            {
                break;
            }
            // Horse 1
            if (splinePointIndex1 < splinePoints1.Length)
            {
                float speedChange1 = acceleration * deltaTime;
                currentSpeed1 = currentSpeed1 < targetSpeed1 ? math.clamp(currentSpeed1 + speedChange1, 0, targetSpeed1) : math.clamp(currentSpeed1 - speedChange1, targetSpeed1, maxSpeed);
                float3 targetPosition1 = splinePoints1[splinePointIndex1];
                float distance1 = math.distance(lastPosition1, targetPosition1);
                float timeToReach1 = distance1 / currentSpeed1;

                if (timeToReach1 > 0)
                {
                    // Update Position
                    float3 direction = math.normalize(targetPosition1 - lastPosition1);
                    float3 newPosition = lastPosition1 + direction * currentSpeed1 * deltaTime;
                    positions1.Add(newPosition);
                    lastPosition1 = newPosition;

                    // Apply rotation to bounding box 1
                    quaternion targetRotation1 = quaternion.LookRotationSafe(direction, new float3(0, 1, 0));
                    quaternion smoothRotation1 = math.slerp(lastRotation1, targetRotation1, deltaTime * currentSpeed1);
                    float3x3 rotationMatrix1 = new float3x3(smoothRotation1);
                    quaternions1.Add(smoothRotation1);
                    lastRotation1 = smoothRotation1;

                    // Calculate the corners of bounding box 1
                    float3 newCenter1 = newPosition;
                    NativeArray<float3> localCorners1 = new NativeArray<float3>(8, Allocator.Temp)
                    {
                        [0] = new float3(-extents1.x, -extents1.y, -extents1.z),
                        [1] = new float3(extents1.x, -extents1.y, -extents1.z),
                        [2] = new float3(-extents1.x, extents1.y, -extents1.z),
                        [3] = new float3(extents1.x, extents1.y, -extents1.z),
                        [4] = new float3(-extents1.x, -extents1.y, extents1.z),
                        [5] = new float3(extents1.x, -extents1.y, extents1.z),
                        [6] = new float3(-extents1.x, extents1.y, extents1.z),
                        [7] = new float3(extents1.x, extents1.y, extents1.z)
                    };
                    for (int i = 0; i < 8; i++)
                    {
                        corners1[i] = math.mul(rotationMatrix1, localCorners1[i]) + newCenter1;
                    }
                }
                if (distance1 < thresholdDistance)
                {
                    splinePointIndex1++;
                }
            }

            // Horse 2
            if (splinePointIndex2 < splinePoints2.Length)
            {
                float speedChange2 = acceleration * deltaTime;
                currentSpeed2 = currentSpeed2 < targetSpeed2 ? math.clamp(currentSpeed2 + speedChange2, 0, targetSpeed2) : math.clamp(currentSpeed2 - speedChange2, targetSpeed2, maxSpeed);
                float3 targetPosition2 = splinePoints2[splinePointIndex2];
                float distance2 = math.distance(lastPosition2, targetPosition2);
                float timeToReach2 = distance2 / currentSpeed2;

                if (timeToReach2 > 0)
                {
                    // Update Position
                    float3 direction = math.normalize(targetPosition2 - lastPosition2);
                    float3 newPosition = lastPosition2 + direction * currentSpeed2 * deltaTime;
                    positions2.Add(newPosition);
                    lastPosition2 = newPosition;

                    // Apply rotation to bounding box 2
                    quaternion targetRotation2 = quaternion.LookRotationSafe(direction, new float3(0, 1, 0));
                    quaternion smoothRotation2 = math.slerp(lastRotation2, targetRotation2, deltaTime * currentSpeed2);
                    float3x3 rotationMatrix2 = new float3x3(smoothRotation2);
                    quaternions2.Add(smoothRotation2);
                    lastRotation2 = smoothRotation2;

                    // Calculate the corners of bounding box 2
                    float3 newCenter2 = newPosition;
                    NativeArray<float3> localCorners2 = new NativeArray<float3>(8, Allocator.Temp)
                    {
                        [0] = new float3(-extents2.x, -extents2.y, -extents2.z),
                        [1] = new float3(extents2.x, -extents2.y, -extents2.z),
                        [2] = new float3(-extents2.x, extents2.y, -extents2.z),
                        [3] = new float3(extents2.x, extents2.y, -extents2.z),
                        [4] = new float3(-extents2.x, -extents2.y, extents2.z),
                        [5] = new float3(extents2.x, -extents2.y, extents2.z),
                        [6] = new float3(-extents2.x, extents2.y, extents2.z),
                        [7] = new float3(extents2.x, extents2.y, extents2.z)
                    };
                    for (int i = 0; i < 8; i++)
                    {
                        corners2[i] = math.mul(rotationMatrix2, localCorners2[i]) + newCenter2;
                    }
                }
                if (distance2 < thresholdDistance)
                {
                    splinePointIndex2++;
                }
            }
            // Check for collision
            isColliding[0] = AreBoundingBoxesColliding(corners1, corners2);
            if (isColliding[0] == true)
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
                axes[index++] = math.normalize(math.cross(axes[i], axes[j]));
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
