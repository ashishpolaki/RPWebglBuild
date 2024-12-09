using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace HorseRace
{
    public class SpawnManager : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private HorseController horsePrefab;
        [SerializeField] private HorseJockeyMaterials horseJockeyMaterials;
        [SerializeField] private Transform[] spawnPoints;
        #endregion

        #region Private Variables
        private List<int> spawnHorses = new List<int>();
        #endregion

        #region Unity Methods
        private void Start()
        {
            spawnHorses = GameManager.Instance.HorsesToSpawnList;
            SpawnHorses();
        }
        #endregion

        #region Subscribed Methods
        private void SpawnHorses()
        {
            LoadCharacter();
            HorseController[] horseControllers = new HorseController[spawnHorses.Count];
            HostRaceData hostRaceData = new HostRaceData();
            for (int i = 0; i < spawnHorses.Count; i++)
            {
                int horseNumber = spawnHorses[i];
                //Instantiate
                HorseController horse = Instantiate(horsePrefab, spawnPoints[horseNumber - 1].position, spawnPoints[horseNumber - 1].rotation);
                horse.gameObject.name = $"Horse {horseNumber}";

                //Load CHaracter Data Only in Load Game Scene
                HorseControllerLoad horseControllerLoad = horse as HorseControllerLoad;
                if (horseControllerLoad != null)
                {
                    horseControllerLoad.SetCharacter(UGSManager.Instance.HostRaceData.characterCustomisationDatas[horseNumber]);
                    horseControllerLoad.Character.EnableFace();
                    RenderTexture renderTexture = GameManager.Instance.CaptureObject.Capture(horseControllerLoad.Character.gameObject);
                    horseControllerLoad.Character.EnableFullBody();
                    hostRaceData.currentRaceAvatars[horseNumber] = renderTexture;
                }

                //Generate Random Materials
                horse.InitializeMaterials(horseJockeyMaterials.horseMaterials[Utils.GenerateRandomNumber(0, horseJockeyMaterials.horseMaterials.Length)]);
                horse.SetHorseNumber(horseNumber);
                horseControllers[i] = horse;
            }
            UGSManager.Instance.SetHostRaceData(hostRaceData);
            GameManager.Instance.RaceManager.Initialize(horseControllers);
            GameManager.Instance.CameraController.SetTargetGroup(horseControllers.Select(x => x.transform).ToList());
        }
        #endregion


        #region Temp Load CharacterData
        private void LoadCharacter()
        {
            //Delete Later
            //HostRaceData hostRaceData = new HostRaceData();
            //List<UGS.RaceLobbyParticipant> raceLobbyParticipants = new List<UGS.RaceLobbyParticipant>();
            //raceLobbyParticipants.Add(new UGS.RaceLobbyParticipant() { HorseNumber = horsesByNumber.Keys.ElementAt(0) });
            //raceLobbyParticipants.Add(new UGS.RaceLobbyParticipant() { HorseNumber = horsesByNumber.Keys.ElementAt(1) });
            //raceLobbyParticipants.Add(new UGS.RaceLobbyParticipant() { HorseNumber = horsesByNumber.Keys.ElementAt(2) });
            //hostRaceData.qualifiedPlayers = raceLobbyParticipants;
            //UGSManager.Instance.SetHostRaceData(hostRaceData);

            Dictionary<int, CharacterCustomisationEconomy> characterCustomisationDatas = new Dictionary<int, CharacterCustomisationEconomy>();
            characterCustomisationDatas[GameManager.Instance.HorsesInPreRaceOrderList[0]] = new CharacterCustomisationEconomy();
            characterCustomisationDatas[GameManager.Instance.HorsesInPreRaceOrderList[1]] = new CharacterCustomisationEconomy();
            characterCustomisationDatas[GameManager.Instance.HorsesInPreRaceOrderList[2]] = new CharacterCustomisationEconomy();
            characterCustomisationDatas[GameManager.Instance.HorsesInPreRaceOrderList[2]].customParts.Add(new CustomPartEconomy()
            {
                styleNumber = 0,
                type = (int)BlendPartType.FacialHair,
            });
            HostRaceData hostRaceData = new HostRaceData();
            hostRaceData.characterCustomisationDatas = characterCustomisationDatas;
            UGSManager.Instance.SetHostRaceData(hostRaceData);
        }
        #endregion
    }
}
