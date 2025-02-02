using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSManager : MonoBehaviour
{

    public Transform bg1, bg2, ground1, ground2, topSpawnPos, bottomSpawnPos;
    public GameObject[] pillarPrefabs;
    public Vector3 bgSize, groundSize;

    public float speed = 2f;

    void Start()
    {
        bgSize = bg1.GetComponent<SpriteRenderer>().bounds.size;
        groundSize = ground1.GetComponent<SpriteRenderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        MoveBackgrounds();
    }

    void MoveBackgrounds()
    {
        Vector3 movement = Vector3.left * Time.deltaTime * speed;
        ground1.position += movement;
        ground2.position += movement;
        bg1.position += movement / 4;
        bg2.position += movement / 4;

        if (bg1.position.x < -bgSize.x)
            bg1.position += Vector3.right * bgSize.x * 2;
        if (bg2.position.x < -bgSize.x)
            bg2.position += Vector3.right * bgSize.x * 2;
        if (ground1.position.x < -groundSize.x)
            ground1.position += Vector3.right * groundSize.x * 2;
        if (ground2.position.x < -groundSize.x)
            ground2.position += Vector3.right * groundSize.x * 2;
    }
}
