using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPosition : MonoBehaviour

{
    public GameObject[] target;
    public GameObject standard;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            if(i < 5)
            {
                int angle = (-1) - i;
                target[i].transform.RotateAround(standard.transform.position, new Vector3(0f, 1f, 0f), (11f * angle));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
