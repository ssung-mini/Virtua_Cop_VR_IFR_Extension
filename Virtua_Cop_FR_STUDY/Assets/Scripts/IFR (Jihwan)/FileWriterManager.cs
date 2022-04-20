using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class FileWriterManager : MonoBehaviour
{
    public int participantNum;                     // 피험자 번호
    public int trialNum;

    private int programType;                        // Measurement = 1, VIsual search task = 2
    private int measurementType;                    // FoV = 1, Rate = 2

    // 저장 경로는 바탕화면 (Desktop)
    // 바탕화면에 Exp 폴더 생성 후, Participant's identity에 맞게 분류.
    [Header("Directory (root : Desktop")]
    private string path;

    Stream fileStream_Measurement;                  // about Measurement 1, 2
    Stream fileStream_VisualSearchTask;             // about Visual search task result
    Stream fileStream_HeadMovement;                 // about Head rotation
    Stream fileStream_RealGazePosition;             // about gaze position world position (full)
    Stream fileStream_EyeMovement;                  // about gaze position in HMD (filtered)
    Stream fileStream_EyeMovementFullData;          // about gaze position in HMD (full)
    Stream fileStream_RealGazePositionFullData;     // about gaze position world position (filtered)

    StreamWriter fileWriter_Measurement;
    StreamWriter fileWriter_VisualSearchTask;
    StreamWriter fileWriter_HeadMovement;
    StreamWriter fileWriter_RealGazePosition;
    StreamWriter fileWriter_EyeMovement;
    StreamWriter fileWriter_EyeMovementFullData;
    StreamWriter fileWriter_RealGazePositionFullData;

    string fileName_Total_Measurement;
    string fileName_Total_VisualSearchTask;
    string fileName_Total_HeadMovement;
    string fileName_Total_RealGazePosition;
    string fileName_Total_EyeMovement;
    string fileName_Total_EyeMovementFullData;
    string fileName_Total_RealGazePositionFullData;

    // 파일 입력이 한번이라도 이루어졌는가 = column 작성 되었는가?
    bool initWriting_Measurement = false;
    bool initWriting_VisualSearchTask = false;
    bool initWriting_HeadMovement = false;
    bool initWriting_RealGazePosition = false;
    bool initWriting_EyeMovement = false;
    bool initWriting_EyeMovementFullData = false;
    bool initWriting_RealGazePositionFullData = false;

    void Awake()
    {
        path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\DownsamplingData\\" + participantNum.ToString() + "\\";

        if (SceneManager.GetActiveScene().name == "Classroom")
        {
            programType = 1;
            measurementType = GameObject.Find("ExperimentManager").GetComponent<ExperimentManager>().experimentType;
            if (measurementType == 1) path = path + "SizeMeasure\\";
            else if (measurementType == 2) path = path + "DownsamplingRateMeasure\\";
            MakeDataDir_Measurement();
        }

        if (SceneManager.GetActiveScene().name == "Classroom_VisualSearchTask")
        {
            programType = 2;
            string path2;
            for (int i = 1; i <= 500; i++)
            {
                path2 = path + "VisualSearchTask\\" + i.ToString() + "\\";
                DirectoryInfo di = new DirectoryInfo(path2);
                if (di.Exists == false)
                {
                    trialNum = i;
                    path = path2;
                    break;
                }
            }
            MakeDataDir_VisualSearchTask();
        }
    }

    // 프로그램 종료시 스트림 닫는 메소드.
    void OnApplicationQuit()
    {
        if (programType == 1 && fileStream_Measurement.CanRead) fileStream_Measurement.Close();
        if (programType == 2)
        {
            if (fileStream_VisualSearchTask.CanRead) fileStream_VisualSearchTask.Close();
            if (fileStream_HeadMovement.CanRead) fileStream_HeadMovement.Close();
            if (fileStream_RealGazePosition.CanRead) fileStream_RealGazePosition.Close();
            if (fileStream_EyeMovement.CanRead) fileStream_EyeMovement.Close();
            if (fileStream_EyeMovementFullData.CanRead) fileStream_EyeMovementFullData.Close();
            if (fileStream_RealGazePositionFullData.CanRead) fileStream_RealGazePositionFullData.Close();
        }
    }

    /// <summary>
    ///   Rawdata 보관하는 디렉토리 생성 및 해당 디렉토리에 Rawdata를 생성하는 함수
    /// </summary>
    public void MakeDataDir_Measurement()
    {
        // 'path = 바탕화면 + 사용자 정의 경로 (실험 데이터 폴더) + 피험자 별 폴더' 디렉토리 생성
        //path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + path;
        Directory.CreateDirectory(path);
        Debug.Log(path);

        string fileName_Measurement = "Measurement_FoVSize";
        if (measurementType == 2) fileName_Measurement = "Measurement_DownsamplingRate";

        string format = ".csv";
        string nowTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

        // Dynamic blurring 적용 여부에 따라 general data, fms data 파일 명명
        fileName_Total_Measurement = path + fileName_Measurement + "(" + nowTime + ")" + format;
        // 정의된 실험 데이터 폴더에 데이터 파일을 생성하고, 파일 쓰기를 위한 스트림 인스턴스 생성
        fileStream_Measurement = new FileStream(fileName_Total_Measurement, FileMode.Create, FileAccess.ReadWrite);
        fileWriter_Measurement = new StreamWriter(fileStream_Measurement);
    }

    public void MakeDataDir_VisualSearchTask()
    {
        // 'path = 바탕화면 + 사용자 정의 경로 (실험 데이터 폴더) + 피험자 별 폴더' 디렉토리 생성
        //path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + path;
        Directory.CreateDirectory(path);
        Debug.Log(path);

        // Rawdata 파일 이름 및 형식 정의
        string fileName_VisualSearchTask = "VisualSearchTaskResult_";
        string fileName_HeadMovement = "HeadMovement_";
        string fileName_RealGazePosition = "RealGazePosition(Filtered)";
        string fileName_EyeMovement = "EyeGazeData(Filtered)";
        string fileName_EyeMovementFullData = "EyeGazeData(Full)";
        string fileName_RealGazePositionFullData = "RealGazePosition(Full)";

        string format = ".csv";
        string nowTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

        // Dynamic blurring 적용 여부에 따라 general data, fms data 파일 명명
        fileName_Total_VisualSearchTask = path + fileName_VisualSearchTask + "(" + nowTime + ")" + format;
        fileName_Total_HeadMovement = path + fileName_HeadMovement + "(" + nowTime + ")" + format;
        fileName_Total_RealGazePosition = path + fileName_RealGazePosition + format;
        fileName_Total_EyeMovement = path + fileName_EyeMovement + format;
        fileName_Total_EyeMovementFullData = path + fileName_EyeMovementFullData + format;
        fileName_Total_RealGazePositionFullData = path + fileName_RealGazePositionFullData + format;

        // 정의된 실험 데이터 폴더에 데이터 파일을 생성하고, 파일 쓰기를 위한 스트림 인스턴스 생성
        fileStream_VisualSearchTask = new FileStream(fileName_Total_VisualSearchTask, FileMode.Create, FileAccess.ReadWrite);
        fileStream_HeadMovement = new FileStream(fileName_Total_HeadMovement, FileMode.Create, FileAccess.ReadWrite);
        fileStream_RealGazePosition = new FileStream(fileName_Total_RealGazePosition, FileMode.Create, FileAccess.ReadWrite);
        fileStream_EyeMovement = new FileStream(fileName_Total_EyeMovement, FileMode.Create, FileAccess.ReadWrite);
        fileStream_EyeMovementFullData = new FileStream(fileName_Total_EyeMovementFullData, FileMode.Create, FileAccess.ReadWrite);
        fileStream_RealGazePositionFullData = new FileStream(fileName_Total_RealGazePositionFullData, FileMode.Create, FileAccess.ReadWrite);

        fileWriter_VisualSearchTask = new StreamWriter(fileStream_VisualSearchTask);
        fileWriter_HeadMovement = new StreamWriter(fileStream_HeadMovement);
        fileWriter_RealGazePosition = new StreamWriter(fileStream_RealGazePosition);
        fileWriter_EyeMovement = new StreamWriter(fileStream_EyeMovement);
        fileWriter_EyeMovementFullData = new StreamWriter(fileStream_EyeMovementFullData);
        fileWriter_RealGazePositionFullData = new StreamWriter(fileStream_RealGazePositionFullData);
    }

    /// <summary>
    /// Measurement 1, 2의 결과를 기록.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public bool WriteMeasurementResult(List<String> list)
    {
        // 스트림에 write를 할 수 있는 상태인지 확인
        if (fileStream_Measurement.CanWrite)
        {
            try
            {
                // 최초 general data 기록시, 컬럼 정보 삽입
                if (initWriting_Measurement != true)
                {
                    string columnInfo = "Number of Trial,Dowmsampling Size (outRadDeg),Downsampling Rate,Animation Number";
                    fileWriter_Measurement.WriteLine(columnInfo);
                    fileWriter_Measurement.Flush();
                    initWriting_Measurement = true;
                }

                fileWriter_Measurement = new StreamWriter(fileStream_Measurement);
                string inputLine = "";

                foreach (String data in list)
                {
                    inputLine += data + ",";
                }

                fileWriter_Measurement.WriteLine(inputLine);
                fileWriter_Measurement.Flush();
            }
            catch (System.Exception e)
            {
                Debug.LogAssertion("경고! 데이터 디렉토리와 스트림이 확인되었으나, 기록할 수 없습니다. - " + e);
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Visual search task 결과를 저장.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="visualSearchTask"></param>
    /// <returns></returns>
    public bool WriteVisualSearchTaskResult(List<String> list)
    {
        // 스트림에 write를 할 수 있는 상태인지 확인
        if (fileStream_VisualSearchTask.CanWrite)
        {
            try
            {
                // 최초 general data 기록시, 컬럼 정보 삽입
                if (initWriting_VisualSearchTask != true)
                {
                    string columnInfo = "Number of Trial,Color/Shape,Target Order,Dowmsampling Size (outRadDeg),Downsampling Rate,Response Time,Success(O)/Fail(X)";
                    fileWriter_VisualSearchTask.WriteLine(columnInfo);
                    fileWriter_VisualSearchTask.Flush();
                    initWriting_VisualSearchTask = true;
                }

                fileWriter_VisualSearchTask = new StreamWriter(fileStream_VisualSearchTask);
                string inputLine = "";

                foreach (String data in list)
                {
                    inputLine += data + ",";
                }

                fileWriter_VisualSearchTask.WriteLine(inputLine);
                fileWriter_VisualSearchTask.Flush();
            }
            catch (System.Exception e)
            {
                Debug.LogAssertion("경고! 데이터 디렉토리와 스트림이 확인되었으나, 기록할 수 없습니다. - " + e);
                return false;
            }
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 현재 보고 있는 곳이 렌즈상에서 어느 position 인지 x,y 둘다 (0~1). Full data
    /// </summary>
    /// <param name="gazePosition"></param>
    /// <param name="nowTrial"></param>
    /// <returns></returns>
    public bool WriteEyeMovementFullData(Vector2 gazePosition, int nowTrial)
    {
        // 스트림에 write를 할 수 있는 상태인지 확인
        if (fileStream_EyeMovementFullData.CanWrite)
        {
            try
            {
                // 최초 general data 기록시, 컬럼 정보 삽입
                if (initWriting_EyeMovementFullData != true)
                {
                    string columnInfo = "Number of Trial,gazePosition_x,gazePosition_y";
                    fileWriter_EyeMovementFullData.WriteLine(columnInfo);
                    fileWriter_EyeMovementFullData.Flush();
                    initWriting_EyeMovementFullData = true;
                }

                fileWriter_EyeMovementFullData = new StreamWriter(fileStream_EyeMovementFullData);
                string inputLine = "";
                
                inputLine = nowTrial + "," + gazePosition.x + "," + gazePosition.y;

                fileWriter_EyeMovementFullData.WriteLine(inputLine);
                fileWriter_EyeMovementFullData.Flush();
            }
            catch (System.Exception e)
            {
                Debug.LogAssertion("경고! 데이터 디렉토리와 스트림이 확인되었으나, 기록할 수 없습니다. - " + e);
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 현재 보고 있는 곳이 렌즈상에서 어느 position 인지 x,y 둘다 (0~1). Filtered
    /// </summary>
    /// <param name="gazePosition"></param>
    /// <param name="nowTrial"></param>
    /// <returns></returns>
    public bool WriteEyeMovement(Vector2 gazePosition, int nowTrial)
    {
        // 스트림에 write를 할 수 있는 상태인지 확인
        if (fileStream_EyeMovement.CanWrite)
        {
            try
            {
                // 최초 general data 기록시, 컬럼 정보 삽입
                if (initWriting_EyeMovement != true)
                {
                    string columnInfo = "Number of Trial,gazePosition_x,gazePosition_y";
                    fileWriter_EyeMovement.WriteLine(columnInfo);
                    fileWriter_EyeMovement.Flush();
                    initWriting_EyeMovement = true;
                }

                fileWriter_EyeMovement = new StreamWriter(fileStream_EyeMovement);
                string inputLine = "";

                inputLine = nowTrial + "," + gazePosition.x + "," + gazePosition.y;

                fileWriter_EyeMovement.WriteLine(inputLine);
                fileWriter_EyeMovement.Flush();
            }
            catch (System.Exception e)
            {
                Debug.LogAssertion("경고! 데이터 디렉토리와 스트림이 확인되었으나, 기록할 수 없습니다. - " + e);
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Head rotation을 기록.
    /// </summary>
    /// <param name="headRotation"></param>
    /// <param name="nowTrial"></param>
    /// <returns></returns>
    public bool WriterHeadMovement(Vector3 headRotation, int nowTrial)
    {
        if(fileStream_HeadMovement.CanWrite)
        {
            try
            {
                // 최초 general data 기록시, 컬럼 정보 삽입
                if (initWriting_HeadMovement != true)
                {
                    string columnInfo = "Number of Trial,headRotation_x,headRotation_y,headRotation_z";
                    fileWriter_HeadMovement.WriteLine(columnInfo);
                    fileWriter_HeadMovement.Flush();
                    initWriting_HeadMovement = true;
                }

                fileWriter_HeadMovement = new StreamWriter(fileStream_HeadMovement);
                string inputLine = "";

                //inputLine = nowTrial + "," + headRotation.eulerAngles.x + "," + headRotation.eulerAngles.y + "," + headRotation.eulerAngles.z;
                inputLine = nowTrial + "," + headRotation.x + "," + headRotation.y + "," + headRotation.z;

                fileWriter_HeadMovement.WriteLine(inputLine);
                fileWriter_HeadMovement.Flush();
            }
            catch (System.Exception e)
            {
                Debug.LogAssertion("경고! 데이터 디렉토리와 스트림이 확인되었으나, 기록할 수 없습니다. - " + e);
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Eye movement + Read rotation 포함한 현재 보고있는 world position 기록. Filtered
    /// </summary>
    /// <param name="eyeGazePosition"></param>
    /// <param name="nowTrial"></param>
    /// <returns></returns>
    public bool WriterRealGazePosition(Vector3 eyeGazePosition, int nowTrial)
    {
        if(fileStream_RealGazePosition.CanWrite)
        {
            try
            {
                if(initWriting_RealGazePosition != true)
                {
                    string columnInfo = "Number of Trial,realGazePosition_x,realGazePosition_y,realGazePosition_z";
                    fileWriter_RealGazePosition.WriteLine(columnInfo);
                    fileWriter_RealGazePosition.Flush();
                    initWriting_RealGazePosition = true;
                }

                fileWriter_RealGazePosition = new StreamWriter(fileStream_RealGazePosition);
                string inputLine = "";

                inputLine = nowTrial + "," + eyeGazePosition.x + "," + eyeGazePosition.y + "," + eyeGazePosition.z;

                fileWriter_RealGazePosition.WriteLine(inputLine);
                fileWriter_RealGazePosition.Flush();
            }
            catch (System.Exception e)
            {
                Debug.LogAssertion("경고! 데이터 디렉토리와 스트림이 확인되었으나, 기록할 수 없습니다. - " + e);
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Eye movement + Read rotation 포함한 현재 보고있는 world position 기록. Full data
    /// </summary>
    /// <param name="eyeGazePosition"></param>
    /// <param name="nowTrial"></param>
    /// <returns></returns>
    public bool WriterRealGazePositionFullData(Vector3 eyeGazePosition, int nowTrial)
    {
        if (fileStream_RealGazePositionFullData.CanWrite)
        {
            try
            {
                if (initWriting_RealGazePositionFullData != true)
                {
                    string columnInfo = "Number of Trial,realGazePosition_x,realGazePosition_y,realGazePosition_z";
                    fileWriter_RealGazePositionFullData.WriteLine(columnInfo);
                    fileWriter_RealGazePositionFullData.Flush();
                    initWriting_RealGazePositionFullData = true;
                }

                fileWriter_RealGazePositionFullData = new StreamWriter(fileStream_RealGazePositionFullData);
                string inputLine = "";

                inputLine = nowTrial + "," + eyeGazePosition.x + "," + eyeGazePosition.y + "," + eyeGazePosition.z;

                fileWriter_RealGazePositionFullData.WriteLine(inputLine);
                fileWriter_RealGazePositionFullData.Flush();
            }
            catch (System.Exception e)
            {
                Debug.LogAssertion("경고! 데이터 디렉토리와 스트림이 확인되었으나, 기록할 수 없습니다. - " + e);
                return false;
            }
            return true;
        }
        return false;
    }
}