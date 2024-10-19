using UnityEngine;

namespace UI.Screen.Tab
{
    public class BaseTab : MonoBehaviour, IScreenTab
    {
        [SerializeField] private ScreenTabType screenTabType;

        public ScreenTabType ScreenTabType => screenTabType;
        public bool IsOpen { get => gameObject.activeSelf; }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }
    }
    public interface IScreenTab
    {
        public ScreenTabType ScreenTabType { get; }
        public bool IsOpen { get; }
        public void Open();
        public void Close();
    }
}
public enum ScreenTabType
{
    None,
    LoginPlayer,
    RegisterPlayer,
    RaceSchedule,
    PlayerName,
    RoleSelection,
    Lobby,
    RegisterVenue,
    RaceInProgress,
    RaceResults
}
