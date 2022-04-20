using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearEnable : MonoBehaviour
{
    public GameObject _clearText;
    public static GameObject clearText;

    // Start is called before the first frame update
    void Awake()
    {
        clearText = _clearText;
        clearText.SetActive(false);
    }

    public static void StageClearEnable()
    {
        clearText.SetActive(true);
        StaticCoroutine.DoCoroutine(GameClearQuit());
    }


    // 게임 종료
    public static void GameQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // 5초 후 종료 코루틴
    public static IEnumerator GameClearQuit()
    {
        yield return new WaitForSecondsRealtime(5);
        GameQuit();
    }
}
