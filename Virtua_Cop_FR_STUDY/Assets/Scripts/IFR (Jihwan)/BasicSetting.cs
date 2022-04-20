//using System.Collections;
//using System.Collections.Generic;
//using Tobii.XR;
//using Tobii.G2OM;
using UnityEngine;
using UnityEngine.XR;
using ViveSR.anipal.Eye;
using System;

public class BasicSetting : MonoBehaviour
{
    void Awake()
    {
        //EyeCalibration();
    }

    private void EyeCalibration()
    {
        int state = 1;
        while (state != 0)
            state = SRanipal_Eye_API.LaunchEyeCalibration(IntPtr.Zero);
    }
}