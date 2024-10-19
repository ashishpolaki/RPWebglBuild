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
            HorseController[] horseControllers = new HorseController[spawnHorses.Count];
            for (int i = 0; i < spawnHorses.Count; i++)
            {
                int horseNumber = spawnHorses[i];
                //Instantiate
                HorseController horse = Instantiate(horsePrefab, spawnPoints[horseNumber - 1].position, spawnPoints[horseNumber - 1].rotation);
                horse.gameObject.name = $"Horse {horseNumber}";

                //Generate Random Materials
                List<Material> randomHorseMats = new List<Material>() { horseJockeyMaterials.horseMaterials[Utils.GenerateRandomNumber(0, horseJockeyMaterials.horseMaterials.Length)], horseJockeyMaterials.horseHairMaterials[Utils.GenerateRandomNumber(0, horseJockeyMaterials.horseHairMaterials.Length)] };
                List<Material> randomJockeyMats = new List<Material>() { horseJockeyMaterials.jockeyMaterials[Utils.GenerateRandomNumber(0, horseJockeyMaterials.jockeyMaterials.Length)] };
                horse.InitializeMaterials(randomHorseMats, randomJockeyMats);
                horse.SetHorseNumber(horseNumber);
                horseControllers[i] = horse;
            }
            GameManager.Instance.RaceManager.Initialize(horseControllers);
            GameManager.Instance.CameraController.SetTargetGroup(horseControllers.Select(x => x.transform).ToList());
        }
        #endregion
    }
}
