using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SnakeController : MonoBehaviour
{

    public float moveSpeed = 0.5f;
    private Vector2 direction = Vector2.right;

    private List<Transform> snakeBody = new List<Transform>();
    public Transform snakePartPrefab;
    public Transform playerContainer;
    public GameObject gameOverPanel;
    private int score = 0;
    public TextMeshProUGUI scoreText; // UI 텍스트 컴포넌트를 통해 점수 표시
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI coinText;
    public int coin = 0;
    private bool isGameOver = false;  // 클래스 변수로 추가



    private void Start()
    {
        // 1. 선택된 캐릭터를 머리로 생성
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        GameObject head = Instantiate(playerPrefab, new Vector3(0, -1, 0), Quaternion.identity);
        head.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        head.transform.SetParent(playerContainer, false);
        head.transform.position = new Vector3(0, -1, 0); // Z = 0 으로 지정
        head.SetActive(true);

        // 2. Collider2D가 없다면 BoxCollider2D 추가
        if (head.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = head.AddComponent<BoxCollider2D>();
            collider.isTrigger = true; // 트리거로 설정
        }

        // 3. Rigidbody2D가 없다면 추가
        if (head.GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = head.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        snakeBody.Add(head.transform); // 머리를 snakeBody의 첫 번째로 등록

        // 4. 초기 꼬리 2개 추가
        AddBodyPart();
        AddBodyPart();

        HeadTriggerRelay relay = head.AddComponent<HeadTriggerRelay>();
        relay.controller = this;

        UpdateScoreText();
    }

    private float moveTimer = 0f;
    private float moveDelay = 0.3f;

    private void Update()
    {
        // 1. 이동 타이머 갱신
        moveTimer += Time.deltaTime;
        if (moveTimer >= moveDelay)
        {
            MoveSnake();  // 뱀 이동
            moveTimer = 0f;
        }

        // 2. 게임 오버 체크 (헤드가 화면을 벗어난 경우)
        CheckGameOver();
    }

    private void MoveSnake()
    {
        // 새 머리 위치 계산 (정수 위치로 스냅)
        Vector2 newHeadPosition = (Vector2)snakeBody[0].position + direction;
        newHeadPosition.x = Mathf.Round(newHeadPosition.x);
        newHeadPosition.y = Mathf.Round(newHeadPosition.y);

        // 몸통을 앞부분 위치로 이동
        for (int i = snakeBody.Count - 1; i > 0; i--)
        {
            snakeBody[i].position = new Vector3(
                snakeBody[i - 1].position.x,
                snakeBody[i - 1].position.y,
                0f
            );
        }

        // 머리 이동
        snakeBody[0].position = new Vector3(newHeadPosition.x, newHeadPosition.y, 0f);
    }


    public void SetDirection(string newDirection)
    {
        switch (newDirection)
        {
            case "Up": direction = Vector2.up; break;
            case "Down": direction = Vector2.down; break;
            case "Left": direction = Vector2.left; break;
            case "Right": direction = Vector2.right; break;
        }
    }

    private void AddBodyPart()
    {
        // 꼬리 위치를 머리의 위치에서 한 칸 떨어지게 설정 (현재 머리 방향 반대쪽)
        Vector2 spawnPos = (Vector2)snakeBody[snakeBody.Count - 1].position - direction;

        // 새로운 꼬리 생성
        Transform newPart = Instantiate(snakePartPrefab, spawnPos, Quaternion.identity, playerContainer);

        // 태그 설정
        newPart.tag = "Body";

        // 콜라이더 없으면 추가
        if (newPart.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D col = newPart.gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;  // 트리거 설정
        }

        // 생성된 꼬리를 리스트에 추가
        snakeBody.Add(newPart);
    }



    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("[SnakeController] OnTriggerEnter2D 호출됨 - 충돌 대상: " + collision.name);

        if (collision.CompareTag("Food"))
        {
            Destroy(collision.gameObject);
            AddBodyPart();
            SnakeManager.instance.SpawnFood();

            // 점수 증가 및 UI 갱신 추가
            score += 2;
            coin += 1;
            UpdateScoreText();

            SnakeManager.instance.AddScore(2);
            SnakeManager.instance.AddCoin(1);
        }
        else if (collision.CompareTag("Body"))
        {
            GameOver();
        }
    }


    private void UpdateScoreText()
    {
        if(scoreText != null)
        {
            scoreText.text = "점수: " + score.ToString();
        }
    }
    






    private void CheckGameOver()
    {
        Vector3 headPosition = snakeBody[0].position;

        // 카메라의 월드 좌표를 얻기 위해 화면 크기 계산
        Vector3 topLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)); // 화면의 왼쪽 위
        Vector3 bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)); // 화면의 오른쪽 아래

        // 헤드 위치가 화면을 벗어났다면 게임 오버 처리
        if (headPosition.x < topLeft.x || headPosition.x > bottomRight.x || headPosition.y > topLeft.y || headPosition.y < bottomRight.y)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        if (isGameOver) return;  // 이미 게임 오버면 무시
        isGameOver = true;
        SnakeManager.instance.score = score;
        SnakeManager.instance.coin = Mathf.FloorToInt(score / 2f);
        SnakeManager.instance.Fail(true);
    }

    // SnakeController.cs
    public void HandleTriggerEnter(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }







}
