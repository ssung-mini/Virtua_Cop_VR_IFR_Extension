using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CsvWritingManager : MonoBehaviour
{
    private static float timeRealGaze = 0;     // 응시하고 있는 시간이 0.2초 이상인지 아닌지 확인, Saccadic movement 잡기 위한 시간 체크.
    private static Vector3 previousRealGaze;
    public GameObject gazePosition;
    public EyeDataManager eyeDataManager;
    public GameObject Camera;

    public static float time_start;
    public static float time_current;
    public static bool isEnded = true;

    // Participants Index
    public int _participantsIndex;
    public static int participantsIndex;

    // trialNumber : 1 ~ 6
    public int _trialNumber;
    public static int trialNumber;

    public static string[,,] targetWritingTime = new string[10, 10, 6];

    public static int totalEnemyKill = 0;
    public static int totalHostageKill = 0;
    public static int totalTargetKill = 0;

    public static List<float> checkPercentage = new List<float>();

    // Real-time CsvFireWriter Init (총 5개) => Eye Data & Head Data
    static CsvFileWriter FullRealGazePosition;
    static List<string> FullRealGazeColumns;

    static CsvFileWriter FullEyeMovement;
    static List<string> FullEyeColumns;

    static CsvFileWriter FilteredRealGazePosition;
    static List<string> FilteredRealGazecolumns;

    static CsvFileWriter FilteredEyeMovement;
    static List<string> FilteredEyeColumns;

    static CsvFileWriter HeadMovement;
    static List<string> HeadColumns;

    // 디렉토리 저장 경로
    static string path;

    private void Start()
    {
        participantsIndex = _participantsIndex;
        trialNumber = _trialNumber;

         path = "Assets/Resources/IFR_Study_WritingData/" + participantsIndex.ToString() + "/Trial_" + trialNumber.ToString();

        // 배열 초기화 (Null값 Writing Error 방지)
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                for (int k = 0; k < 6; k++)
                {
                    targetWritingTime[i, j, k] = "\0";
                }
            }
        }

        // 폴더 유무 확인
        
        DirectoryInfo di = new DirectoryInfo(path);
        if (!di.Exists)
            di.Create();

        FullRealGazePosition = new CsvFileWriter(path + "/RealGazePosition(Full).csv");
        FullEyeMovement = new CsvFileWriter(path + "/EyeMovement(Full).csv");
        FilteredRealGazePosition = new CsvFileWriter(path + "/RealGazePosition(Filtered).csv");
        FilteredEyeMovement = new CsvFileWriter(path + "/EyeMovement(Filtered).csv");
        HeadMovement = new CsvFileWriter(path + "/HeadMovement.csv");

        FullRealGazeColumns = new List<string>() { "CurrentTime", "RealGazePos_x", "RealGazePos_y", "RealGazePos_z" };
        FullEyeColumns = new List<string>() { "CurrentTime", "GazePos_x", "GazePos_y" };
        FilteredRealGazecolumns = new List<string>() { "CurrentTime", "RealGazePos_x", "RealGazePos_y", "RealGazePos_z" };
        FilteredEyeColumns = new List<string>() { "CurrentTime", "GazePos_x", "GazePos_y" };
        HeadColumns = new List<string>() { "CurrentTime", "HeadRot_x", "HeadRot_y", "HeadRot_z" };


        // EyeData 첫 Row 설정
        FullRealGazePosition.WriteRow(FullRealGazeColumns);
        FullRealGazeColumns.Clear();
        FullEyeMovement.WriteRow(FullEyeColumns);
        FullEyeColumns.Clear();
        FilteredRealGazePosition.WriteRow(FilteredRealGazecolumns);
        FilteredRealGazecolumns.Clear();
        FilteredEyeMovement.WriteRow(FilteredEyeColumns);
        FilteredEyeColumns.Clear();
        HeadMovement.WriteRow(HeadColumns);
        HeadColumns.Clear();
    }

    void Update()
    {
        if (!isEnded)
        {
            Check_Timer();  // 타이머 시작

            // Eye와 Head 데이터 실시간으로 추가
            // isMoving == false를 통해 스테이지 클리어 후 움직일 때는 기록 X
            if (MovementFlow.isMoving == false)
            {
                WritingEyeData();
                WritingHeadData();
            }
        }
            
    }

    static void WritingTargetTime()
    {
        using (var writer = new CsvFileWriter(path + "/TargetTime.csv"))
        {
            List<string> columns = new List<string>() { "Target Tag", "Appear", "Death", "Appear~Death", "x-position", "z-position" };// making Index Row
            writer.WriteRow(columns);
            columns.Clear();
            for(int i=0; i<10; i++)
            {
                for(int j=0; j<10; j++)
                {
                    
                    columns.Add(targetWritingTime[i, j, 0]);    // 타겟 태그
                    columns.Add(targetWritingTime[i, j, 1]);    // 등장 시간

                    if(targetWritingTime[i, j, 2].Equals("\0") == false)        // 처치하지 않은 Hostage에서 에러가 발생하므로 추가
                    {
                        float appear = float.Parse(targetWritingTime[i, j, 1]);
                        float death = float.Parse(targetWritingTime[i, j, 2]);

                        columns.Add(targetWritingTime[i, j, 2]);    // 제거 시간
                        columns.Add((death - appear).ToString());   // 제거까지 걸린 시간 (제거 - 등장)
                    }

                    else  // 표 형태를 맞춰주기 위함
                    {
                        columns.Add("\0");
                        columns.Add("\0");
                    }

                    // X, Z 포지션 (Y는 무조건 0으로 고정이므로 따로 출력하지 않았음)
                    columns.Add(targetWritingTime[i, j, 4]);    // X-Position
                    columns.Add(targetWritingTime[i, j, 5]);    // Z-Position

                    writer.WriteRow(columns);
                    columns.Clear();
                }
            }

            Debug.Log("Complete save TargetTime.csv");     
        }
    }

    static void WritingPlayTime()
    {
        using (var writer = new CsvFileWriter(path + "/PlayTime.csv"))
        {
            List<string> columns = new List<string>() { "Participants", "Playtime" };// making Index Row
            writer.WriteRow(columns);
            columns.Clear();

            columns.Add(participantsIndex.ToString());  // 피험자 번호
            columns.Add(time_current.ToString());       // 전체 게임 시간 (시작부터 totalEnemyKill == 50까지)

            writer.WriteRow(columns);
            columns.Clear();

            Debug.Log("Complete save PlayTime.csv");

        }
    }

    static void WritingAccuracy()
    {
        using (var writer = new CsvFileWriter(path + "/Accuracy.csv"))
        {
            List<string> columns = new List<string>() { "TargetNum", "Accuracy" };// making Index Row
            writer.WriteRow(columns);
            columns.Clear();

            for (int i = 0; i < totalTargetKill; i++)
            {
                columns.Add((i + 1).ToString());  // 타겟 넘버
                columns.Add(checkPercentage[i].ToString());       // 정확도 (에너미 / 전체)

                writer.WriteRow(columns);
                columns.Clear();
            }

            Debug.Log("Complete save Accuracy.csv");

        }
    }

    void WritingEyeData()
    {
        // FullData Writing
        // Full RealGazePoistion (Saccadic movement 제외하지 않은 모든 움직임)
        FullRealGazeColumns.Add(time_current.ToString());
        FullRealGazeColumns.Add(eyeDataManager.realGazePosition.x.ToString());
        FullRealGazeColumns.Add(eyeDataManager.realGazePosition.y.ToString());
        FullRealGazeColumns.Add(eyeDataManager.realGazePosition.z.ToString());

        FullRealGazePosition.WriteRow(FullRealGazeColumns);
        FullRealGazeColumns.Clear();


        // Full EyeMovement (현재 보고있는 곳이 렌즈의 어느 위치인지 기록)
        FullEyeColumns.Add(time_current.ToString());
        FullEyeColumns.Add(eyeDataManager.gazePosition.x.ToString());
        FullEyeColumns.Add(eyeDataManager.gazePosition.y.ToString());

        FullEyeMovement.WriteRow(FullEyeColumns);
        FullEyeColumns.Clear();
        

        // FilteredData Writing
        gazePosition.GetComponent<Transform>().position = eyeDataManager.realGazePosition;                  // Head rotation + Eye movement 결합된 현재 실제로 보고 있는 곳의 위치.
        Vector2 xyNow = new Vector2(gazePosition.transform.position.x, gazePosition.transform.position.y);  // 화면상 현재 보고 있는 점의 위치.
        Vector2 xyPrevious = new Vector2(previousRealGaze.x, previousRealGaze.y);                           // 화면상 바로 이전 frame에서 봤던 점의 위치.
        var distance = Vector2.Distance(xyNow, xyPrevious);                                                 // 두 점간의 거리. 
        //Debug.Log("Distance: " + distance);
        if (distance < 1.0f)
        {
            timeRealGaze += Time.deltaTime;
            if (timeRealGaze > 0.2f)
            {
                timeRealGaze = 0.0f;

                // Filtered RealGazePosition (Saccadiv movement 제외된 현제 보고있는 위치의 world position)
                FilteredRealGazecolumns.Add(time_current.ToString());
                FilteredRealGazecolumns.Add(eyeDataManager.realGazePosition.x.ToString());
                FilteredRealGazecolumns.Add(eyeDataManager.realGazePosition.y.ToString());
                FilteredRealGazecolumns.Add(eyeDataManager.realGazePosition.z.ToString());

                FilteredRealGazePosition.WriteRow(FilteredRealGazecolumns);
                FilteredRealGazecolumns.Clear();

                // Filtered EyeMovement (Saccadic movement 제외된 현재 보고있는 HMD 렌즈상의 위치)
                FilteredEyeColumns.Add(time_current.ToString());
                FilteredEyeColumns.Add(eyeDataManager.gazePosition.x.ToString());
                FilteredEyeColumns.Add(eyeDataManager.gazePosition.y.ToString());

                FilteredEyeMovement.WriteRow(FilteredEyeColumns);
                FilteredEyeColumns.Clear();
            }
        }
    }

    void WritingHeadData()
    {
        // Head의 Rotation 기록 (Head rotation 값은 inspector 값 그대로 가져옴)
        var headRotation = UnityEditor.TransformUtils.GetInspectorRotation(Camera.transform);

        HeadColumns.Add(time_current.ToString());
        HeadColumns.Add(headRotation.x.ToString());
        HeadColumns.Add(headRotation.y.ToString());
        HeadColumns.Add(headRotation.z.ToString());

        HeadMovement.WriteRow(HeadColumns);
        HeadColumns.Clear();
    }



    // Timer 설정
    public static void Check_Timer()
    {
        time_current = Time.time - time_start;
    }

    public static void End_Timer()
    {
        Debug.Log("End");
        isEnded = true;
        WritingTargetTime();
        WritingPlayTime();
        WritingAccuracy();

        FullRealGazePosition.Dispose();
        FullEyeMovement.Dispose();
        FilteredRealGazePosition.Dispose();
        FilteredEyeMovement.Dispose();
        HeadMovement.Dispose();
    }

    public static void Reset_Timer()
    {
        time_start = Time.time;
        time_current = 0;
        isEnded = false;
        Debug.Log("Start");
    }

    // using문 사용 시 자동으로 dispose되지만, play 버튼 클릭으로 강제적으로 프로그램 종료 시에 에러를 대비해 추가
    private void OnApplicationQuit() 
    {
        if(!isEnded)
        {
            FullRealGazePosition.Dispose();
            FullEyeMovement.Dispose();
            FilteredRealGazePosition.Dispose();
            FilteredEyeMovement.Dispose();
            HeadMovement.Dispose();
        }
    }

    private void LateUpdate()
    {
        previousRealGaze = gazePosition.transform.position;
    }
}
