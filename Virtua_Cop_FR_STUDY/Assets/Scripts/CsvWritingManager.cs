using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CsvWritingManager : MonoBehaviour
{
    private static float timeRealGaze = 0;     // �����ϰ� �ִ� �ð��� 0.2�� �̻����� �ƴ��� Ȯ��, Saccadic movement ��� ���� �ð� üũ.
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

    // Real-time CsvFireWriter Init (�� 5��) => Eye Data & Head Data
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

    // ���丮 ���� ���
    static string path;

    private void Start()
    {
        participantsIndex = _participantsIndex;
        trialNumber = _trialNumber;

         path = "Assets/Resources/IFR_Study_WritingData/" + participantsIndex.ToString() + "/Trial_" + trialNumber.ToString();

        // �迭 �ʱ�ȭ (Null�� Writing Error ����)
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

        // ���� ���� Ȯ��
        
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


        // EyeData ù Row ����
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
            Check_Timer();  // Ÿ�̸� ����

            // Eye�� Head ������ �ǽð����� �߰�
            // isMoving == false�� ���� �������� Ŭ���� �� ������ ���� ��� X
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
                    
                    columns.Add(targetWritingTime[i, j, 0]);    // Ÿ�� �±�
                    columns.Add(targetWritingTime[i, j, 1]);    // ���� �ð�

                    if(targetWritingTime[i, j, 2].Equals("\0") == false)        // óġ���� ���� Hostage���� ������ �߻��ϹǷ� �߰�
                    {
                        float appear = float.Parse(targetWritingTime[i, j, 1]);
                        float death = float.Parse(targetWritingTime[i, j, 2]);

                        columns.Add(targetWritingTime[i, j, 2]);    // ���� �ð�
                        columns.Add((death - appear).ToString());   // ���ű��� �ɸ� �ð� (���� - ����)
                    }

                    else  // ǥ ���¸� �����ֱ� ����
                    {
                        columns.Add("\0");
                        columns.Add("\0");
                    }

                    // X, Z ������ (Y�� ������ 0���� �����̹Ƿ� ���� ������� �ʾ���)
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

            columns.Add(participantsIndex.ToString());  // ������ ��ȣ
            columns.Add(time_current.ToString());       // ��ü ���� �ð� (���ۺ��� totalEnemyKill == 50����)

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
                columns.Add((i + 1).ToString());  // Ÿ�� �ѹ�
                columns.Add(checkPercentage[i].ToString());       // ��Ȯ�� (���ʹ� / ��ü)

                writer.WriteRow(columns);
                columns.Clear();
            }

            Debug.Log("Complete save Accuracy.csv");

        }
    }

    void WritingEyeData()
    {
        // FullData Writing
        // Full RealGazePoistion (Saccadic movement �������� ���� ��� ������)
        FullRealGazeColumns.Add(time_current.ToString());
        FullRealGazeColumns.Add(eyeDataManager.realGazePosition.x.ToString());
        FullRealGazeColumns.Add(eyeDataManager.realGazePosition.y.ToString());
        FullRealGazeColumns.Add(eyeDataManager.realGazePosition.z.ToString());

        FullRealGazePosition.WriteRow(FullRealGazeColumns);
        FullRealGazeColumns.Clear();


        // Full EyeMovement (���� �����ִ� ���� ������ ��� ��ġ���� ���)
        FullEyeColumns.Add(time_current.ToString());
        FullEyeColumns.Add(eyeDataManager.gazePosition.x.ToString());
        FullEyeColumns.Add(eyeDataManager.gazePosition.y.ToString());

        FullEyeMovement.WriteRow(FullEyeColumns);
        FullEyeColumns.Clear();
        

        // FilteredData Writing
        gazePosition.GetComponent<Transform>().position = eyeDataManager.realGazePosition;                  // Head rotation + Eye movement ���յ� ���� ������ ���� �ִ� ���� ��ġ.
        Vector2 xyNow = new Vector2(gazePosition.transform.position.x, gazePosition.transform.position.y);  // ȭ��� ���� ���� �ִ� ���� ��ġ.
        Vector2 xyPrevious = new Vector2(previousRealGaze.x, previousRealGaze.y);                           // ȭ��� �ٷ� ���� frame���� �ô� ���� ��ġ.
        var distance = Vector2.Distance(xyNow, xyPrevious);                                                 // �� ������ �Ÿ�. 
        //Debug.Log("Distance: " + distance);
        if (distance < 1.0f)
        {
            timeRealGaze += Time.deltaTime;
            if (timeRealGaze > 0.2f)
            {
                timeRealGaze = 0.0f;

                // Filtered RealGazePosition (Saccadiv movement ���ܵ� ���� �����ִ� ��ġ�� world position)
                FilteredRealGazecolumns.Add(time_current.ToString());
                FilteredRealGazecolumns.Add(eyeDataManager.realGazePosition.x.ToString());
                FilteredRealGazecolumns.Add(eyeDataManager.realGazePosition.y.ToString());
                FilteredRealGazecolumns.Add(eyeDataManager.realGazePosition.z.ToString());

                FilteredRealGazePosition.WriteRow(FilteredRealGazecolumns);
                FilteredRealGazecolumns.Clear();

                // Filtered EyeMovement (Saccadic movement ���ܵ� ���� �����ִ� HMD ������� ��ġ)
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
        // Head�� Rotation ��� (Head rotation ���� inspector �� �״�� ������)
        var headRotation = UnityEditor.TransformUtils.GetInspectorRotation(Camera.transform);

        HeadColumns.Add(time_current.ToString());
        HeadColumns.Add(headRotation.x.ToString());
        HeadColumns.Add(headRotation.y.ToString());
        HeadColumns.Add(headRotation.z.ToString());

        HeadMovement.WriteRow(HeadColumns);
        HeadColumns.Clear();
    }



    // Timer ����
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

    // using�� ��� �� �ڵ����� dispose������, play ��ư Ŭ������ ���������� ���α׷� ���� �ÿ� ������ ����� �߰�
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
