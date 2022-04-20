using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Tobii.XR.Examples;

public class VisualSearchTaskManager : MonoBehaviour
{
    [ReadOnly]
    public int targetOrderNum;      // 자극 등장 순서.

    [ReadOnly]
    private int searchTaskTrial;

    [ReadOnly]
    private int nowTrial;

    [ReadOnly]
    private int targetType;         // Color: Red(왼쪽) / Blue(오른쪽) | Shape: Tetrahydron(왼쪽) / Sphere(오른쪽)

    [ReadOnly]
    private float jitteringTime;

    [ReadOnly]
    private float responseTime;

    [ReadOnly]
    private float timeRealGaze;     // 응시하고 있는 시간이 0.2초 이상인지 아닌지 확인, Saccadic movement 잡기 위한 시간 체크.

    [ReadOnly]
    private int[,] order;

    public bool practiceSession;    // 연습 session 인경우 기록 X.
    public bool shapeBlock;         // shape를 확인하는 block인 경우 check. false인 경우 color session

    private int targetCount;
    private bool waiting;
    private bool isStarting;

    private List<String> data;
    public List<GameObject> targetList;

    [SerializeField]
    private Vector3 previousRealGaze;

    public GameObject tetrahydronObject;                        // shape 맞추는 task에 필요한 tetrahydron 개체.
    public GameObject targets;                                  // 자극들.
    public GameObject centerPoint;                              // 자극 위치 중 정중앙에 위치함으로써 각 trial 시작 시 시야 정렬점
    public GameObject Camera;                                   // 카메라.
    public GameObject gazePosition;                             // 눈이랑, 헤드무브먼트를 결합한 상태에서 어디를 보고 있는지
    public GameObject eyeFixationPoint;                         // 시야 고정점.
    public EyeDataManager eyeDataManager;                       // 시야 정보 확보.
    public FileWriterManager fileWriterManager;
    public DynamicBlurring_VisualSearchTask cam;                // 시야 상태 조절.
    public ControllerScript_VisualSearchTask leftController;    // 응답.
    public ControllerScript_VisualSearchTask rightController;   // 응답.

    public AudioClip soundEffect;
    public AudioSource myAudio;

