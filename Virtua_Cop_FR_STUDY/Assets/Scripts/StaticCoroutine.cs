using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCoroutine : MonoBehaviour
{
    private static StaticCoroutine mInstance = null;

    private static StaticCoroutine Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.FindObjectOfType(typeof(StaticCoroutine)) as StaticCoroutine;

                if (mInstance == null)
                {
                    mInstance = new GameObject("StaticCoroutine").AddComponent<StaticCoroutine>();
                }
            }
            return mInstance;
        }
    }

    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this as StaticCoroutine;
        }
    }

    IEnumerator Perform(IEnumerator coroutine)
    {
        yield return StartCoroutine(coroutine);
        Die();
    }

    public static void DoCoroutine(IEnumerator coroutine)
    {
        //���⼭ �ν��Ͻ��� �ִ� �ڷ�ƾ�� ����� ���̴�.
        Instance.StartCoroutine(Instance.Perform(coroutine));
    }

    void Die()
    {
        mInstance = null;
        Destroy(gameObject);
    }

    void OnApplicationQuit()
    {
        mInstance = null;
    }



}
