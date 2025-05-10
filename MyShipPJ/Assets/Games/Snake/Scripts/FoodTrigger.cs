using UnityEngine;

public class FoodTrigger : MonoBehaviour
{
    public SnakeController snakeController; // SnakeController를 연결하기 위한 변수

    void Start()
    {
        // SnakeController가 없으면 자동으로 찾기
        if (snakeController == null)
        {
            snakeController = FindObjectOfType<SnakeController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))  // 플레이어(머리)와 충돌 시
        {
            Debug.Log("FoodTrigger와 충돌!");

            // SnakeController의 충돌 처리 메서드 호출
            snakeController.OnTriggerEnter2D(collision);

            // 충돌 후 먹이 제거 (충돌은 OnTriggerEnter2D에서 처리됨)
            Destroy(gameObject);  // 먹이 오브젝트 제거
        }
    }
}