    void Awake()
    {
        nowTrial = 0;
        searchTaskTrial = 62;
        responseTime = 0.0f;
        targetCount = searchTaskTrial;
        targetType = UnityEngine.Random.Range(0, 2);
        waiting = false;

        timeRealGaze = 0.0f;
        gazePosition.transform.position = eyeFixationPoint.transform.position;
        previousRealGaze = eyeFixationPoint.transform.position;
        
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\DownsamplingData\\" + fileWriterManager.participantNum.ToString() + "\\";
        string path2;
        for (int i = 1; i <= 500; i++)
        {
            path2 = path + "VisualSearchTask\\" + i.ToString() + "\\";
            DirectoryInfo di = new DirectoryInfo(path2);
            if (di.Exists == false)
            {
                targetOrderNum = (i - 1) % 6;   // 자극의 시퀀스도 control 되어야하는지 물어보고 아니면 그냥 8하고 해야되면 2하고.
                break;
            }
        }

        data = new List<String>();
        order = new int[6, 62] {
            { 0, 41, 35, 4, 43, 38, 48, 52, 39, 42, 18, 47, 50, 26, 23, 37, 49, 24, 27, 51, 45, 53, 40, 20, 44, 21, 28, 22, 46, 29, 31, 9, 30, 2, 33, 54, 25, 11, 36, 12, 6, 34, 5, 15, 3, 55, 16, 57, 17, 58, 10, 1, 32, 19, 13, 56, 7, 59, 8, 60, 14, 61 },
            { 14, 61, 29, 50, 26, 27, 38, 49, 22, 5, 17, 3, 55, 13, 56, 46, 39, 42, 24, 1, 48, 32, 19, 52, 16, 28, 15, 18, 47, 37, 35, 60, 33, 54, 31, 30, 2, 53, 40, 20, 44, 21, 23, 9, 57, 0, 41, 51, 45, 4, 43, 25, 10, 58, 7, 59, 8, 6, 34, 11, 36, 12 },
            { 20, 40, 23, 9, 57, 17, 3, 53, 42, 24, 47, 37, 58, 48, 32, 22, 5, 7, 8, 6, 34, 11, 36, 12, 55, 0, 44, 2, 45, 4, 43, 25, 10, 14, 61, 56, 46, 39, 21, 52, 16, 59, 19, 50, 26, 27, 51, 60, 33, 54, 31, 18, 35, 29, 28, 15, 38, 49, 13, 41, 30, 1 },
            { 52, 37, 8, 6, 34, 14, 39, 50, 25, 10, 57, 48, 33, 55, 16, 56, 46, 42, 36, 12, 19, 20, 40, 32, 58, 28, 0, 26, 27, 15, 38, 23, 9, 60, 21, 54, 17, 3, 35, 29, 11, 53, 22, 59, 24, 47, 61, 7, 51, 43, 2, 31, 18, 5, 45, 4, 44, 49, 13, 41, 1, 30 },
            { 37, 2, 31, 18, 5, 45, 4, 55, 47, 49, 13, 41, 1, 17, 3, 7, 42, 15, 8, 51, 14, 33, 23, 16, 56, 9, 52, 6, 39, 50, 25, 61, 38, 60, 43, 10, 27, 48, 21, 30, 57, 44, 54, 22, 59, 24, 34, 35, 29, 11, 53, 40, 32, 58, 36, 12, 19, 20, 28, 0, 26, 46 },
            { 61, 28, 58, 57, 17, 18, 22, 46, 29, 56, 31, 9, 47, 50, 24, 43, 52, 19, 13, 37, 35, 55, 7, 59, 48, 39, 32, 49, 1, 51, 45, 53, 6, 34, 41, 38, 27, 4, 25, 11, 44, 21, 10, 54, 5, 15, 16, 40, 2, 33, 26, 23, 36, 12, 42, 20, 3, 30, 8, 60, 14, 0 }
        };
        /*order = new int[2, 62] {
            { 0, 41, 35, 4, 43, 38, 48, 52, 39, 42, 18, 47, 50, 26, 23, 37, 49, 24, 27, 51, 45, 53, 40, 20, 44, 21, 28, 22, 46, 29, 31, 9, 30, 2, 33, 54, 25, 11, 36, 12, 6, 34, 5, 15, 3, 55, 16, 57, 17, 58, 10, 1, 32, 19, 13, 56, 7, 59, 8, 60, 14, 61 },
            { 61, 28, 58, 57, 17, 18, 22, 46, 29, 56, 31, 9, 47, 50, 24, 43, 52, 19, 13, 37, 35, 55, 7, 59, 48, 39, 32, 49, 1, 51, 45, 53, 6, 34, 41, 38, 27, 4, 25, 11, 44, 21, 10, 54, 5, 15, 16, 40, 2, 33, 26, 23, 36, 12, 42, 20, 3, 30, 8, 60, 14, 0 }
        };*/

        /*order = new int[8, 80] { 
            { 79, 6, 39, 69, 32, 19, 72, 46, 55, 14, 30, 41, 49, 35, 29, 53, 44, 7, 64, 75, 78, 63, 10, 0, 70, 31, 5, 40, 36, 58, 71, 66, 34, 22, 65, 68, 62, 50, 56, 60, 73, 67, 61, 27, 45, 57, 13, 37, 74, 59, 1, 15, 24, 16, 77, 2, 28, 54, 33, 17, 18, 42, 3, 52, 9, 25, 4, 12, 47, 20, 11, 43, 23, 76, 38, 26, 48, 51, 21, 8 },
            { 0, 40, 7, 11, 6, 68, 41, 58, 67, 31, 5, 39, 29, 10, 30, 53, 32, 19, 44, 36, 20, 23, 49, 35, 27, 50, 21, 38, 37, 26, 48, 28, 52, 47, 24, 51, 46, 55, 45, 54, 33, 57, 62, 34, 22, 60, 73, 59, 69, 56, 61, 70, 65, 72, 74, 64, 75, 78, 63, 71, 66, 14, 76, 16, 77, 2, 13, 1, 15, 79, 17, 18, 42, 3, 9, 25, 8, 12, 43, 4 },
            { 52, 51, 17, 18, 42, 60, 73, 59, 47, 24, 77, 2, 13, 31, 58, 67, 5, 39, 46, 1, 15, 79, 54, 21, 55, 45, 26, 48, 38, 37, 0, 40, 28, 9, 25, 4, 7, 11, 6, 68, 12, 43, 57, 44, 41, 33, 74, 64, 36, 20, 75, 65, 49, 35, 72, 66, 8, 29, 10, 50, 76, 16, 14, 78, 63, 71, 62, 34, 23, 27, 69, 56, 22, 61, 70, 30, 53, 32, 19, 3 },
            { 39, 13, 31, 15, 18, 12, 43, 38, 37, 0, 19, 52, 42, 2, 25, 4, 57, 44, 58, 67, 5, 55, 3, 26, 7, 11, 6, 68, 28, 9, 51, 17, 65, 49, 40, 24, 77, 33, 73, 59, 46, 79, 47, 54, 1, 21, 74, 60, 50, 72, 66, 76, 16, 14, 8, 29, 48, 45, 41, 64, 36, 20, 75, 10, 56, 35, 62, 34, 23, 69, 22, 27, 78, 63, 71, 32, 61, 70, 30, 53 },
            { 40, 15, 24, 11, 51, 52, 5, 39, 67, 6, 43, 79, 2, 13, 31, 58, 47, 59, 46, 77, 37, 0, 55, 8, 44, 41, 18, 68, 28, 42, 25, 4, 72, 74, 62, 57, 9, 54, 45, 60, 73, 1, 12, 7, 17, 38, 20, 21, 34, 27, 61, 56, 35, 29, 75, 65, 26, 48, 64, 36, 33, 49, 10, 22, 23, 32, 63, 16, 50, 71, 69, 14, 78, 66, 76, 30, 3, 70, 53, 19 },
            { 31, 58, 18, 42, 12, 43, 57, 44, 55, 45, 6, 68, 77, 46, 1, 26, 48, 52, 15, 79, 54, 21, 24, 74, 2, 0, 67, 34, 23, 27, 69, 56, 22, 61, 70, 5, 39, 3, 36, 20, 60, 73, 51, 17, 59, 47, 64, 33, 4, 7, 11, 13, 38, 30, 53, 75, 37, 41, 8, 29, 10, 49, 35, 72, 66, 65, 25, 50, 76, 16, 14, 78, 63, 71, 62, 40, 28, 9, 19, 32 },
            { 18, 4, 73, 59, 47, 20, 75, 65, 49, 35, 72, 13, 54, 21, 55, 45, 26, 48, 39, 46, 1, 15, 79, 6, 66, 8, 29, 7, 11, 42, 60, 10, 24, 77, 2, 68, 12, 43, 57, 44, 41, 50, 76, 33, 74, 38, 37, 0, 40, 28, 9, 64, 36, 63, 71, 62, 70, 30, 53, 32, 19, 56, 34, 3, 31, 58, 67, 5, 52, 23, 27, 16, 14, 78, 69, 51, 25, 22, 61, 17 },
            { 67, 21, 65, 45, 26, 48, 38, 37, 0, 40, 28, 9, 25, 51, 17, 18, 49, 55, 4, 7, 11, 6, 68, 12, 43, 57, 44, 41, 33, 74, 64, 36, 20, 75, 50, 76, 16, 14, 78, 63, 71, 10, 53, 32, 19, 24, 59, 77, 2, 13, 31, 58, 52, 5, 62, 34, 23, 61, 70, 30, 42, 60, 47, 39, 35, 72, 66, 8, 29, 73, 27, 69, 56, 22, 46, 1, 15, 79, 54, 3 }
        };*/

        for (int i = 0; i < targetCount; i++)
        {
            targetList.Add(targets.transform.GetChild(i).gameObject);
            targetList[i].GetComponent<MeshRenderer>().material.color = Color.gray;
        }

        isStarting = false;
        centerPoint.GetComponent<HighlightAtGaze_Practice>().focused = false;
        centerPoint.SetActive(true);
        targets.SetActive(false);
    }

