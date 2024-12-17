using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Speed", menuName = "ScriptableObjects/HorseSpeedModifiers")]
public class HorseSpeedSO : ScriptableObject
{
    public float2 targetSpeedRange = new float2(13,16);
    public float2 initialAccelerationRange = new float2(6,10);
    public int finishRaceControlPointIndex; 
    public float finishRaceAccelerationIncrement = 0.01f;
    public float finishRaceAccelerationMultiplier = 0.05f;
    public float acceleration = 2;
    public float maxSpeed;
}
