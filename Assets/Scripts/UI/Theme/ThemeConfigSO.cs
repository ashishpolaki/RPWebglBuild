using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThemeUI", menuName = "ScriptableObjects/Theme/ThemeConfig")]
public class ThemeConfigSO : ScriptableObject
{
    public List<ThemeDataSO> themeDataList;

    public ThemeDataSO GenerateRandomTheme()
    {
        if (themeDataList.Count > 0)
        {
            return themeDataList[Utils.GenerateRandomNumber(0, themeDataList.Count)];
        }
        return null;
    }
}
