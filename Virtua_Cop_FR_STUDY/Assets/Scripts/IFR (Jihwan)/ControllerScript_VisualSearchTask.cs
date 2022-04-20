using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerScript_VisualSearchTask : MonoBehaviour
{
    public SteamVR_ActionSet _downsamplingSet;
    public SteamVR_Action_Boolean downsamplingRecognition;
    public SteamVR_Action_Boolean visualSearchTaskResponse;
    public SteamVR_Action_Vector2 visualSearchTaskDirection;

    public AudioClip soundEffect;
    public AudioSource myAudio;

    public int leftorright;
    public bool leftTrigger;
    public bool rightTrigger;
    public bool Check;
    public Vector2 direction;

    void Awake()
    {
        leftTrigger = false;
        rightTrigger = false;        
    }
    void Start()
    {
        _downsamplingSet.Activate();
        myAudio = gameObject.GetComponent<AudioSource>();
        soundEffect = myAudio.clip;
    }

    void Update()
    {
        leftTrigger = visualSearchTaskResponse.GetLastStateDown(SteamVR_Input_Sources.LeftHand);
        rightTrigger = visualSearchTaskResponse.GetLastStateDown(SteamVR_Input_Sources.RightHand);
        if (leftTrigger) leftorright = 1;
        else leftorright = 0;
    }
}
