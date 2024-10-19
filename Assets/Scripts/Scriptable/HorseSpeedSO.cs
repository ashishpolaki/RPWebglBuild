using UnityEngine;

[CreateAssetMenu(fileName = "Speed", menuName = "HorseRace/HorseSpeedModifiers")]
public class HorseSpeedSO : ScriptableObject
{
    public float startMinSpeed;
    public float startMaxSpeed;
    public float inRaceMinSpeed;
    public float inRaceMaxSpeed;
    public float inRaceMinSpeedInterval;
    public float inRaceMaxSpeedInterval;
}
