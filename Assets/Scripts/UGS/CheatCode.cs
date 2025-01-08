#if CHEAT_CODE
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;


public class CheatCode : MonoBehaviour
{
    public static CheatCode Instance;

    [SerializeField] private Button cheatButton;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button saveBtn;
    [SerializeField] private GameObject cheatPanel;
    [SerializeField] private Toggle cheatToggle;
    [SerializeField] private InputField latitudeInput;
    [SerializeField] private InputField longitudeInput;
    [SerializeField] private InputField venueNameInput;

    private bool IsCheatPanelActive;
    public bool IsCheatEnabled;

    public string CheatDateTime { get; private set; }
    public float Latitude { get; private set; }
    public float Longitude { get; private set; }
    public string VenueName { get; private set; }

    [SerializeField] private TimeAdjustmentSettings cheatTime;
    [SerializeField] private DateSelectorUI dateSelectorUI;

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        cheatButton.onClick.AddListener(() => CheatPanel());
        closeBtn.onClick.AddListener(() => CheatPanel());
        saveBtn.onClick.AddListener(() => Save());
        cheatToggle.onValueChanged.AddListener((value) => CheatToggle(value));
    }
    private void OnDisable()
    {
        cheatButton.onClick.RemoveAllListeners();
        cheatToggle.onValueChanged.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
        saveBtn.onClick.RemoveAllListeners();
    }
    private void CheatPanel()
    {
        IsCheatPanelActive = !IsCheatPanelActive;
        cheatPanel.SetActive(IsCheatPanelActive);
        dateSelectorUI.SetDate(DateTime.Now);
        cheatTime.SetTime(DateTime.Now);
    }
    private void CheatToggle(bool _val)
    {
        IsCheatEnabled = _val;
    }
    #endregion

    private async void Save()
    {
        CheatDateTime = ConvertDateTimeToUTC(cheatTime.ReturnTime(), dateSelectorUI.ReturnDate());
        Latitude = !string.IsNullOrEmpty(latitudeInput.text) ? float.Parse(latitudeInput.text) : 0;
        Longitude = !string.IsNullOrEmpty(longitudeInput.text) ? float.Parse(longitudeInput.text) : 0;
        VenueName = !string.IsNullOrEmpty(venueNameInput.text) ? StringUtils.RemoveSymbolsAndSpaces(venueNameInput.text) : string.Empty;
        await UGSManager.Instance.CloudCode.SetCheatCode(CheatDateTime, IsCheatEnabled);
    }

    public DateTime GetCheatDateTime()
    {
        return DateTime.ParseExact(CheatDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None);
    }

    private string ConvertDateTimeToUTC(string timeString, string dateString)
    {
        string dateTimeString = $"{dateString} {timeString}";
        DateTime localDateTime = DateTime.ParseExact(dateTimeString, "dd-MM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
        TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
        DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, localTimeZone);
        return utcDateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    //PlayerIds
    public string Ajith1 = "yOMFiqaN6zF0z9xNQ95s2H1Wvagc";
}
#endif
