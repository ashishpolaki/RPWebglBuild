using UnityEngine;

namespace HorseRace
{
    public class GatesController : MonoBehaviour
    {
        [Tooltip("Each Horse gate animator's array")]
        [SerializeField] private Animator[] horseGateAnimators;

        #region Unity Methods
        private void OnEnable()
        {
            EventManager.Instance.OnRaceStartEvent += OnRaceStart;
        }
        private void OnDisable()
        {
            EventManager.Instance.OnRaceStartEvent -= OnRaceStart;
        }
        #endregion

        #region Subscribed Methods
        private void OnRaceStart()
        {
            for (int i = 0; i < horseGateAnimators.Length; i++)
            {
                horseGateAnimators[i].SetTrigger("Open");
            }
        }
        #endregion
    }
}