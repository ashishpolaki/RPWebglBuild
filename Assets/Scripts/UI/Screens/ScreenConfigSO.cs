using System.Collections.Generic;
using UnityEngine;

namespace UI.Screen
{
    [CreateAssetMenu(fileName = "ScreenConfig", menuName = "ScriptableObjects/UIScreenConfig")]
    public class ScreenConfigSO : ScriptableObject
    {
        public List<BaseScreen> screens;
    }
}
