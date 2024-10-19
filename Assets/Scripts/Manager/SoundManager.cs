using System.Linq;
using UnityEngine;
using HorseRace;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    #region Inspector variables
    [SerializeField] private Sounds[] sounds;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        PlaySound(SoundType.Cheer, _isVolumeQuiet: false, _loop: true);
        PlaySound(SoundType.BeforeRaceStart, _isVolumeQuiet: false);
        PlaySound(SoundType.HorseGallop, _loop: true);
        PlaySound(SoundType.RaceMusic, _loop: true);
    }
    private void OnEnable()
    {
        EventManager.Instance.OnRaceStartEvent += RaceStarted;
    }
    private void OnDisable()
    {
        EventManager.Instance.OnRaceStartEvent -= RaceStarted;
    }
    #endregion

    #region Subscribed Methods
    /// <summary>
    /// Play Sounds on Race Start
    /// </summary>
    private void RaceStarted()
    {
        //Play Gunshot Sound
        PlaySound(SoundType.RaceStart, _isVolumeQuiet: false);

        //Set Gallop and Racemusic volumes
        SetSoundVolume(SoundType.HorseGallop);
        SetSoundVolume(SoundType.RaceMusic);
    }
    #endregion

    public void StopSound(SoundType soundType)
    {
        foreach (var item in sounds)
        {
            if(item.soundType == soundType)
            {
                item.audioObject.StopBackgroundMusic();
            }
        }
    }

    #region Private Methods
    /// <summary>
    /// Play audio continuously or just once.
    /// </summary>
    /// <param name="_soundType"></param>
    /// <param name="_isVolumeQuiet"></param>
    /// <param name="_loop"></param>
    private void PlaySound(SoundType _soundType, bool _isVolumeQuiet = true, bool _loop = false)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].soundType == _soundType)
            {
                if (!_loop)
                {
                    Sounds sound = GetSound(_soundType);
                    AudioManager.Instance.PlaySoundEffect(sound.audioClip, transform.position, _isVolumeQuiet ? 0 : sounds[i].volume);
                }
                else
                {
                    sounds[i].audioObject = AudioManager.Instance.PlaySoundEffectOnLoop(sounds[i].audioClip, transform.position, _isVolumeQuiet ? 0 : sounds[i].volume);
                }
                break;
            }
        }
    }

    /// <summary>
    /// Get SoundData via soundtype.
    /// </summary>
    /// <param name="soundType"></param>
    /// <returns></returns>
    private Sounds GetSound(SoundType soundType)
    {
        Sounds sound = sounds.First(x => x.soundType == soundType);
        return sound;
    }

    /// <summary>
    /// Set Sound Volume via soundtype.
    /// </summary>
    /// <param name="soundType"></param>
    private void SetSoundVolume(SoundType soundType)
    {
        Sounds sound = GetSound(soundType);
        sound.audioObject.AudioSource.volume = sound.volume;
    }

    /// <summary>
    /// Increase Horse Gallop Sounds 
    /// </summary>
    public void IncreaseGallopSound()
    {
        GetSound(SoundType.Cheer).DecreaseVolume();
        GetSound(SoundType.HorseGallop).IncreaseVolume();
    }

    /// <summary>
    /// Reset Gallop Sounds of Horses
    /// </summary>
    public void ResetGallopSound()
    {
        GetSound(SoundType.Cheer).IncreaseVolume();
        GetSound(SoundType.HorseGallop).DecreaseVolume();
    }
    #endregion
}
