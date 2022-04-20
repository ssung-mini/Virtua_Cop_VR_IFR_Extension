using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerPathController : MonoBehaviour
{
    [SerializeField]
    private int[] pathType;          // 1 ~ 5 각각 2번씩.

    private int nowTrial;

    public bool animExit;       // Animation이 진행중인가?
    public bool isEyeFixed;
    public ExperimentManager experimentManager;
    public DynamicBlurring cam;
    Animation anim;

    public ControllerScript leftController;
    public ControllerScript rightController;

    void Start()
    {
        pathType = experimentManager.GetComponent<ExperimentManager>().pathType;
        anim = GetComponent<Animation>();
    }

    void Update()
    {
        if (leftController.trigger || rightController.trigger || experimentManager.initiationStarter || experimentManager.initPlaying) StopAllCoroutines();

        else if (!experimentManager.initiationStarter && !experimentManager.initPlaying)
        {
            isEyeFixed = cam.eye_fixed;
            //Debug.Log("Eye: " + isEyeFixed);
            nowTrial = experimentManager.nowTrial;
            string animationName = "Route_" + pathType[nowTrial];
            //Debug.Log("현재 경로명: " + animationName);
            //Debug.Log(animationName + " 경로가 재생중인가?: " + this.GetComponent<Animation>().IsPlaying(animationName));

            if (!anim.IsPlaying(animationName) && !animExit)
            {
                Debug.Log("Animation length (seconds): " + anim[animationName].length);
                StartCoroutine(AnimationPlaying(anim, animationName));
            }
            else if (animExit)
            {
                experimentManager.initiationStarter = true;
                StopAllCoroutines();
                //StopCoroutine(AnimationPlaying(anim, animationName));
            }
            else
            {
                if (!isEyeFixed) anim[animationName].speed = 0;    // 주시상실시 애니메이션 일시정지.
                else anim[animationName].speed = 1;   // 제대로 보고 있는 경우 애니메이션 원상복구.
            }
        }
    }

    IEnumerator AnimationPlaying(Animation anim, string name)
    {
        anim.Rewind(name);
        anim.Play(name);
        Debug.Log(name + " 경로가 재생중.");
        yield return new WaitForSeconds((anim[name].length) * 10);   // 주시상실이 너무 길어져 animation play time이 다 지나서 끝나는 일이 없도록 넉넉하게 시간 줌.
        animExit = true;
    }
}