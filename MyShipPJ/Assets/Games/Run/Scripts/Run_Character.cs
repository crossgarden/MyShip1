using UnityEngine;

public class Run_Character : MonoBehaviour
{
    Rigidbody2D rigid;
    public float force = 5f;
    public float fastFallGravity = 8f; // 빠르게 떨어질 때의 중력 값
    private float normalGravity = 3f; // 일반 중력 값
    private bool isGrounded = false; // 땅에 닿았는지 여부
    private bool isFastFalling = false; // 빠르게 떨어지는 중인지 여부

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.freezeRotation = true; // 회전 방지 (쓰러짐 해결)
        rigid.gravityScale = normalGravity; // 중력 초기값 설정
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Jump_and_Fall();
        }
    }

    void Jump_and_Fall()
    {
        if (isGrounded)
            Jump();
        else if (!isFastFalling && rigid.velocity.y < 0) //점프 중에만 FastFall 가능
            FastFall();
    }

    void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, force); // 기존 X 속도 유지
        isGrounded = false;
        isFastFalling = false;
        AudioManager.instance.PlaySFX(AudioManager.SFXClip.FAIL);
    }

    void FastFall()
    {
        isFastFalling = true;
        rigid.gravityScale = fastFallGravity; // 중력 증가로 빠르게 떨어짐
        AudioManager.instance.PlaySFX(AudioManager.SFXClip.FAIL);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            RunManager.instance.GameOver(isOver: true);
        }
        else if (other.tag == "Coin")
        {
            AudioManager.instance.PlaySFX(AudioManager.SFXClip.SUCCESS);
            Destroy(other.gameObject);
            RunManager.instance.GetCoin();
        }
        else if (other.tag == "Untagged")
        {
            Land();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Untagged")
        {
            Land();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Untagged")
        {
            isGrounded = false; // 땅에서 떨어짐
        }
    }

    private void Land() //땅 착지 감지
    {
        isGrounded = true;
        isFastFalling = false;
        rigid.gravityScale = normalGravity; // 중력 원래대로 복구
        rigid.velocity = new Vector2(rigid.velocity.x, 0); // Y축 속도 0으로 고정
    }
}