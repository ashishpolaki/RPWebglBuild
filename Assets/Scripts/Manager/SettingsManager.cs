using System;
using System.Threading.Tasks;
using AarquieSolutions.SettingsSystem;
using UnityEngine;

public class SettingsManager
{
    public static Setting<float> masterVolume;
    public static Setting<float> sfxVolume;
    public static Setting<float> bgmVolume;

    [RuntimeInitializeOnLoadMethod((RuntimeInitializeLoadType.AfterSceneLoad))]
    public static void InitializeSettings()
    {
        LoadAudioSettings();
    }

    private static async void LoadAudioSettings()
    {
        //a delay is added since Unity doesn't allow setting values for Audio Mixers on Awake
        await Task.Delay((int)(Time.deltaTime * 1000)); //converting seconds to milliseconds before using it

        masterVolume = new Setting<float>("MasterVolume", 1f, AudioManager.Instance.SetMasterVolume);
        sfxVolume = new Setting<float>("SFXVolume", 1f, AudioManager.Instance.SetSoundEffectsVolume);
        bgmVolume = new Setting<float>("BGMVolume", 1f, AudioManager.Instance.SetBackgroundMusicVolume);
    }
}

namespace AarquieSolutions.SettingsSystem
{
    public class Setting<TSettingType> where TSettingType : IComparable, IConvertible
    {
        private readonly string key;

        private TSettingType value;

        public TSettingType Value
        {
            get => value;
            set
            {
                this.value = value;
                if (valueUpdatedDelegate != null)
                {
                    valueUpdatedDelegate(this.value);
                }

                SaveSetting();
            }
        }

        private readonly TSettingType defaultValue;
        private event Action<TSettingType> valueUpdatedDelegate;


        public Setting(string settingKey, TSettingType defaultValue, Action<TSettingType> valueUpdated = null)
        {
            this.key = settingKey;
            this.defaultValue = defaultValue;
            this.valueUpdatedDelegate = valueUpdated;
            Value = LoadValue();
        }


        public TSettingType LoadValue()
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return defaultValue;
            }

            if (typeof(TSettingType) == typeof(bool))
            {
                return (TSettingType)(object)(PlayerPrefs.GetInt(key) == 1);
            }
            else if (typeof(TSettingType) == typeof(int))
            {
                return (TSettingType)(object)PlayerPrefs.GetInt(key);
            }
            else if (typeof(TSettingType) == typeof(float))
            {
                return (TSettingType)(object)PlayerPrefs.GetFloat(key);
            }
            else if (typeof(TSettingType) == typeof(string))
            {
                return (TSettingType)(object)PlayerPrefs.GetString(key);
            }
            else
            {
                Debug.LogError("Setting is not a type that is supported.");
                return defaultValue;
            }
        }

        public void SaveSetting()
        {
            if (typeof(TSettingType) == typeof(bool))
            {
                PlayerPrefs.SetInt(key, (int)Convert.ChangeType(Value.CompareTo(true) == 0 ? 1 : 0, typeof(int)));
            }
            else if (typeof(TSettingType) == typeof(int))
            {
                PlayerPrefs.SetInt(key, (int)Convert.ChangeType(Value, typeof(int)));
            }
            else if (typeof(TSettingType) == typeof(float))
            {
                PlayerPrefs.SetFloat(key, (float)Convert.ChangeType(Value, typeof(float)));
            }
            else if (typeof(TSettingType) == typeof(string))
            {
                PlayerPrefs.SetString(key, (string)Convert.ChangeType(Value, typeof(string)));
            }

            PlayerPrefs.Save();
        }
    }
}