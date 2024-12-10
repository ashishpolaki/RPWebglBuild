using HorseRace;
using HorseRace.Camera;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    #region Inspector Variables
    [SerializeField] private float raceTimeScale;
    #endregion

    #region Private Variables
    private List<int> horsesToSpawnList = new List<int>();
    #endregion

    #region Properties
    public CameraController CameraController { get; private set; }
    public RaceManager RaceManager { get; private set; }
    public CaptureObject CaptureObject { get; private set; }
    public GPS GPS;


    public RaceStats CurrentRaceData { get; private set; }
    public List<int> HorsesInRaceOrderList { get; private set; }
    public List<int> HorsesInPreRaceOrderList { get; private set; }
    public List<int> HorsesToSpawnList
    {
        get
        {
            if (horsesToSpawnList.Count == 0)
            {
                for (int i = 1; i <= 12; i++)
                {
                    horsesToSpawnList.Add(i);
                }
            }
            return horsesToSpawnList;
        }
        set { horsesToSpawnList = value; }
    }
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        GPS = new GPS();
        LoadHorsesInRaceOrder();
         HorsesToSpawnList = new List<int>(HorsesInPreRaceOrderList);
        //HorsesToSpawnList = new List<int>() { HorsesInPreRaceOrderList[0], HorsesInPreRaceOrderList[1], HorsesInPreRaceOrderList[2] };
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        Time.timeScale = raceTimeScale;
    }
#endif

    public void SetCaptureObject(CaptureObject captureObject)
    {
        CaptureObject = captureObject;
    }
    public void SetCameraController(CameraController cameraController)
    {
        CameraController = cameraController;
    }
    public void SetRaceManager(RaceManager raceManager)
    {
        RaceManager = raceManager;
    }
    public void SetCurrentRaceHorsesOrder(List<int> _horsesInRaceOrderList)
    {
        HorsesInRaceOrderList = new List<int>(_horsesInRaceOrderList);
    }
    public List<int> LoadHorsesInRaceOrder()
    {
        HorseRaceResults horseRaceResults = new HorseRaceResults();
        int currentRaceIndex = 0;
        string currentFileName = string.Empty;
        (currentRaceIndex, currentFileName, CurrentRaceData) = horseRaceResults.LoadRandomRace();
        foreach (var waypoint in CurrentRaceData.waypoints)
        {
            if (waypoint.number == "WinnersList")
            {
                HorsesInPreRaceOrderList = waypoint.positions.Select(x => x.horseNumber).ToList();
            }
        }
        return new List<int>(HorsesInPreRaceOrderList);
    }

    public void FetchLocation()
    {
        StartCoroutine(GPS.IEGetLocation());
    }
}
