using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Practice_StartTarget : MonoBehaviour
{
    public GameObject enemy1, enemy2, hostage1, hostage2;

    public void Die()
    {
        enemy1.GetComponent<Animator>().SetBool("Start", true);
        enemy2.GetComponent<Animator>().SetBool("Start", true);
        hostage1.GetComponent<Animator>().SetBool("Start", true);
        hostage2.GetComponent<Animator>().SetBool("Start", true);
        print("Animation Start");

        Destroy(gameObject);
    }
}
