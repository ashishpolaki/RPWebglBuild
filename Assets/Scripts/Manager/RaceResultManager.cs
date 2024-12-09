using HorseRace;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class RaceResultManager : MonoBehaviour
{
    [SerializeField] private List<Transform> horseTransformsList;
    [SerializeField] private List<Transform> characterTransformsList;
    [SerializeField] private List<HorseControllerLoad> horses;
    [SerializeField] private RuntimeAnimatorController jockeyStandAnimatorController;

    public void InitializeRaceResults(List<HorseControllerLoad> _horses)
    {
        horses = _horses;

        for (int i = 0; i < horses.Count ; i++)
        {
            //Load Horse
            horses[i].RemoveNavmeshAgent();
            horses[i].transform.position = horseTransformsList[i].position;
            horses[i].transform.rotation = horseTransformsList[i].rotation;

            //Load Character
            horses[i].Character.transform.parent = null;
            horses[i].Character.transform.position = characterTransformsList[i].position;
            horses[i].Character.transform.rotation = characterTransformsList[i].rotation;

            //Change Character Animator
            horses[i].Character.ChangeAnimator(jockeyStandAnimatorController);
        }

        UIController.Instance.ScreenEvent(ScreenType.RaceResults, UIScreenEvent.Open);
        UIController.Instance.ScreenEvent(ScreenType.Race, UIScreenEvent.Close);
    }
}
