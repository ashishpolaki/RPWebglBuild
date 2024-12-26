using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDataContainer : MonoBehaviour
{
    public static GameDataContainer Instance;

    #region Private Variables
    private Dictionary<Type, GameData> gameDataList = new Dictionary<Type, GameData>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void OnApplicationQuit()
    {
        // Dispose of all game data
        foreach (var gameData in gameDataList.Values)
        {
            if (gameData is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        gameDataList.Clear();
    }
    #endregion

    #region Public Methods
    public void SetGameEvent<T>(T newData, bool canSetDefaultValues = false) where T : GameData
    {
        if (gameDataList.TryGetValue(typeof(T), out GameData existingData))
        {
            // Update only the modified fields
            UpdateModifiedFields((T)existingData, newData, canSetDefaultValues);
        }
        else
        {
            gameDataList[typeof(T)] = newData;
        }
    }
    public T GetGameEvent<T>() where T : GameData, new()
    {
        if (gameDataList.TryGetValue(typeof(T), out GameData gameData))
        {
            return (T)gameData;
        }
        return new T();
    }
    #endregion

    #region Private Methods
    private void UpdateModifiedFields<T>(T existingData, T newData, bool setDefaultValues) where T : GameData
    {
        // Get all the fields of the class T
        var fields = typeof(T).GetFields();
        foreach (var field in fields)
        {
            // Get the value of the field from the new data
            var newValue = field.GetValue(newData);

            //Set the default value if the condition is true
            if (setDefaultValues || !IsDefaultValue(newValue))
            {
                field.SetValue(existingData, newValue);
            }
        }
    }

    private bool IsDefaultValue(object value)
    {
        //Returns true if the value is null 
        if (value == null)
        {
            return true;
        }
        var type = value.GetType();
        //Return true if the type is a value type and the value is the default value of the type
        if (type.IsValueType)
        {
            object instance = Activator.CreateInstance(type);
            bool isDefault = instance.Equals(value);
            return isDefault;
        }
        //Return true if the type is a reference type(string) and the value is null
        else if (type == typeof(string))
        {
            return string.IsNullOrEmpty((string)value);
        }
        //Return true if the type is a collection and the collection is empty
        else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
        {
            var enumerable = value as System.Collections.IEnumerable;
            return !enumerable.GetEnumerator().MoveNext();
        }
        //Return true if the type is a DateTime and the value is the default value of DateTime
        else if (type == typeof(DateTime))
        {
            return (DateTime)value == default;
        }

        return false;
    }
    #endregion

}
