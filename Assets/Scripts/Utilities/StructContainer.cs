using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StructContainer
{
    
}

#region Particle
[System.Serializable]
public struct Particle
{
    public ParticleType particleType;
    public ParticleSystem particleSystem;
    public int poolSize;
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
#endregion

#region Save/load Race Stats
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

#region Character
[System.Serializable]
public struct CharacterData
{
    public SyntyCharacterPartType CharacterPartType;
    public SkinnedMeshRenderer meshRenderer;
}
[System.Serializable]
public struct BlendShapePart
{
    public BlendShapePartData[] partData;
}

[System.Serializable]
public struct BlendShapePartData
{
    public string name;
    public string[] blendShapeNames;
    public SyntyCharacterPartType[] syntyCharacterPartTypes;
}
#endregion