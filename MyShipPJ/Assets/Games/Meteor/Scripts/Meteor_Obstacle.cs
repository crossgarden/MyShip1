using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor_Obstacle : MonoBehaviour
{

    void Update()
    {
        Move();
        FlipX();
    }

    void Move() //운석,코인 낙하
    {
        Vector3 movement = Vector3.down * Time.deltaTime * Meteor_Manager.instance.speed;
        transform.position += movement;

        if (transform.position.y < -8)
        {
            Destroy(gameObject);
        }
    }

    void FlipX()
    {
        // 현재 localScale의 x 값을 반전시킴
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }
}
