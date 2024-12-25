using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HorseRace
{
    public class RaceWinnerSpawn : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer[] horseMeshes;
        [SerializeField] private Animator jockeyAnimator;
        [SerializeField] private float fadeValue = 0.25f;

        private void Start()
        {
            ChangeAnimationState();
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