    void Start()
    {
        myAudio = gameObject.GetComponent<AudioSource>();
        soundEffect = myAudio.clip;
    }

    void Update()
    {
        fileWriterManager.WriterRealGazePositionFullData(eyeDataManager.realGazePosition, nowTrial);    // Saccadic movement 제외하지 않은 모든 움직임.
        fileWriterManager.WriteEyeMovementFullData(eyeDataManager.gazePosition, nowTrial);              // 현재 보고있는 곳이 렌즈의 어느 위치인지 기록.

        if (nowTrial >= searchTaskTrial)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        if(!isStarting && centerPoint.GetComponent<HighlightAtGaze_Practice>().focused)
        {
            centerPoint.SetActive(false);
            isStarting = true;
            Initiation();
        }
        else if(isStarting && nowTrial < searchTaskTrial && !waiting)
        {
            responseTime += Time.deltaTime;

            fileWriterManager.WriterHeadMovement(UnityEditor.TransformUtils.GetInspectorRotation(Camera.transform), nowTrial);  // Head rotation 값은 inspector 값 그대로 가져옴.

            gazePosition.GetComponent<Transform>().position = eyeDataManager.realGazePosition;                  // Head rotation + Eye movement 결합된 현재 실제로 보고 있는 곳의 위치.
            Vector2 xyNow = new Vector2(gazePosition.transform.position.x, gazePosition.transform.position.y);  // 화면상 현재 보고 있는 점의 위치.
            Vector2 xyPrevious = new Vector2(previousRealGaze.x, previousRealGaze.y);                           // 화면상 바로 이전 frame에서 봤던 점의 위치.
            var distance = Vector2.Distance(xyNow, xyPrevious);                                                 // 두 점간의 거리. 
            //Debug.Log("Distance: " + distance);

            if (distance < 1.0f)
            {
                timeRealGaze += Time.deltaTime;
                if(timeRealGaze > 0.2f)
                {
                    //Debug.Log("Time: " + timeRealGaze);
                    timeRealGaze = 0.0f;
                    gazePosition.GetComponent<MeshRenderer>().material.color = Color.black;

                    fileWriterManager.WriteEyeMovement(eyeDataManager.gazePosition, nowTrial);                              // Saccadic movement 제외된 현재 보고있는 HMD 렌즈상의 위치.
                    fileWriterManager.WriterRealGazePosition(gazePosition.GetComponent<Transform>().position, nowTrial);    // Saccadiv movement 제외된 현제 보고있는 위치의 world position.
                }
            }
            else gazePosition.GetComponent<MeshRenderer>().material.color = Color.red;

            StopAllCoroutines();
            if (leftController.leftTrigger || rightController.rightTrigger)
            {
                Debug.Log(nowTrial + " is finished.");
                if(!practiceSession)
                {
                    data.Add(nowTrial.ToString());
                    if (shapeBlock) data.Add("Shape");
                    else data.Add("Color");
                    data.Add(targetOrderNum.ToString());
                    data.Add(cam.outRadDeg.ToString());
                    data.Add(cam.downsamplingRate.ToString());
                    data.Add(responseTime.ToString());
                    if (targetType == leftController.leftorright) data.Add("O");
                    else data.Add("X");
                    fileWriterManager.WriteVisualSearchTaskResult(data);
                    data.Clear();
                }
                if (targetType == leftController.leftorright) Debug.Log("Right answer");
                else Debug.Log("Wrong");
                jitteringTime = UnityEngine.Random.Range(0.5f, 1.5f);
                StartCoroutine(Waiting());
            }
        }
    }

