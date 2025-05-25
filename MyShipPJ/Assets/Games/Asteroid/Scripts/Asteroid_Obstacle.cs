using UnityEngine;

public class Asteroid_Obstacle : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.down; // 기본값 설정 (Manager에서 덮어쓸 것)

    void Update()
    {
        Move();
        //FlipX();
    }

    void Move()
    {
        // 설정된 방향으로 이동
        Vector3 movement = moveDirection * Time.deltaTime * Asteroid_Manager.instance.speed;
        transform.position += movement;

        // 화면 밖으로 나갔는지 체크 (모든 방향에 대해)
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.x < -0.1f || viewportPos.x > 1.1f || viewportPos.y < -0.1f || viewportPos.y > 1.1f)
        {
            Destroy(gameObject);
        }
    }

    void FlipX()
    {
        // X축으로 주기적으로 반전 (시각적 효과)
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }
}