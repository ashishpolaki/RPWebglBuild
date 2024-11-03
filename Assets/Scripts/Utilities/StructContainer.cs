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

#region UI
[System.Serializable]
public struct ThemeUI
{
    public TextMeshProUGUI[] textGroup;
    public TextMeshProUGUI[] buttonTextGroup;
    public Image[] bodyBGImageGroup;
    public Image[] bodyImageGroup;
    public Image[] inputFieldBGImageGroup; 
    public Image[] inputFieldImageGroup; 
    public Image characterImage;
    public Image cloudCommentImage;

    public void SetThemeData(ThemeDataSO themeData)
    {
        //Set sprite
        if (characterImage != null)
        {
            characterImage.sprite = themeData.character;
        }

        //Set color
        if (cloudCommentImage != null)
        {
            cloudCommentImage.color = themeData.cloudColor;
            cloudCommentImage.GetComponent<Outline>().effectColor = themeData.cloudOutlineColor;
        }

        if (bodyBGImageGroup.Length > 0)
        {
            foreach (Image image in bodyBGImageGroup)
            {
                image.color = themeData.bodyBGColor;
                image.GetComponent<Outline>().effectColor = themeData.bodyBGOutlineColor;
            }
        }
        if (textGroup.Length > 0)
        {
            foreach (TextMeshProUGUI text in textGroup)
            {
                text.color = themeData.textColor;
                text.outlineColor = themeData.textOutlineColor;
                text.outlineWidth = 0.2f;
            }
        }

        if(buttonTextGroup.Length > 0)
        {
            foreach (TextMeshProUGUI text in buttonTextGroup)
            {
                text.color = themeData.buttonTextColor;
                text.outlineColor = themeData.buttonTextOutlineColor;
                text.outlineWidth = 0.2f;
            }
        }

        if (bodyImageGroup.Length > 0)
        {
            foreach (Image image in bodyImageGroup)
            {
                image.color = themeData.bodyColor;
                image.GetComponent<Outline>().effectColor = themeData.bodyOutlineColor;
            }
        }

        if (inputFieldBGImageGroup.Length > 0)
        {
            foreach (Image image in inputFieldBGImageGroup)
            {
                image.color = themeData.inputFieldBGColor;
                image.GetComponent<Outline>().effectColor = themeData.inputFieldBGOutlineColor;
            }
        }

        if (inputFieldImageGroup.Length > 0)
        {
            foreach (Image image in inputFieldImageGroup)
            {
                image.color = themeData.inputFieldColor;
            }
        }

    }
}
#endregion

