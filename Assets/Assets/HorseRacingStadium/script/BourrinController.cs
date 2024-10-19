using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BourrinController : MonoBehaviour
{
    [SerializeField]
    private Animator animatorController;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            Debug.Log("Animator Trigger 0");
            animatorController.SetTrigger("0");
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            animatorController.SetTrigger("1");
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            animatorController.SetTrigger("2");
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            animatorController.SetTrigger("3");
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            animatorController.SetTrigger("4");
        }

        if (Input.GetKeyUp(KeyCode.Y))
        {
            animatorController.SetTrigger("5");
        }
    }
}
