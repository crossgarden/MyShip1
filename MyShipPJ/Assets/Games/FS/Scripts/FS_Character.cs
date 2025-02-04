using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_Character : MonoBehaviour
{

    Rigidbody2D rigid;
    public float force = 5f;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Up();
        }

        Tilt();
    }

    void Up()
    {
        rigid.velocity = Vector2.up * force;
    }

    void Tilt()
    {
        float fallRotation = rigid.velocity.y * force;
        fallRotation = Mathf.Clamp(fallRotation, -60, 20);
        transform.rotation = Quaternion.Euler(0, 0, fallRotation);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Obstacle"){
            FSManager.instance.GameOver(isOver: true);
        }

        if(other.tag == "Coin"){
            AudioManager.instance.PlaySFX(AudioManager.SFXClip.SUCCESS);
            Destroy(other.gameObject);
            FSManager.instance.GetCoin();
        }
    }
}
