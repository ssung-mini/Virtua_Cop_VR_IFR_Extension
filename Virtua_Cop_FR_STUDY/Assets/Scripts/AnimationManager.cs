using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    // 인스펙터에서 게임오브젝트(타겟) 받아옴
    public GameObject[] _s1Targets;
    public GameObject[] _s2Targets;
    public GameObject[] _s3Targets;
    public GameObject[] _s4Targets;
    public GameObject[] _s5Targets;
    public GameObject[] _s6Targets;
    public GameObject[] _s7Targets;
    public GameObject[] _s8Targets;
    public GameObject[] _s9Targets;
    public GameObject[] _s10Targets;

    // static GameObject 변수
    public static GameObject[] s1Targets;
    public static GameObject[] s2Targets;
    public static GameObject[] s3Targets;
    public static GameObject[] s4Targets;
    public static GameObject[] s5Targets;
    public static GameObject[] s6Targets;
    public static GameObject[] s7Targets;
    public static GameObject[] s8Targets;
    public static GameObject[] s9Targets;
    public static GameObject[] s10Targets;

    public AudioClip _tikSound;
    public static AudioClip tikSound;

    public float _waitTime;
    public static float waitTime;
    //static int[] randArray;


    private void Awake()
    {
        // 인스펙터에서 받은 게임오브젝트들을 스크립트에서 작동하기 위해 static 변수로 옮겨줌
        s1Targets = _s1Targets;
        s2Targets = _s2Targets;
        s3Targets = _s3Targets;
        s4Targets = _s4Targets;
        s5Targets = _s5Targets;
        s6Targets = _s6Targets;
        s7Targets = _s7Targets;
        s8Targets = _s8Targets;
        s9Targets = _s9Targets;
        s10Targets = _s10Targets;

        waitTime = _waitTime;
        tikSound = _tikSound;

        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public static void StartStageOne()
    {
        int[] standArray = {0, 3, 8, 5, 9, 1, 2, 6, 7, 4 };
        StaticCoroutine.DoCoroutine(ActiveStage1Anim(standArray));
    }

    public static void StartStageTwo()
    {
        int[] standArray = { 9, 2, 6, 0, 7, 5, 3, 4, 8, 1 };
        StaticCoroutine.DoCoroutine(ActiveStage2Anim(standArray));
    }

    public static void StartStageThree()
    {
        int[] standArray = { 3, 4, 5, 8, 0, 6, 2, 9, 7, 1 };
        StaticCoroutine.DoCoroutine(ActiveStage3Anim(standArray));
    }

    public static void StartStageFour()
    {
        int[] standArray = { 1, 6, 3, 7, 4, 5, 2, 9, 8, 0 };
        StaticCoroutine.DoCoroutine(ActiveStage4Anim(standArray));
    }

    public static void StartStageFive()
    {
        int[] standArray = { 6, 3, 1, 7, 9, 2, 8, 0, 5, 4 };
        StaticCoroutine.DoCoroutine(ActiveStage5Anim(standArray));
    }

    public static void StartStageSix()
    {
        int[] standArray = { 9, 6, 1, 5, 0, 3, 7, 8, 4, 2 };
        StaticCoroutine.DoCoroutine(ActiveStage6Anim(standArray));
    }

    public static void StartStageSeven()
    {
        int[] standArray = { 2, 1, 7, 5, 4, 6, 8, 9, 3, 0 };
        StaticCoroutine.DoCoroutine(ActiveStage7Anim(standArray));
    }
    public static void StartStageEight()
    {
        int[] standArray = { 6, 9, 1, 5, 4, 2, 8, 7, 0, 3 };
        StaticCoroutine.DoCoroutine(ActiveStage8Anim(standArray));
    }
    public static void StartStageNine()
    {
        int[] standArray = { 0, 4, 3, 5, 9, 2, 7, 8, 6, 1 };
        StaticCoroutine.DoCoroutine(ActiveStage9Anim(standArray));
    }
    public static void StartStageTen()
    {
        int[] standArray = { 5, 8, 3, 0, 6, 1, 7, 4, 9, 2 };
        StaticCoroutine.DoCoroutine(ActiveStage10Anim(standArray));
    }


    // 랜덤 변수(Stand할 순서 랜덤화 (0~9))
    // Counterbalance를 위하여 설계하였지만 사용하지 않게 됨
    /*
    public static int[] GetRandomInt()
    {
        int length = 10;
        int[] ranArr = new int[length];
        for (int i = 0; i < 10; i++)
        {
            ranArr[i] = i;
        }
        for (int i = 0; i < length; ++i)

        {
            int ranIdx = Random.Range(i, length);

            int tmp = ranArr[ranIdx];

            ranArr[ranIdx] = ranArr[i];

            ranArr[i] = tmp;
        }

        return ranArr;
    }
    */
    

    // 스테이지별 애니메이션 실행
    public static IEnumerator ActiveStage1Anim(int[] standArray)
    {
        print("Stage1");
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            s1Targets[standArray[i]].GetComponent<Animator>().SetBool("Stand", true);
            s1Targets[standArray[i]].GetComponent<AudioSource>().PlayOneShot(tikSound);
            CsvWritingManager.targetWritingTime[0, standArray[i], 0] = s1Targets[standArray[i]].tag;
            CsvWritingManager.targetWritingTime[0, standArray[i], 1] = CsvWritingManager.time_current.ToString();
            CsvWritingManager.targetWritingTime[0, standArray[i], 4] = s10Targets[standArray[i]].transform.position.x.ToString();
            CsvWritingManager.targetWritingTime[0, standArray[i], 5] = s10Targets[standArray[i]].transform.position.z.ToString();
        }
    }

    public static IEnumerator ActiveStage2Anim(int[] standArray)
    {
        print("Stage2");
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(waitTime);
            s2Targets[standArray[i]].GetComponent<Animator>().SetBool("Stand", true);
            s2Targets[standArray[i]].GetComponent<AudioSource>().PlayOneShot(tikSound);
            CsvWritingManager.targetWritingTime[1, standArray[i], 0] = s2Targets[standArray[i]].tag;
            CsvWritingManager.targetWritingTime[1, standArray[i], 1] = CsvWritingManager.time_current.ToString();
            CsvWritingManager.targetWritingTime[1, standArray[i], 4] = s10Targets[standArray[i]].transform.position.x.ToString();
            CsvWritingManager.targetWritingTime[1, standArray[i], 5] = s10Targets[standArray[i]].transform.position.z.ToString();
        }
    }

    public static IEnumerator ActiveStage3Anim(int[] standArray)
    {
        print("Stage3");
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(waitTime);
            s3Targets[standArray[i]].GetComponent<Animator>().SetBool("Stand", true);
            s3Targets[standArray[i]].GetComponent<AudioSource>().PlayOneShot(tikSound);
            CsvWritingManager.targetWritingTime[2, standArray[i], 0] = s3Targets[standArray[i]].tag;
            CsvWritingManager.targetWritingTime[2, standArray[i], 1] = CsvWritingManager.time_current.ToString();
            CsvWritingManager.targetWritingTime[2, standArray[i], 4] = s10Targets[standArray[i]].transform.position.x.ToString();
            CsvWritingManager.targetWritingTime[2, standArray[i], 5] = s10Targets[standArray[i]].transform.position.z.ToString();
        }
    }

    public static IEnumerator ActiveStage4Anim(int[] standArray)
    {
        print("Stage4");
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(waitTime);
            s4Targets[standArray[i]].GetComponent<Animator>().SetBool("Stand", true);
            s4Targets[standArray[i]].GetComponent<AudioSource>().PlayOneShot(tikSound);
            CsvWritingManager.targetWritingTime[3, standArray[i], 0] = s4Targets[standArray[i]].tag;
            CsvWritingManager.targetWritingTime[3, standArray[i], 1] = CsvWritingManager.time_current.ToString();
            CsvWritingManager.targetWritingTime[3, standArray[i], 4] = s10Targets[standArray[i]].transform.position.x.ToString();
            CsvWritingManager.targetWritingTime[3, standArray[i], 5] = s10Targets[standArray[i]].transform.position.z.ToString();
        }
    }

    public static IEnumerator ActiveStage5Anim(int[] standArray)
    {
        print("Stage5");
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(waitTime);
            s5Targets[standArray[i]].GetComponent<Animator>().SetBool("Stand", true);
            s5Targets[standArray[i]].GetComponent<AudioSource>().PlayOneShot(tikSound);
            CsvWritingManager.targetWritingTime[4, standArray[i], 0] = s5Targets[standArray[i]].tag;
            CsvWritingManager.targetWritingTime[4, standArray[i], 1] = CsvWritingManager.time_current.ToString();
            CsvWritingManager.targetWritingTime[4, standArray[i], 4] = s10Targets[standArray[i]].transform.position.x.ToString();
            CsvWritingManager.targetWritingTime[4, standArray[i], 5] = s10Targets[standArray[i]].transform.position.z.ToString();
        }
    }

    public static IEnumerator ActiveStage6Anim(int[] standArray)
    {
        print("Stage6");
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(waitTime);
            s6Targets[standArray[i]].GetComponent<Animator>().SetBool("Stand", true);
            s6Targets[standArray[i]].GetComponent<AudioSource>().PlayOneShot(tikSound);
            CsvWritingManager.targetWritingTime[5, standArray[i], 0] = s6Targets[standArray[i]].tag;
            CsvWritingManager.targetWritingTime[5, standArray[i], 1] = CsvWritingManager.time_current.ToString();
            CsvWritingManager.targetWritingTime[5, standArray[i], 4] = s10Targets[standArray[i]].transform.position.x.ToString();
            CsvWritingManager.targetWritingTime[5, standArray[i], 5] = s10Targets[standArray[i]].transform.position.z.ToString();
        }
    }

    public static IEnumerator ActiveStage7Anim(int[] standArray)
    {
        print("Stage7");
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(waitTime);
            s7Targets[standArray[i]].GetComponent<Animator>().SetBool("Stand", true);
            s7Targets[standArray[i]].GetComponent<AudioSource>().PlayOneShot(tikSound);
            CsvWritingManager.targetWritingTime[6, standArray[i], 0] = s7Targets[standArray[i]].tag;
            CsvWritingManager.targetWritingTime[6, standArray[i], 1] = CsvWritingManager.time_current.ToString();
            CsvWritingManager.targetWritingTime[6, standArray[i], 4] = s10Targets[standArray[i]].transform.position.x.ToString();
            CsvWritingManager.targetWritingTime[6, standArray[i], 5] = s10Targets[standArray[i]].transform.position.z.ToString();
        }
    }

    public static IEnumerator ActiveStage8Anim(int[] standArray)
    {
        print("Stage8");
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(waitTime);
            s8Targets[standArray[i]].GetComponent<Animator>().SetBool("Stand", true);
            s8Targets[standArray[i]].GetComponent<AudioSource>().PlayOneShot(tikSound);
            CsvWritingManager.targetWritingTime[7, standArray[i], 0] = s8Targets[standArray[i]].tag;
            CsvWritingManager.targetWritingTime[7, standArray[i], 1] = CsvWritingManager.time_current.ToString();
            CsvWritingManager.targetWritingTime[7, standArray[i], 4] = s10Targets[standArray[i]].transform.position.x.ToString();
            CsvWritingManager.targetWritingTime[7, standArray[i], 5] = s10Targets[standArray[i]].transform.position.z.ToString();
        }
    }

    public static IEnumerator ActiveStage9Anim(int[] standArray)
    {
        print("Stage9");
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(waitTime);
            s9Targets[standArray[i]].GetComponent<Animator>().SetBool("Stand", true);
            s9Targets[standArray[i]].GetComponent<AudioSource>().PlayOneShot(tikSound);
            CsvWritingManager.targetWritingTime[8, standArray[i], 0] = s9Targets[standArray[i]].tag;
            CsvWritingManager.targetWritingTime[8, standArray[i], 1] = CsvWritingManager.time_current.ToString();
            CsvWritingManager.targetWritingTime[8, standArray[i], 4] = s10Targets[standArray[i]].transform.position.x.ToString();
            CsvWritingManager.targetWritingTime[8, standArray[i], 5] = s10Targets[standArray[i]].transform.position.z.ToString();
        }
    }

    public static IEnumerator ActiveStage10Anim(int[] standArray)
    {
        print("Stage10");
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(waitTime);
            s10Targets[standArray[i]].GetComponent<Animator>().SetBool("Stand", true);
            s10Targets[standArray[i]].GetComponent<AudioSource>().PlayOneShot(tikSound);
            CsvWritingManager.targetWritingTime[9, standArray[i], 0] = s10Targets[standArray[i]].tag;
            CsvWritingManager.targetWritingTime[9, standArray[i], 1] = CsvWritingManager.time_current.ToString();
            CsvWritingManager.targetWritingTime[9, standArray[i], 4] = s10Targets[standArray[i]].transform.position.x.ToString();
            CsvWritingManager.targetWritingTime[9, standArray[i], 5] = s10Targets[standArray[i]].transform.position.z.ToString();
        }
    }
}
