using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI;

namespace HorseRace
{
    public class RaceWinnerSpawn : MonoBehaviour
    {
        [SerializeField] private WinnerHorseJockeyDataSO winnerHorseJockeyDataSO;
        [SerializeField] private SkinnedMeshRenderer[] horseMeshes;
        [SerializeField] private SkinnedMeshRenderer[] jockeyMeshes;
        [SerializeField] private Animator jockeyAnimator;
        [SerializeField] private float fadeValue = 0.25f;

        private void Start()
        {
            ChangeAnimationState();
            InitializeMaterials();
            //   ShowRaceResults();
            UIController.Instance.ScreenEvent(ScreenType.RaceResults, UIScreenEvent.Open);
        }
       
        private async void ShowRaceResults()
        {
            //Get the horses with race Positions
            List<int> racePositionHorseNumbers = GameManager.Instance.HorsesInRaceOrderList;

            //Get the lobby players
            List<UGS.RaceLobbyParticipant> raceLobbyParticipants = await UGSManager.Instance.CloudSave.TryGetRaceLobby(UGSManager.Instance.PlayerData.playerID, StringUtils.RACELOBBY);

            //Create race result data.
            UGS.RaceResult raceResult = new UGS.RaceResult();
            foreach (var raceLobbyParticipant in raceLobbyParticipants)
            {
                raceResult.playerRaceResults.Add(new UGS.PlayerRaceResult
                {
                    PlayerID = raceLobbyParticipant.PlayerID,
                    HorseNumber = raceLobbyParticipant.HorseNumber,
                    RacePosition = racePositionHorseNumbers.IndexOf(raceLobbyParticipant.HorseNumber) + 1
                });
            }

            //Upload race results in cloud
            Func<Task> raceResultResponse = () => UGSManager.Instance.CloudCode.SendRaceResults(raceResult);
            await LoadingScreen.Instance.PerformAsyncWithLoading(raceResultResponse);
        }
        private void InitializeMaterials()
        {
            for (int i = 0; i < horseMeshes.Length; i++)
            {
                horseMeshes[i].materials = winnerHorseJockeyDataSO.winnerHorseMaterials;
            }
            for (int i = 0; i < jockeyMeshes.Length; i++)
            {
                jockeyMeshes[i].materials = winnerHorseJockeyDataSO.WinnerJockeyMaterials;
            }
        }
        private void ChangeAnimationState()
        {
            //Play random animation from the Animator Controller.
            RuntimeAnimatorController runtimeAnimatorController = jockeyAnimator.runtimeAnimatorController;
            AnimationClip[] clips = runtimeAnimatorController.animationClips;
            int randomNumber = UnityEngine.Random.Range(0, clips.Length);
            jockeyAnimator.CrossFade(clips[randomNumber].name, fadeValue, 0);

            //Invoke the same method after the animation clip length
            Invoke("ChangeAnimationState", clips[randomNumber].length);
        }
    }
}
