using UnityEngine;
using UGS;
using System.Threading.Tasks;

public class UGSManager : MonoBehaviour
{
    public static UGSManager Instance;

    #region Properties
    //Local Cloud Data Storage
    public PlayerData PlayerData { get => GameDataContainer.Instance.GetGameEvent<PlayerData>(); }
    public PlayerRaceData PlayerRaceData { get => GameDataContainer.Instance.GetGameEvent<PlayerRaceData>(); }
    public HostRaceData HostRaceData { get => GameDataContainer.Instance.GetGameEvent<HostRaceData>(); }
    public VenueRegistrationData VenueRegistrationData { get => GameDataContainer.Instance.GetGameEvent<VenueRegistrationData>(); }

    //UGS
    public Authentication Authentication { get; private set; }
    public CloudCode CloudCode { get; private set; }
    public CloudSave CloudSave { get; private set; }
    public Economy Economy { get; private set; }

    public GPS GPS;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUGS();
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        Authentication.OnSignedInEvent += LoginSuccessful;
        Authentication.OnPlayerNameChanged += HandlePlayerNameChangeEvent;
    }
    private void OnDisable()
    {
        if (Authentication != null)
        {
            Authentication.OnSignedInEvent -= LoginSuccessful;
            Authentication.OnPlayerNameChanged -= HandlePlayerNameChangeEvent;
            Authentication.DeSubscribeEvents();
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initialize Unity Gaming Services
    /// </summary>
    private async void InitializeUGS()
    {
        Authentication = new Authentication();
        CloudCode = new CloudCode();
        CloudSave = new CloudSave();
        Economy = new Economy();

        await Authentication.InitializeUnityServices();
        CloudCode.InitializeBindings();
    }
    private void LoginSuccessful()
    {
        CloudCode.SubscribeToPlayerMessages();
        Economy.InitializeEconomy();

        //Set PlayerData
        using (PlayerData playerData = new PlayerData())
        {
            playerData.playerID = Authentication.PlayerID;
            playerData.playerName = Authentication.PlayerName;
            SetPlayerData(playerData);
        }
    }
    private void HandlePlayerNameChangeEvent(string _playerName)
    {
        using (PlayerData playerData = new PlayerData())
        {
            playerData.playerName = _playerName;
            SetPlayerData(playerData);
        }
    }
    #endregion

    #region Local Cloud Data Storage
    public void SetPlayerData(PlayerData playerData, bool _allowDefaultValues = false)
    {
        GameDataContainer.Instance.SetGameEvent(playerData, _allowDefaultValues);
    }
    public void SetPlayerRaceData(PlayerRaceData playerRaceData, bool _allowDefaultValues = false)
    {
        GameDataContainer.Instance.SetGameEvent(playerRaceData, _allowDefaultValues);
    }
    public void SetHostRaceData(HostRaceData hostRaceData, bool _allowDefaultValues = false)
    {
        GameDataContainer.Instance.SetGameEvent(hostRaceData, _allowDefaultValues);
    }
    public void SetVenueRegistrationData(VenueRegistrationData venueRegistrationData, bool canSetDefaultValues = false)
    {
        GameDataContainer.Instance.SetGameEvent(venueRegistrationData, canSetDefaultValues);
    }
    #endregion

    #region Public Methods
    public async Task<string> GetHostVenueName(float _latitude, float _longitude)
    {
#if CHEATCODE_ENABLE
        _latitude = CheatCode.Instance.IsCheatEnabled ? CheatCode.Instance.Latitude : _latitude;
        _longitude = CheatCode.Instance.IsCheatEnabled ? CheatCode.Instance.Longitude : _longitude;
#endif
        if (string.IsNullOrEmpty(PlayerData.hostID))
        {
            float latitude = _latitude;
            float longitude = _longitude;
            string hostVenueName = await CloudSave.GetHostVenueName(StringUtils.HOSTVENUE, latitude, longitude);
            using (PlayerData playerData = new PlayerData())
            {
                playerData.hostVenueName = hostVenueName;
                SetPlayerData(playerData);
            }
            return hostVenueName;
        }
        else
        {
            return PlayerData.hostID;
        }
    }

    /// <summary>
    /// Reset all the data stored in the game.
    /// </summary>
    public void ResetData()
    {
        SetPlayerData(new PlayerData(), true);
        SetVenueRegistrationData(new VenueRegistrationData(), true);
        SetPlayerRaceData(new PlayerRaceData(), true);
        Authentication.ResetData();
    }
    #endregion

}

