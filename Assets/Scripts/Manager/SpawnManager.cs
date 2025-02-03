using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UGS;

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
                // LoadCharactersCheat();
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

                //Generate Horse Material
                int materialIndex = 0;
                if (UGSManager.Instance != null && UGSManager.Instance.HostRaceData.horseCustomisationDatas.ContainsKey(horseNumber))
                {
                    materialIndex = UGSManager.Instance.HostRaceData.horseCustomisationDatas[horseNumber].bodyColorIndex;
                }
                else
                {
                    materialIndex = Utils.GenerateRandomNumber(0, horseJockeyMaterials.horseMaterials.Length - 1);
                }
                horse.InitializeMaterials(horseJockeyMaterials.horseMaterials[materialIndex]);
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
        private void LoadCharactersCheat()
        {
            //Delete Later
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            GameManager.Instance.LoadHorsesInRaceOrder();
            HostRaceData hostRaceData = new HostRaceData();
            Dictionary<int, CharacterCustomisationEconomy> characterCustomisationDatas = new Dictionary<int, CharacterCustomisationEconomy>();
            foreach (var horseNumber in GameManager.Instance.HorsesToSpawnList)
            {
                characterCustomisationDatas[horseNumber] = new CharacterCustomisationEconomy();

                int upperArm = Utils.GenerateRandomNumber(0, 1);
                int lowerArm = Utils.GenerateRandomNumber(0, 1);
                characterCustomisationDatas[horseNumber].upperOutfit = new UpperOutfitEconomy()
                {
                    torso = Utils.GenerateRandomNumber(0, 3),
                    leftUpperArm = upperArm,
                    rightUpperArm = upperArm,
                    leftLowerArm = lowerArm,
                    rightLowerArm = lowerArm,
                };

                int hips = Utils.GenerateRandomNumber(0, 3);
                int leg = Utils.GenerateRandomNumber(0, 1);
                int foot = Utils.GenerateRandomNumber(0, 1);
                characterCustomisationDatas[horseNumber].lowerOutfit = new LowerOutfitEconomy()
                {
                    hips = hips,
                    leftLeg = leg,
                    rightLeg = leg,
                    leftFoot = foot,
                    rightFoot = foot,
                };
            }
            hostRaceData.characterCustomisationDatas = characterCustomisationDatas;
            UGSManager.Instance.SetHostRaceData(hostRaceData);
        }
        #endregion
    }
}
