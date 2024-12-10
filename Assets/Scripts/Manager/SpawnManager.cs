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
        [SerializeField] private bool isLoadGameScene;
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
            if (isLoadGameScene)
            {
                LoadCharacter();
            }
            HorseController[] horseControllers = new HorseController[spawnHorses.Count];
            HostRaceData hostRaceData = new HostRaceData();
            for (int i = 0; i < spawnHorses.Count; i++)
            {
                int horseNumber = spawnHorses[i];
                //Instantiate
                HorseController horse = Instantiate(horsePrefab, spawnPoints[horseNumber - 1].position, spawnPoints[horseNumber - 1].rotation);
                horse.gameObject.name = $"Horse {horseNumber}";

                //Load CHaracter Data Only in Load Game Scene
                if (isLoadGameScene)
                {
                    HorseControllerLoad horseControllerLoad = horse as HorseControllerLoad;
                    if (horseControllerLoad != null)
                    {
                        horseControllerLoad.Character.CreateNewTexture();
                        horseControllerLoad.SetCharacter(UGSManager.Instance.HostRaceData.characterCustomisationDatas[horseNumber]);
                        horseControllerLoad.Character.EnableFace();
                        RenderTexture renderTexture = GameManager.Instance.CaptureObject.Capture(horseControllerLoad.Character.gameObject);
                        horseControllerLoad.Character.EnableFullBody();
                        hostRaceData.currentRaceAvatars[horseNumber] = renderTexture;
                    }
                }

                //Generate Random Materials
                horse.InitializeMaterials(horseJockeyMaterials.horseMaterials[Utils.GenerateRandomNumber(0, horseJockeyMaterials.horseMaterials.Length)]);
                horse.SetHorseNumber(horseNumber);
                horseControllers[i] = horse;
            }
            if (isLoadGameScene)
            {
                UGSManager.Instance.SetHostRaceData(hostRaceData);
            }
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
            HostRaceData hostRaceData = new HostRaceData();
            Dictionary<int, CharacterCustomisationEconomy> characterCustomisationDatas = new Dictionary<int, CharacterCustomisationEconomy>();
            foreach (var horseNumber in GameManager.Instance.HorsesToSpawnList)
            {
                characterCustomisationDatas[horseNumber] = new CharacterCustomisationEconomy();

                int upperArm = Utils.GenerateRandomNumber(0, 3);
                int lowerArm = Utils.GenerateRandomNumber(0, 3);
                characterCustomisationDatas[horseNumber].upperOutfit = new UpperOutfitEconomy()
                {
                    torso = Utils.GenerateRandomNumber(0, 3),
                    leftUpperArm = upperArm,
                    rightUpperArm = upperArm,
                    leftLowerArm =  lowerArm,
                    rightLowerArm = lowerArm,
                };

                int hips = Utils.GenerateRandomNumber(0, 3);
                int leg = Utils.GenerateRandomNumber(0, 3);
                int foot = Utils.GenerateRandomNumber(0, 3);
                characterCustomisationDatas[horseNumber].lowerOutfit = new LowerOutfitEconomy()
                {
                    hips = hips,
                    leftLeg = leg,
                    rightLeg = leg,
                    leftFoot = foot,
                    rightFoot = foot,
                };

                int randomRange = Utils.GenerateRandomNumber(0, 3);
                switch (randomRange)
                {
                    case 0:
                        characterCustomisationDatas[horseNumber].customParts.Add(new CustomPartEconomy()
                        {
                            styleNumber = Utils.GenerateRandomNumber(0, 3),
                            type = (int)BlendPartType.FacialHair,
                        });
                        break;
                    case 1:
                        break;
                    case 2:
                        characterCustomisationDatas[horseNumber].customParts.Add(new CustomPartEconomy()
                        {
                            styleNumber = Utils.GenerateRandomNumber(0, 3),
                            type = (int)BlendPartType.Hair,
                        });
                        break;
                }
            }
            hostRaceData.characterCustomisationDatas = characterCustomisationDatas;
            UGSManager.Instance.SetHostRaceData(hostRaceData);
        }
        #endregion
    }
}
