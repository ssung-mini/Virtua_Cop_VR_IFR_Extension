using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTarget : Target
{
    public override void TargetCount()
    {
        // �� Ÿ���� Enemy�� ��
        if(CompareTag("Enemy"))
        {
            ++MovementFlow.stageEnemyKill[MovementFlow.stageCount];
            ++CsvWritingManager.totalEnemyKill;
        }

        // �� Ÿ���� Hostage�� ��
        if (CompareTag("Hostage"))
        {
            ++MovementFlow.stageHostageKill[MovementFlow.stageCount];
            ++CsvWritingManager.totalHostageKill;
        }

        // TotalTargetKill ���� �� �ۼ�Ʈ ���
        ++CsvWritingManager.totalTargetKill;
        CsvWritingManager.checkPercentage.Add(((float)(CsvWritingManager.totalEnemyKill)) / (float)(CsvWritingManager.totalTargetKill));


        // �������� �� Enemy 5�� ���� ��
        if (CsvWritingManager.totalEnemyKill != 50)
        {
            if (MovementFlow.stageEnemyKill[MovementFlow.stageCount] == 5)
            {
                MovementFlow.isMoving = true;
                if (MovementFlow.weaponObject.activeSelf)
                    MovementFlow.weaponObject.SetActive(false);
            }
        }

        // Enemy 50�� ���� ���� �� Ÿ�̸� ���� �� ���� ���� �ڷ�ƾ ����
        if (CsvWritingManager.totalEnemyKill == 50)
        {
            CsvWritingManager.End_Timer();
            ClearEnable.StageClearEnable();
        }
    }
}
