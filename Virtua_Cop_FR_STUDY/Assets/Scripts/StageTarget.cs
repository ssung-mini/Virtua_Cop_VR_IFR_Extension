using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTarget : Target
{
    public override void TargetCount()
    {
        // 쏜 타겟이 Enemy일 때
        if(CompareTag("Enemy"))
        {
            ++MovementFlow.stageEnemyKill[MovementFlow.stageCount];
            ++CsvWritingManager.totalEnemyKill;
        }

        // 쏜 타겟이 Hostage일 때
        if (CompareTag("Hostage"))
        {
            ++MovementFlow.stageHostageKill[MovementFlow.stageCount];
            ++CsvWritingManager.totalHostageKill;
        }

        // TotalTargetKill 증가 및 퍼센트 계산
        ++CsvWritingManager.totalTargetKill;
        CsvWritingManager.checkPercentage.Add(((float)(CsvWritingManager.totalEnemyKill)) / (float)(CsvWritingManager.totalTargetKill));


        // 스테이지 당 Enemy 5명 제거 시
        if (CsvWritingManager.totalEnemyKill != 50)
        {
            if (MovementFlow.stageEnemyKill[MovementFlow.stageCount] == 5)
            {
                MovementFlow.isMoving = true;
                if (MovementFlow.weaponObject.activeSelf)
                    MovementFlow.weaponObject.SetActive(false);
            }
        }

        // Enemy 50명 전원 제거 시 타이머 종료 및 게임 종료 코루틴 실행
        if (CsvWritingManager.totalEnemyKill == 50)
        {
            CsvWritingManager.End_Timer();
            ClearEnable.StageClearEnable();
        }
    }
}