    void LateUpdate()
    {
        previousRealGaze = gazePosition.transform.position;
    }

    IEnumerator Waiting()
    {
        waiting = true;
        responseTime = 0.0f;
        targetList[order[targetOrderNum, nowTrial]].GetComponent<MeshRenderer>().material.color = Color.gray;
        targets.SetActive(false);
        nowTrial += 1;

        yield return new WaitForSecondsRealtime(jitteringTime);

        myAudio.PlayOneShot(soundEffect);
        isStarting = false;
        centerPoint.SetActive(true);
    }

    void Initiation()
    {
        waiting = false;
        targets.SetActive(true);
        for (int i = 0; i < targetCount; i++)
        {
            targets.transform.GetChild(i).gameObject.SetActive(false);
        }
        if (shapeBlock)
        {
            targets.transform.GetChild(order[targetOrderNum, nowTrial]).gameObject.SetActive(true);
            targetType = UnityEngine.Random.Range(0, 2);
            if (targetType == 0) targetList[order[targetOrderNum, nowTrial]].GetComponent<MeshFilter>().mesh = tetrahydronObject.GetComponent<MeshFilter>().sharedMesh;
        }
        else
        {
            targets.transform.GetChild(order[targetOrderNum, nowTrial]).gameObject.SetActive(true);
            targetType = UnityEngine.Random.Range(0, 2);
            if (targetType == 0) targetList[order[targetOrderNum, nowTrial]].GetComponent<MeshRenderer>().material.color = Color.red;
            else targetList[order[targetOrderNum, nowTrial]].GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }
}