using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    private bool direction;
    private Transform target;
    private float speed;

    void Awake()
    {
        target = gameObject.GetComponent<Transform>();
        //target.position = new Vector3(0.0f, 0.0f, 7.0f);
        direction = true;
        speed = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(direction)
        {
            if (target.transform.position.x <= 7.0f) target.Translate(Vector3.right * Time.deltaTime * speed);
            else direction = false;
        }

        else
        {
            if (target.transform.position.x >= -7.0f) target.Translate(Vector3.left * Time.deltaTime * speed);
            else direction = true;
        }
    }
}
