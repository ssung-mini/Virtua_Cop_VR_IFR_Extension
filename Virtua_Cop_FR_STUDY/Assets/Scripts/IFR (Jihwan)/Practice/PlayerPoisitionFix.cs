using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPoisitionFix : MonoBehaviour
{
    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        UnityEngine.XR.InputTracking.disablePositionalTracking = true;
        transform.position = new Vector3(0.0f, 2.0f, 8.0f);
    }
}
