using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run_Obstacle : MonoBehaviour
{

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 movement = Vector3.left * Time.deltaTime * RunManager.instance.speed;
        transform.position += movement;

        if (transform.position.x < -20)
        {
            Destroy(gameObject);
        }
    }
}
