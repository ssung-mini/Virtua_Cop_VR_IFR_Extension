using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    [Range(1, 2)]
    public int experimentType;          // 1: FoV measure, 2: Sampling rate measure.
    
    public bool initPlaying;
    public float initTime;

    public int originalRate;            // testMode = 1 (FoV를 조절하는 경우) Rate 저장
    public float originalFoV;           // testMode = 2 (Rate를 조절하는 경우) FoV 저장
    
    [ReadOnly]
    public float waitingTime = 10.0f;   // Initiation 및 Fixation loss시 휴식

    [ReadOnly]
    public int totalTrial;              // 총 trial 횟수.

    [ReadOnly]
    public int totalPath;               // path의 종류.

    [ReadOnly]
    public int eachTrial;               // 한 path당 몇번 반복할 것인가.

    [ReadOnly]
    public int nowTrial;                // 현재 몇번째 시도인가?
    
    [ReadOnly]
    public int[] pathType;              // 1 ~ 5 각각 2번씩.

    [ReadOnly]
    public bool initiationStarter;

    public Canvas fixationLossCanvas;
    public GameObject cam2;
    public GameObject player;
    public GameObject eyeFixationPoint;
    public DynamicBlurring cam;
    public PlayerPathController pathController;
    public DownsamplingController downsamplingController;
    public ControllerScript leftController;
    public ControllerScript rightController;
    
    void Awake()
    {
        if(experimentType == 1)
        {
            originalFoV = 110.0f;               // ExperimentType = 1일때 110으로 시작 / 2일때는 1에서 구한 피험자 결과값 Average 구해서 적용.
            originalRate = 4;                   // ExperimentType = 1일때는 4 / 2일때는 1로 시작.
        }
        else if (experimentType == 2)
        {
            /*List<Dictionary<string, object>> data = CSVReader.Read();

            for(var i =0; i<data.Count; i++)
            {
                Debug.Log("Data: " + data[i]);
            }*/
            //originalFoV = 110.0f;                 // Downsampling rate 구할때 값 구해서 적용할것.
            originalRate = 1;                       // ExperimentType = 1일때는 4 / 2일때는 1로 시작.
        }

        nowTrial = 0;
        totalPath = 10;                         // path 종류는 1~10까지.
        eachTrial = 1;                          // 각 path당 1번씩.
        totalTrial = totalPath * eachTrial;     // 그래서 total 위의 두개를 곱한것.
        initiationStarter = false;              // Controller 응답 후 초기화 진행?

        pathType = new int[totalTrial * 2];
        int[] count = new int[totalTrial * 2];

        for (int i = 0; i < totalTrial; i++)
        {
            count[i] = 0;
        }

        for (int i = 0; i < totalTrial; i++)
        {
            while(true)
            {
                int temp = UnityEngine.Random.Range(1, totalPath + 1);
                if (count[temp] < eachTrial)
                {
                    pathType[i] = temp;   // path는 1~5 사이의 타입.
                    count[temp] += 1;
                    break;
                }
            }
        }
    }

    void Start()
    {
        Initiation();
    }

    void Update()
    {
        if (initiationStarter)
        {
            if (!initPlaying) StartCoroutine(WaitingForInitiation());
            else StopCoroutine(WaitingForInitiation());
        }
    }

    private void Initiation()
    {
        Debug.Log("현재 " + nowTrial + "번째.");
        initiationStarter = false;
        cam.outRadDeg = originalFoV;
        cam.downsamplingRate = originalRate;
        downsamplingController.onGoing = true;
        pathController.animExit = false;
    }

    IEnumerator WaitingForInitiation()
    {
        nowTrial += 1;
        if (nowTrial >= totalTrial)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        Debug.Log("현재 " + nowTrial + "번째.");

        eyeFixationPoint.SetActive(false);
        fixationLossCanvas.gameObject.SetActive(true);
        
        initiationStarter = false;
        cam.outRadDeg = originalFoV;
        cam.downsamplingRate = originalRate;
        downsamplingController.onGoing = true;
        pathController.animExit = false;

        initPlaying = true;

        yield return new WaitForSecondsRealtime(waitingTime);

        fixationLossCanvas.gameObject.SetActive(false);
        eyeFixationPoint.SetActive(true);
        initPlaying = false;

    }
}