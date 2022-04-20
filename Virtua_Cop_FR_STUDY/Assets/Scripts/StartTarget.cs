using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTarget : MonoBehaviour
{
    public void Die()
    {
        Destroy(gameObject);
        AnimationManager.StartStageOne();
        CsvWritingManager.Reset_Timer();
        print("Start");
    }
}
