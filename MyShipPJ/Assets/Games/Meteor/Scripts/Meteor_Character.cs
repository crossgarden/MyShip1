using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor_Character : MonoBehaviour
{
    Rigidbody2D rigid;
    [SerializeField]
    float moveSpeed = 5f;

    private float minX, maxX;
    private float characterHalfWidth;
    private int characterDirection = 1; // -1: 오른쪽, +1: 왼쪽

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        CalculateScreenBounds();
    }

    void Update()
    {
        HandleInput();
        ClampPosition();
    }

    void ClampPosition()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        transform.position = clampedPosition;
    }

    void CalculateScreenBounds()
    {
        // SpriteRenderer 대신 Collider2D 사용 (더 안정적)
        Collider2D collider = GetComponent<Collider2D>();
        characterHalfWidth = collider != null ? collider.bounds.extents.x : 0.5f;

        float cameraHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        minX = Camera.main.transform.position.x - cameraHalfWidth + characterHalfWidth;
        maxX = Camera.main.transform.position.x + cameraHalfWidth - characterHalfWidth;
    }

    void HandleInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 inputPosition = Input.mousePosition;

            if (inputPosition.x < Screen.width / 2)
            {
                // 왼쪽 이동
                rigid.velocity = new Vector2(-moveSpeed, rigid.velocity.y);
                if (characterDirection != 1)
                {
                    characterDirection = 1;
                    FlipCharacter();
                }
            }
            else
            {
                // 오른쪽 이동
                rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
                if (characterDirection != -1)
                {
                    characterDirection = -1;
                    FlipCharacter();
                }
            }
        }
        else
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
    }

    // Transform.scale을 이용한 좌우 반전
    void FlipCharacter()
    {
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * characterDirection;
        transform.localScale = newScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            Meteor_Manager.instance.GameOver(isOver: true);
        }

        if (other.tag == "Coin")
        {
            AudioManager.instance.PlaySFX(AudioManager.SFXClip.SUCCESS);
            Destroy(other.gameObject);
            Meteor_Manager.instance.GetCoin();
        }
    }
}