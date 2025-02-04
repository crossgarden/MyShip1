using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_Pillar : MonoBehaviour
{

    void Update()
    {
        Move();
    }

    void Move(){
        Vector3 movement = Vector3.left * Time.deltaTime * FSManager.instance.speed;
        transform.position += movement;

        if (transform.position.x < -20){
            Destroy(gameObject);
        } 
    }
}
