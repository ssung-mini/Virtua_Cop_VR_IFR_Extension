using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class DownsamplingController : MonoBehaviour
{
    private int experimentType;

    public bool onGoing;                            // 주시 상실 등으로 잠깐 멈출때, FoV (outRadDeg)가 감소하는 것을 방지.
    public bool isEyeFixed;                         // 주시가 이루어지고 있는지 확인.

    public DynamicBlurring cam;
    public ExperimentManager experimentManager;

    public ControllerScript leftController;
    public ControllerScript rightController;

    [ReadOnly]
    public float WaitingTime;                      // outRadDeg가 줄어드는 간격.

    [ReadOnly]
    public float downsamplingSizeChange;            // Downsampling area의 크기가 frame당 줄어드는 정도.

    [ReadOnly]
    public bool recognitionFail;                    // FoV가 임계점에 도달해도 이미지 품질 저하를 인지하지 못하는 경우 실패 처리.


    void Awake()
    {
        //experimentType = experimentManager.experimentType;
        WaitingTime = 1.0f;                         // FoV (outRadDeg)를 감소되는 시간 간격.
        downsamplingSizeChange = 0.1f;              // FoV (outRadDeg)가 1/60초당 0.1 degree씩 감소함.
    }

    // Update is called once per frame
    void Update()
    {
        isEyeFixed = cam.eye_fixed;
        experimentType = experimentManager.experimentType;
        //Debug.Log(experimentType);

        //if (leftController.trigger || rightController.trigger) StopAllCoroutines();
        if (leftController.trigger || rightController.trigger || experimentManager.initiationStarter || experimentManager.initPlaying) StopAllCoroutines();

        else if (!experimentManager.initiationStarter && !experimentManager.initPlaying && isEyeFixed)
        {
            switch (experimentType)
            {
                case 1:         // 1: FoV measurement.
                    if (cam.outRadDeg > 15f && onGoing)
                    {
                        onGoing = false;
                        StartCoroutine(FoVDecreasement());
                    }
                    else if (onGoing)      // 이미지 품질저하 감지 실패.
                    {
                        var temp = GameObject.Find("Controller (left)").GetComponent<ControllerScript>();
                        temp.DataWrite();
                        recognitionFail = true;
                        experimentManager.initiationStarter = true;
                        StopAllCoroutines();
                    }
                    else StopCoroutine(FoVDecreasement());
                    break;

                case 2:         // 2: Downsampling Rate measurement.
                    if (cam.outRadDeg < 512 && onGoing)
                    {
                        onGoing = false;
                        StartCoroutine(RateDecreasement());
                    }
                    else if (onGoing)      // 이미지 품질저하 감지 실패.
                    {
                        var temp = GameObject.Find("Controller (left)").GetComponent<ControllerScript>();
                        temp.DataWrite();
                        recognitionFail = true;
                        experimentManager.initiationStarter = true;
                        StopAllCoroutines();
                    }
                    else StopCoroutine(RateDecreasement());
                    break;
            }
        }
    }

    IEnumerator FoVDecreasement()
    {
        cam.outRadDeg -= downsamplingSizeChange;
        yield return new WaitForSecondsRealtime(WaitingTime / 60);
        onGoing = true;
    }

    IEnumerator RateDecreasement()
    {
        cam.downsamplingRate += 1;
        yield return new WaitForSecondsRealtime(WaitingTime * 2);   // rate는 1씩 증가하니까 .
        onGoing = true;
    }
}
