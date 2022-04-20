using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Valve.VR;

public class ControllerScript : MonoBehaviour
{
    public GameObject EyeFixationPoint;
    public DynamicBlurring cam;
    public DownsamplingController downsamplingController;
    public ExperimentManager experimentManager;
    public FileWriterManager fileWriterManager;

    //public SteamVR_ActionSet _defaultSet;
    public SteamVR_ActionSet _downsamplingSet;
    public SteamVR_Action_Boolean downsamplingRecognition;

    public AudioClip soundEffect;
    public AudioSource myAudio;

    public bool trigger;
    public bool Check;
    
    private List<String> data;

    void Awake()
    {
        data = new List<String>();
        trigger = false;
    }

    void Start()
    {
        _downsamplingSet.Activate();
        myAudio = gameObject.GetComponent<AudioSource>();
        soundEffect = myAudio.clip;
    }

    void Update()
    {
        trigger = downsamplingRecognition.GetLastStateDown(SteamVR_Input_Sources.Any);
        if (trigger && !experimentManager.initPlaying)
        {
            DataWrite();
            myAudio.PlayOneShot(soundEffect);
            cam.eye_fixed = false;
            experimentManager.initiationStarter = true;
        }
    }

    public void DataWrite()
    {
        data.Add((experimentManager.nowTrial + 1).ToString());
        data.Add(cam.outRadDeg.ToString());
        data.Add(cam.downsamplingRate.ToString());
        data.Add(experimentManager.pathType[experimentManager.nowTrial].ToString());
        fileWriterManager.WriteMeasurementResult(data);
        //gameObject.GetComponent<FileWriterManager>().WriteMeasurementResult(data);
        data.Clear();
    }
}
