using UnityEngine;
using UnityEngine.Experimental.Rendering;

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
    public ControlPointSave[] controlPoints;
    public string overtakeData;
}
[System.Serializable]
public struct ControlPointSave
{
    public float speed;
    public float acceleration;
    public int splineIndex;
    public int controlPointIndex;
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
public struct CharacterPartUI
{
    public CharacterPartUIType characterPartType;
    public Sprite icon;
}
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

#region Texture
[System.Serializable]
public struct CharacterPartTextureColors
{
    public Vector2Int uvCoordinates; 
    public Color textureColor; 
}
[System.Serializable]
public struct RenderTextureSettings
{
   public GraphicsFormat colorFormat;
   public GraphicsFormat depthStencilFormat;
   public Vector2Int renderTextureSize;
}
[System.Serializable]
public struct CaptureTextureSettings
{
    public BlendPartType blendPartType;
    public Vector3 Offset;
    public float FieldOfView;
    public Vector2Int RenderTextureSize;
}
[System.Serializable]
public struct  CaptureOutfitTextureSettings
{
    public OutfitType outfitType;
    public Vector3 Offset;
    public float FieldOfView;
    public Vector2Int RenderTextureSize;
}
#endregion