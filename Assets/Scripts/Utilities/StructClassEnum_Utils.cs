using UnityEngine;

public class StructClassEnum_Utils { }


#region Particle
[System.Serializable]
public struct Particle
{
    public ParticleType particleType;
    public ParticleSystem particleSystem;
    public int poolSize;
}
public enum ParticleType
{
    None,
    HorseDustCloud
}
#endregion

#region Camera
[System.Serializable]
public struct SpecialCameraAngle
{
    public int priorityLevel;
    public CameraType cameraType;
    public HorseRace.Camera.CameraAngle cameraAngle;
}
public enum CameraType
{
    None,
    Overtake,
    RiderCloseUp
}
public enum CameraMode
{
    Basic,
    Special
}
#endregion

#region Sound
[System.Serializable]
public struct Sounds
{
    public SoundType soundType;
    public AudioClip audioClip;
    public AudioObject audioObject { get; set; }
    public float volume;
    public float changeVolume;

    public void IncreaseVolume()
    {
        audioObject.AudioSource.volume += changeVolume;
    }
    public void DecreaseVolume()
    {
        audioObject.AudioSource.volume -= changeVolume;
    }
}
public enum SoundType
{
    RaceStart,
    Cheer,
    HorseGallop,
    RaceMusic,
    BeforeRaceStart,
    None
}
#endregion

#region Save/load/Verify Race Stats
[System.Serializable]
public class SaveRaceStats
{
    public RaceStats[] raceStats;
}
[System.Serializable]
public class RaceStats
{
    public string raceIdentifier;
    public int predeterminedWinner;
    public Waypoint[] waypoints;
    public HorseData[] horsesData;
}
[System.Serializable]
public struct Waypoint
{
    public string number;
    public Position[] positions;
}

[System.Serializable]
public struct Position
{
    public int position;
    public int horseNumber;
}
[System.Serializable]
public struct HorseData
{
    public string horseNumber;
    public string overtakeData;
    public string raceData;
}
[System.Serializable]
public struct HorseVelocity
{
    public string x;
    public string z;
}
[System.Serializable]
public struct RaceVarianceResults
{
    public string raceFileName;
    public int raceIndex;
    public VarianceRacePosition[] variances;
}
[System.Serializable]
public struct VarianceRacePosition
{
    public int racePosition;
    public int savedHorseNumber;
    public int currentHorseNumber;
}
#endregion

