using System.Collections.Generic;
using UnityEngine;
using UGS;
using System.Threading.Tasks;

public class UGSManager : MonoBehaviour
{
    public static UGSManager Instance;

    #region Properties
    //Local Cloud Data Storage
    public PlayerData PlayerData { get => GameDataContainer.Instance.GetGameEvent<PlayerData>(); }
    public LocationData LocationData { get => GameDataContainer.Instance.GetGameEvent<LocationData>(); }
    public RaceData RaceData { get => GameDataContainer.Instance.GetGameEvent<RaceData>(); }

    //UGS
    public Authentication Authentication { get; private set; }
    public CloudCode CloudCode { get; private set; }
    public CloudSave CloudSave { get; private set; }
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
    private void InitializeUGS()
    {
        Authentication = new Authentication();
        CloudCode = new CloudCode();
        CloudSave = new CloudSave();
        Authentication.InitializeUnityServices();
    }
    private void LoginSuccessful()
    {
        CloudCode.InitializeBindings();
        CloudCode.SubscribeToPlayerMessages();

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
    public void SetLocationData(LocationData locationData, bool canSetDefaultValues = false)
    {
        GameDataContainer.Instance.SetGameEvent(locationData, canSetDefaultValues);
    }
    public void SetRaceData(RaceData raceData, bool canSetDefaultValues = false)
    {
        GameDataContainer.Instance.SetGameEvent(raceData, canSetDefaultValues);
    }
    #endregion

    #region Public Methods
    public async Task SetLobbyPlayers()
    {
        LoadingScreen.Instance.Show();
        List<CurrentRacePlayerCheckIn> racePlayerCheckIns = await CloudSave.GetRaceCheckinParticipants(PlayerData.playerID, StringUtils.RACECHECKIN);
        if (racePlayerCheckIns != null && racePlayerCheckIns.Count > 0)
        {
            RaceLobbyHandler raceLobbyHandler = new RaceLobbyHandler(racePlayerCheckIns);
            RaceData raceData = new RaceData();
            raceData.lobbyQualifiedPlayers = new Dictionary<string, (string, int)>(await raceLobbyHandler.GetQualifiedPlayers());
            raceData.unQualifiedPlayers = new List<string>(await raceLobbyHandler.GetUnQualifiedPlayers());
            SetRaceData(raceData);
            raceLobbyHandler.Dispose(); //Dispose the RaceLobbyHandler after usage
        }
        LoadingScreen.Instance.Hide();
    }

    /// <summary>
    /// Fetch the current location of the player.
    /// </summary>
    /// <returns></returns>
    public async Task FetchCurrentLocation()
    {
        bool gpsLocationFound = await GPS.TryGetLocationAsync();
        if (gpsLocationFound)
        {
            LocationData locationData = new LocationData();
            locationData.latitude = GPS.CurrentLocationLatitude;
            locationData.longitude = GPS.CurrentLocationLongitude;
            SetLocationData(locationData);
            locationData.Dispose();
        }
    }

    /// <summary>
    /// If host id already exists, return the host id. Else, get the host id from the cloud and return it.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetHostID()
    {
        if (string.IsNullOrEmpty(PlayerData.hostID))
        {
            double latitude = CheatCode.Instance.IsCheatEnabled ? CheatCode.Instance.Latitude : LocationData.latitude;
            double longitude = CheatCode.Instance.IsCheatEnabled ? CheatCode.Instance.Longitude : LocationData.longitude;
            string hostID = await CloudSave.GetHostID(StringUtils.HOSTVENUE, latitude, longitude);
            using (PlayerData playerData = new PlayerData())
            {
                playerData.hostID = hostID;
                SetPlayerData(playerData);
            }
            return hostID;
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
        SetLocationData(new LocationData(), true);
        SetRaceData(new RaceData(), true);
        Authentication.ResetData();
    }
    #endregion
}

