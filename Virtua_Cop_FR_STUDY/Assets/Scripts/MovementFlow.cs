using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageMoveSet   // Class 배열 정의 (Inspecter에 2차원 배열을 Visualizing하기 위함)
{
    public GameObject[] moveProgress;   // 한 스테이지 Moving 안에서 여러 Waypoint로 이동하기 위함
}

public class MovementFlow : MonoBehaviour
{
    public GameObject player;

    //public GameObject[] wayPoints;        //Legacy System setting
    

    // WayPoint 2차원 배열 (형태 상 Class 배열)
    public StageMoveSet[] wayPointSet;
    
    // Moving시 무기 Visualizing 해제를 위함
    public GameObject _weaponObject;
    public static GameObject weaponObject;

    public static int[] stageEnemyKill = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public static int[] stageHostageKill = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    public static int stageCount;
    public static int movingProgressCount;

    public static bool isMoving = false;

    private void Awake()
    {
        weaponObject = _weaponObject;
        stageCount = 0;
        movingProgressCount = 0;
    }


    // Update is called once per frame
    private void FixedUpdate()      // 프레임 고정을 위해 FixedUpdate()를 사용함 (일반적인 Update는 컴퓨터 성능에 따라 반복 횟수가 다름)
    {
        if (isMoving)
        {
            StageClearMove();
        }

        /* Legacy System Setting
         * 
            if (stageEnemyKill[0] == 5)
            {
                Stage1Clear();
            }

            if (stageEnemyKill[1] == 5)
            {
                Stage2Clear();
            }

            if (stageEnemyKill[2] == 5)
            {
                Stage3Clear();
            }

            if (stageEnemyKill[3] == 5)
            {
                Stage4Clear();
            }

            if (stageEnemyKill[4] == 5)
            {
                Stage5Clear();
            }

            if (stageEnemyKill[5] == 5)
            {
                Stage6Clear();
            }

            if (stageEnemyKill[6] == 5)
            {
                Stage7Clear();
            }

            if (stageEnemyKill[7] == 5)
            {
                Stage8Clear();
            }

            if (stageEnemyKill[8] == 5)
            {
                Stage9Clear();
            }
            */

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("MovingProgress"))
        {
            movingProgressCount++;
        }

        // Stage 1 Clear
        if(other.CompareTag("WayPoint 2"))
        {
            //print("Way Point 2's Enter Trigger");
            isMoving = false;
            stageEnemyKill[0] = 0;
            stageCount = 1;
            movingProgressCount = 0;
            weaponObject.SetActive(true);
            AnimationManager.StartStageTwo();
        }

        // Stage 2 Clear
        if (other.CompareTag("WayPoint 3"))
        {
            //print("Way Point 3's Enter Trigger");
            isMoving = false;
            stageEnemyKill[1] = 0;
            stageCount = 2;
            movingProgressCount = 0;
            weaponObject.SetActive(true);
            AnimationManager.StartStageThree();
        }

        // Stage 3 Clear
        if (other.CompareTag("WayPoint 4"))
        {
            //print("Way Point 4's Enter Trigger");
            isMoving = false;
            stageEnemyKill[2] = 0;
            stageCount = 3;
            movingProgressCount = 0;
            weaponObject.SetActive(true);
            AnimationManager.StartStageFour();
        }

        // Stage 4 Clear
        if (other.CompareTag("WayPoint 5"))
        {
            //print("Way Point 5's Enter Trigger");
            isMoving = false;
            stageEnemyKill[3] = 0;
            stageCount = 4;
            movingProgressCount = 0;
            weaponObject.SetActive(true);
            AnimationManager.StartStageFive();
        }

        // Stage 5 Clear
        if (other.CompareTag("WayPoint 6"))
        {
            //print("Way Point 6's Enter Trigger");
            isMoving = false;
            stageEnemyKill[4] = 0;
            stageCount = 5;
            movingProgressCount = 0;
            weaponObject.SetActive(true);
            AnimationManager.StartStageSix();
        }

        // Stage 6 Clear
        if (other.CompareTag("WayPoint 7"))
        {
            //print("Way Point 7's Enter Trigger");
            isMoving = false;
            stageEnemyKill[5] = 0;
            stageCount = 6;
            movingProgressCount = 0;
            weaponObject.SetActive(true);
            AnimationManager.StartStageSeven();
        }

        // Stage 7 Clear
        if (other.CompareTag("WayPoint 8"))
        {
            //print("Way Point 8's Enter Trigger");
            isMoving = false;
            stageEnemyKill[6] = 0;
            stageCount = 7;
            movingProgressCount = 0;
            weaponObject.SetActive(true);
            AnimationManager.StartStageEight();
        }

        // Stage 8 Clear
        if (other.CompareTag("WayPoint 9"))
        {
            //print("Way Point 9's Enter Trigger");
            isMoving = false;
            stageEnemyKill[7] = 0;
            stageCount = 8;
            movingProgressCount = 0;
            weaponObject.SetActive(true);
            AnimationManager.StartStageNine();
        }

        // Stage 9 Clear
        if (other.CompareTag("WayPoint 10"))
        {
            //print("Way Point 10's Enter Trigger");
            isMoving = false;
            stageEnemyKill[8] = 0;
            stageCount = 9;
            movingProgressCount = 0;
            weaponObject.SetActive(true);
            AnimationManager.StartStageTen();
        }

    }

    void StageClearMove()
    {
        Vector3 direction = wayPointSet[stageCount].moveProgress[movingProgressCount].transform.position - player.transform.position;
        Quaternion rot = Quaternion.LookRotation(direction);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, rot, Time.deltaTime * 3.0f);
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPointSet[stageCount].moveProgress[movingProgressCount].transform.position, 0.051f);
    }




    // Legacy System Setting
    /*
    void Stage1Clear()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPoints[1].transform.position, 0.05f);
    }

    void Stage2Clear()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPoints[2].transform.position, 0.05f);
    }

    void Stage3Clear()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPoints[3].transform.position, 0.05f);
    }

    void Stage4Clear()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPoints[4].transform.position, 0.05f);
    }

    void Stage5Clear()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPoints[5].transform.position, 0.05f);
    }

    void Stage6Clear()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPoints[6].transform.position, 0.05f);
    }

    void Stage7Clear()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPoints[7].transform.position, 0.05f);
    }

    void Stage8Clear()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPoints[8].transform.position, 0.05f);
    }

    void Stage9Clear()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPoints[9].transform.position, 0.05f);
    }
    */
}
